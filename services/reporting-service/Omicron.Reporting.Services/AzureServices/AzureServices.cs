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
    using System.IO;
    using System.Threading.Tasks;
    using Azure.Storage;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using Omicron.Reporting.Resources.Exceptions;

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

        /// <inheritdoc/>
        public async Task<bool> UploadElementToAzure(string azureAccount, string azureKey, string url, Stream fileStream, string contentType)
        {
            try
            {
                var blobUir = new Uri(url);
                var credentials = new StorageSharedKeyCredential(azureAccount, azureKey);
                var blobClient = new BlobClient(blobUir, credentials);
                var config = new BlobHttpHeaders
                {
                    ContentType = contentType,
                };

                await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
                await blobClient.UploadAsync(fileStream, config);
                return true;
            }
            catch (Exception ex)
            {
                throw new CustomServiceException(ex.Message, System.Net.HttpStatusCode.BadRequest);
            }
        }
    }
}
