//------------------------------------------------------------------------------
// <copyright file="PrinterStatus.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.Runtime
{
    /// <summary>
    /// Printer status
    /// </summary>
    public enum PrinterStatus
    {
        /// <summary>
        /// Unknown status
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Installed status
        /// </summary>
        Installed = 1,

        /// <summary>
        /// Uninstalled status
        /// </summary>
        UnInstalled = 2
    }
}
