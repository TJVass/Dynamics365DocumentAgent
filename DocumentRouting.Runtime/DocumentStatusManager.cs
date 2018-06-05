//------------------------------------------------------------------------------
// <copyright file="DocumentStatusManager.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The document status manager.
    /// </summary>
    public class DocumentStatusManager : IDocumentStatusManager
    {
        /// <summary>
        /// Max number of rows in the document statuses
        /// </summary>
        private readonly int maxNumberOfRows;

        /// <summary>
        /// Document status list
        /// </summary>
        private readonly IList<DocumentPrintingStatusContract> documentStatuses;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentStatusManager" /> class.
        /// </summary>
        /// <param name="documentStatuses">A list to for document printing status</param>
        /// <param name="maxNumberOfRows">Max number of rows in the list</param>
        public DocumentStatusManager(IList<DocumentPrintingStatusContract> documentStatuses, int maxNumberOfRows = 1000)
        {
            if (maxNumberOfRows < 10 || maxNumberOfRows > 10000)
            {
                throw new ArgumentException("The number of rows must be between 10 and 10,000");
            }

            this.documentStatuses = documentStatuses ?? throw new ArgumentNullException(nameof(documentStatuses));
            this.maxNumberOfRows = maxNumberOfRows;
        }

        /// <summary>
        /// Adds the printing status in the Table
        /// </summary>
        /// <param name="statusContract">The status contract.</param>
        public void AddPrintingStatus(DocumentPrintingStatusContract statusContract)
        {
            // To make sure tha the number of document printing status is no more than max number of rows.
            if (this.documentStatuses.Count >= this.maxNumberOfRows)
            {
                this.documentStatuses.RemoveAt(this.maxNumberOfRows - 1);
            }

            statusContract.StatusDateTime = DateTime.Now;
            this.documentStatuses.Insert(0, statusContract);
        }

        /// <summary>
        /// Updates the status with "Success" or Failure" once the printing is done.
        /// </summary>
        /// <param name="rowId">The row Id which needs to be updated</param>
        /// <param name="statusString">The string value indicating status.</param>
        public void UpdatePrintingStatus(string rowId, string statusString)
        {
            DocumentPrintingStatusContract status = this.documentStatuses.FirstOrDefault(s => s.Id == rowId);

            if (status != null)
            {
                status.Status = statusString;
                status.StatusDateTime = DateTime.Now;
            }
        }
    }
}
