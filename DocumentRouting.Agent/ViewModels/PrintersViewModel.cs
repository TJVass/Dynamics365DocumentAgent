//------------------------------------------------------------------------------
// <copyright file="PrintersViewModel.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Microsoft.Dynamics.AX.Framework.DocumentRouting.Runtime;

    /// <summary>
    /// View model for the printers dialog
    /// </summary>
    public class PrintersViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The printer data provider
        /// </summary>
        private readonly IPrinterDataManager printerDataManager;

        /// <summary>
        /// The error handler strategy
        /// </summary>
        private readonly Action<string> errorHandlerStrategy;

        /// <summary>
        /// The is loading
        /// </summary>
        private bool isLoading;

        /// <summary>
        /// The printers
        /// </summary>
        private List<PrinterData> printers = new List<PrinterData>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintersViewModel" /> class.
        /// </summary>
        /// <param name="printerDataManager">The printer data manager.</param>
        /// <param name="errorHandlerStrategy">The error handler strategy.</param>
        public PrintersViewModel(IPrinterDataManager printerDataManager, Action<string> errorHandlerStrategy)
        {
            this.printerDataManager = printerDataManager ?? throw new ArgumentNullException(nameof(printerDataManager));
            this.errorHandlerStrategy = errorHandlerStrategy ?? throw new ArgumentNullException(nameof(errorHandlerStrategy));

            this.RegisteredPrintersCommand = new DelegateCommandAsync(this.UpdateRegisteredPrinters);
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when registered printers was updated.
        /// </summary>
        public event EventHandler RegisteredPrintersUpdated;

        /// <summary>
        /// Gets or sets the printers.
        /// </summary>
        public List<PrinterData> Printers
        {
            get
            {
                return this.printers;
            }

            set
            {
                this.printers = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is loading.
        /// </summary>
        public bool IsLoading
        {
            get
            {
                return this.isLoading;
            }

            set
            {
                this.isLoading = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the registered printers command.
        /// </summary>
        public IAsyncCommand RegisteredPrintersCommand { get; }

        /// <summary>
        /// Populates the printers.
        /// </summary>
        /// <returns>The task</returns>
        public async Task PopulatePrinters()
        {
            try
            {
                this.IsLoading = true;
                this.Printers = await this.printerDataManager.GetPrinters();
            }
            catch (Exception ex)
            {
                // TODO Error Log
                this.errorHandlerStrategy(ex.Message);
            }
            finally
            {
                this.IsLoading = false;
            }
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
        /// Called when registered printers updated.
        /// </summary>
        protected void OnRegisteredPrintersUpdated()
        {
            this.RegisteredPrintersUpdated?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Updates the registered printers.
        /// </summary>
        /// <returns>The task</returns>
        private async Task UpdateRegisteredPrinters()
        {
            try
            {
                this.IsLoading = true;

                await Task.Run(async () => await this.printerDataManager.UpdateRegisteredPrinters(this.Printers));
                this.OnRegisteredPrintersUpdated();
            }
            catch (Exception ex)
            {
                // TODO Error Log
                this.errorHandlerStrategy(ex.Message);
            }
            finally
            {
                this.IsLoading = false;
            }
        }
    }
}
