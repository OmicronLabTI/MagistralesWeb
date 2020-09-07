// <summary>
// <copyright file="DatabaseContext.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Axity.Warehouses.Entities.Context
{
    using Axity.Warehouses.Entities.Model;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Class DBcontext.
    /// </summary>
    public class DatabaseContext : DbContext, IDatabaseContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseContext"/> class.
        /// </summary>
        /// <param name="options">Connection Options.</param>
        public DatabaseContext(DbContextOptions options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets raw material requests.
        /// </summary>
        /// <value>
        /// Object requests.
        /// </value>
        public virtual DbSet<RawMaterialRequestModel> RawMaterialRequests { get; set; }

        /// <summary>
        /// Gets or sets raw material request details.
        /// </summary>
        /// <value>
        /// Object request details.
        /// </value>
        public virtual DbSet<RawMaterialRequestDetailModel> RawMaterialRequestDetails { get; set; }
    }
}
