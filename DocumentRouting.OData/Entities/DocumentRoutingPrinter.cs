//------------------------------------------------------------------------------
// <copyright file="DocumentRoutingPrinter.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.OData.Entities
{

    /// <summary>
    /// Document routing printer entity class
    /// </summary>
    public sealed class DocumentRoutingPrinter
    {
        /// <summary>
        /// Gets or sets printer name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets printer description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the printer path
        /// </summary>
        public string Path { get; set; }
    }
}
