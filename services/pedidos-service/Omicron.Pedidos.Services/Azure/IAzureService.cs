// <summary>
// <copyright file="IAzureService.cs" company="Axity">
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
        /// <param name="filesToUpload">Files To Upload.</param>
        /// <returns>the stream.</returns>
        Task<bool> UploadElementToAzure(string azureAccount, string azureKey, Tuple<string, MemoryStream, string> filesToUpload);

        /// <summary>
        /// Validates if the file exist.
        /// </summary>
        /// <param name="azureAccount">the account.</param>
        /// <param name="azureKey">the key.</param>
        /// <param name="fileNames">the complete rote.</param>
        /// <returns>the data.</returns>
        Task<List<Tuple<string, bool>>> ValidateIfExist(string azureAccount, string azureKey, List<string> fileNames);
    }
}
