// <summary>
// <copyright file="DatabaseContext.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Entities.Context
{
    using Microsoft.EntityFrameworkCore;
    using Omicron.Pedidos.Entities.Model;
    using Omicron.Pedidos.Entities.Model.Db;

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
        /// Gets or sets CatUser.
        /// </summary>
        /// <value>
        /// Object UserModel CatUser.
        /// </value>
        public virtual DbSet<UserOrderModel> UserOrderModel { get; set; }

        /// <summary>
        /// Gets or sets CatUser.
        /// </summary>
        /// <value>
        /// Object UserModel CatUser.
        /// </value>
        public virtual DbSet<OrderLogModel> OrderLogModel { get; set; }

        /// <summary>
        /// Gets or sets UserOrderSignature.
        /// </summary>
        /// <value>
        /// Object UserModel UserOrderSignature.
        /// </value>
        public virtual DbSet<UserOrderSignatureModel> UserOrderSignatureModel { get; set; }

        /// <summary>
        /// Gets or sets custom component lists.
        /// </summary>
        /// <value>
        /// Object custom lists.
        /// </value>
        public virtual DbSet<CustomComponentListModel> CustomComponentLists { get; set; }

        /// <summary>
        /// Gets or sets components of custom lists .
        /// </summary>
        /// <value>
        /// Object componets of custom lists.
        /// </value>
        public virtual DbSet<ComponentCustomComponentListModel> ComponentsCustomComponentLists { get; set; }

        /// <summary>
        /// Gets or sets components of custom lists .
        /// </summary>
        /// <value>
        /// Object componets of custom lists.
        /// </value>
        public virtual DbSet<ParametersModel> ParametersModel { get; set; }

        /// <summary>
        /// Gets or sets components of custom lists .
        /// </summary>
        /// <value>
        /// Object componets of custom lists.
        /// </value>
        public virtual DbSet<ProductionOrderQr> ProductionOrderQr { get; set; }

        /// <summary>
        /// Gets or sets components of custom lists .
        /// </summary>
        /// <value>
        /// Object componets of custom lists.
        /// </value>
        public virtual DbSet<ProductionRemisionQrModel> ProductionRemisionQrModel { get; set; }

        /// <summary>
        /// Gets or sets components of custom lists .
        /// </summary>
        /// <value>
        /// Object componets of custom lists.
        /// </value>
        public virtual DbSet<ProductionFacturaQrModel> ProductionFacturaQrModel { get; set; }
    }
}
