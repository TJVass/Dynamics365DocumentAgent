//------------------------------------------------------------------------------
// <copyright file="NotificationHubInfo.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.OData.Entities
{
    /// <summary>
    /// The notification hub info.
    /// </summary>
    public sealed class NotificationHubInfo
    {
        /// <summary>
        /// Gets or sets the notification hub URL.
        /// </summary>
        public string NotificationHubUrl { get; set; }

        /// <summary>
        /// Gets or sets the authorization header.
        /// </summary>
        public string AuthorizationHeader { get; set; }
    }
}
