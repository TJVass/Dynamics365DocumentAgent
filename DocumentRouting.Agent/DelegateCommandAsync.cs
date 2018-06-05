//------------------------------------------------------------------------------
// <copyright file="DelegateCommandAsync.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// The delegate asynchronus command
    /// </summary>
    public class DelegateCommandAsync : IAsyncCommand
    {
        /// <summary>
        /// The command
        /// </summary>
        private readonly Func<Task> command;

        /// <summary>
        /// The can execute
        /// </summary>
        private readonly Func<bool> canExecute;

        /// <summary>
        /// The is executing
        /// </summary>
        private bool isExecuting;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommandAsync"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        public DelegateCommandAsync(Func<Task> command)
            : this(command, () => true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommandAsync"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="canExecute">The can execute.</param>
        public DelegateCommandAsync(Func<Task> command, Func<bool> canExecute)
        {
            this.command = command;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to <see langword="null" />.</param>
        /// <returns>
        ///   <see langword="true" /> if this command can be executed; otherwise, <see langword="false" />.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            return !this.isExecuting && this.canExecute();
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to <see langword="null" />.</param>
        public async void Execute(object parameter)
        {
            await this.ExecuteAsync();
        }

        /// <summary>
        /// Executes the asynchronous.
        /// </summary>
        /// <returns>The task</returns>
        public async Task ExecuteAsync()
        {
            try
            {
                this.isExecuting = true;
                this.RaiseCanExecuteChanged();
                await this.command();
            }
            finally
            {
                this.isExecuting = false;
                this.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Raises the can execute changed.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
