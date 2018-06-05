//------------------------------------------------------------------------------
// <copyright file="DialogViewController.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting
{
    /// <summary>
    /// The dialog view controller.
    /// </summary>
    public class DialogViewController : IDialogViewController
    {
        /// <summary>
        /// Show printers dialog
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public void ShowPrintersDialog(PrintersViewModel viewModel)
        {
            var printersView = new Printers(viewModel);
            printersView.ShowDialog();
        }

        /// <summary>
        /// Show settings dialog
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public void ShowSettingsDialog(SettingsViewModel viewModel)
        {
            var settingsView = new SettingsView(viewModel);
            settingsView.ShowDialog();
        }
    }
}
