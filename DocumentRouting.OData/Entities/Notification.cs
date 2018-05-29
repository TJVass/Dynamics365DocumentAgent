//------------------------------------------------------------------------------
// <copyright file="Notification.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.OData.Entities
{
    /// <summary>
    /// The notification.
    /// </summary>
    public sealed class Notification
    {
        /// <summary>
        /// Gets or sets the job identifier.
        /// </summary>
        public string JobId { get; set; }

        /// <summary>
        /// Gets or sets the type of the document contract.
        /// </summary>
        public DocumentContractType DocumentContractType { get; set; }

        /// <summary>
        /// Gets or sets the type of the target.
        /// </summary>
        public TargetType TargetType { get; set; }
    }
}
