//------------------------------------------------------------------------------
// <copyright file="MetafileDocumentPrinter.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Drawing.Printing;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Dynamics.AX.Framework.DocumentRouting.OData.Entities;

    /// <summary>
    /// The document printer for files of Metafile type.
    /// </summary>
    internal class MetafileDocumentPrinter : IDocumentPrinter
    {
        /// <summary>
        /// The current page number
        /// </summary>
        private int currentPageNumber;

        /// <summary>
        /// The EMF images
        /// </summary>
        private List<Metafile> emfImages;

        /// <summary>
        /// Prints the specified document.
        /// </summary>
        /// <param name="document">The document.</param>
        public bool Print(IDocumentContract document)
        {
            var documentContract = document as MetafileDocumentContract ?? throw new ArgumentException("Invalide type", nameof(document));
            if (documentContract.PageSizes == null || documentContract.PageSizes.Count() == 0)
            {
                throw new ArgumentNullException(Resources.NoPageSizes);
            }

            var pageSizesList = documentContract.PageSizes.ToList();

            this.emfImages = MetafileHelper.GetEmfImagesToPePrinted(pageSizesList, documentContract.Contents);
            if (this.emfImages == null || this.emfImages.Count == 0)
            {
                // Issue an instrumentation error and return as there is nothing to print
                throw new ArgumentException(Resources.NoMetafilesToPrint);
            }

            // Now determine the printer settings necessary
            PageSettings pageSettingsOfPrinter = null;
            var pageSettingsHelper = new PageSettingsHelper();

            if (!string.IsNullOrWhiteSpace(documentContract.Settings))
            {
                pageSettingsHelper.IsLandscapeSetOnReportDesign = this.emfImages[0].Height < this.emfImages[0].Width;

                try
                {
                    pageSettingsOfPrinter = pageSettingsHelper.ProcessPageSettings(documentContract.Settings);
                }
                catch (Exception)
                {
                    // TODO Error log Resources.UnableToInitializePrinterSettings
                    return false;
                }
            }

            // PageSettings returned after processing should not be null. Throw if it is.
            if (pageSettingsOfPrinter == null)
            {
                throw new ArgumentNullException(Resources.NoPageSettings);
            }
            
            // Send the print job to printer with the pagesettings obtained
            int retryCount = 1;
            try
            {
                while (retryCount <= Constants.MaximumRetry)
                {
                    try
                    {
                        this.currentPageNumber = 0;
                        using (var printDocument = new PrintDocument())
                        {
                            printDocument.DocumentName = documentContract.Name;

                            printDocument.DefaultPageSettings = pageSettingsOfPrinter;
                            printDocument.PrinterSettings = pageSettingsOfPrinter.PrinterSettings;
                            printDocument.PrintPage += this.PrintEachPage;

                            // use standard print controller instead of print controller with status dialog. 
                            printDocument.PrintController = new StandardPrintController();
                            printDocument.Print();
                        }

                        break;
                    }
                    catch (InvalidPrinterException)
                    {
                        if (retryCount < Constants.MaximumRetry)
                        {
                            retryCount++;
                        }
                        else
                        {
                            string message = string.Format(CultureInfo.InvariantCulture, Resources.InvalidPrinter, pageSettingsOfPrinter.PrinterSettings.PrinterName);
                            //TODO Error log 
                            return false;
                        }
                    }
                }
            }
            finally
            {
                if (this.emfImages != null && this.emfImages.Count > 0)
                {
                    foreach (var page in this.emfImages)
                    {
                        if (page != null)
                        {
                            page.Dispose();
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// The event handler which handles the print job
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">The event args</param>
        private void PrintEachPage(object sender, PrintPageEventArgs e)
        { 
            // get the image to be printed and its header information
            var metafile = this.emfImages[this.currentPageNumber];

            Point[] points = ScaleToFitHelper.ScaleToFitMetafileForPrinting(e.PageSettings, e.Graphics, metafile);
            Debug.Assert(points != null, "ScaleToFitMetafileForPrinting must not return null.");
            Debug.Assert(points.Length == 1, "ScaleToFitMetafileForPrinting must return a Point array of length 1.");
            Debug.Assert(this.emfImages != null, "The images to be sent to the printer must not be null.");

            if (points != null && points.Length >= 1)
            {
                // draw the image to the printer
                e.Graphics.DrawImageUnscaled(metafile, points[0]);
            }
            else
            {                
                throw new InvalidOperationException(Resources.PointNotSpecified);
            }

            // determine if there are more pages to print
            e.HasMorePages = ++this.currentPageNumber < this.emfImages.Count;
        }
    }
}