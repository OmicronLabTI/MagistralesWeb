// <summary>
// <copyright file="SapService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Services.Sap
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Omicron.SapAdapter.DataAccess.DAO.Sap;
    using Omicron.SapAdapter.Entities.Model;
    using Omicron.SapAdapter.Services.Utils;

    /// <summary>
    /// The sap class.
    /// </summary>
    public class SapService : ISapService
    {
        private readonly ISapDao sapDao;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapService"/> class.
        /// </summary>
        /// <param name="sapDao">sap dao.</param>
        public SapService(ISapDao sapDao)
        {
            this.sapDao = sapDao ?? throw new ArgumentNullException(nameof(sapDao));
        }

        /// <summary>
        /// Get the orders.
        /// </summary>
        /// <returns>get the orders.</returns>
        public async Task<ResultModel> GetOrders()
        {
            var orders = await this.sapDao.GetAllOrders();

            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, orders, null);
        }
    }
}
