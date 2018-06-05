//------------------------------------------------------------------------------
// <copyright file="ValidationExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.OData
{
    using System;

    /// <summary>
    /// Extensions for common validations
    /// </summary>
    internal static class ValidationExtensions
    {
        /// <summary>
        /// Throws if null.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns>The object if it's not null, otherwise throws exception.</returns>
        public static T ThrowIfNull<T>(this T obj, string parameterName = "")
        {
            if (obj == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            return obj;
        }

        /// <summary>
        /// Throws if null, empty, or consists only of white-space characters.
        /// </summary>
        /// <param name="obj">The string object.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns>The string object if it's not null, empty, or consists only of white-space characters, otherwise throws exception.</returns>
        public static string ThrowIfNullOrWhiteSpace(this string obj, string parameterName = "")
        {
            if (string.IsNullOrWhiteSpace(obj))
            {
                throw new ArgumentNullException(parameterName);
            }

            return obj;
        }
    }
}
