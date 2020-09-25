// <summary>
// <copyright file="ISapFileService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.SapFile
{
    using System.Threading.Tasks;
    using Omicron.Pedidos.Entities.Model;

    /// <summary>
    /// Interface User Service.
    /// </summary>
    public interface ISapFileService
    {
        /// <summary>
        /// gets the data.
        /// </summary>
        /// <param name="data">the data.</param>
        /// <param name="route">the route.</param>
        /// <returns>the returns.</returns>
        Task<ResultModel> PostSimple(object data, string route);
    }
}
