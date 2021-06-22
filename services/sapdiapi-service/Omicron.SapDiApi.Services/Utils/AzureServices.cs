// <summary>
// <copyright file="AzureServices.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapDiApi.Services.Utils
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Azure.Storage;
    using Microsoft.Azure.Storage.Auth;
    using Microsoft.Azure.Storage.Blob;

    public class AzureServices
    {
        private readonly string azureAccount;

        private readonly string azureKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureServices"/> class.
        /// </summary>
        /// <param name="account">Object to mapper.</param>
        /// <param name="key">Object to modelDao.</param>
        public AzureServices(string account, string key)
        {
            this.azureAccount = account;
            this.azureKey = key;
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
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
