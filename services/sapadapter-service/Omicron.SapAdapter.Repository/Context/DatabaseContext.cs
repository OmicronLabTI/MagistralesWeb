// <summary>
// <copyright file="DatabaseContext.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Entities.Context
{
    using Microsoft.EntityFrameworkCore;
    using Omicron.SapAdapter.Entities.Model;

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
        /// Gets or sets OrderModel.
        /// </summary>
        /// <value>
        /// Object UserModel OrderModel.
        /// </value>
        public virtual DbSet<OrderModel> OrderModel { get; set; }

        /// <summary>
        /// Gets or sets OrderModel.
        /// </summary>
        /// <value>
        /// Object UserModel OrderModel.
        /// </value>
        public virtual DbSet<AsesorModel> AsesorModel { get; set; }

        /// <summary>
        /// Gets or sets OrderModel.
        /// </summary>
        /// <value>
        /// Object UserModel OrderModel.
        /// </value>
        public virtual DbSet<DetallePedidoModel> DetallePedido { get; set; }

        /// <summary>
        /// Gets or sets OrderModel.
        /// </summary>
        /// <value>
        /// Object UserModel OrderModel.
        /// </value>
        public virtual DbSet<ProductoModel> ProductoModel { get; set; }

        /// <summary>
        /// Gets or sets OrderModel.
        /// </summary>
        /// <value>
        /// Object UserModel OrderModel.
        /// </value>
        public virtual DbSet<OrdenFabricacionModel> OrdenFabricacionModel { get; set; }

        /// <summary>
        /// model creating.
        /// </summary>
        /// <param name="builder">the builder.</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<DetallePedidoModel>().HasKey(table => new
            {
                table.PedidoId,
                table.DetalleId,
            });
        }
    }
}
