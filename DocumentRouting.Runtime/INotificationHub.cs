//------------------------------------------------------------------------------
// <copyright file="INotificationHub.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.Runtime
{
    using System.Threading.Tasks;

    /// <summary>
    /// The interface for notification hub.
    /// </summary>
    public interface INotificationHub
    {
        /// <summary>
        /// Subscribes to events.
        /// </summary>
        /// <returns> The task.</returns>
        Task SubscribeToEvents();

        /// <summary>
        /// Unsubscribes from events.
        /// </summary>
        /// <returns> The task.</returns>
        Task UnsubscribeFromEvents();
    }
}