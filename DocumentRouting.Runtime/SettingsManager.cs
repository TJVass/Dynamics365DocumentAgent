//------------------------------------------------------------------------------
// <copyright file="SettingsManager.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.Runtime
{
    using System.IO;
    using System.Xml.Serialization;

    /// <summary>
    /// The settings manager
    /// </summary>
    public class SettingsManager : ISettingsManager
    {
        /// <summary>
        /// The settings file name
        /// </summary>
        private const string SettingsFileName = "DocumentRoutingSettings.dat";

        /// <summary>
        /// The settings file path
        /// </summary>
        private readonly string settingsFilePath = Path.Combine(Path.GetTempPath(), SettingsFileName);

        /// <summary>
        /// Serves as a read/write file lock
        /// </summary>
        private static readonly object FileLock = new object();

        /// <summary>
        /// The serializer
        /// </summary>
        private readonly XmlSerializer serializer;

        /// <summary>
        /// The settings
        /// </summary>
        private Settings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsManager"/> class.
        /// </summary>
        public SettingsManager()
        {
            this.serializer = new XmlSerializer(typeof(Settings));
        }

        /// <summary>
        /// Gets the settings instance.
        /// </summary>
        /// <returns>The settings instance.</returns>
        public Settings GetSettings()
        {
            if (this.settings == null && File.Exists(this.settingsFilePath))
            {
                lock (FileLock)
                {
                    using (var fileStream = File.OpenRead(this.settingsFilePath))
                    {
                        this.settings = (Settings)this.serializer.Deserialize(fileStream);
                    }
                }
            }

            return this.settings;
        }

        /// <summary>
        /// Saves the specified settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public void SaveSettings(Settings settings)
        {
            this.settings = settings;
            lock (FileLock)
            {
                using (var fileStream = File.OpenWrite(this.settingsFilePath))
                {
                    this.serializer.Serialize(fileStream, this.settings);
                }
            }
        }
    }
}
