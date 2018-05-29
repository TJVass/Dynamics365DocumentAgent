//------------------------------------------------------------------------------
// <copyright file="DocumentContract.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.OData.Entities
{

    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The abstract definition of the DocumentContract object.
    /// </summary>
    [DataContract]
    public abstract class DocumentContract : IDocumentContract
    {
        /// <summary>
        /// Gets or sets the document name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the document contents.
        /// </summary>
        [DataMember]
        public byte[] Contents { get; set; }

        /// <summary>
        /// Gets or sets the document target
        /// </summary>
        [DataMember]
        public TargetType TargetType { get; set; }

        /// <summary>
        /// Gets or sets the activity id used to track the document
        /// </summary>
        [DataMember]
        public Guid ActivityID { get; set; }

        /// <summary>
        /// Gets or sets the settings used to handle the document
        /// </summary>
        /// <remarks>This must be overridden as it could differ between types.</remarks>
        [DataMember]
        public virtual string Settings { get; set; }
    }
}
