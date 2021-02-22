// <summary>
// <copyright file="IReportingService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.Reporting
{
    using System.Threading.Tasks;
    using Omicron.Pedidos.Entities.Model;

    /// <summary>
    /// reporting service.
    /// </summary>
    public interface IReportingService
    {
        /// <summary>
        /// get orders with the data.
        /// </summary>
        /// <param name="dataToSend">the assesor data.</param>
        /// <param name="route">route to send.</param>
        /// <returns>the return.</returns>
        Task<ResultModel> PostReportingService(object dataToSend, string route);

        /// <summary>
        /// Makes a get to reporting services.
        /// </summary>
        /// <param name="route">the route to send.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetReportingService(string route);
    }
}
