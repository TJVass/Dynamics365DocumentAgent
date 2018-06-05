//------------------------------------------------------------------------------
// <copyright file="DocumentRoutingODataClientFactory.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.OData
{
    using System;

    /// <summary>
    /// The factory for <see cref="IDocumentRoutingODataClient"/> instances.
    /// </summary>
    public class DocumentRoutingODataClientFactory : IDocumentRoutingODataClientFactory
    {
        /// <summary>
        /// Format string for the endpoint
        /// </summary>
        private const string ODataEndPointString = "/data/";

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentRoutingODataClientFactory"/> class.
        /// </summary>
        /// <param name="baseUrl">The base URL.</param>
        /// <param name="authorizationProvider">The authorization provider.</param>
        public DocumentRoutingODataClientFactory(Uri baseUrl, IAuthenticationHeaderProvider authorizationProvider)
        {
            this.BaseUrl = baseUrl.ThrowIfNull(nameof(baseUrl));
            this.AuthorizationProvider = authorizationProvider.ThrowIfNull(nameof(authorizationProvider));

            this.BaseUrl = new Uri(this.BaseUrl, DocumentRoutingODataClientFactory.ODataEndPointString);
        }

        /// <summary>
        /// Gets the base URL.
        /// </summary>
        public Uri BaseUrl { get; }

        /// <summary>
        /// Gets the authorization provider.
        /// </summary>
        public IAuthenticationHeaderProvider AuthorizationProvider { get; }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>The new instance of <see cref="IDocumentRoutingODataClient"/></returns>
        public IDocumentRoutingODataClient Create()
        {
            var restClient = new RestClient(this.AuthorizationProvider);
            return new DocumentRoutingODataClient(this.BaseUrl, restClient);
        }
    }
}
