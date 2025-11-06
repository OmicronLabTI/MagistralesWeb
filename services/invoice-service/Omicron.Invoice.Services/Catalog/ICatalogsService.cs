// <summary>
// <copyright file="ICatalogsService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Services.Catalog
{
    using System.Threading.Tasks;

    /// <summary>
    /// Contract for catalog service.
    /// </summary>
    public interface ICatalogsService
    {
        /// <summary>
        /// Method for get parameters by name.
        /// </summary>
        /// <param name="route">Parameter names.</param>
        /// <returns>Parameters.</returns>
        Task<ResultDto> GetParams(string route);

        /// <summary>
        /// Method for get parameters by name.
        /// </summary>
        /// <param name="dataToSend">Parameter names.</param>
        /// <param name="route">route.</param>
        /// <returns>Parameters.</returns>
        Task<ResultDto> PostCatalogs(object dataToSend, string route);
    }
}
