//------------------------------------------------------------------------------
// <copyright file="DocumentRoutingODataClient.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.OData
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using System.Web;
    using Microsoft.Dynamics.AX.Framework.DocumentRouting.OData.Entities;

    /// <summary>
    /// The implementation of <see cref="IDocumentRoutingODataClient"/>.
    /// </summary>
    internal class DocumentRoutingODataClient : IDocumentRoutingODataClient
    {
        /// <summary>
        /// The base URL
        /// </summary>
        private readonly Uri baseUrl;

        /// <summary>
        /// The rest client
        /// </summary>
        private readonly IRestClient restClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentRoutingODataClient"/> class.
        /// </summary>
        /// <param name="baseUrl">The base URL.</param>
        /// <param name="restClient">The rest client.</param>
        public DocumentRoutingODataClient(Uri baseUrl, IRestClient restClient)
        {
            this.baseUrl = baseUrl.ThrowIfNull(nameof(baseUrl));
            this.restClient = restClient.ThrowIfNull(nameof(restClient));
        }

        /// <summary>
        /// Gets the registered printers.
        /// </summary>
        /// <returns>
        /// The collection of registered printers.
        /// </returns>
        public async Task<List<DocumentRoutingPrinter>> GetRegisteredPrinters()
        {
            var requestUri = new Uri(this.baseUrl, "DocumentRoutingThirdPartyApps/Microsoft.Dynamics.DataEntities.GetRegisteredPrinters");
            var resultValue = await this.restClient.GetRequestAsync<ReturnValue<List<DocumentRoutingPrinter>>>(requestUri);
            return resultValue.value;
        }

        /// <summary>
        /// Registers the printers.
        /// </summary>
        /// <param name="printers">The printers.</param>
        /// <returns>
        /// The task.
        /// </returns>
        public async Task RegisterPrinters(IEnumerable<DocumentRoutingPrinter> printers)
        {
            printers.ThrowIfNull(nameof(printers));
            foreach (var printer in printers)
            {
                printer.Name.ThrowIfNullOrWhiteSpace("printer.Name");
                printer.Path.ThrowIfNullOrWhiteSpace("printer.Path");
            }

            var requestUri = new Uri(this.baseUrl, "DocumentRoutingThirdPartyApps/Microsoft.Dynamics.DataEntities.RegisterPrinters");
            await this.restClient.PostRequestAsync(requestUri, printers);
        }

        /// <summary>
        /// Unregisters the printers.
        /// </summary>
        /// <param name="printers">The printers.</param>
        /// <returns>
        /// The task.
        /// </returns>
        public async Task UnregisterPrinters(IEnumerable<DocumentRoutingPrinter> printers)
        {
            printers.ThrowIfNull(nameof(printers));
            foreach (var printer in printers)
            {
                printer.Name.ThrowIfNullOrWhiteSpace("printer.Name");
                printer.Path.ThrowIfNullOrWhiteSpace("printer.Path");
            }

            var requestUri = new Uri(this.baseUrl, "DocumentRoutingThirdPartyApps/Microsoft.Dynamics.DataEntities.UnregisterPrinters");
            await this.restClient.PostRequestAsync(requestUri, printers);
        }

        /// <summary>
        /// Subscribes to routing events.
        /// </summary>
        /// <param name="notificationHubUrl">The notification hub URL.</param>
        /// <param name="authorizationInfo">The authorization information.</param>
        /// <returns> The task. </returns>
        public async Task SubscribeToRoutingEvents(NotificationsRegistration notificationHubInfo)
        {
            notificationHubInfo.ThrowIfNull(nameof(notificationHubInfo));

            var requestUri = new Uri(this.baseUrl, "DocumentRoutingThirdPartyApps/Microsoft.Dynamics.DataEntities.SubscribeToRoutingEvents");
            await this.restClient.PostRequestAsync(requestUri, notificationHubInfo);
        }

        /// <summary>
        /// Gets the download document URI.
        /// </summary>
        /// <param name="jobId">The job identifier.</param>
        /// <returns>
        /// The Uri to download document.
        /// </returns>
        public async Task<Uri> GetDownloadDocumentUri(string jobId)
        {
            jobId.ThrowIfNullOrWhiteSpace(nameof(jobId));

            var queryCondition = HttpUtility.UrlEncode(string.Format(CultureInfo.InvariantCulture, "DocumentRoutingThirdPartyApps/Microsoft.Dynamics.DataEntities.GetDownloadDocumentUri('{0}')", jobId));
            var requestUri = new Uri(this.baseUrl, queryCondition);

            var resultValue = await this.restClient.GetRequestAsync<ReturnValue<string>>(requestUri);
            return new Uri(resultValue.value);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.restClient != null)
            {
                this.restClient.Dispose();
            }
        }
    }
}
