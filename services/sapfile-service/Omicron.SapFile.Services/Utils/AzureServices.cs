// <summary>
// <copyright file="AzureServices.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapFile.Services.Utils
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Azure.Storage.Auth;
    using Microsoft.Azure.Storage.Blob;
    using Newtonsoft.Json;
    using Omicron.SapFile.Log;

    /// <summary>
    /// Azure class.
    /// </summary>
    public class AzureServices
    {
        private readonly string azureAccount;

        private readonly string azureKey;

        private readonly ILoggerProxy _loggerProxy;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureServices"/> class.
        /// </summary>
        /// <param name="account">Object to mapper.</param>
        /// <param name="key">Object to modelDao.</param>
        /// <param name="loggerProxy">Logger Proxy.</param>
        public AzureServices(string account, string key, ILoggerProxy loggerProxy)
        {
            this.azureAccount = account;
            this.azureKey = key;
            this._loggerProxy = loggerProxy;
        }

        /// <summary>
        /// Gets a specific stream.
        /// </summary>
        /// <param name="url">the url.</param>
        /// <returns>the stram.</returns>
        public async Task SaveToPathFromAzure(string url, string fileName, string fileRoute)
        {
            try
            {
                var blobUir = new Uri(url);
                var storageCredentials = new StorageCredentials(this.azureAccount, this.azureKey);
                var container = new CloudBlobContainer(blobUir, storageCredentials);
                var blob = container.GetBlockBlobReference(fileName);
                await blob.DownloadToFileAsync(fileRoute, FileMode.Create);
            }
            catch (Exception ex)
            {
                this._loggerProxy?.Error(
                    $"Omicron.SapFile.Azure - Error to download the recipe to server in {fileRoute}. Error: {ex.Message} - {ex.InnerException} - {ex.StackTrace}");
                throw;
            }
        }
    }
}
