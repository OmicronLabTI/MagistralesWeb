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
    using Omicron.SapAdapter.Entities.Model.DbModels;

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
        /// Gets or sets salesPersonModel.
        /// </summary>
        /// <value>
        /// Object salesPersonModel.
        /// </value>
        public virtual DbSet<SalesPersonModel> SalesPersonModel { get; set; }

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
        /// Gets or sets OrderModel.
        /// </summary>
        /// <value>
        /// Object UserModel OrderModel.
        /// </value>
        public virtual DbSet<DetalleFormulaModel> DetalleFormulaModel { get; set; }

        /// <summary>
        /// Gets or sets OrderModel.
        /// </summary>
        /// <value>
        /// Object UserModel OrderModel.
        /// </value>
        public virtual DbSet<ItemWarehouseModel> ItemWarehouseModel { get; set; }

        /// <summary>
        /// Gets or sets OrderModel.
        /// </summary>
        /// <value>
        /// Object UserModel OrderModel.
        /// </value>
        public virtual DbSet<Users> Users { get; set; }

        /// <summary>
        /// Gets or sets OrderModel.
        /// </summary>
        /// <value>
        /// Object UserModel OrderModel.
        /// </value>
        public virtual DbSet<BatchesQuantity> BatchesQuantity { get; set; }

        /// <summary>
        /// Gets or sets OrderModel.
        /// </summary>
        /// <value>
        /// Object UserModel OrderModel.
        /// </value>
        public virtual DbSet<Batches> Batches { get; set; }

        /// <summary>
        /// Gets or sets OrderModel.
        /// </summary>
        /// <value>
        /// Object UserModel OrderModel.
        /// </value>
        public virtual DbSet<BatchesTransactionQtyModel> BatchesTransactionQtyModel { get; set; }

        /// <summary>
        /// Gets or sets OrderModel.
        /// </summary>
        /// <value>
        /// Object UserModel OrderModel.
        /// </value>
        public virtual DbSet<BatchTransacitions> BatchTransacitions { get; set; }

        /// <summary>
        /// Gets or sets OrderModel.
        /// </summary>
        /// <value>
        /// Object UserModel OrderModel.
        /// </value>
        public virtual DbSet<AttachmentModel> AttachmentModel { get; set; }

        /// <summary>
        /// model creating.
        /// </summary>
        /// <param name="modelBuilder">the builder.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DetallePedidoModel>().HasKey(table => new
            {
                table.PedidoId,
                table.DetalleId,
            });

            modelBuilder.Entity<DetalleFormulaModel>().HasKey(table => new
            {
                table.OrderFabId,
                table.LineNum,
            });

            modelBuilder.Entity<ItemWarehouseModel>().HasKey(table => new
            {
                table.ItemCode,
                table.WhsCode,
            });

            modelBuilder.Entity<BatchesTransactionQtyModel>().HasKey(table => new
            {
                table.LogEntry,
                table.ItemCode,
                table.SysNumber,
            });

            modelBuilder.Entity<AttachmentModel>().HasKey(table => new
            {
                table.AbsEntry,
                table.Line,
            });
        }
    }
}
