// <summary>
// <copyright file="IAzureService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.Services.Catalogs
{
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface Azure.
    /// </summary>
    public interface IAzureService
    {
        /// <summary>
        /// Retrieves a file from Azure storage asynchronously.
        /// </summary>
        /// <param name="account"> Azure storage account name. </param>
        /// <param name="key"> Azure storage account access key. </param>
        /// <param name="url"> URL to the file in Azure storage. </param>
        /// <param name="streamOut"> Stream to store the downloaded file. </param>
        /// <returns> A task that represents the asynchronous operation, containing the file stream. </returns>
        Task GetElementsFromAzure(string account, string key, string url, Stream streamOut);
    }
}
