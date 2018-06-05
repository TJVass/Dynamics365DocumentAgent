//------------------------------------------------------------------------------
// <copyright file="INotificationHandler.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.Runtime
{
    using System.Threading.Tasks;
    using Microsoft.Dynamics.AX.Framework.DocumentRouting.OData.Entities;

    /// <summary>
    /// The interface for notification handler.
    /// </summary>
    public interface INotificationHandler
    {
        /// <summary>
        /// Handles the notification.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <returns>The task.</returns>
        Task HandleNotification(Notification notification);
    }
}