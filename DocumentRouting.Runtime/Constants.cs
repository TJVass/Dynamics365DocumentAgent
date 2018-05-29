//------------------------------------------------------------------------------
// <copyright file="Constants.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.Runtime
{
    /// <summary>
    /// Constants values
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// A string containing empty
        /// </summary>
        internal const string Empty = "empty";

        /// <summary>
        /// Maximum time we retry
        /// </summary>
        internal const int MaximumRetry = 3;

        /// <summary>
        /// Use to initialize double/float value.
        /// </summary>
        internal const float InitializeDouble = 0.0f;

        /// <summary>
        /// Default dots-per-inch resolution for printing. This value gives a high enough
        /// resolution so that sizing can be done on a report image and it remains readable.
        /// </summary>
        internal const float DefaultScaleToFitDpiResolution = 300f;

        /// <summary>
        /// The hundredth of an inch factor.
        /// </summary>
        internal const float HundredthInchFactor = 100.0f;

        /// <summary>
        /// The PDF file extension
        /// </summary>
        internal const string PdfExtension = ".pdf";

        /// <summary>
        /// The Dynamics 365 for Operations documents directory used to save PDF files in for printing.
        /// </summary>
        internal const string OperationsDocumentsDirectoryName = "OperationsDocuments";

        /// <summary>
        /// The check installed printer retry count
        /// </summary>
        internal const int CheckInstalledPrinterRetryCount = 3;
    }
}