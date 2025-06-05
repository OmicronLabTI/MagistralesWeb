// <summary>
// <copyright file="DetailOrderJoinModelWrap.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Entities.Model.Wraps
{
    using Omicron.SapAdapter.Entities.Model.DbModels;

    /// <summary>
    /// Class for the dao.
    /// </summary>
    public class DetailOrderJoinModelWrap
    {
        /// <summary>
        /// Gets or sets DetallePedidoModel.
        /// </summary>
        /// <value>
        /// DetallePedidoModel DetallePedidoModel.
        /// </value>
        public DetallePedidoModel DetallePedidoModel { get; set; }

        /// <summary>
        /// Gets or sets OrdenFabricacionModel.
        /// </summary>
        /// <value>
        /// OrdenFabricacionModel.
        /// </value>
        public OrdenFabricacionModel OrdenFabricacionModel { get; set; }

        /// <summary>
        /// Gets or sets ProductoModel.
        /// </summary>
        /// <value>
        /// ProductoModel.
        /// </value>
        public ProductoModel ProductoModel { get; set; }

        /// <summary>
        /// Gets or sets ProductFirmModel.
        /// </summary>
        /// <value>
        /// ProductFirmModel.
        /// </value>
        public ProductFirmModel ProductFirmModel { get; set; }

        /// <summary>
        /// Gets or sets OrderModel.
        /// </summary>
        /// <value>
        /// OrderModel.
        /// </value>
        public OrderModel OrderModel { get; set; }

        /// <summary>
        /// Gets or sets CatalogProductModel.
        /// </summary>
        /// <value>
        /// CatalogProductModel.
        /// </value>
        public CatalogProductModel CatalogProductModel { get; set; }
    }
}
