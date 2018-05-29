//------------------------------------------------------------------------------
// <copyright file="IDocumentRoutingODataClientFactory.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.OData
{
    using System;

    /// <summary>
    /// The interface for OData client factory.
    /// </summary>
    public interface IDocumentRoutingODataClientFactory
    {
        /// <summary>
        /// Gets the authorization provider.
        /// </summary>
        IAuthenticationHeaderProvider AuthorizationProvider { get; }

        /// <summary>
        /// Gets the base URL.
        /// </summary>
        Uri BaseUrl { get; }

        /// <summary>
        /// Creates the instance of OData client.
        /// </summary>
        /// <returns>The instance of OData client.</returns>
        IDocumentRoutingODataClient Create();
    }
}