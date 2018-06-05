//------------------------------------------------------------------------------
// <copyright file="WinSpoolPrinter.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.WHS.DeviceCom.Device.Printer
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    class WinSpoolPrinter
    {
        // Structure and API declarions:
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pDocName;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pDataType;
        }
        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        static extern bool OpenPrinter(string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

        /// <summary>
        /// Sends bytes to the print queue.
        /// </summary>
        /// <param name="szPrinterName">
        /// The printer name.
        /// </param>
        /// <param name="pBytes">
        /// An unmanaged array of bytes to print.
        /// </param>
        /// <param name="dwCount">
        /// The size of the array to print.
        /// </param>
        /// <returns>
        /// An empty string if the operation was successful; otherwise an error message.
        /// </returns>
        static string SendBytesToPrinter(string szPrinterName, IntPtr pBytes, Int32 dwCount)
        {
            Int32 dwWritten = 0;
            IntPtr hPrinter = new IntPtr(0);
            DOCINFOA di = new DOCINFOA();
            bool bSuccess = false; // Assume failure unless you specifically succeed.
            bool bOpenPrinterOK = false;
            string errorMessage = "";

            di.pDocName = "Label";
            di.pDataType = "RAW";

            try
            {
                bOpenPrinterOK = OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero);
                if (bOpenPrinterOK)
                {
                    if (StartDocPrinter(hPrinter, 1, di))
                    {
                        if (StartPagePrinter(hPrinter))
                        {
                            if (WritePrinter(hPrinter, pBytes, dwCount, out dwWritten))
                            {
                                EndPagePrinter(hPrinter);
                                bSuccess = true;
                            }
                        }
                    }
                }
            }
            finally
            {
                // If we did not succeed in printing, GetLastError may give more information about why not.
                if (!bSuccess)
                {
                    errorMessage = new Win32Exception(Marshal.GetLastWin32Error()).Message;
                }

                if (bOpenPrinterOK)
                {
                    bool bPrinterClosedOK = ClosePrinter(hPrinter);

                    //if we only have a failure from closing the printer we get that, otherwise we want to keep the previous error and not potentially overwrite it with the value from the printers call
                    if (bSuccess && !bPrinterClosedOK)
                    {
                        errorMessage = new Win32Exception(Marshal.GetLastWin32Error()).Message;
                    }
                }
            }

            return errorMessage;
        }

        /// <summary>
        /// Sends a string to printer.
        /// </summary>
        /// <param name="szPrinterName">
        /// The printer name.
        /// </param>
        /// <param name="szString">
        /// A string to print.
        /// </param>
        /// <returns>
        /// An empty string if the operation was successful; otherwise an error message.
        /// </returns>
        public static string SendStringToPrinter(string szPrinterName, string szString)
        {
            IntPtr pBytes = new IntPtr(0);
            Int32 dwCount;
            string errorMessage = "";

            try
            {
                dwCount = szString.Length;
                // allocate memory for the string
                pBytes = Marshal.StringToCoTaskMemAnsi(szString);
                errorMessage = SendBytesToPrinter(szPrinterName, pBytes, dwCount);
            }
            finally
            {
                // if we managed to allocate memory then we need to free it up
                if (pBytes != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(pBytes);
                }
            }

            return errorMessage;
        }
    }
}
