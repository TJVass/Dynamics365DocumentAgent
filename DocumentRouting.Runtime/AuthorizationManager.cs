//--------------------------------------------------------------------------
//  <copyright file="AuthenticationHelper.cs"  company="Microsoft Corporation">
//      Copyright (c) Microsoft Corporation. All rights reserved.
//  </copyright>
//--------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.Runtime
{
    using System;
    using System.Net.Http.Headers;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;

    /// <summary>
    /// The authorization manager
    /// </summary>
    public class AuthorizationManager : IAuthorizationManager
    {
        /// <summary>
        /// The settings manager
        /// </summary>
        private readonly ISettingsManager settingsManager;

        /// <summary>
        /// The authentication result
        /// </summary>
        private AuthenticationResult authenticationResult;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationManager"/> class.
        /// </summary>
        /// <param name="settingsManager">The settings manager.</param>
        public AuthorizationManager(ISettingsManager settingsManager)
        {
            this.settingsManager = settingsManager ?? throw new ArgumentNullException(nameof(settingsManager));
        }

        /// <summary>
        /// Gets or sets a value indicating prompt behavior authentication. The default value is Auto.
        /// </summary>
        public PromptBehavior PromptBehavior { get; set; } = PromptBehavior.Auto;

        /// <summary>
        /// Gets or sets the token cache.
        /// </summary>
        public TokenCache TokenCache { get; set; } = new FileCache();

        /// <summary>
        /// Gets the access token.
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetAccessToken()
        {
            var authenticationResult = await this.GetAuthenticationResult();
            return authenticationResult?.AccessToken;
        }

        /// <summary>
        /// Gets the authentication header.
        /// </summary>
        /// <returns>
        /// The instance of <see cref="T:System.Net.Http.Headers.AuthenticationHeaderValue" />
        /// </returns>
        public async Task<AuthenticationHeaderValue> GetAuthenticationHeader()
        {
            var authenticationResult = await this.GetAuthenticationResult();
            return AuthenticationHeaderValue.Parse(authenticationResult?.CreateAuthorizationHeader());
        }

        /// <summary>
        /// Signs out, clears the token and cookie cache.
        /// </summary>
        public void SignOut()
        {
            this.TokenCache.Clear();
            this.authenticationResult = null;

            // Clear cookies from the browser control.
            this.ClearCookies();
        }

        /// <summary>
        /// Gets the authentication context.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns>The  authentication context.</returns>
        private AuthenticationContext GetAuthContext(Settings settings)
        {
            var uriBuilder = new UriBuilder(settings.AADInstanceUri)
            {
                Path = settings.Tenant
            };

            var authorityUri = uriBuilder.ToString();
            return new AuthenticationContext(authorityUri, this.TokenCache);
        }

        /// <summary>
        /// Gets the authentication result.
        /// </summary>
        /// <returns>The authentication result.</returns>
        private async Task<AuthenticationResult> GetAuthenticationResult()
        {
            // If the current auth result is up to date return it
            if (this.IsAuthenticationResultCurrent(this.authenticationResult))
            {
                return this.authenticationResult;
            }

            this.authenticationResult = await this.AcquireNewAuthenticationResult();
            return this.authenticationResult;
        }

        /// <summary>
        /// Acquires the new authentication result.
        /// </summary>
        /// <returns></returns>
        private async Task<AuthenticationResult> AcquireNewAuthenticationResult()
        {
            // Validate settings
            var settings = this.settingsManager.GetSettings();
            var resourceId = settings.AXRootUrl;
            var clientId = settings.ApplicationId.ToString();
            var redirectUri = new Uri(settings.RedirectUri, UriKind.Absolute);
            if (string.IsNullOrWhiteSpace(resourceId))
            {
                throw new ArgumentNullException(nameof(settings.AXRootUrl));
            }

            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new ArgumentNullException(nameof(settings.ApplicationId));
            }

            var authContext = this.GetAuthContext(settings);
            AuthenticationResult result = null;

            try
            {
                result = await authContext.AcquireTokenSilentAsync(resourceId, clientId).ConfigureAwait(false);
            }
            catch (AdalException adalException)
            {
                if (adalException.ErrorCode == AdalError.FailedToAcquireTokenSilently
                    || adalException.ErrorCode == AdalError.InteractionRequired)
                {
                    result = await authContext.AcquireTokenAsync(
                        resourceId,
                        clientId,
                        redirectUri,
                        new PlatformParameters(this.PromptBehavior))
                        .ConfigureAwait(false); ;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets a value indicating whether the authentication result is current.
        /// </summary>
        /// <param name="result">The <c>AuthenticationResult</c> object to inspect.</param>
        /// <returns>True if current; otherwise false.</returns>
        private bool IsAuthenticationResultCurrent(AuthenticationResult result)
        {
            if (result == null ||
                string.IsNullOrEmpty(result.AccessToken) ||
                result.ExpiresOn == null)
            {
                return false;
            }

            // Compute one minute from now to give a slight time buffer
            var oneMinuteFromNow = DateTimeOffset.UtcNow.AddMinutes(1.0);

            // Compare with the expires on value
            var isCurrent = oneMinuteFromNow < result.ExpiresOn;
            return isCurrent;
        }

        /// <summary>
        /// Native assembly method import
        /// </summary>
        /// <param name="intPointer">Handle on which to set information</param>
        /// <param name="dwordOption">Internet option to be set</param>
        /// <param name="longPointerBuffer">Pointer to a buffer that contains the option setting</param>
        /// <param name="longPointerDWordBufferLength">Size of the buffer</param>
        /// <returns>Returns TRUE if successful, or FALSE otherwise</returns>
        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetSetOption(IntPtr intPointer, int dwordOption, IntPtr longPointerBuffer, int longPointerDWordBufferLength);

        /// <summary>
        /// Clears cookies from the browser control used by ADAL.
        /// </summary>
        private void ClearCookies()
        {
            const int INTERNET_OPTION_END_BROWSER_SESSION = 42;
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_END_BROWSER_SESSION, IntPtr.Zero, 0);
        }
    }
}
