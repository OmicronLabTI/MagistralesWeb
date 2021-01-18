// <summary>
// <copyright file="IDatabaseContext.cs" company="Axity">
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
    /// Interface IDataBaseContext.
    /// </summary>
    public interface IDatabaseContext
    {
        /// <summary>
        /// Gets or sets OrderModel.
        /// </summary>
        /// <value>
        /// Object UserModel OrderModel.
        /// </value>
        DbSet<OrderModel> OrderModel { get; set; }

        /// <summary>
        /// Gets or sets OrderModel.
        /// </summary>
        /// <value>
        /// Object UserModel OrderModel.
        /// </value>
        DbSet<AsesorModel> AsesorModel { get; set; }

        /// <summary>
        /// Gets or sets OrderModel.
        /// </summary>
        /// <value>
        /// Object UserModel OrderModel.
        /// </value>
        DbSet<DetallePedidoModel> DetallePedido { get; set; }

        /// <summary>
        /// Gets or sets OrderModel.
        /// </summary>
        /// <value>
        /// Object UserModel OrderModel.
        /// </value>
        DbSet<ProductoModel> ProductoModel { get; set; }

        /// <summary>
        /// Gets or sets OrderModel.
        /// </summary>
        /// <value>
        /// Object UserModel OrderModel.
        /// </value>
        DbSet<OrdenFabricacionModel> OrdenFabricacionModel { get; set; }

        /// <summary>
        /// Gets or sets OrderModel.
        /// </summary>
        /// <value>
        /// Object UserModel OrderModel.
        /// </value>
        DbSet<DetalleFormulaModel> DetalleFormulaModel { get; set; }

        /// <summary>
        /// Gets or sets OrderModel.
        /// </summary>
        /// <value>
        /// Object UserModel OrderModel.
        /// </value>
        DbSet<ItemWarehouseModel> ItemWarehouseModel { get; set; }

        /// <summary>
        /// Gets or sets OrderModel.
        /// </summary>
        /// <value>
        /// Object UserModel OrderModel.
        /// </value>
        DbSet<Users> Users { get; set; }

        /// <summary>
        /// Gets or sets OrderModel.
        /// </summary>
        /// <value>
        /// Object UserModel OrderModel.
        /// </value>
        DbSet<BatchesQuantity> BatchesQuantity { get; set; }

        /// <summary>
        /// Gets or sets OrderModel.
        /// </summary>
        /// <value>
        /// Object UserModel OrderModel.
        /// </value>
        DbSet<Batches> Batches { get; set; }

        /// <summary>
        /// Gets or sets OrderModel.
        /// </summary>
        /// <value>
        /// Object UserModel OrderModel.
        /// </value>
        DbSet<BatchesTransactionQtyModel> BatchesTransactionQtyModel { get; set; }

        /// <summary>
        /// Gets or sets OrderModel.
        /// </summary>
        /// <value>
        /// Object UserModel OrderModel.
        /// </value>
        DbSet<BatchTransacitions> BatchTransacitions { get; set; }

        /// <summary>
        /// Gets or sets OrderModel.
        /// </summary>
        /// <value>
        /// Object UserModel OrderModel.
        /// </value>
        DbSet<AttachmentModel> AttachmentModel { get; set; }

        /// <summary>
        /// Gets or sets OrderModel.
        /// </summary>
        /// <value>
        /// Object UserModel OrderModel.
        /// </value>
        DbSet<DeliveryDetailModel> DeliveryDetailModel { get; set; }

        /// <summary>
        /// Gets or sets OrderModel.
        /// </summary>
        /// <value>
        /// Object UserModel OrderModel.
        /// </value>
        DbSet<DeliverModel> DeliverModel { get; set; }

        /// <summary>
        /// Gets or sets OrderModel.
        /// </summary>
        /// <value>
        /// Object UserModel OrderModel.
        /// </value>
        DbSet<InvoiceHeaderModel> InvoiceHeaderModel { get; set; }

        /// <summary>
        /// Gets or sets OrderModel.
        /// </summary>
        /// <value>
        /// Object UserModel OrderModel.
        /// </value>
        DbSet<InvoiceDetailModel> InvoiceDetailModel { get; set; }
    }
}
