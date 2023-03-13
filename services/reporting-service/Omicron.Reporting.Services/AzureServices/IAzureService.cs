// <summary>
// <copyright file="IAzureService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Reporting.Services.AzureServices
{
    using System.IO;
    using System;
    using System.Threading.Tasks;
    using Azure.Storage.Blobs.Models;

    /// <summary>
    /// interface for azure.
    /// </summary>
    public interface IAzureService
    {
        /// <summary>
        /// Gets a file from azure.
        /// </summary>
        /// <param name="azureAccount">the account.</param>
        /// <param name="azureKey">the key.</param>
        /// <param name="urlToDownload">urlToDownload To download.</param>
        /// <returns>the stream.</returns>
        Task<BlobDownloadInfo> GetlementFromAzure(string azureAccount, string azureKey, string urlToDownload);

        /// <summary>
        /// Gets a file from azure.
        /// </summary>
        /// <param name="azureAccount">the account.</param>
        /// <param name="azureKey">the key.</param>
        /// <param name="url">Url.</param>
        /// <param name="fileStream">Files To Upload.</param>
        /// <param name="contentType">Content type.</param>
        /// <returns>the stream.</returns>
        Task<bool> UploadElementToAzure(string azureAccount, string azureKey, string url, Stream fileStream, string contentType);
    }
}
