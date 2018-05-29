//--------------------------------------------------------------------------
//  <copyright file="MainWindow.xaml.cs"  company="Microsoft Corporation">
//      Copyright (c) Microsoft Corporation. All rights reserved.
//  </copyright>
//--------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting
{
    using System.Windows;
    using Microsoft.Dynamics.AX.Framework.DocumentRouting.Runtime;

    /// <summary>
    /// Interaction logic for the main window.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Document Routing Agent view model
        /// </summary>
        private DocumentRoutingAgentViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();

            this.viewModel = new DocumentRoutingAgentViewModel(this.ShowErrorMessage);
            this.DataContext = this.viewModel;
        }

        /// <summary>
        /// Show error message box
        /// </summary>
        /// <param name="errorMessage">The error message</param>
        private void ShowErrorMessage(string errorMessage)
        {
            if (!string.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show(errorMessage, StringResources.ErrorCaption, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Event hander invoked when the Exit button is clicked.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}