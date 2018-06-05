//------------------------------------------------------------------------------
// <copyright file="PrinterDataManager.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.AX.Framework.DocumentRouting.OData;
    using Microsoft.Dynamics.AX.Framework.DocumentRouting.OData.Entities;

    /// <summary>
    /// Data manager for printers.
    /// </summary>
    public class PrinterDataManager : IPrinterDataManager
    {
        /// <summary>
        /// The document routing OData client factory
        /// </summary>
        private readonly IDocumentRoutingODataClientFactory documentRoutingODataClientFactory;

        /// <summary>
        /// The printers provider
        /// </summary>
        private readonly IPrintersProvider printersProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrinterDataManager" /> class.
        /// </summary>
        /// <param name="documentRoutingODataClientFactory">The document routing o data client factory.</param>
        /// <param name="printersProvider">The printers provider.</param>
        public PrinterDataManager(IDocumentRoutingODataClientFactory documentRoutingODataClientFactory, IPrintersProvider printersProvider)
        {
            this.documentRoutingODataClientFactory = documentRoutingODataClientFactory ?? throw new ArgumentNullException(nameof(documentRoutingODataClientFactory));
            this.printersProvider = printersProvider ?? throw new ArgumentNullException(nameof(printersProvider));
        }

        /// <summary>
        /// Gets the printer list.
        /// </summary>
        /// <returns>Async task</returns>
        public async Task<List<PrinterData>> GetPrinters()
        {
            var availablePrinters = new List<PrinterData>();

            var installedPrinters = this.GetInstalledPrinters();
            var registeredPrinters = await this.GetRegisteredPrinters();

            foreach (var registeredPrinter in registeredPrinters)
            {
                var isAvailable = installedPrinters.Any(p => string.Equals(registeredPrinter.PrinterPath, p.PrinterPath, StringComparison.OrdinalIgnoreCase));
                registeredPrinter.PrinterStatus = isAvailable ? PrinterStatus.Installed : PrinterStatus.UnInstalled;
                registeredPrinter.IsRegistered = true;
                availablePrinters.Add(registeredPrinter);
            }

            foreach (var installedPrinter in installedPrinters)
            {
                if (!registeredPrinters.Any(p => string.Equals(installedPrinter.PrinterPath, p.PrinterPath, StringComparison.OrdinalIgnoreCase)))
                {
                    installedPrinter.IsRegistered = false;
                    installedPrinter.PrinterStatus = PrinterStatus.Installed;
                    availablePrinters.Add(installedPrinter);
                }
            }

            return availablePrinters;
        }

        /// <summary>
        /// Gets the registered printers.
        /// </summary>
        /// <returns>
        /// The registered printers collection.
        /// </returns>
        public async Task<List<PrinterData>> GetRegisteredPrinters()
        {
            using (var client = this.documentRoutingODataClientFactory.Create())
            {
                var printers = await client.GetRegisteredPrinters();
                return printers.Select(p => new PrinterData { PrinterName = p.Name, PrinterPath = p.Path, IsRegistered = true }).ToList();
            }
        }

        /// <summary>
        /// Gets the installed printers.
        /// </summary>
        /// <returns>
        /// The installed printers collection.
        /// </returns>
        public List<PrinterData> GetInstalledPrinters()
        {
            var result = new List<PrinterData>();
            foreach (string printerPath in this.printersProvider.GetInstalledPrinters())
            {
                result.Add(new PrinterData
                {
                    PrinterPath = printerPath,
                    PrinterName = this.PrinterNameFromPath(printerPath),
                    PrinterDescription = string.Empty,
                    PrinterStatus = PrinterStatus.Installed
                });
            }

            return result;
        }

        /// <summary>
        /// Updates the registered printers.
        /// </summary>
        /// <param name="printers">The printers.</param>
        /// <returns>
        /// The task
        /// </returns>
        public async Task UpdateRegisteredPrinters(IEnumerable<PrinterData> printers)
        {
            using (var client = this.documentRoutingODataClientFactory.Create())
            {
                var registeredPrinters = printers.Where(p => p.IsRegistered).Select(p => new DocumentRoutingPrinter { Name = p.PrinterName, Path = p.PrinterPath, Description = p.PrinterDescription });
                var unregisteredPrinters = printers.Where(p => p.IsRegistered).Select(p => new DocumentRoutingPrinter { Name = p.PrinterName, Path = p.PrinterPath, Description = p.PrinterDescription });

                await client.RegisterPrinters(registeredPrinters);
                await client.UnregisterPrinters(unregisteredPrinters);
            }
        }

        /// <summary>
        /// Printers the name from path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The printer name.</returns>
        private string PrinterNameFromPath(string path)
        {
            var value = path;
            if (path.StartsWith(@"\\", StringComparison.InvariantCultureIgnoreCase))
            {
                value = Path.GetFileName(path);
            }

            return value;
        }
    }
}
