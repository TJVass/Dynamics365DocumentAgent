//------------------------------------------------------------------------------
// <copyright file="IRestClient.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.OData
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// An interface for RestClient
    /// </summary>
    internal interface IRestClient : IDisposable
    {
        /// <summary>
        /// Send a HTTP Get request
        /// </summary>
        /// <typeparam name="T">The result object type</typeparam>
        /// <param name="uri">The absolute URI of the get request</param>
        /// <param name="activityId">activity Id</param>
        /// <returns>The get request result</returns>
        Task<T> GetRequestAsync<T>(Uri uri, string activityId = null);

        /// <summary>
        /// Send a HTTP POST request with return value and zero content
        /// </summary>
        /// <typeparam name="T">The post object type</typeparam>
        /// <param name="uri">The absolute URI of the post request</param>
        /// <param name="activityId">activity Id</param>
        /// <returns>The async task for the post request</returns>
        Task<T> PostRequestAsync<T>(Uri uri, string activityId = null);

        /// <summary>
        /// Send a HTTP POST request without return value
        /// </summary>
        /// <typeparam name="T">The post object type</typeparam>
        /// <param name="uri">The absolute URI of the post request</param>
        /// <param name="value">The post object</param>
        /// <param name="activityId">activity Id</param>
        /// <returns>The async task for the post request</returns>
        Task PostRequestAsync<T>(Uri uri, T value, string activityId = null);

        /// <summary>
        /// Send a HTTP POST request with return value and content
        /// </summary>
        /// <typeparam name="TValue">The post object type</typeparam>
        /// <typeparam name="TResult">The result object type</typeparam>
        /// <param name="uri">The absolute URI of the post request</param>
        /// <param name="value">The post object</param>
        /// <param name="activityId">activity Id</param>
        /// <returns>The post request result</returns>
        Task<TResult> PostRequestAsync<TValue, TResult>(Uri uri, TValue value, string activityId = null);

        /// <summary>
        /// Send a HTTP PUT request to update a record
        /// </summary>
        /// <typeparam name="TValue">The put object type</typeparam>
        /// <param name="uri">The absolute URI of the put request</param>
        /// <param name="value">The put object</param>
        /// <param name="activityId">activity Id</param>
        /// <returns>The async task</returns>
        Task PutRequestAsync<TValue>(Uri uri, TValue value, string activityId = null);
    }
}
