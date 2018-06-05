//------------------------------------------------------------------------------
// <copyright file="DocumentRoutingAgentViewModel.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows.Data;
    using System.Windows.Input;
    using Microsoft.Dynamics.AX.Framework.DocumentRouting.OData;
    using Microsoft.Dynamics.AX.Framework.DocumentRouting.Runtime;

    /// <summary>
    /// The document routing agent view model.
    /// </summary>
    public class DocumentRoutingAgentViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Document statuses lock
        /// </summary>
        private readonly object documentStatusesLock = new object();

        /// <summary>
        /// The dialog view controller
        /// </summary>
        private readonly IDialogViewController dialogViewController;

        /// <summary>
        /// The settings manager
        /// </summary>
        private readonly ISettingsManager settingsManager;

        /// <summary>
        /// The error handler strategy
        /// </summary>
        private readonly Action<string> errorHandlerStrategy;

        /// <summary>
        /// The authorization manager
        /// </summary>
        private readonly IAuthorizationManager authorizationManager;

        /// <summary>
        /// The is signed in
        /// </summary>
        private bool isSignedIn;

        /// <summary>
        /// The agent status
        /// </summary>
        private StatusBarMessageValue status;

        /// <summary>
        /// The notification hub
        /// </summary>
        private NotificationHub notificationHub;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentRoutingAgentViewModel" /> class.
        /// </summary>
        /// <param name="errorHandlerStrategy">The error handler strategy.</param>
        public DocumentRoutingAgentViewModel(Action<string> errorHandlerStrategy)
            : this(new DialogViewController(), new SettingsManager(), errorHandlerStrategy)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentRoutingAgentViewModel" /> class.
        /// </summary>
        /// <param name="dialogViewController">The dialog view controller.</param>
        /// <param name="settingsManager">The settings manager.</param>
        /// <param name="errorHandlerStrategy">The error handler strategy.</param>
        public DocumentRoutingAgentViewModel(IDialogViewController dialogViewController, ISettingsManager settingsManager, Action<string> errorHandlerStrategy)
            : this(dialogViewController, settingsManager, new AuthorizationManager(settingsManager), errorHandlerStrategy)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentRoutingAgentViewModel" /> class.
        /// </summary>
        /// <param name="dialogViewController">The dialog view controller.</param>
        /// <param name="settingsManager">The settings manager.</param>
        /// <param name="authorizationManager">The authorization manager.</param>
        /// <param name="errorHandlerStrategy">The error handler strategy.</param>
        public DocumentRoutingAgentViewModel(
            IDialogViewController dialogViewController,
            ISettingsManager settingsManager,
            IAuthorizationManager authorizationManager,
            Action<string> errorHandlerStrategy)
        {
            this.dialogViewController = dialogViewController ?? throw new ArgumentNullException(nameof(dialogViewController));
            this.settingsManager = settingsManager ?? throw new ArgumentNullException(nameof(settingsManager));
            this.errorHandlerStrategy = errorHandlerStrategy ?? throw new ArgumentNullException(nameof(errorHandlerStrategy));
            this.authorizationManager = authorizationManager ?? throw new ArgumentNullException(nameof(authorizationManager));

            this.ShowSettingsCommand = new DelegateCommand(this.ShowSettings);
            this.ShowPrintersCommand = new DelegateCommand(this.ShowPrinters);
            this.SignInCommand = new DelegateCommandAsync(this.SignIn, this.CanSignIn);
            this.SignOutCommand = new DelegateCommandAsync(this.SignOut);

            BindingOperations.EnableCollectionSynchronization(this.DocumentStatuses, this.documentStatusesLock);

            this.DocumentStatusManager = new DocumentStatusManager(this.DocumentStatuses);
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the show settings command.
        /// </summary>
        public ICommand ShowSettingsCommand { get; }

        /// <summary>
        /// Gets the show printers command.
        /// </summary>
        public ICommand ShowPrintersCommand { get; }

        /// <summary>
        /// Gets the sign in command.
        /// </summary>
        public DelegateCommandAsync SignInCommand { get; }

        /// <summary>
        /// Gets the sign out command.
        /// </summary>
        public ICommand SignOutCommand { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the agent is currently signed in or not
        /// </summary>
        public bool IsSignedIn
        {
            get
            {
                return this.isSignedIn;
            }

            set
            {
                this.isSignedIn = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public StatusBarMessageValue Status
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
        /// Gets the document status collection
        /// </summary>
        public ObservableCollection<DocumentPrintingStatusContract> DocumentStatuses { get; } = new ObservableCollection<DocumentPrintingStatusContract>();

        /// <summary>
        /// Gets or sets the document status manager.
        /// </summary>
        public DocumentStatusManager DocumentStatusManager { get; set; }

        /// <summary>
        /// Called when property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Signs out.
        /// </summary>
        private async Task SignOut()
        {
            this.authorizationManager.SignOut();
            this.IsSignedIn = false;
            this.Status = StatusBarMessageValue.SignedOut;
            await this.UnsubscribeFromEvents();
        }

        /// <summary>
        /// Signs in.
        /// </summary>
        /// <returns> The task.</returns>
        private async Task SignIn()
        {
            try
            {
                this.Status = StatusBarMessageValue.SigningIn;

                var token = await this.authorizationManager.GetAccessToken();
                if (!string.IsNullOrWhiteSpace(token))
                {
                    this.IsSignedIn = true;
                    await this.SubscribeToEvents();
                }
            }
            finally
            {
                if (this.IsSignedIn)
                {
                    this.Status = StatusBarMessageValue.SignedIn;
                }
                else
                {
                    this.Status = StatusBarMessageValue.SignedOut;
                }
            }
        }

        /// <summary>
        /// Checks the can sign in.
        /// </summary>
        /// <returns>The true if can try to sign in, otherwise false.</returns>
        private bool CanSignIn()
        {
            var settings = this.settingsManager.GetSettings();

            var result = !string.IsNullOrWhiteSpace(settings.AADInstanceUri) &&
                    !string.IsNullOrWhiteSpace(settings.AXRootUrl) &&
                    !string.IsNullOrWhiteSpace(settings.RedirectUri) &&
                    !string.IsNullOrWhiteSpace(settings.Tenant) &&
                    settings.ApplicationId != Guid.Empty;

            return result;
        }

        /// <summary>
        /// Shows the printers.
        /// </summary>
        private void ShowPrinters()
        {
            var settings = this.settingsManager.GetSettings();
            var odataClientFactory = new DocumentRoutingODataClientFactory(new Uri(settings.AXRootUrl), this.authorizationManager);
            var printerDataManager = new PrinterDataManager(odataClientFactory, new PrintersProvider()); 
            var viewModel = new PrintersViewModel(printerDataManager, this.errorHandlerStrategy);
            this.dialogViewController.ShowPrintersDialog(viewModel);
        }

        /// <summary>
        /// Shows the settings.
        /// </summary>
        private void ShowSettings()
        {
            var viewModel = new SettingsViewModel(this.settingsManager, this.errorHandlerStrategy);
            viewModel.SettingsSaved += (sender, args) => { this.SignInCommand.RaiseCanExecuteChanged(); };
            this.dialogViewController.ShowSettingsDialog(viewModel);
        }

        /// <summary>
        /// Subscribes to events.
        /// </summary>
        /// <returns>The task.</returns>
        private async Task SubscribeToEvents()
        {
            var settings = this.settingsManager.GetSettings();
            var odataClientFactory = new DocumentRoutingODataClientFactory(new Uri(settings.AXRootUrl), this.authorizationManager);

            this.notificationHub = new NotificationHub(new NotificationHandler(odataClientFactory, new PrintersProvider(), this.DocumentStatusManager), odataClientFactory);
            await this.notificationHub.SubscribeToEvents();
        }

        /// <summary>
        /// Unsubscribes from the events.
        /// </summary>
        /// <returns>The task.</returns>
        private async Task UnsubscribeFromEvents()
        {
            if (this.notificationHub != null)
            {
                await this.notificationHub.UnsubscribeFromEvents();
            }
        }
    }
}
