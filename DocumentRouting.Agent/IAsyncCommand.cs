//------------------------------------------------------------------------------
// <copyright file="IAsyncCommand.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting
{
    using System.Threading.Tasks;
    using System.Windows.Input;

    /// <summary>
    /// The interface for asynchronus command
    /// </summary>
    public interface IAsyncCommand : ICommand
    {
        /// <summary>
        /// Executes the asynchronous.
        /// </summary>
        /// <returns>The task</returns>
        Task ExecuteAsync();
    }
}
