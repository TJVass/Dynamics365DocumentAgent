//------------------------------------------------------------------------------
// <copyright file="IAuthorizationManager.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.Runtime
{
    using System.Threading.Tasks;
    using Microsoft.Dynamics.AX.Framework.DocumentRouting.OData;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;

    /// <summary>
    /// The interface for authorization manager.
    /// </summary>
    public interface IAuthorizationManager : IAuthenticationHeaderProvider
    {
        /// <summary>
        /// Gets or sets the prompt behavior.
        /// </summary>
        PromptBehavior PromptBehavior { get; set; }

        /// <summary>
        /// Gets or sets the token cache.
        /// </summary>
        TokenCache TokenCache { get; set; }

        /// <summary>
        /// Gets the access token.
        /// </summary>
        /// <returns>The access token.</returns>
        Task<string> GetAccessToken();

        /// <summary>
        /// Performs sign out operation.
        /// </summary>
        void SignOut();
    }
}