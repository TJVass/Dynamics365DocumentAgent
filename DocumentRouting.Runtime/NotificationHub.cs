//------------------------------------------------------------------------------
// <copyright file="NotificationHub.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.Runtime
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.AX.Framework.DocumentRouting.OData;

    /// <summary>
    /// The notification hub
    /// </summary>
    public class NotificationHub : INotificationHub
    {
        /// <summary>
        /// The notification handler
        /// </summary>
        private readonly INotificationHandler notificationHandler;

        /// <summary>
        /// The document routing o data client factory
        /// </summary>
        private readonly IDocumentRoutingODataClientFactory documentRoutingODataClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationHub"/> class.
        /// </summary>
        /// <param name="notificationHandler">The notification handler.</param>
        /// <param name="documentRoutingODataClientFactory">The document routing o data client factory.</param>
        public NotificationHub(INotificationHandler notificationHandler, IDocumentRoutingODataClientFactory documentRoutingODataClientFactory)
        {
            this.notificationHandler = notificationHandler ?? throw new ArgumentNullException(nameof(notificationHandler));
            this.documentRoutingODataClientFactory = documentRoutingODataClientFactory ?? throw new ArgumentNullException(nameof(documentRoutingODataClientFactory));
        }

        /// <summary>
        /// Subscribes to events.
        /// </summary>
        public async Task SubscribeToEvents()
        {
            using (var client = this.documentRoutingODataClientFactory.Create())
            {
                await client.SubscribeToRoutingEvents(new OData.Entities.NotificationsRegistration());
            }
        }

        /// <summary>
        /// Unsubscribes from events.
        /// </summary>
        public Task UnsubscribeFromEvents()
        {
            return Task.CompletedTask;
        }
    }
}
