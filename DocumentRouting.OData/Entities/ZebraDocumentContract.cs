//------------------------------------------------------------------------------
// <copyright file="ZebraDocumentContract.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.OData.Entities
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Document contract for routing Zebra printer barcode label documents.
    /// </summary>
    [DataContract]
    public class ZebraDocumentContract : DocumentContract
    {
        /// <summary>
        /// Gets or sets the settings. If printing then this property contains the network printer path.
        /// </summary>
        [DataMember]
        public override string Settings { get; set; }
    }
}
