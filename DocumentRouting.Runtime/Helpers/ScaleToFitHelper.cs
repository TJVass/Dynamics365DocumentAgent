//--------------------------------------------------------------------------
//  <copyright file="ScaleToFitHelper.cs" company="Microsoft Corporation">
//      Copyright (c) Microsoft Corporation. All rights reserved.
//  </copyright>
//--------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.Runtime
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Drawing.Printing;

    /// <summary>
    /// Contains helper methods that enable scale-to-fit printing
    /// of rendered reports (EMF images) to be sent to print.
    /// </summary>
    public static class ScaleToFitHelper
    {
        #region Class Members

        /// <summary>
        /// The desired dots per inch width for printing. 
        /// </summary>
        private static float desiredPrintDpiWidth;

        /// <summary>
        /// The desired dots per inch height for printing. 
        /// </summary>
        private static float desiredPrintDpiHeight;

        #endregion Class Members

        #region Class Properties

        /// <summary>
        /// Gets or sets the desired dots per inch width for printing.
        /// </summary>
        /// <remarks>The default value is 300.0f</remarks>
        public static float DesiredPrintDpiWidth
        {
            get
            {
                if (ScaleToFitHelper.desiredPrintDpiWidth <= Constants.InitializeDouble)
                {
                    ScaleToFitHelper.desiredPrintDpiWidth = Constants.DefaultScaleToFitDpiResolution;
                }

                return ScaleToFitHelper.desiredPrintDpiWidth;
            }

            set
            {
                ScaleToFitHelper.desiredPrintDpiWidth = Math.Max(value, Constants.InitializeDouble);
            }
        }

        /// <summary>
        /// Gets or sets the desired dots per inch height for printing.
        /// </summary>
        /// <remarks>The default value is 300.0f</remarks>
        public static float DesiredPrintDpiHeight
        {
            get
            {
                if (ScaleToFitHelper.desiredPrintDpiHeight <= Constants.InitializeDouble)
                {
                    ScaleToFitHelper.desiredPrintDpiHeight = Constants.DefaultScaleToFitDpiResolution;
                }

                return ScaleToFitHelper.desiredPrintDpiHeight;
            }

            set
            {
                ScaleToFitHelper.desiredPrintDpiHeight = Math.Max(value, Constants.InitializeDouble);
            }
        }

        /// <summary>
        /// Gets a <c>SizeF</c> object containing the desired dots per inch width and height values.
        /// </summary>
        public static SizeF DesiredPrintDpi
        {
            get
            {
                return new SizeF(ScaleToFitHelper.DesiredPrintDpiWidth, ScaleToFitHelper.DesiredPrintDpiHeight);
            }
        }

        #endregion Class Properties

        #region Internal Methods

        /// <summary>
        /// Performs scale-to-fit operations on the <c>Metafile</c> object.
        /// </summary>
        /// <param name="pageSettings">The <c>PageSettings</c> object used for printing.</param>
        /// <param name="graphics">The <c>Graphics</c> object used for printing.</param>
        /// <param name="metafile">The <c>Metafile</c> object that is to be scaled.</param>
        /// <returns>A <c>Point</c> array containing a single value that designates the position where the bottom corner of the metafile image is to be stretched.</returns>
        public static Point[] ScaleToFitMetafileForPrinting(PageSettings pageSettings, Graphics graphics, Metafile metafile)
        {
            if (pageSettings == null)
            {
                throw new ArgumentNullException(nameof(pageSettings));
            }

            if (graphics == null)
            {
                throw new ArgumentNullException(nameof(graphics));
            }

            if (metafile == null)
            {
                throw new ArgumentNullException(nameof(metafile));
            }

            if (pageSettings.PaperSize == null)
            {
                throw new ArgumentNullException("pageSettings.PaperSize");
            }

            if (pageSettings.PaperSize.Height <= 0)
            {
                throw new ArgumentNullException("pageSettings.PaperSize.Height");
            }

            if (pageSettings.PaperSize.Width <= 0)
            {
                throw new ArgumentNullException("pageSettings.PaperSize.Width");
            }

            var points = new[] { new Point(0, 0) };
            try
            {
                if (metafile != null)
                {
                    var metafileHeader = metafile.GetMetafileHeader();

                    // get the sizes of the image and the current printer settings printable area
                    var metafileDpi = new SizeF(metafileHeader.DpiX, metafileHeader.DpiY);
                    var paperSize = new SizeF(pageSettings.PaperSize.Width, pageSettings.PaperSize.Height);

                    // get the factor by which to scale the metafile
                    var dpiFactor = metafileDpi.DivideBy(ScaleToFitHelper.DesiredPrintDpi);

                    // account for the normalization in the graphic object
                    graphics.ScaleTransform(dpiFactor.Width, dpiFactor.Height, MatrixOrder.Prepend);

                    var metafileSizeNormalized = metafile.Size.ToSizeF().DivideBy(ScaleToFitHelper.DesiredPrintDpi).MultiplyBy(Constants.HundredthInchFactor);

                    // By default if no scaling is required, we don't want the scale to affect the values in the matrix.
                    float scaleToFitFactor = 1;

                    // If the printer is set to Landscape then we need to swap the width and height values
                    var metafileSizeOriented = pageSettings.Landscape ? metafileSizeNormalized.ToggleOrientation() : metafileSizeNormalized;

                    // Does the normalized and oriented metafile fit inside the paper size? 
                    // (The metafile should cover over the entire page, including hard margins.)
                    if (!metafileSizeOriented.FitsInside(paperSize))
                    {
                        // must perform scale to fit operation, now update the scale to fit factor
                        scaleToFitFactor = paperSize.DivideBy(metafileSizeOriented).GetSmallestSide();
                    }

                    // we now have all the information we need to scale
                    graphics.ScaleTransform(scaleToFitFactor, scaleToFitFactor, MatrixOrder.Append);

                    // Shift the metafile by hard margin size to align to top left side of paper.
                    int hardMarginOffsetX = (int)pageSettings.HardMarginX * -1;
                    int hardMarginOffsetY = (int)pageSettings.HardMarginY * -1;
                    points = new[] { new Point(hardMarginOffsetX, hardMarginOffsetY) };

                    var matrix = graphics.Transform;
                    matrix.Invert();
                    matrix.TransformPoints(points);
                }
            }
            catch (Exception ex)
            {
               // TODO Error Log
                throw ex;
            }

            return points;
        }

        #endregion Internal Methods
    }
}
