// <summary>
// <copyright file="AzureService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.Services.Catalogs
{
    /// <summary>
    /// Class Azure Service.
    /// </summary>
    public class AzureService : IAzureService
    {
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureService"/> class.
        /// </summary>
        /// <param name="logger">Logger instance for logging operations.</param>
        public AzureService(ILogger logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task GetElementsFromAzure(string account, string key, string url, Stream streamOut)
        {
            try
            {
                var blobUir = new Uri(url);
                var credentials = new StorageSharedKeyCredential(account, key);
                var blobClient = new BlobClient(blobUir, credentials);
                await blobClient.DownloadToAsync(streamOut);
                streamOut.Position = 0;
            }
            catch (Exception ex)
            {
                this.logger.Error(string.Format(ServiceConstants.ErrorDownload, url, ex.Message, ex.StackTrace));
            }
        }
    }
}
