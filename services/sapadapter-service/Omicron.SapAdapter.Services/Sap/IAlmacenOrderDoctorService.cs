// <summary>
// <copyright file="IAlmacenOrderDoctorService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Services.Sap
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.SapAdapter.Dtos.Models;
    using Omicron.SapAdapter.Entities.Model;

    /// <summary>
    /// interface for Almacen order doctor.
    /// </summary>
    public interface IAlmacenOrderDoctorService
    {
        /// <summary>
        /// Gets the advance look up.
        /// </summary>
        /// <param name="parameters">the parameters.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> SearchAlmacenOrdersByDoctor(Dictionary<string, string> parameters);

        /// <summary>
        /// Gets the details.
        /// </summary>
        /// <param name="details">the details.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> SearchAlmacenOrdersDetailsByDoctor(DoctorOrdersSearchDeatilDto details);

        /// <summary>
        /// Gets the advance look up.
        /// </summary>
        /// <param name="saleorderid">the parameters.</param>
        /// <returns>the data.</returns>
        Task<ResultModel> GetOrderdetail(int saleorderid);
    }
}