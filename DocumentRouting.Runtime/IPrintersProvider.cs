//------------------------------------------------------------------------------
// <copyright file="IPrintersProvider.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.Runtime
{
    /// <summary>
    /// The interface for printers provider.
    /// </summary>
    public interface IPrintersProvider
    {
        /// <summary>
        /// Gets the installed printers.
        /// </summary>
        /// <returns>The printer paths collection.</returns>
        List<string> GetInstalledPrinters();
    }
}