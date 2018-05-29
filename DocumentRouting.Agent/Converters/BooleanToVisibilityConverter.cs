//--------------------------------------------------------------------------
//  <copyright file="BooleanToVisibilityConverter.cs" company="Microsoft Corporation">
//      Copyright (c) Microsoft Corporation. All rights reserved.
//  </copyright>
//--------------------------------------------------------------------------
namespace Microsoft.Dynamics.AX.Framework.DocumentRouting
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Boolean to Visibility Enumeration converter
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets the true value.
        /// </summary>
        public Visibility TrueValue { get; set; } = Visibility.Visible;

        /// <summary>
        /// Gets or sets the false value.
        /// </summary>
        public Visibility FalseValue { get; set; } = Visibility.Collapsed;

        /// <summary>
        /// Converts a Boolean Value to a Visibility Enumeration Value
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolean = value as bool?;
            if (boolean == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return boolean.Value ? this.TrueValue : this.FalseValue;
        }

        /// <summary>
        /// Converts a Visibility Enumeration Value to a Boolean Value
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visibility = value as Visibility?;
            if (visibility == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (visibility.Equals(this.TrueValue))
            {
                return true;
            }

            if (visibility.Equals(this.FalseValue))
            {
                return false;
            }

            throw new ArgumentException("Invalid visibility value", nameof(value));
        }
    }
}
