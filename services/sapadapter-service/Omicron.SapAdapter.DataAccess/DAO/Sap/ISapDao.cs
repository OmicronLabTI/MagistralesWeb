// <summary>
// <copyright file="ISapDao.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.DataAccess.DAO.Sap
{
    using Omicron.SapAdapter.Entities.Model;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// the IsapDao.
    /// </summary>
    public interface ISapDao
    {
        /// <summary>
        /// Get the orders.
        /// </summary>
        /// <returns>get the orders.</returns>
        Task<IEnumerable<CompleteOrder>> GetAllOrders(DateTime date);

    }
}
