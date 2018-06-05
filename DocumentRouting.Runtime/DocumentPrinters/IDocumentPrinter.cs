//------------------------------------------------------------------------------
// <copyright file="IDocumentPrinter.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.Runtime
{
    using Microsoft.Dynamics.AX.Framework.DocumentRouting.OData.Entities;

    /// <summary>
    /// The interface for document printer.
    /// </summary>
    internal interface IDocumentPrinter
    {
        /// <summary>
        /// Prints the specified document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns>True if document was printed successfully.</returns>
        bool Print(IDocumentContract document);
    }
}