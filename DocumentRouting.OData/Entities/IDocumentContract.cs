//------------------------------------------------------------------------------
// <copyright file="IDocumentContract.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.OData.Entities
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The IDocumentContract interface definition
    /// </summary>
    public interface IDocumentContract
    {
        /// <summary>
        /// Gets or sets the document name
        /// </summary>
        [DataMember]
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the document contents.
        /// </summary>
        [DataMember]
        byte[] Contents { get; set; }

        /// <summary>
        /// Gets or sets the document target
        /// </summary>
        [DataMember]
        TargetType TargetType { get; set; }

        /// <summary>
        /// Gets or sets the activity id used to track the document
        /// </summary>
        [DataMember]
        Guid ActivityID { get; set; }

        /// <summary>
        /// Gets or sets the settings used to handle the document
        /// </summary>
        /// <remarks>This must be overridden as it could differ between types.</remarks>
        [DataMember]
        string Settings { get; set; }
    }
}
