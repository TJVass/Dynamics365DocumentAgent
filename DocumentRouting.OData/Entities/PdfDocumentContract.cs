//------------------------------------------------------------------------------
// <copyright file="PdfDocumentContract.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.OData.Entities
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Document contract for routing PDF documents.
    /// </summary>
    [DataContract]
    public class PdfDocumentContract : DocumentContract
    {
        /// <summary>
        /// Gets or set the settings used to handle the document
        /// </summary>
        [DataMember]
        public override string Settings { get; set; }
    }
}
