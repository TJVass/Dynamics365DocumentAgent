//------------------------------------------------------------------------------
// <copyright file="DocumentContractType.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.OData.Entities
{
    /// <summary>
    /// The DocumentContractType type enumeration.
    /// </summary>
    public enum DocumentContractType
    {
        /// <summary>
        /// Metafile, image, type
        /// </summary>
        Metafile,

        /// <summary>
        /// PDF document type
        /// </summary>
        Pdf,

        /// <summary>
        /// Zebra label, text, type
        /// </summary>
        Zebra
    }
}
