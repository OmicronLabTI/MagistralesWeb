// <summary>
// <copyright file="AzureServices.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.Azure
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using global::Azure.Storage;
    using global::Azure.Storage.Blobs;
    using global::Azure.Storage.Blobs.Models;

    /// <summary>
    /// Class for azure.
    /// </summary>
    public class AzureServices : IAzureService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureServices"/> class.
        /// </summary>
        public AzureServices()
        {
        }

        /// <inheritdoc/>
        public async Task<bool> UploadElementToAzure(string azureAccount, string azureKey, Tuple<string, MemoryStream, string> filesToUpload)
        {
            try
            {
                var blobUir = new Uri(filesToUpload.Item1);
                var credentials = new StorageSharedKeyCredential(azureAccount, azureKey);
                var blobClient = new BlobClient(blobUir, credentials);
                var config = new BlobHttpHeaders
                {
                    ContentType = filesToUpload.Item3,
                };

                await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
                await blobClient.UploadAsync(filesToUpload.Item2, config);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<List<Tuple<string, bool>>> ValidateIfExist(string azureAccount, string azureKey, List<string> fileNames)
        {
            var credentials = new StorageSharedKeyCredential(azureAccount, azureKey);
            Uri blobUir;
            var tupleToReturn = new List<Tuple<string, bool>>();
            foreach (var file in fileNames)
            {
                blobUir = new Uri(file);
                var blobClient = new BlobClient(blobUir, credentials);
                var exist = await blobClient.ExistsAsync();
                tupleToReturn.Add(new Tuple<string, bool>(file, exist));
            }

            return tupleToReturn;
        }
    }
}
