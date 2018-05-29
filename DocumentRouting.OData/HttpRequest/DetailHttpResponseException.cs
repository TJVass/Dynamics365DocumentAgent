//------------------------------------------------------------------------------
// <copyright file="DetailHttpResponseException.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.OData
{
    using System;

    /// <summary>
    /// A custom exception to log the HttpResponse detailed error.
    /// </summary>
    public class DetailHttpResponseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DetailHttpResponseException" /> class
        /// </summary>
        /// <param name="detail">Exception detail</param>
        public DetailHttpResponseException(string detail) : base(detail)
        {
        }
    }
}
