//------------------------------------------------------------------------------
// <copyright file="StatusBarMessageValue.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting
{
    /// <summary>
    /// Contains different message values used to display status text.
    /// </summary>
    public enum StatusBarMessageValue
    {
        /// <summary>
        /// No message
        /// </summary>
        None,

        /// <summary>
        /// The waiting value
        /// </summary>
        Waiting,

        /// <summary>
        /// The checking value
        /// </summary>
        Checking,

        /// <summary>
        /// The routing value
        /// </summary>
        Routing,

        /// <summary>
        /// The signing in value
        /// </summary>
        SigningIn,

        /// <summary>
        /// The signed in value
        /// </summary>
        SignedIn,

        /// <summary>
        /// The signed out value
        /// </summary>
        SignedOut,

        /// <summary>
        /// The unable to sign in value
        /// </summary>
        UnableToSignIn,

        /// <summary>
        /// The no active printers value
        /// </summary>
        NoActivePrinters,

        /// <summary>
        /// Windows service performs document routing
        /// </summary>
        RunAsWindowsService,

        /// <summary>
        /// Document routing agent is waiting to be closed
        /// </summary>
        WaitingToClose
    }
}
