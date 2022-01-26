// <summary>
// <copyright file="AzureServices.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Reporting.Services.AzureServices
{
    using System;
    using System.Threading.Tasks;
    using Azure.Storage;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;

    /// <summary>
    /// Class for azure.
    /// </summary>
    public class AzureServices : IAzureService
    {
        /// <inheritdoc/>
        public async Task<BlobDownloadInfo> GetlementFromAzure(string azureAccount, string azureKey, string urlToDownload)
        {
            try
            {
                var blobUir = new Uri(urlToDownload);
                var credentials = new StorageSharedKeyCredential(azureAccount, azureKey);
                var blobClient = new BlobClient(blobUir, credentials);
                var response = await blobClient.DownloadAsync();
                return response?.Value;
            }
            catch
            {
                return null;
            }
        }
    }
}
