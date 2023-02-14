// <summary>
// <copyright file="CompleteOrderModelWrap.cs" company="Axity">
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
    public class CompleteOrderModelWrap
    {
        /// <summary>
        /// Gets or sets OrderModel.
        /// </summary>
        /// <value>
        /// OrderModel OrderModel.
        /// </value>
        public OrderModel OrderModel { get; set; }

        /// <summary>
        /// Gets or sets ClientCatalogModel.
        /// </summary>
        /// <value>
        /// ClientCatalogModel ClientCatalogModel.
        /// </value>
        public ClientCatalogModel ClientCatalogModel { get; set; }

        /// <summary>
        /// Gets or sets AsesorModel.
        /// </summary>
        /// <value>
        /// AsesorModel AsesorModel.
        /// </value>
        public AsesorModel AsesorModel { get; set; }

        /// <summary>
        /// Gets or sets DetallePedidoModel.
        /// </summary>
        /// <value>
        /// DetallePedidoModel DetallePedidoModel.
        /// </value>
        public DetallePedidoModel DetallePedidoModel { get; set; }
    }
}
