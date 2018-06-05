//------------------------------------------------------------------------------
// <copyright file="ISettingsManager.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.Runtime
{
    /// <summary>
    /// The interface for settings manager.
    /// </summary>
    public interface ISettingsManager
    {
        /// <summary>
        /// Gets the settings instance.
        /// </summary>
        /// <returns></returns>
        Settings GetSettings();

        /// <summary>
        /// Saves the specified settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        void SaveSettings(Settings settings);
    }
}
