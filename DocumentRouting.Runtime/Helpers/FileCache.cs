//------------------------------------------------------------------------------
// <copyright file="FileCache.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Dynamics.AX.Framework.DocumentRouting.Runtime
{
    using System.IO;
    using System.Security.Cryptography;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;

    /// <summary>
    /// Provides a token cache using DPAPI
    /// </summary>
    internal class FileCache : TokenCache
    {
        private const string TokenFileName = "TokenCache.dat";

        /// <summary>
        /// Serves as a read/write file lock
        /// </summary>
        private static readonly object FileLock = new object();

        /// <summary>
        /// The name of the file used for the cache.
        /// </summary>
        private readonly string cacheFilePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCache" /> class.
        /// </summary>
        public FileCache()
        {
            // set the member
            this.cacheFilePath = Path.Combine(Path.GetTempPath(), FileCache.TokenFileName);

            // set notification method properties
            this.BeforeAccess = this.BeforeAccessNotification;
            this.AfterAccess = this.AfterAccessNotification;

            // read the cache
            this.ReadFileCache();
        }

        /// <summary>
        /// Clears and empties the store.
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            if (File.Exists(this.cacheFilePath))
            {
                File.Delete(this.cacheFilePath);
            }
        }

        /// <summary>
        /// Called by ADAL before cache access.
        /// </summary>
        /// <param name="args"><c>TokenCacheNotificationArgs</c> argument object.</param>
        public void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            this.ReadFileCache();
        }

        /// <summary>
        /// Called by ADAL after cache access.
        /// </summary>
        /// <param name="args"><c>TokenCacheNotificationArgs</c> argument object.</param>
        public void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            if (this.HasStateChanged)
            {
                this.WriteFileCache();
            }
        }

        /// <summary>
        /// Reads the file cache.
        /// </summary>
        private void ReadFileCache()
        {
            // read the cache
            lock (FileLock)
            {
                byte[] contents = null;
                if (File.Exists(this.cacheFilePath))
                {
                    contents = ProtectedData.Unprotect(File.ReadAllBytes(this.cacheFilePath), null, DataProtectionScope.CurrentUser);
                }

                this.Deserialize(contents);
            }
        }

        /// <summary>
        /// Writes current info to store.
        /// </summary>
        private void WriteFileCache()
        {
            lock (FileLock)
            {
                // Persist any changes to the store
                File.WriteAllBytes(this.cacheFilePath, ProtectedData.Protect(this.Serialize(), null, DataProtectionScope.CurrentUser));

                // Flip the flag
                this.HasStateChanged = false;
            }
        }
    }
}
