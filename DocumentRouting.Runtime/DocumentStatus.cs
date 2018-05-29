//--------------------------------------------------------------------------
//  <copyright file="DocumentStatus.cs"  company="Microsoft Corporation">
//      Copyright (c) Microsoft Corporation. All rights reserved.
//  </copyright>
//--------------------------------------------------------------------------
namespace Microsoft.Dynamics.AX.Framework.DocumentRouting
{
    /// <summary>
    /// Document status enumeration
    /// </summary>
    public enum DocumentStatus
    {
        /// <summary>
        /// Pending status
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Printing status
        /// </summary>
        Printing = 1,

        /// <summary>
        /// Success status
        /// </summary>
        Success = 2,

        /// <summary>
        /// Failure status
        /// </summary>
        Failure = 3
    }
}