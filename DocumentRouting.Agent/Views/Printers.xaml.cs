//------------------------------------------------------------------------------
// <copyright file="Printers.xaml.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting
{
    using System.Windows;
    using Microsoft.Dynamics.AX.Framework.DocumentRouting.Runtime;

    /// <summary>
    /// Interaction logic for Printers.xaml
    /// </summary>
    public partial class Printers : Window
    {
        /// <summary>
        /// The view model
        /// </summary>
        private readonly PrintersViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="Printers" /> class.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public Printers(PrintersViewModel viewModel)
        {
            this.InitializeComponent();

            this.viewModel = viewModel;
            this.DataContext = this.viewModel;
            this.viewModel.RegisteredPrintersUpdated += (sender, args) => { this.Close(); };
        }

        /// <summary>
        /// Handles the Loaded event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await this.viewModel.PopulatePrinters();
        }
    }
}
