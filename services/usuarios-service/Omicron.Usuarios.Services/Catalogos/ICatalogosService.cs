// <summary>
// <copyright file="ICatalogosService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Usuarios.Services.Catalogos
{
    using System.Threading.Tasks;
    using Omicron.Usuarios.Entities.Model;

    /// <summary>
    /// the interface of catalogs services.
    /// </summary>
    public interface ICatalogosService
    {
        /// <summary>
        /// get orders with the data.
        /// </summary>
        /// <param name="route">route to send.</param>
        /// <returns>the return.</returns>
        Task<ResultModel> GetCatalogos(string route);
    }
}
