//------------------------------------------------------------------------------
// <copyright file="IDialogViewController.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting
{
    /// <summary>
    /// Interface to <see cref="DialogViewController"/>
    /// </summary>
    public interface IDialogViewController
    {
        /// <summary>
        /// Show settings dialog
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        void ShowSettingsDialog(SettingsViewModel viewModel);

        /// <summary>
        /// Show printers dialog
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        void ShowPrintersDialog(PrintersViewModel viewModel);
    }
}
