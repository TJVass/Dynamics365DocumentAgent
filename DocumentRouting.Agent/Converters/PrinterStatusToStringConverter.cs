//------------------------------------------------------------------------------
// <copyright file="PrinterStatusToStringConverter.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using Microsoft.Dynamics.AX.Framework.DocumentRouting.Runtime;

    /// <summary>
    /// PrinterStatus (Installed/Uninstalled) Value to "Yes" or "No" converter
    /// </summary>
    [ValueConversion(typeof(PrinterStatus), typeof(string))]
    public class PrinterStatusToStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns <see langword="null" />, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var printerStatus = value as PrinterStatus?;
            if (printerStatus == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return printerStatus == PrinterStatus.Installed ? StringResources.YesLabel : StringResources.NoLabel;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns <see langword="null" />, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
