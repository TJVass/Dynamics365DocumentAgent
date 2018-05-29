//--------------------------------------------------------------------------
//  <copyright file="PrinterData.cs"  company="Microsoft Corporation">
//      Copyright (c) Microsoft Corporation. All rights reserved.
//  </copyright>
//--------------------------------------------------------------------------
namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.Runtime
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// The document printing status information.
    /// </summary>
    public class PrinterData : INotifyPropertyChanged
    {
        /// <summary>
        /// Is current printer registered
        /// </summary>
        private bool isRegistered;

        /// <summary>
        /// printer name
        /// </summary>
        private string printerName;

        /// <summary>
        /// printer path
        /// </summary>
        private string printerPath;

        /// <summary>
        /// printer status
        /// </summary>
        private PrinterStatus status;

        /// <summary>
        /// The printer description
        /// </summary>
        private string printerDescription;

        /// <summary>
        /// PropertyChanged Event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets a value indicating whether this printer is registered or not
        /// </summary>
        public bool IsRegistered
        {
            get
            {
                return this.isRegistered;
            }

            set
            {
                this.isRegistered = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the printer name
        /// </summary>
        public string PrinterName
        {
            get
            {
                return this.printerName;
            }

            set
            {
                this.printerName = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the printer path
        /// </summary>
        public string PrinterPath
        {
            get
            {
                return this.printerPath;
            }

            set
            {
                this.printerPath = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the printer description
        /// </summary>
        public string PrinterDescription
        {
            get
            {
                return this.printerDescription;
            }

            set
            {
                this.printerDescription = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the printer status
        /// </summary>
        public PrinterStatus PrinterStatus
        {
            get
            {
                return this.status;
            }

            set
            {
                this.status = value;
                this.OnPropertyChanged();
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
    }
}