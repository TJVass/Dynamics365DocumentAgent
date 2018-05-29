//--------------------------------------------------------------------------
//  <copyright file="DocumentPrintingStatusContract.cs"  company="Microsoft Corporation">
//      Copyright (c) Microsoft Corporation. All rights reserved.
//  </copyright>
//--------------------------------------------------------------------------
namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.Runtime
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// The document printing status information.
    /// </summary>
    public class DocumentPrintingStatusContract : INotifyPropertyChanged
    {
        /// <summary>
        /// Status Id
        /// </summary>
        private string id;

        /// <summary>
        /// document name
        /// </summary>
        private string documentName;

        /// <summary>
        /// Printer name
        /// </summary>
        private string printerName;

        /// <summary>
        /// Number of pages
        /// </summary>
        private string pages;

        /// <summary>
        /// status date time
        /// </summary>
        private DateTime statusDateTime;

        /// <summary>
        /// Document status
        /// </summary>
        private string status;

        /// <summary>
        /// PropertyChanged Event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the document status Id
        /// </summary>
        public string Id
        {
            get
            {
                return this.id;
            }

            set
            {
                this.id = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the document name
        /// </summary>
        public string DocumentName
        {
            get
            {
                return this.documentName;
            }

            set
            {
                this.documentName = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the number of pages
        /// </summary>
        public string Pages
        {
            get
            {
                return this.pages;
            }

            set
            {
                this.pages = value;
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
        /// Gets or sets the status date time
        /// </summary>
        public DateTime StatusDateTime
        {
            get
            {
                return this.statusDateTime;
            }

            set
            {
                this.statusDateTime = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the status
        /// </summary>
        public string Status
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}