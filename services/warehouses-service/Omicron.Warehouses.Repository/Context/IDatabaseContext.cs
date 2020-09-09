// <summary>
// <copyright file="IDatabaseContext.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Warehouses.Entities.Context
{
    using Microsoft.EntityFrameworkCore;
    using Omicron.Warehouses.Entities.Model;

    /// <summary>
    /// Interface IDataBaseContext.
    /// </summary>
    public interface IDatabaseContext
    {
        /// <summary>
        /// Gets or sets raw material requests.
        /// </summary>
        /// <value>
        /// Object requests.
        /// </value>
        DbSet<RawMaterialRequestModel> RawMaterialRequests { get; set; }

        /// <summary>
        /// Gets or sets raw material request details.
        /// </summary>
        /// <value>
        /// Object request details.
        /// </value>
        DbSet<RawMaterialRequestDetailModel> RawMaterialRequestDetails { get; set; }
    }
}
