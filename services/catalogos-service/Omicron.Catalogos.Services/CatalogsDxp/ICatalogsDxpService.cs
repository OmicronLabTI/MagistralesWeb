// <summary>
// <copyright file="ICatalogsDxpService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.Services.CatalogsDxp
{
    /// <summary>
    /// Interface catalogs dxp.
    /// </summary>
    public interface ICatalogsDxpService
    {
        /// <summary>
        /// Asynchronously sends a POST request to the catalogs dxp service.
        /// </summary>
        /// <param name="data"> Data to send in the POST request. </param>
        /// <param name="route"> Catalogs dxp service endpoint URL. </param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation, containing a <see cref="ResultDto"/> with the response data from the catalogs dxp.</returns>
        Task<ResultDto> Post(object data, string route);

        /// <summary>
        /// Asynchronously sends a GET request to the catalogs dxp service.
        /// </summary>
        /// <param name="route"> Catalogs dxp service endpoint URL. </param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation, containing a <see cref="ResultDto"/> with the response data from the catalogs dxp.</returns>
        Task<ResultDto> Get(string route);
    }
}
