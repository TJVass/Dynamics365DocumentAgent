//------------------------------------------------------------------------------
// <copyright file="MetafileDocumentContract.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.OData.Entities
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents the document contract for metafiles, images, to be routed.
    /// </summary>
    [DataContract]
    public class MetafileDocumentContract : DocumentContract
    {
        /// <summary>
        /// Gets or sets the settings used to handle the document
        /// </summary>
        [DataMember]
        public override string Settings { get; set; }

        /// <summary>
        /// Gets or sets the settings used to handle the document
        /// </summary>
        [DataMember]
        public IEnumerable<int> PageSizes { get; set; }
    }
}
