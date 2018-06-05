//------------------------------------------------------------------------------
// <copyright file="NotificationsRegistration.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.OData.Entities
{
    /// <summary>
    /// The notification registration info.
    /// </summary>
    public sealed class NotificationsRegistration
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
