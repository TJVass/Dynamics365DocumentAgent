//------------------------------------------------------------------------------
// <copyright file="ZebraDocumentPrinter.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.Runtime
{
    using System;
    using System.Diagnostics;
    using System.Text;
    using Microsoft.Dynamics.AX.Framework.DocumentRouting.OData.Entities;
    using Microsoft.Dynamics.AX.WHS.DeviceCom;

    /// <summary>
    /// The printer for documents of Zebra type
    /// </summary>
    internal class ZebraDocumentPrinter : IDocumentPrinter
    {
        /// <summary>
        /// Prints the specified document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns>
        /// True if document was printed successfully.
        /// </returns>
        public bool Print(IDocumentContract document)
        {
            var documentContract = document as ZebraDocumentContract ?? throw new ArgumentException("Invalid type", nameof(document));
            Debug.Assert(documentContract != null, "Parameter 'documentContract' must not be null.");
            Debug.Assert(documentContract.Contents != null && documentContract.Contents.Length > 0, "Parameter 'documentContract.Contents' must not be null or empty.");
            Debug.Assert(!string.IsNullOrEmpty(documentContract.Settings), "Parameter 'documentContract.Settings' must not be null or empty.");

            // The printer name is the only thing in settings when printing via Zebra
            string printerName = documentContract.Settings;
            string contents = Encoding.ASCII.GetString(documentContract.Contents);

            string errorMessage = Printer.SendStringToPrinter(printerName, contents);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                // TODO Error log
                return false;
            }

            return true;
        }
    }
}