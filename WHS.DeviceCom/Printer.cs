//------------------------------------------------------------------------------
// <copyright file="Printer.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.WHS.DeviceCom
{
    using Microsoft.Dynamics.AX.WHS.DeviceCom.Device.Printer;

    /// <summary>
    /// An abstract implementation of printing via Microsoft Dynamics 365 for Finance and Operations.
    /// </summary>
    public class Printer
    {
        /// <summary>
        /// Sends a string to a printer.
        /// </summary>
        /// <param name="printerName">
        /// The printer which should print the provided string.
        /// </param>
        /// <param name="value">
        /// The string to be printed out.
        /// </param>
        /// <returns>
        /// An empty string if the operation was successful; otherwise an error message.
        /// </returns>
        public static string SendStringToPrinter(string printerName, string value)
        {
            return WinSpoolPrinter.SendStringToPrinter(printerName, value);
        }
    }
}
