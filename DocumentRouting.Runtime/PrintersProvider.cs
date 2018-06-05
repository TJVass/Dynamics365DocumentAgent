//------------------------------------------------------------------------------
// <copyright file="PrintersProvider.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.Runtime
{
    using System.Collections.Generic;
    using System.Drawing.Printing;

    /// <summary>
    /// The printers provider.
    /// </summary>
    public class PrintersProvider : IPrintersProvider
    {
        /// <summary>
        /// Send To OneNote printer
        /// </summary>
        private const string OneNote = "Send To OneNote";

        /// <summary>
        /// Microsoft XPS printer
        /// </summary>
        private const string MicrosoftXps = "Microsoft XPS";

        /// <summary>
        /// Microsoft Print to PDF
        /// </summary>
        private const string MicrosoftPdf = "Microsoft Print to PDF";

        /// <summary>
        /// The list of excluded printers.
        /// </summary>
        private readonly List<string> excludedPrinters;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintersProvider"/> class.
        /// </summary>
        public PrintersProvider()
        {
            this.excludedPrinters = new List<string>
            {
                // Add the string constants to the list
                OneNote,
                MicrosoftXps,
                MicrosoftPdf,

                // Add the full localized names to the list in case installation performs localization
                Resources.SendToOneNotePrinterName,
                Resources.MicrosoftXpsDocumentWriterPrinterName,
                Resources.MicrosoftPrintToPDFPrinterName
            };
        }

        /// <summary>
        /// Gets the installed printers.
        /// </summary>
        /// <returns>The printer paths collection.</returns>
        public List<string> GetInstalledPrinters()
        {
            var result = new List<string>();
            foreach (string printerPath in PrinterSettings.InstalledPrinters)
            {
                if (!string.IsNullOrWhiteSpace(printerPath) && !this.excludedPrinters.Contains(printerPath))
                {
                    result.Add(printerPath);
                }
            }

            return result;
        }
    }
}
