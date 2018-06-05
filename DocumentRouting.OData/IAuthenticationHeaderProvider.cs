//------------------------------------------------------------------------------
// <copyright file="IAuthenticationHeaderProvider.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.OData
{
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    /// <summary>
    /// The authorization header provider
    /// </summary>
    public interface IAuthenticationHeaderProvider
    {
        /// <summary>
        /// Gets the authentication header.
        /// </summary>
        /// <returns>The instance of <see cref="AuthenticationHeaderValue"/></returns>
        Task<AuthenticationHeaderValue> GetAuthenticationHeader();
    }
}