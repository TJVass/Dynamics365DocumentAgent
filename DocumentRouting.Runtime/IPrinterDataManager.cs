//--------------------------------------------------------------------------
//  <copyright file="IPrinterDataManager.cs"  company="Microsoft Corporation">
//      Copyright (c) Microsoft Corporation. All rights reserved.
//  </copyright>
//--------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.Runtime
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for printer data manager
    /// </summary>
    public interface IPrinterDataManager
    {
        /// <summary>
        /// Gets the printer list.
        /// </summary>
        /// <returns>Async task</returns>
        Task<List<PrinterData>> GetPrinters();

        /// <summary>
        /// Gets the installed printers.
        /// </summary>
        /// <returns>The installed printers collection.</returns>
        List<PrinterData> GetInstalledPrinters();

        /// <summary>
        /// Gets the registered printers.
        /// </summary>
        /// <returns>The registered printers collection.</returns>
        Task<List<PrinterData>> GetRegisteredPrinters();

        /// <summary>
        /// Updates the registered printers.
        /// </summary>
        /// <param name="printers">The printers.</param>
        /// <returns>The task</returns>
        Task UpdateRegisteredPrinters(IEnumerable<PrinterData> printers);
    }
}