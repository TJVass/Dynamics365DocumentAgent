//------------------------------------------------------------------------------
// <copyright file="Settings.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting
{
    using System;

    /// <summary>
    /// The settings.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Gets or sets the application identifier.
        /// </summary>
        public Guid ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the aad instance URI.
        /// </summary>
        public string AADInstanceUri { get; set; }

        /// <summary>
        /// Gets or sets the tenant.
        /// </summary>
        public string Tenant { get; set; }

        /// <summary>
        /// Gets or sets the ax root URL.
        /// </summary>
        public string AXRootUrl { get; set; }

        /// <summary>
        /// Gets or sets the redirect URI.
        /// </summary>
        public string RedirectUri { get; set; }
    }
}
