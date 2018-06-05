//------------------------------------------------------------------------------
// <copyright file="SettingsView.xaml.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class SettingsView : Window
    {
        /// <summary>
        /// The view model
        /// </summary>
        private readonly SettingsViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsView" /> class.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public SettingsView(SettingsViewModel viewModel)
        {
            this.InitializeComponent();

            this.viewModel = viewModel;
            this.DataContext = this.viewModel;
            this.viewModel.SettingsSaved += (sender, args) => { this.Close(); };
        }

        /// <summary>
        /// Handles the Loaded event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.viewModel.PopulateSettings();
        }
    }
}
