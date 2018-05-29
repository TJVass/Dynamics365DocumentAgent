//--------------------------------------------------------------------------
//  <copyright file="MetafileHelper.cs"  company="Microsoft Corporation">
//      Copyright (c) Microsoft Corporation. All rights reserved.
//  </copyright>
//--------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;

    /// <summary>
    /// Contains helper methods for <c>Metafile</c> objects.
    /// </summary>
    internal static class MetafileHelper
    {
        /// <summary>
        /// Gets the image from the downloaded stream
        /// </summary>
        /// <param name="pageSizesList">List of page sizes</param>
        /// <param name="allPages">Report that needs to be printed</param>
        /// <returns>Returns the images</returns>
        public static List<Metafile> GetEmfImagesToPePrinted(List<int> pageSizesList, byte[] allPages)
        {
            List<Metafile> emfImages = null;
            Debug.Assert(pageSizesList != null && pageSizesList.Count > 0, "Parameter 'pageSizesList' must not be null or empty.");

            // everything matches up, so split up the array into individual pages
            emfImages = MetafileHelper.SplitByteArrayIntoPages(allPages, pageSizesList);
            if (emfImages == null || emfImages.Count == 0)
            {
                throw new ArgumentNullException(Resources.NoEmfImage);
            }

            return emfImages;
        }

        /// <summary>
        /// Splits the flattened byte array into individual printed pages represented by a list of Metafile objects.
        /// </summary>
        /// <param name="allPages">The byte array containing all pages.</param>
        /// <param name="pageSizes">A list of page sizes in bytes.</param>
        /// <returns>A list of Metafile objects.</returns>
        /// <remarks>It is the caller's responsibility to properly dispose of the MemoryStream returned.</remarks>
        private static List<Metafile> SplitByteArrayIntoPages(byte[] allPages, List<int> pageSizes)
        {
            var pages = new List<Metafile>();

            // get the total size and ensure it is the same as the byte array
            var totalSize = 0;
            foreach (var pageSize in pageSizes)
            {
                // pull out a page of bytes
                var page = new byte[pageSize];
                Buffer.BlockCopy(allPages, totalSize, page, 0, pageSize);

                using (var stream = new MemoryStream(page))
                {
                    pages.Add(new Metafile(stream));
                }

                // increment the total size for validation purposes and to use as an offset
                totalSize += pageSize;
            }

            if (totalSize != allPages.LongLength)
            {
                string message = string.Format(CultureInfo.CurrentCulture, Resources.ReportPageSizesInvalid, totalSize, allPages.LongLength);
                throw new ArgumentException(message);
            }

            return pages;
        }
    }
}