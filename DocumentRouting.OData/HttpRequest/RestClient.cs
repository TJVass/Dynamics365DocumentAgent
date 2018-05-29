//------------------------------------------------------------------------------
// <copyright file="RestClient.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.OData
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    /// <summary>
    /// Client to call OData service
    /// </summary>
    internal class RestClient : IRestClient, IDisposable
    {
        /// <summary>
        /// header for activity Id
        /// </summary>
        private const string ActivityIdHeader = "x-ms-dyn-externalidentifier";

        /// <summary>
        /// The authorization header provider.
        /// </summary>
        private IAuthenticationHeaderProvider authorizationHeaderProvider;

        /// <summary>
        /// Http client handler
        /// </summary>
        private readonly HttpClientHandler webClientHandler;

        /// <summary>
        /// Http web client
        /// </summary>
        private readonly HttpClient webClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="RestClient" /> class
        /// </summary>
        /// <param name="authorizationHeaderProvider">The authorization header provider.</param>
        public RestClient(IAuthenticationHeaderProvider authorizationHeaderProvider)
        {
            this.authorizationHeaderProvider = authorizationHeaderProvider.ThrowIfNull(nameof(authorizationHeaderProvider));
            this.webClientHandler = new HttpClientHandler() { UseCookies = false };
            this.webClient = new HttpClient(this.webClientHandler);

            this.webClient.DefaultRequestHeaders.Accept.Clear();
            this.webClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Send a HTTP Get request
        /// </summary>
        /// <typeparam name="T">The result object type</typeparam>
        /// <param name="uri">The absolute URI of the get request</param>
        /// <param name="activityId">activity Id</param>
        /// <returns>The get request result</returns>
        public async Task<T> GetRequestAsync<T>(Uri uri, string activityId = null)
        {
            uri.ThrowIfNull(nameof(uri));

            await this.InitializeWebClient(activityId);

            var responseMessage = await this.webClient.GetAsync(uri).ConfigureAwait(false);
            await responseMessage.EnsureSuccessAsync();

            T result = await responseMessage.Content.ReadAsAsync<T>().ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Send a HTTP POST request without return value
        /// </summary>
        /// <typeparam name="T">The post object type</typeparam>
        /// <param name="uri">The absolute URI of the post request</param>
        /// <param name="value">The post object</param>
        /// <param name="activityId">activity Id</param>
        /// <returns>The async task for the post request</returns>
        public async Task PostRequestAsync<T>(Uri uri, T value, string activityId = null)
        {
            uri.ThrowIfNull(nameof(uri));
            value.ThrowIfNull(nameof(value));

            await this.InitializeWebClient(activityId);

            var response = await this.webClient.PostAsJsonAsync(uri.AbsoluteUri, value).ConfigureAwait(false);
            await response.EnsureSuccessAsync();
        }

        /// <summary>
        /// Send a HTTP POST request with return value and zero content
        /// </summary>
        /// <typeparam name="T">The post object type</typeparam>
        /// <param name="uri">The absolute URI of the post request</param>
        /// <param name="activityId">activity Id</param>
        /// <returns>The async task for the post request</returns>
        public async Task<T> PostRequestAsync<T>(Uri uri, string activityId = null)
        {
            uri.ThrowIfNull(nameof(uri));

            await this.InitializeWebClient(activityId);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
            HttpResponseMessage response = await this.webClient.SendAsync(request).ConfigureAwait(false);
            await response.EnsureSuccessAsync();

            T result = await response.Content.ReadAsAsync<T>().ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Send a HTTP POST request with return value and content
        /// </summary>
        /// <typeparam name="TValue">The post object type</typeparam>
        /// <typeparam name="TResult">The result object type</typeparam>
        /// <param name="uri">The absolute URI of the post request</param>
        /// <param name="value">The post object</param>
        /// <param name="activityId">activity Id</param>
        /// <returns>The post request result</returns>
        public async Task<TResult> PostRequestAsync<TValue, TResult>(Uri uri, TValue value, string activityId = null)
        {
            uri.ThrowIfNull(nameof(uri));
            value.ThrowIfNull(nameof(value));

            await this.InitializeWebClient(activityId);

            HttpResponseMessage response = await this.webClient.PostAsJsonAsync(uri.AbsoluteUri, value).ConfigureAwait(false);
            await response.EnsureSuccessAsync();

            var result = await response.Content.ReadAsAsync<TResult>().ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Send a HTTP PUT request to update a record
        /// </summary>
        /// <typeparam name="TValue">The put object type</typeparam>
        /// <param name="uri">The absolute URI of the put request</param>
        /// <param name="value">The put object</param>
        /// <param name="activityId">activity Id</param>
        /// <returns>The async task</returns>
        public async Task PutRequestAsync<TValue>(Uri uri, TValue value, string activityId = null)
        {
            uri.ThrowIfNull(nameof(uri));
            value.ThrowIfNull(nameof(value));

            await this.InitializeWebClient(activityId);

            var response = await this.webClient.PutAsJsonAsync(uri.AbsoluteUri, value).ConfigureAwait(false);
            await response.EnsureSuccessAsync();
        }

        /// <summary>
        /// Dispose method
        /// </summary>
        public void Dispose()
        {
            if (this.webClientHandler != null)
            {
                this.webClientHandler.Dispose();
            }

            if (this.webClient != null)
            {
                this.webClient.Dispose();
            }
        }

        /// <summary>
        /// Initialize web client
        /// </summary>
        /// <param name="activityId">activity Id</param>
        private async Task InitializeWebClient(string activityId)
        {
            this.webClient.DefaultRequestHeaders.Authorization = await this.authorizationHeaderProvider.GetAuthenticationHeader();

            if (!string.IsNullOrEmpty(activityId))
            {
                this.webClient.DefaultRequestHeaders.Remove(ActivityIdHeader);
                this.webClient.DefaultRequestHeaders.Add(ActivityIdHeader, activityId);
            }
        }
    }
}