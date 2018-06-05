//------------------------------------------------------------------------------
// <copyright file="XmlHelper.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.Runtime
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Contains XML helper functionality
    /// </summary>
    internal static class XmlHelper
    {
        /// <summary>
        /// Default namespace
        /// </summary>
        private const string DefaultNamespace = @"http://schemas.microsoft.com/sqlserver/2005/06/30/reporting/reportingservices";

        /// <summary>
        /// The maximum number of retries.
        /// </summary>
        private const int MaxRetries = 2;

        /// <summary>
        /// Performs Xml serialization on the object
        /// If a different namespace is to be used then use the overloaded XmlSerialize method.
        /// </summary>
        /// <param name="type">The type of object</param>
        /// <param name="obj">The object to be serialized</param>
        /// <returns>Xml string representing the object</returns>
        public static string XmlSerialize(Type type, object obj)
        {
            return XmlSerialize(type, obj, DefaultNamespace);
        }

        /// <summary>
        /// Performs Xml serialization on the object
        /// </summary>
        /// <param name="type">The type of object</param>
        /// <param name="obj">The object to be serialized</param>
        /// <param name="defaultNamespace">The url of the default namespace</param>
        /// <returns>Xml string representing the object</returns>
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "Examined the code for  MemoryStream and XmlTextWriter, its safe to dispose twice.")]
        public static string XmlSerialize(Type type, object obj, string defaultNamespace)
        {
            string str = string.Empty;

            if (obj != null)
            {
                using (var stream = new MemoryStream())
                {
                    using (var writer = new XmlTextWriter(stream, Encoding.UTF8))
                    {
                        XmlSerializer serializer = null;

                        if (string.IsNullOrEmpty(defaultNamespace))
                        {
                            serializer = new XmlSerializer(type);
                        }
                        else
                        {
                            serializer = new XmlSerializer(type, defaultNamespace);
                        }

                        serializer.Serialize(writer, obj);

                        writer.Flush();

                        stream.Seek(0, SeekOrigin.Begin);

                        str = ByteArrayToString(stream.ToArray());
                    }
                }
            }

            return str;
        }

        /// <summary>
        /// Deserializes and object from an Xml string.
        /// If a different namespace is to be used then use the overloaded XmlDeserialize method.
        /// </summary>
        /// <param name="type">Type of object to be deserialized</param>
        /// <param name="xml">Xml string representing object</param>
        /// <returns>Deserialized object</returns>
        public static object XmlDeserialize(Type type, string xml)
        {
            return XmlDeserialize(type, xml, DefaultNamespace);
        }

        /// <summary>
        /// Deserializes and object from an Xml string
        /// </summary>
        /// <param name="type">Type of object to be deserialized</param>
        /// <param name="xml">Xml string representing object</param>
        /// <param name="defaultNamespace">The url of the default namespace.</param>       
        /// <returns>Deserialized object</returns>
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "Examined the code for  MemoryStream and XmlTextReader, its safe to dispose twice.")]
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "False positive: the textReader object is being disposed of properly.")]
        [SuppressMessage("Microsoft.Security.Xml", "CA3053:Use XmlSecureResolver", Justification = "XmlResolver is explicitly set to null.")]
        public static object XmlDeserialize(Type type, string xml, string defaultNamespace)
        {
            object obj = null;
            int retryCount = 1;
            TextReader textReader = null;

            if (!string.IsNullOrEmpty(xml))
            {
                while (retryCount <= MaxRetries)
                {
                    try
                    {
                        textReader = new StringReader(xml);
                        var readerSettings = new XmlReaderSettings()
                        {
                            DtdProcessing = DtdProcessing.Ignore,
                            IgnoreProcessingInstructions = true,
                            XmlResolver = null
                        };

                        using (var xmlReader = XmlReader.Create(textReader, readerSettings))
                        {
                            XmlSerializer serializer = null;
                            if (string.IsNullOrEmpty(defaultNamespace))
                            {
                                serializer = new XmlSerializer(type);
                            }
                            else
                            {
                                serializer = new XmlSerializer(type, defaultNamespace);
                            }

                            try
                            {
                                obj = serializer.Deserialize(xmlReader);
                                break;
                            }
                            catch (InvalidOperationException invalidEx)
                            {
                                // Eat the exception. This means that the string passed in does not represent
                                // a valid object of the specified type.  Method returns null.
                                // TODO Error log
                                throw invalidEx;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        retryCount++;
                        if (retryCount > MaxRetries)
                        {
                            string message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                            string exceptionType = ex.InnerException != null ? ex.InnerException.GetType().ToString() : ex.GetType().ToString();
                            // TODO Error log
                        }
                    }
                    finally
                    {
                        if (textReader != null)
                        {
                            textReader.Dispose();
                        }
                    }
                }
            }

            return obj;
        }

        /// <summary>
        /// Trims an xml string to get rid of unwanted values in front of string       
        /// </summary>
        /// <param name="xmlString"> The xml string</param>
        /// <param name="stringName"> The name of the parameter</param>
        /// <returns> The trimmed xml string</returns>
        public static string TrimXmlString(string xmlString, string stringName)
        {
            if (!string.IsNullOrEmpty(xmlString))
            {
                int index = xmlString.IndexOf("<?xml", StringComparison.OrdinalIgnoreCase);
                if (index >= 0)
                {
                    xmlString = xmlString.Substring(index);
                }
                else
                {
                    throw new InvalidDataException(string.Format(CultureInfo.InvariantCulture, Resources.XmlDeclarationNotFound, stringName));
                }
            }

            return xmlString;
        }

        /// <summary>
        /// Converts a string into an array of bytes
        /// </summary>
        /// <param name="str">String to be converted to a byte array</param>
        /// <returns>Resultant byte array</returns>
        private static byte[] StringToByteArray(string str)
        {
            var bytes = new UTF8Encoding(true).GetBytes(str);
            return bytes;
        }

        /// <summary>
        /// Converts an array of bytes into a string
        /// </summary>
        /// <param name="bytes">byte array to be converted to a string</param>
        /// <returns>Resultant string</returns>
        private static string ByteArrayToString(byte[] bytes)
        {
            var encoder = new UTF8Encoding(true);
            return encoder.GetString(bytes);
        }
    }
}