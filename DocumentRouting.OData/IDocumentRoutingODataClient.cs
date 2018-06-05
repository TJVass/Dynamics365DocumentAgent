//------------------------------------------------------------------------------
// <copyright file="IDocumentRoutingODataClient.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.OData
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.AX.Framework.DocumentRouting.OData.Entities;

    /// <summary>
    /// Interface for OData client
    /// </summary>
    public interface IDocumentRoutingODataClient : IDisposable
    {
        /// <summary>
        /// Gets the registered printers.
        /// </summary>
        /// <returns>The collection of registered printers.</returns>
        Task<List<DocumentRoutingPrinter>> GetRegisteredPrinters();

        /// <summary>
        /// Registers the printers.
        /// </summary>
        /// <param name="printers">The printers.</param>
        /// <returns>The task.</returns>
        Task RegisterPrinters(IEnumerable<DocumentRoutingPrinter> printers);

        /// <summary>
        /// Unregisters the printers.
        /// </summary>
        /// <param name="printers">The printers.</param>
        /// <returns>The task.</returns>
        Task UnregisterPrinters(IEnumerable<DocumentRoutingPrinter> printers);

        /// <summary>
        /// Subscribes to routing events.
        /// </summary>
        /// <param name="notificationHubInfo">The notification hub information.</param>
        /// <returns>The task.<returns>
        Task SubscribeToRoutingEvents(NotificationsRegistration notificationHubInfo);

        /// <summary>
        /// Gets the download document URI.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <returns>The Uri to download document.</returns>
        Task<Uri> GetDownloadDocumentUri(string jobId);

    }
}
