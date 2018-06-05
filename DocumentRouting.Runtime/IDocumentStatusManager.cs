//------------------------------------------------------------------------------
// <copyright file="IDocumentStatusManager.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.Runtime
{
    using System;

    /// <summary>
    /// The interface for document status manager
    /// </summary>
    public interface IDocumentStatusManager
    {
        /// <summary>
        /// Adds the printing status.
        /// </summary>
        /// <param name="statusContract">The status contract.</param>
        void AddPrintingStatus(DocumentPrintingStatusContract statusContract);

        /// <summary>
        /// Updates the printing status.
        /// </summary>
        /// <param name="rowId">The row identifier.</param>
        /// <param name="statusString">The status string.</param>
        void UpdatePrintingStatus(string rowId, string statusString);
    }
}