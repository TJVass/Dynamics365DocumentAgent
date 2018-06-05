//------------------------------------------------------------------------------
// <copyright file="ReturnValue.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.OData.Entities
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// A class for JSON deserialization of OData return value
    /// </summary>
    /// <typeparam name="T">The value type</typeparam>
    internal class ReturnValue<T>
    {
        /// <summary>
        /// Gets or sets the value property
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Deserialization required.")]
        public T value { get; set; }
    }
}
