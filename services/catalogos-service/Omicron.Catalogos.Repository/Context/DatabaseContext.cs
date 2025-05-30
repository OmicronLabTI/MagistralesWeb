// <summary>
// <copyright file="DatabaseContext.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.Entities.Context
{
    using Microsoft.EntityFrameworkCore;
    using Omicron.Catalogos.Entities.Model;
    using Omicron.Catalogos.Entities.Interceptor;

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

        /// <inheritdoc/>
        public virtual DbSet<UserModel> CatUser { get; set; }

        /// <summary>
        /// Gets or sets the roles tables.
        /// </summary>
        /// <value>
        /// The roles tables.
        /// </value>
        public virtual DbSet<RoleModel> RoleModel { get; set; }

        /// <summary>
        /// Gets or sets parameters model.
        /// </summary>
        /// <value>
        /// Object parameters model.
        /// </value>
        public virtual DbSet<ParametersModel> ParametersModel { get; set; }

        /// <summary>
        /// Gets or sets parameters model.
        /// </summary>
        /// <value>
        /// Object parameters model.
        /// </value>
        public virtual DbSet<ClassificationQfbModel> ClassificationQfbModel { get; set; }

        /// <summary>
        /// Gets or sets warehouses model.
        /// </summary>
        /// <value>
        /// Object warehouses model.
        /// </value>
        public virtual DbSet<WarehouseModel> WarehousesModel { get; set; }

        /// <summary>
        /// Gets or sets sortingroute model.
        /// </summary>
        /// <value>
        /// Object sortingroute model.
        /// </value>
        public virtual DbSet<SortingRouteModel> SortingRouteModel { get; set; }

        /// <inheritdoc/>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // optionsBuilder.AddInterceptors(new UtcDateTimeInterceptor(), new UtcDateTimeQueryInterceptor());
        }
    }
}
