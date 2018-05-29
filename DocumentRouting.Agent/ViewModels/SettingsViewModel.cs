//------------------------------------------------------------------------------
// <copyright file="SettingsViewModel.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Microsoft.Dynamics.AX.Framework.DocumentRouting.Runtime;

    /// <summary>
    /// View model for the settings dialog
    /// </summary>
    public class SettingsViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The error handler strategy
        /// </summary>
        private readonly Action<string> errorHandlerStrategy;

        /// <summary>
        /// The settings manager
        /// </summary>
        private readonly ISettingsManager settingsManager;

        /// <summary>
        /// The application identifier
        /// </summary>
        private string applicationId;

        /// <summary>
        /// The aad instance URI
        /// </summary>
        private string aadInstanceUri;

        /// <summary>
        /// The tenant
        /// </summary>
        private string tenant;

        /// <summary>
        /// The ax root URL
        /// </summary>
        private string rootUrl;

        /// <summary>
        /// The redirect URI
        /// </summary>
        private string redirectUri;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewModel" /> class.
        /// </summary>
        /// <param name="settingsManager">The settings manager.</param>
        /// <param name="errorHandlerStrategy">The error handler strategy.</param>
        public SettingsViewModel(ISettingsManager settingsManager, Action<string> errorHandlerStrategy)
        {
            this.errorHandlerStrategy = errorHandlerStrategy ?? throw new ArgumentNullException(nameof(errorHandlerStrategy));
            this.settingsManager = settingsManager ?? throw new ArgumentNullException(nameof(settingsManager));

            this.SaveSettingsCommand = new DelegateCommand(this.SaveSettings, this.CanSaveSettings);
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when registered printers was updated.
        /// </summary>
        public event EventHandler SettingsSaved;

        /// <summary>
        /// Gets the save settings command.
        /// </summary>
        public DelegateCommand SaveSettingsCommand { get; }

        /// <summary>
        /// Gets or sets the application identifier.
        /// </summary>
        public string ApplicationId
        {
            get
            {
                return this.applicationId;
            }

            set
            {
                this.applicationId = value;
                this.OnSettingsPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets the aad instance URI.
        /// </summary>
        public string AADInstanceUri
        {
            get
            {
                return this.aadInstanceUri;
            }

            set
            {
                this.aadInstanceUri = value;
                this.OnSettingsPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets the tenant.
        /// </summary>
        public string Tenant
        {
            get
            {
                return this.tenant;
            }

            set
            {
                this.tenant = value;
                this.OnSettingsPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets the ax root URL.
        /// </summary>
        public string AXRootUrl
        {
            get
            {
                return this.rootUrl;
            }

            set
            {
                this.rootUrl = value;
                this.OnSettingsPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets the redirect URI.
        /// </summary>
        public string RedirectUri
        {
            get
            {
                return this.redirectUri;
            }

            set
            {
                this.redirectUri = value;
                this.OnSettingsPropertyChange();
            }
        }

        /// <summary>
        /// Populates the settings.
        /// </summary>
        public void PopulateSettings()
        {
            try
            {
                var settingsModel = this.settingsManager.GetSettings();

                this.AADInstanceUri = settingsModel.AADInstanceUri;
                this.ApplicationId = settingsModel.ApplicationId.ToString();
                this.AXRootUrl = settingsModel.AXRootUrl;
                this.RedirectUri = settingsModel.RedirectUri;
                this.Tenant = settingsModel.Tenant;
            }
            catch (Exception ex)
            {
                this.errorHandlerStrategy(ex.Message);
            }
        }

        /// <summary>
        /// Called when [settings property change].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void OnSettingsPropertyChange([CallerMemberName] string propertyName = null)
        {
            this.OnPropertyChanged(propertyName);
            this.SaveSettingsCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Called when property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Called when settings saved.
        /// </summary>
        protected void OnSettingsSaved()
        {
            this.SettingsSaved?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Saves the settings.
        /// </summary>
        private void SaveSettings()
        {
            try
            {
                var settingsModel = this.settingsManager.GetSettings();

                settingsModel.AADInstanceUri = this.AADInstanceUri;
                settingsModel.ApplicationId = new Guid(this.ApplicationId);
                settingsModel.AXRootUrl = this.AXRootUrl;
                settingsModel.RedirectUri = this.RedirectUri;
                settingsModel.Tenant = this.Tenant;

                this.settingsManager.SaveSettings(settingsModel);
                this.OnSettingsSaved();
            }
            catch (Exception ex)
            {
                // TODO Error Log
                this.errorHandlerStrategy(ex.Message);
            }
        }

        /// <summary>
        /// Determines whether this instance can save settings.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance can save settings; otherwise, <c>false</c>.
        /// </returns>
        private bool CanSaveSettings()
        {
            var result = Uri.IsWellFormedUriString(this.AADInstanceUri, UriKind.RelativeOrAbsolute) &&
                    Uri.IsWellFormedUriString(this.AXRootUrl, UriKind.RelativeOrAbsolute) &&
                    Uri.IsWellFormedUriString(this.RedirectUri, UriKind.RelativeOrAbsolute) &&
                    !string.IsNullOrWhiteSpace(this.Tenant) &&
                    Guid.TryParse(this.ApplicationId, out Guid appId) &&
                    appId != Guid.Empty;

            return result;
        }
    }
}
