//------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.DocumentRouting
{
    using System;
    using System.Windows;
    using System.Windows.Threading;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Handles the Startup event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="StartupEventArgs"/> instance containing the event data.</param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += this.CurrentDomain_UnhandledException;
            this.DispatcherUnhandledException += this.App_DispatcherUnhandledException;
        }

        /// <summary>
        /// Handles the UnhandledException event of the CurrentDomain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var rootEx = e.ExceptionObject as Exception;
            var ex = rootEx;
            while (ex != null && ex.InnerException != null)
            {
                if (ex is AggregateException)
                {
                    ex = ex.InnerException;
                }
                else
                {
                    break;
                }
            }

            if (ex != null)
            {
                // TODO: Error Log
                MessageBox.Show(ex.Message, null, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Handles the DispatcherUnhandledException event of the App control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DispatcherUnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var rootEx = e.Exception;
            var ex = rootEx;
            while (ex != null && ex.InnerException != null)
            {
                if (ex is AggregateException)
                {
                    ex = ex.InnerException;
                }
                else
                {
                    break;
                }
            }

            if (ex != null)
            {
                // TODO: Error Log
                MessageBox.Show(ex.Message, null, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            e.Handled = true;
        }
    }
}
