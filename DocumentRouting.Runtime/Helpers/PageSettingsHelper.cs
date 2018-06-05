//--------------------------------------------------------------------------
//  <copyright file="PageSettingsHelper.cs"  company="Microsoft Corporation">
//      Copyright (c) Microsoft Corporation. All rights reserved.
//  </copyright>
//--------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.Runtime
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing.Printing;
    using System.Globalization;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// The class handles the processing of page settings of the page.
    /// </summary>
    internal class PageSettingsHelper
    {
        /// <summary>
        /// Gets or sets a value indicating whether the orientation of the report is landscape or not.
        /// </summary>
        internal bool IsLandscapeSetOnReportDesign { get; set; }

        /// <summary>
        /// Performs Xml serialization on the page settings object
        /// </summary>
        /// <param name="pageSettings">The PageSettings to be serialized</param>
        /// <returns>Xml string representing the object</returns>
        /// <remarks>
        /// The PageSettings class is marked as Serializable and not XmlSerializable.
        /// If we use XmlTextWriter (like in Proxy.XmlSerialize) to serialize the page setting object 
        /// we get an MemoryAccessViolation exception and it crashes the client. The reason is that 
        /// members of the Page Settings object cannot be serialized into the xml encoding type we have 
        /// using in the XmlHelper.XmlSerialize method.
        /// </remarks>
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "MemoryStream does not get disposed multiple times")]
        internal static string SerializePageSettings(PageSettings pageSettings)
        {
            var str = string.Empty;
            var retryCount = 1;

            if (pageSettings != null)
            {
                while (retryCount <= 2)
                {
                    try
                    {
                        using (var stream = new MemoryStream())
                        {
                            var serializer = new XmlSerializer(pageSettings.GetType());
                            serializer.Serialize(stream, pageSettings);
                            if (stream.Length > 0)
                            {
                                stream.Seek(0, SeekOrigin.Begin);
                                using (StreamReader reader = new StreamReader(stream))
                                {
                                    str = reader.ReadToEnd();
                                }

                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        retryCount++;
                        if (retryCount > 2)
                        {
                            var message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                            var exceptionType = ex.InnerException != null ? ex.InnerException.GetType().ToString() : ex.GetType().ToString();
                            
                            // TODO Error Log
                        }
                    }
                }
            }

            return str;
        }

        /// <summary>
        /// Processes the PageSetting string set by user and then converts to a object which can be set to printer
        /// </summary>
        /// <param name="pageSettingString">The page setting object</param>      
        /// <returns>Returns the page settings object</returns>
        [SuppressMessage("Microsoft.Usage", "CA2202:DoNotDisposeObjectsMultipleTimes", Justification = "False positive. We are not disposing multiple times.")]
        internal PageSettings ProcessPageSettings(string pageSettingString)
        {
            PageSettings pageSettings = null;
            try
            {
                pageSettings = this.ProcessPageSettingsInternal(pageSettingString);
            }
            catch (Exception)
            {
                if (pageSettings == null)
                {
                    pageSettings = this.GetDefaultPageSettings(pageSettingString);
                }

                // TODO Error Log
            }

            return pageSettings;
        }

        /// <summary>
        /// Gets the printer name or path from the printer page settings xml.
        /// </summary>
        /// <param name="pageSettings">The serialized printer page settings.</param>
        /// <returns>The printer name or path.</returns>
        [SuppressMessage("Microsoft.Usage", "CA2202:DoNotDisposeObjectsMultipleTimes", Justification = "False positive. We are not disposing multiple times.")]
        [SuppressMessage("Microsoft.Security.Xml", "CA3053:Use XmlSecureResolver", Justification = "XmlResolver is explicitly set to null.")]
        internal static string PrinterNameFromPageSettings(string pageSettings)
        {
            if (string.IsNullOrWhiteSpace(pageSettings))
            {
                return null;
            }

            var value = string.Empty;

            TextReader textReader = null;
            try
            {
                textReader = new StringReader(pageSettings);
                var readerSettings = new XmlReaderSettings()
                {
                    DtdProcessing = DtdProcessing.Prohibit,
                    XmlResolver = null
                };

                using (var xmlReader = XmlReader.Create(textReader, readerSettings))
                {
                    var doc = new XmlDocument
                    {
                        XmlResolver = null
                    };

                    doc.Load(xmlReader);

                    value = PageSettingsHelper.GetPrinterNameFromPageSettings(doc.DocumentElement);
                }
            }
            finally
            {
                if (textReader != null)
                {
                    textReader.Dispose();
                }
            }

            return value;
        }

        /// <summary>
        /// Gets the orientation of the page from the xml and sets it based on the logic
        /// </summary>
        /// <param name="pageSettingsRootNode">The page settings root node</param>
        /// <returns>Returns whether it is landscape in boolean</returns>
        internal bool IsLandscape(XmlNode pageSettingsRootNode)
        {
            if (pageSettingsRootNode == null)
            {
                throw new ArgumentException(Resources.PageSettingsNotNull);
            }

            // If orientation set by user is empty, it means that user want to preserve the orientation of the report design
            string orientation = this.GetXmlNodeValue(@"/PageSettings/Landscape", pageSettingsRootNode);
            if (string.IsNullOrEmpty(orientation))
            {
                return this.IsLandscapeSetOnReportDesign;
            }

            return Convert.ToBoolean(orientation, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the non-empty nodes in the xml sent by the user
        /// </summary>
        /// <param name="root">The root node of xml</param>
        /// <returns>Returns the no-empty node list</returns>
        internal XmlNodeList GetNonEmptyNodes(XmlNode root)
        {
            if (root == null)
            {
                throw new ArgumentException(Resources.PageSettingsNotNull);
            }

            // Gets the non empty node from the pageSettings
            XmlNodeList nonEmptyNodes = root.SelectNodes("descendant-or-self::*[string-length(text()) > 0] ");
            return nonEmptyNodes;
        }

        /// <summary>
        /// Transfers the page settings set by user to page setting object sent to printer
        /// </summary>
        /// <param name="pageSettings">The page settings object</param>
        /// <param name="nonEmptyNodes">The non empty nodes</param>
        /// <returns>Returns the final page settings</returns>
        [SuppressMessage("Microsoft.Usage", "CA2202:DoNotDisposeObjectsMultipleTimes", Justification = "False positive. We are not disposing multiple times.")]
        [SuppressMessage("Microsoft.Security.Xml", "CA3053:UseXmlSecureResolver", Justification = "The xml being loaded is safe..")]
        internal string TransferPageSettingsSetByUserToPrinter(string pageSettings, XmlNodeList nonEmptyNodes)
        {
            TextReader textReader = null;
            string modifiedPageSettings = null;

            try
            {
                pageSettings = XmlHelper.TrimXmlString(pageSettings, "PageSettings");

                if (!string.IsNullOrWhiteSpace(pageSettings))
                {
                    textReader = new StringReader(pageSettings);
                    var readerSettings = new XmlReaderSettings
                    {
                        DtdProcessing = DtdProcessing.Prohibit,
                        XmlResolver = null
                    };

                    using (var xmlReader = XmlReader.Create(textReader, readerSettings))
                    {
                        var pageSettingsDoc = new XmlDocument();
                        pageSettingsDoc.Load(xmlReader);
                        var rootPageSettings = pageSettingsDoc.DocumentElement;

                        // Iterate through all the non empty decendant paths and transfer the element value to the printersettings
                        // which will finally sent to printer
                        if (rootPageSettings != null)
                        {
                            foreach (XmlNode node in nonEmptyNodes)
                            {
                                var nodeFullPath = this.GetFullPathOfNode(node);
                                var nodeLocal = rootPageSettings.SelectSingleNode(nodeFullPath);
                                nodeLocal.InnerText = node.InnerText;
                            }
                        }

                        modifiedPageSettings = rootPageSettings.OuterXml;
                    }
                }
            }
            finally
            {
                if (textReader != null)
                {
                    textReader.Dispose();
                }
            }

            return modifiedPageSettings;
        }

        /// <summary>
        /// Gets the printer name from the root node
        /// </summary>
        /// <param name="root">The root node of the page settings</param>
        /// <returns>Returns the printer name</returns>
        internal static string GetPrinterNameFromPageSettings(XmlNode root)
        {
            if (root == null)
            {
                throw new ArgumentException(Resources.PageSettingsNotNull);
            }

            // Set the printer name on the pagesettings and serialize pagesettings
            var printerNameNode = root.SelectSingleNode("/PageSettings/PrinterSettings/PrinterName");

            var printerName = printerNameNode?.InnerText;
            if (string.IsNullOrWhiteSpace(printerName))
            {
                throw new ArgumentNullException(Resources.NoPrinterName);
            }

            return printerName;
        }

        /// <summary>
        /// Gets the value of a xml node given its path
        /// </summary>
        /// <param name="path">The path for which value needs to be found</param>
        /// <param name="root">The root node of the page settings</param>
        /// <returns>Returns the value of the node specified</returns>
        internal string GetXmlNodeValue(string path, XmlNode root)
        {
            string nodeValue = null;
            if (root != null && !string.IsNullOrEmpty(path))
            {
                // Get a node value given the path of the xml
                var nodeLocal = root.SelectSingleNode(path);

                nodeValue = nodeLocal?.InnerText;
            }

            return nodeValue;
        }

        /// <summary>
        /// Gets the serialized page settings for printer
        /// </summary>
        /// <param name="pageSettingsOfPrinter">The page settings that will send to printer</param>
        /// <returns>Returns the serialized page string</returns>
        private string GetSerializedPageSettingsOfPrinter(PageSettings pageSettingsOfPrinter)
        {
            string pageSettingsStringForPrinter = PageSettingsHelper.SerializePageSettings(pageSettingsOfPrinter);

            if (string.IsNullOrEmpty(pageSettingsStringForPrinter))
            {
                throw new ArgumentNullException(Resources.NoPageSettings);
            }

            return pageSettingsStringForPrinter;
        }

        /// <summary>
        /// Processes the PageSetting string set by user and then converts to a object which can be set to printer
        /// </summary>
        /// <param name="pageSettingString">The page setting object</param>      
        /// <returns>Returns the page settings object</returns>
        [SuppressMessage("Microsoft.Usage", "CA2202:DoNotDisposeObjectsMultipleTimes", Justification = "False positive. We are not disposing multiple times.")]
        private PageSettings ProcessPageSettingsInternal(string pageSettingString)
        {
            if (string.IsNullOrEmpty(pageSettingString))
            {
                throw new ArgumentNullException("PageSettings set by user cannot be empty");
            }

            PageSettings pageSettingsOfPrinter = null;
            TextReader textReader = null;

            try
            {
                // Xml Document holding the page settings user has set
                textReader = new StringReader(pageSettingString);

                var readerSettings = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Prohibit,
                    XmlResolver = null
                };

                using (var xmlReader = XmlReader.Create(textReader, readerSettings))
                {
                    XmlDocument pageSettingSetByUser = new XmlDocument
                    {
                        XmlResolver = null
                    };

                    pageSettingSetByUser.Load(xmlReader);

                    var pageSettingsRootNode = pageSettingSetByUser.DocumentElement;

                    // Gets the non empty nodes, i.e. nodes which actually user has set a value
                    var nonEmptyNodes = this.GetNonEmptyNodes(pageSettingsRootNode);

                    // Gets the printer name from the same xml. If printer name is not set or not valid , no need to go further
                    var printerName = PageSettingsHelper.GetPrinterNameFromPageSettings(pageSettingsRootNode);

                    if (string.IsNullOrWhiteSpace(printerName))
                    {
                        throw new ArgumentNullException(Resources.NoPrinterName);
                    }

                    // Get the default page settings of the printer and sets the printer name and orientation
                    pageSettingsOfPrinter = new PageSettings();
                    pageSettingsOfPrinter.PrinterSettings.PrintFileName = Constants.Empty;
                    pageSettingsOfPrinter.PrinterSettings.PrinterName = printerName;
                    pageSettingsOfPrinter.Landscape = this.IsLandscape(pageSettingsRootNode);

                    // serialize the pagesettings
                    var pageSettingsStringForPrinter = this.GetSerializedPageSettingsOfPrinter(pageSettingsOfPrinter);

                    if (string.IsNullOrEmpty(pageSettingsStringForPrinter))
                    {
                        throw new ArgumentNullException("PageSettings set by user cannot be empty");
                    }

                    // Copy the pageSettings from user set to one sent to printer
                    var pageSettingsTobeSet = this.TransferPageSettingsSetByUserToPrinter(pageSettingsStringForPrinter, nonEmptyNodes);

                    // finally deserialize it and give it back
                    pageSettingsOfPrinter = (PageSettings)XmlHelper.XmlDeserialize(typeof(PageSettings), pageSettingsTobeSet, string.Empty);
                    if (pageSettingsOfPrinter == null)
                    {
                        throw new ArgumentNullException(Resources.NoPageSettings);
                    }
                }
            }
            finally
            {
                if (textReader != null)
                {
                    textReader.Dispose();
                }
            }

            return pageSettingsOfPrinter;
        }

        /// <summary>
        /// Gets the full path node of any given node 
        /// </summary>
        /// <param name="node">The node name</param>
        /// <returns>Returns the full path node</returns>
        private string GetFullPathOfNode(XmlNode node)
        {
            var nodeName = node.Name;
            while (node.ParentNode != null && node.ParentNode.NodeType != XmlNodeType.Document)
            {
                node = node.ParentNode;
                nodeName = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", node.Name, nodeName);
            }

            nodeName = string.Format(CultureInfo.InvariantCulture, "/{0}", nodeName);

            return nodeName;
        }

        /// <summary>
        /// Gets the default page settings object if there is any exception condition
        /// </summary>
        /// <param name="pageSettingString">The page settings string object</param>
        /// <returns>Returns the page settings object</returns>
        private PageSettings GetDefaultPageSettings(string pageSettingString)
        {

            // this should not be the case ever, still puttinga  safety net, so that printing doesn't stop and it prints to 
            // default printer.
            if (string.IsNullOrEmpty(pageSettingString))
            {
                throw new ArgumentNullException(Resources.PageSettingsNotNull);
            }

            PageSettings pageSettingsOfPrinter = null;

            // Xml Document holding the page settings user has set
            var textReader = new StringReader(pageSettingString);

            var readerSettings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null
            };

            using (XmlReader xmlReader = XmlReader.Create(textReader, readerSettings))
            {
                var pageSettingSetByUser = new XmlDocument
                {
                    XmlResolver = null
                };

                pageSettingSetByUser.Load(xmlReader);
                var pageSettingsRootNode = pageSettingSetByUser.DocumentElement;

                pageSettingsOfPrinter = new PageSettings();
                var printerName = PageSettingsHelper.GetPrinterNameFromPageSettings(pageSettingsRootNode);

                if (string.IsNullOrEmpty(printerName))
                {
                    throw new ArgumentException(Resources.PrinterNameMustNotBeNull);
                }

                pageSettingsOfPrinter.PrinterSettings.PrinterName = printerName;
                this.SetDefaultPageSettings(pageSettingsOfPrinter, pageSettingsRootNode);
            }

            return pageSettingsOfPrinter;
        }

        /// <summary>
        /// Sets the default page settings properties
        /// </summary>
        /// <param name="pageSettings">The page settings object</param>
        /// <param name="pageSettingsRootNode">The page settings root node</param>
        private void SetDefaultPageSettings(PageSettings pageSettings, XmlNode pageSettingsRootNode)
        {
            if (pageSettingsRootNode == null)
            {
                throw new ArgumentException(Resources.PageSettingsNotNull);
            }

            if (pageSettings == null)
            {
                throw new ArgumentException(Resources.NoPageSettings);
            }

            this.SetOrientation(pageSettings, pageSettingsRootNode);
            this.SetDuplex(pageSettings, pageSettingsRootNode);
            this.SetNumberOfPages(pageSettings, pageSettingsRootNode);
            this.SetCollate(pageSettings, pageSettingsRootNode);
            this.SetPageRange(pageSettings, pageSettingsRootNode);
        }

        /// <summary>
        /// Sets the page range property on page settings
        /// </summary>
        /// <param name="pageSettings">The page settings object</param>
        /// <param name="pageSettingsRootNode">The root node</param>
        private void SetPageRange(PageSettings pageSettings, XmlNode pageSettingsRootNode)
        {
            Debug.Assert(pageSettings != null, Resources.NoPageSettings);
            Debug.Assert(pageSettingsRootNode != null, Resources.PageSettingsNotNull);

            try
            {
                var fromString = this.GetXmlNodeValue(@"/PageSettings/PrinterSettings/FromPage", pageSettingsRootNode);
                var toString = this.GetXmlNodeValue(@"/PageSettings/PrinterSettings/ToPage", pageSettingsRootNode);
                if (!string.IsNullOrEmpty(fromString) && !string.IsNullOrEmpty(toString))
                {
                    pageSettings.PrinterSettings.FromPage = Convert.ToInt32(fromString, CultureInfo.InvariantCulture);
                    pageSettings.PrinterSettings.ToPage = Convert.ToInt32(toString, CultureInfo.InvariantCulture);
                }
            }
            catch (Exception)
            {
                // TODO Error log
            }
        }

        /// <summary>
        /// Sets the collate property on page settings
        /// </summary>
        /// <param name="pageSettings">The page settings object</param>
        /// <param name="pageSettingsRootNode">The root node</param>
        private void SetCollate(PageSettings pageSettings, XmlNode pageSettingsRootNode)
        {
            Debug.Assert(pageSettings != null, Resources.NoPageSettings);
            Debug.Assert(pageSettingsRootNode != null, Resources.PageSettingsNotNull);

            try
            {
                var collateString = this.GetXmlNodeValue(@"/PageSettings/PrinterSettings/Collate", pageSettingsRootNode);
                if (!string.IsNullOrEmpty(collateString))
                {
                    pageSettings.PrinterSettings.Collate = Convert.ToBoolean(collateString, CultureInfo.InvariantCulture);
                }
            }
            catch (Exception)
            {
                // TODO Error log
            }
        }

        /// <summary>
        /// Sets the orientation property on page settings
        /// </summary>
        /// <param name="pageSettings">The page settings object</param>
        /// <param name="pageSettingsRootNode">The root node</param>
        private void SetOrientation(PageSettings pageSettings, XmlNode pageSettingsRootNode)
        {
            Debug.Assert(pageSettings != null, Resources.NoPageSettings);
            Debug.Assert(pageSettingsRootNode != null, Resources.PageSettingsNotNull);

            try
            {
                pageSettings.Landscape = this.IsLandscape(pageSettingsRootNode);
            }
            catch (Exception)
            {
                // TODO Error log
            }
        }

        /// <summary>
        /// Sets the copies property on page settings
        /// </summary>
        /// <param name="pageSettings">The page settings object</param>
        /// <param name="pageSettingsRootNode">The root node</param>
        private void SetNumberOfPages(PageSettings pageSettings, XmlNode pageSettingsRootNode)
        {
            Debug.Assert(pageSettings != null, Resources.NoPageSettings);
            Debug.Assert(pageSettingsRootNode != null, Resources.PageSettingsNotNull);

            try
            {
                string copiesString = this.GetXmlNodeValue(@"/PageSettings/PrinterSettings/Copies", pageSettingsRootNode);
                if (!string.IsNullOrEmpty(copiesString))
                {
                    pageSettings.PrinterSettings.Copies = Convert.ToInt16(copiesString, CultureInfo.InvariantCulture);
                }
            }
            catch (Exception)
            {
                // TODO Error log
            }
        }

        /// <summary>
        /// Sets the duplex property on page settings
        /// </summary>
        /// <param name="pageSettings">The page settings object</param>
        /// <param name="pageSettingsRootNode">The root node</param>
        private void SetDuplex(PageSettings pageSettings, XmlNode pageSettingsRootNode)
        {
            Debug.Assert(pageSettings != null, Resources.NoPageSettings);
            Debug.Assert(pageSettingsRootNode != null, Resources.PageSettingsNotNull);

            try
            {
                var duplexString = this.GetXmlNodeValue(@"/PageSettings/PrinterSettings/Duplex", pageSettingsRootNode);
                if (!string.IsNullOrEmpty(duplexString))
                {
                    switch (duplexString)
                    {
                        case "Simplex":
                            pageSettings.PrinterSettings.Duplex = Duplex.Simplex;
                            break;
                        case "Horizontal":
                            pageSettings.PrinterSettings.Duplex = Duplex.Horizontal;
                            break;
                        case "Vertical":
                            pageSettings.PrinterSettings.Duplex = Duplex.Vertical;
                            break;
                        default:
                            pageSettings.PrinterSettings.Duplex = Duplex.Default;
                            break;
                    }
                }
            }
            catch (Exception)
            {
                // TODO Error log
            }
        }
    }
}
