//--------------------------------------------------------------------------
//  <copyright file="DrawingExtensions.cs" company="Microsoft Corporation">
//      Copyright (c) Microsoft Corporation. All rights reserved.
//  </copyright>
//--------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.Runtime
{
    using System;
    using System.Drawing;

    /// <summary>
    /// Extension methods for System.Drawing namespace objects.
    /// </summary>
    internal static class DrawingExtensions
    {
        /// <summary>
        /// Divides a <c>SizeF</c> object by the values of another <c>SizeF</c> object.
        /// </summary>
        /// <param name="numerator">The <c>SizeF</c> object to use as the numerator.</param>
        /// <param name="denominator">The <c>SizeF</c> object to use as the denominator.</param>
        /// <returns>The resultant <c>SizeF</c> object.</returns>
        public static SizeF DivideBy(this SizeF numerator, SizeF denominator)
        {
            if (numerator == null)
            {
                throw new ArgumentNullException(nameof(numerator));
            }

            if (denominator == null)
            {
                throw new ArgumentNullException(nameof(denominator));
            }

            if (denominator.Width.Equals(0.0f) || denominator.Height.Equals(0.0f))
            {
                throw new ArgumentOutOfRangeException(nameof(denominator), Resources.SizeFDivideByZero);
            }

            return new SizeF(numerator.Width / denominator.Width, numerator.Height / denominator.Height);
        }

        /// <summary>
        /// Converts a <c>Size</c> object to a <c>SizeF</c> object.
        /// </summary>
        /// <param name="size">The object to convert.</param>
        /// <returns>The resultant <c>SizeF</c> object.</returns>
        public static SizeF ToSizeF(this Size size)
        {
            return new SizeF((float)size.Width, (float)size.Height);
        }

        /// <summary>
        /// Multiplies the width and height values of the <c>SizeF</c> objects.
        /// </summary>
        /// <param name="size">This <c>SizeF</c> object.</param>
        /// <param name="s">The value to multiply by.</param>
        /// <returns>The resultant <c>SizeF</c> object.</returns>
        public static SizeF MultiplyBy(this SizeF size, float s)
        {
            if (size == null)
            {
                throw new ArgumentNullException(nameof(size));
            }

            if (s == 0)
            {
                throw new ArgumentNullException(nameof(s));
            }

            return new SizeF(size.Width * s, size.Height * s);
        }

        /// <summary>
        /// Determines if the <c>SizeF</c> objects are equal.
        /// </summary>
        /// <param name="size">This <c>SizeF</c> object.</param>
        /// <param name="s">The <c>SizeF</c> object to compare.</param>
        /// <returns>A value indicating whether the <c>SizeF</c> objects are equal.</returns>
        public static bool Equals(this SizeF size, SizeF s)
        {
            if (size == null)
            {
                throw new ArgumentNullException(nameof(size));
            }

            if (s == null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            return size.Width.Equals(s.Width) && size.Height.Equals(s.Height);
        }

        /// <summary>
        /// Determines if the object fits inside the supplied <c>SizeF</c> object.
        /// </summary>
        /// <param name="size">The <c>SizeF</c> object to inspect.</param>
        /// <param name="s">The <c>SizeF</c> object providing the bounding size.</param>
        /// <returns>A value indicating if the object fits inside the supplied <c>SizeF</c> object.</returns>
        public static bool FitsInside(this SizeF size, SizeF s)
        {
            if (size == null)
            {
                throw new ArgumentNullException(nameof(size));
            }

            if (s == null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            if (size.Width > s.Width)
            {
                return false;
            }

            if (size.Height > s.Height)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Swaps the width and height values of the <c>SizeF</c> object.
        /// </summary>
        /// <param name="size">The <c>SizeF</c> object.</param>
        /// <returns>The resultant <c>SizeF</c> object.</returns>
        public static SizeF ToggleOrientation(this SizeF size)
        {
            return new SizeF(size.Height, size.Width);
        }

        /// <summary>
        /// Gets the smallest value between the width and height values.
        /// </summary>
        /// <param name="size">The <c>SizeF</c> object to inspect.</param>
        /// <returns>The smallest value.</returns>
        public static float GetSmallestSide(this SizeF size)
        {
            return Math.Min(size.Width, size.Height);
        }
    }
}
