//------------------------------------------------------------------------------
// <copyright file="HttpResponseMessageExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.OData
{
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for HttpResponseMessage
    /// </summary>
    internal static class HttpResponseMessageExtensions
    {
        /// <summary>
        /// Ensure HttpResponse returns a successful success code, otherwise throw an exception with details
        /// </summary>
        /// <param name="response">The HttpResponse object</param>
        /// <returns>An async task</returns>
        public static async Task EnsureSuccessAsync(this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var details = string.Empty;
                if (response.Content != null)
                {
                    details = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }

                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException ex)
                {
                    throw new HttpRequestException(ex.Message, new DetailHttpResponseException(details));
                }
            }
        }
    }
}
