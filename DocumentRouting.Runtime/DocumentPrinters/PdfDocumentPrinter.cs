//------------------------------------------------------------------------------
// <copyright file="PdfDocumentPrinter.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.Runtime
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using Microsoft.Dynamics.AX.Framework.DocumentRouting.OData.Entities;
    using Microsoft.Win32;

    /// <summary>
    /// The printer for documents of PDF type.
    /// </summary>
    internal class PdfDocumentPrinter : IDocumentPrinter
    {
        /// <summary>
        /// Command line format string
        /// </summary>
        internal const string PdfPrintCommandFormatString = "/s /h /t \"{0}\" \"{1}\"";

        /// <summary>
        /// Registry key that contains the installation path to Adobe Acrobat Reader
        /// </summary>
        internal const string AcroReadExeRegistryPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\AcroRd32.exe";

        /// <summary>
        /// Prints the specified document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns>True if document was printed successfully.</returns>
        public bool Print(IDocumentContract document)
        {
            var documentContract = document as PdfDocumentContract ?? throw new ArgumentException("Invalid type", nameof(document));

            // Validate the document contract
            if (documentContract == null)
            {
                throw new ArgumentNullException(nameof(documentContract));
            }

            if (documentContract.Contents == null || documentContract.Contents.Length == 0)
            {
                throw new ArgumentNullException("documentContract.Contents");
            }

            if (string.IsNullOrWhiteSpace(documentContract.Settings))
            {
                throw new ArgumentNullException("documentContract.Settings");
            }

            // Get the printer from the settings
            var printer = PageSettingsHelper.PrinterNameFromPageSettings(documentContract.Settings);
            if (string.IsNullOrWhiteSpace(printer))
            {
                throw new ArgumentNullException("PageSettings.PrinterName");
            }

            // Get the pdf file to send to the printer
            var pdfFilePath = this.DocumentContractToTempFile(documentContract);

            try
            {
                this.AdobeReaderPrint(pdfFilePath, printer);
                return true;
            }
            catch (Exception)
            {
                // TODO Error Log
                return false;
            }
        }

        /// <summary>
        /// Sends the file to the printer via abode reader.
        /// </summary>
        /// <param name="pdfFilePath">The PDF file path.</param>
        /// <param name="printerPath">The printer path.</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        private void AdobeReaderPrint(string pdfFilePath, string printerPath)
        {
            var adobeExePath = this.GetAdobeReaderExePath();
            if (string.IsNullOrWhiteSpace(adobeExePath))
            {
                throw new InvalidOperationException(Resources.AdobeAcrobatNotInstalled);
            }

            if (!File.Exists(pdfFilePath))
            {
                var message = string.Format(CultureInfo.CurrentCulture, Resources.FileNotFound, pdfFilePath);
                throw new FileNotFoundException(message);
            }

            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo()
                {
                    FileName = adobeExePath,
                    Arguments = string.Format(CultureInfo.InvariantCulture, PdfPrintCommandFormatString, pdfFilePath, printerPath)
                };

                process.Start();
            }
        }

        /// <summary>
        /// Gets the path in which Adobe Reader is installed.
        /// </summary>
        /// <returns>The path value.</returns>
        private string GetAdobeReaderExePath()
        {
            string value = string.Empty;

            object keyValue = Registry.GetValue(AcroReadExeRegistryPath, string.Empty, null);
            if (keyValue != null)
            {
                value = keyValue.ToString();
                if (!File.Exists(value))
                {
                    value = string.Empty;
                }
            }

            return value;
        }


        /// <summary>
        /// Uses the <c>DocumentContract</c> object to create a temporary file.
        /// </summary>
        /// <param name="documentContract">The <c>DocumentContract</c> object.</param>
        /// <returns>The path to the file created.</returns>
        private string DocumentContractToTempFile(PdfDocumentContract documentContract)
        {
            Debug.Assert(documentContract != null, "Parameter 'documentContract' must not be null or empty.");
            Debug.Assert(documentContract.Contents != null && documentContract.Contents.Length > 0, "Parameter 'documentContract.Contents' must not be null or empty.");
            Debug.Assert(!string.IsNullOrWhiteSpace(documentContract.Name), "Parameter 'documentContract.Name' must not be null or empty.");

            var filenameOnly = this.DocumentContractFileName(documentContract);
            Debug.Assert(!string.IsNullOrWhiteSpace(filenameOnly), "filenameOnly must not be null or empty.");

            var tempFilePath = this.ByteArrayToTempFile(documentContract.Contents, filenameOnly);

            if (string.IsNullOrWhiteSpace(tempFilePath) || !File.Exists(tempFilePath))
            {
                // create and throw an exception
                string message = string.Format(CultureInfo.CurrentCulture, Resources.FileNotFound, tempFilePath);
                FileNotFoundException ex = new FileNotFoundException(message);

                // TODO Error Log

                throw ex;
            }

            return tempFilePath;
        }

        /// <summary>
        /// Uses the Name of the <c>DocumentContract</c> object to build a file name.
        /// </summary>
        /// <param name="documentContract">The <c>DocumentContract</c> object.</param>
        /// <returns>The file name value.</returns>
        private string DocumentContractFileName(DocumentContract documentContract)
        {
            Debug.Assert(documentContract != null, "Parameter 'documentContract' must not be null or empty.");
            Debug.Assert(!string.IsNullOrWhiteSpace(documentContract.Name), "Parameter 'documentContract.Name' must not be null or empty.");

            string value = Path.GetFileNameWithoutExtension(documentContract.Name);

            value = string.Concat(value, Constants.PdfExtension);

            return value;
        }

        /// <summary>
        /// Saves the byte array to a temporary file.
        /// </summary>
        /// <param name="bytes">The byte array.</param>
        /// <param name="name">The name of the file to save.</param>
        /// <returns>The path and name of the file created.</returns>
        private string ByteArrayToTempFile(byte[] bytes, string name)
        {
            Debug.Assert(bytes != null && bytes.Length > 0, "Parameter 'bytes' must not be null or empty.");
            Debug.Assert(!string.IsNullOrWhiteSpace(name), "Parameter 'name' must not be null or empty.");

            string path = this.GetTempFileDirectory();
            string filename = Path.Combine(path, name);

            File.WriteAllBytes(filename, bytes);

            return filename;
        }

        /// <summary>
        /// Builds the complete file path for where the file is saved.
        /// </summary>
        /// <returns>The complete directory path.</returns>
        /// <remarks>The path is comprised of: the user's temporary path + OperationsDocuments + a time stamp.</remarks>
        private string GetTempFileDirectory()
        {
            string timeStamp = DateTime.Now.ToString("hh.mm.ss.ff", CultureInfo.InvariantCulture);

            string path = Path.Combine(Path.GetTempPath(), Constants.OperationsDocumentsDirectoryName, timeStamp);

            // Create the directory if it doesn't exist. If an exception occurs fall back to use the normal 
            // temp directory
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception)
                {
                    // don't throw for any reason here.
                }
                finally
                {
                    if (!Directory.Exists(path))
                    {
                        path = Path.GetTempPath();
                    }
                }
            }

            return path;
        }
    }
}