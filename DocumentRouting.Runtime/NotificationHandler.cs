//------------------------------------------------------------------------------
// <copyright file="NotificationHandler.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.AX.Framework.DocumentRouting.OData;
    using Microsoft.Dynamics.AX.Framework.DocumentRouting.OData.Entities;
    using Microsoft.WindowsAzure.Storage.Blob;

    /// <summary>
    /// The notification handler.
    /// </summary>
    public class NotificationHandler : INotificationHandler
    {
        /// <summary>
        /// The document routing o data client factory
        /// </summary>
        private readonly IDocumentRoutingODataClientFactory documentRoutingODataClientFactory;

        /// <summary>
        /// The printer data provider
        /// </summary>
        private readonly IPrintersProvider printerDataProvider;

        /// <summary>
        /// The document status manager
        /// </summary>
        private readonly IDocumentStatusManager documentStatusManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationHandler" /> class.
        /// </summary>
        /// <param name="documentRoutingODataClientFactory">The document routing o data client factory.</param>
        /// <param name="printerDataProvider">The printer data provider.</param>
        /// <param name="documentStatusManager">The document status manager.</param>
        public NotificationHandler(IDocumentRoutingODataClientFactory documentRoutingODataClientFactory, IPrintersProvider printerDataProvider, IDocumentStatusManager documentStatusManager)
        {
            this.documentRoutingODataClientFactory = documentRoutingODataClientFactory ?? throw new ArgumentNullException(nameof(documentRoutingODataClientFactory));
            this.printerDataProvider = printerDataProvider ?? throw new ArgumentNullException(nameof(printerDataProvider));
            this.documentStatusManager = documentStatusManager ?? throw new ArgumentNullException(nameof(documentStatusManager));
        }

        /// <summary>
        /// Handles the notification.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <returns>The task</returns>
        public async Task HandleNotification(Notification notification)
        {
            try
            {
                IDocumentContract document = null;
                using (var client = this.documentRoutingODataClientFactory.Create())
                {
                    var uri = await client.GetDownloadDocumentUri(notification.JobId);
                    using (var stream = new MemoryStream())
                    {
                        var blob = new CloudBlockBlob(uri);
                        await blob.DownloadToStreamAsync(stream).ConfigureAwait(false);
                        stream.Seek(0, SeekOrigin.Begin);
                        document = this.DeserializeDocument(stream, notification.DocumentContractType);
                    }
                }

                var printerPath = this.GetPrinterPath(document, notification.DocumentContractType);
                this.AddPrintingStatus(printerPath, document, notification);
                if (!await this.IsPrinterInstalled(printerPath))
                {
                    this.documentStatusManager.UpdatePrintingStatus(notification.JobId, DocumentStatus.Failure.ToString());
                    using (var client = this.documentRoutingODataClientFactory.Create())
                    {
                        await client.UnregisterPrinters(new List<DocumentRoutingPrinter> { new DocumentRoutingPrinter { Path = printerPath } });
                    }

                    return;
                }

                this.documentStatusManager.UpdatePrintingStatus(notification.JobId, DocumentStatus.Printing.ToString());

                IDocumentPrinter documentPrinter = this.GetDocumentPrinter(notification.DocumentContractType);
                var printResult = documentPrinter.Print(document);

                this.documentStatusManager.UpdatePrintingStatus(notification.JobId, printResult ? DocumentStatus.Success.ToString() : DocumentStatus.Failure.ToString());
            }
            catch
            {
                this.documentStatusManager.UpdatePrintingStatus(notification.JobId, DocumentStatus.Failure.ToString());
                // TODO Error Log
            }
        }

        private void AddPrintingStatus(string printerPath, IDocumentContract document, Notification notification)
        {
            var status = new DocumentPrintingStatusContract
            {
                Id = notification.JobId,
                DocumentName = document.Name,
                PrinterName = printerPath,
                Status = DocumentStatus.Pending.ToString()
            };

            this.documentStatusManager.AddPrintingStatus(status);
        }

        /// <summary>
        /// Determines whether printer with the specified printer path is installed.
        /// </summary>
        /// <param name="printerPath">The printer path.</param>
        /// <returns>
        ///   <c>true</c> if is printer installed; otherwise, <c>false</c>.
        /// </returns>
        private async Task<bool> IsPrinterInstalled(string printerPath)
        {
            for (int i = 0; i < Constants.CheckInstalledPrinterRetryCount; i++)
            {
                var installedPrinters = this.printerDataProvider.GetInstalledPrinters();
                var isInstalled = installedPrinters.Contains(printerPath, StringComparer.OrdinalIgnoreCase);
                if (isInstalled)
                {
                    return true;
                }

                var message = string.Format(CultureInfo.InvariantCulture, Resources.PrinterNotInInstalledPrintersListRetry, printerPath);

                await Task.Delay(200);
            }

            return false;
        }

        private string GetPrinterPath(IDocumentContract document, DocumentContractType documentContractType)
        {
            switch (documentContractType)
            {
                case DocumentContractType.Metafile:
                    return PageSettingsHelper.PrinterNameFromPageSettings(document.Settings);
                case DocumentContractType.Pdf:
                    return PageSettingsHelper.PrinterNameFromPageSettings(document.Settings);
                case DocumentContractType.Zebra:
                    return document.Settings;
                default:
                    throw new ArgumentException("Unsupported value", nameof(documentContractType));
            }
        }

        private IDocumentPrinter GetDocumentPrinter(DocumentContractType documentContractType)
        {
            switch (documentContractType)
            {
                case DocumentContractType.Metafile:
                    return new MetafileDocumentPrinter();
                case DocumentContractType.Pdf:
                    return new PdfDocumentPrinter();
                case DocumentContractType.Zebra:
                    return new ZebraDocumentPrinter();
                default:
                    throw new ArgumentException("Unsupported value", nameof(documentContractType));
            }
        }

        private IDocumentContract DeserializeDocument(MemoryStream stream, DocumentContractType documentContractType)
        {
            DataContractSerializer serializer = null;
            switch (documentContractType)
            {
                case DocumentContractType.Metafile:
                    serializer = new DataContractSerializer(typeof(MetafileDocumentContract));
                    break;
                case DocumentContractType.Pdf:
                    serializer = new DataContractSerializer(typeof(PdfDocumentContract));
                    break;
                case DocumentContractType.Zebra:
                    serializer = new DataContractSerializer(typeof(ZebraDocumentContract));
                    break;
                default:
                    throw new ArgumentException("Unsupported value", nameof(documentContractType));
            }

            return (IDocumentContract)serializer.ReadObject(stream);
        }
    }
}
