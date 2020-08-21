// <summary>
// <copyright file="ISapDao.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.DataAccess.DAO.Sap
{    
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.SapAdapter.Entities.Model;
    using Omicron.SapAdapter.Entities.Model.DbModels;
    using Omicron.SapAdapter.Entities.Model.JoinsModels;

    /// <summary>
    /// the IsapDao.
    /// </summary>
    public interface ISapDao
    {
        /// <summary>
        /// Get the orders.
        /// </summary>
        /// <returns>get the orders.</returns>
        Task<IEnumerable<CompleteOrderModel>> GetAllOrdersByFechaIni(DateTime initDate, DateTime endDate);

        /// <summary>
        /// Get the orders.
        /// </summary>
        /// <returns>get the orders.</returns>
        Task<IEnumerable<CompleteOrderModel>> GetAllOrdersByFechaFin(DateTime initDate, DateTime endDate);

        /// <summary>
        /// Get the orders.
        /// </summary>
        /// <returns>get the orders.</returns>
        Task<List<OrderModel>> GetOrdersById(int pedidoID);

        /// <summary>
        /// gets the details.
        /// </summary>
        /// <param name="pedidoId">PedidoID</param>
        /// <returns>the details.</returns>
        Task<IEnumerable<CompleteDetailOrderModel>> GetAllDetails(int pedidoId);

        /// <summary>
        /// Get the orders.
        /// </summary>
        /// <returns>get the orders.</returns>
        Task<IEnumerable<CompleteOrderModel>> GetAllOrdersById(int id);

        /// <summary>
        /// gets the orders by product and item.
        /// </summary>
        /// <param name="pedidoId">the product id.</param>
        /// <param name="productId">the product id.</param>
        /// <returns>the data.</returns>
        Task<OrdenFabricacionModel> GetProdOrderByOrderProduct(int pedidoId, string productId);

        /// <summary>
        /// gets the orders by orderid.
        /// </summary>
        /// <param name="pedidoId">the product id.</param>        
        /// <returns>the data.</returns>
        Task<IEnumerable<OrdenFabricacionModel>> GetFabOrderById(List<int> pedidoId);

        /// <summary>
        /// gets the realtion between WOR1, OITM ans OITW.
        /// </summary>
        /// <param name="orderId">the order id.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<CompleteDetalleFormulaModel>> GetDetalleFormula(int orderId);

        /// <summary>
        /// gets the sap user.
        /// </summary>
        /// <param name="userId">the user id.</param>
        /// <returns>the data.</returns>
        Task<Users> GetSapUserById(int userId);

        /// <summary>
        /// Gets the value for the item code by filters. 
        /// </summary>
        /// <param name="value">the value to look.</param>
        /// <returns>the value.</returns>
        Task<IEnumerable<CompleteDetalleFormulaModel>> GetItemsByContainsItemCode(string value);

        /// <summary>
        /// Gets the value for the item code by filters. 
        /// </summary>
        /// <param name="value">the value to look.</param>
        /// <returns>the value.</returns>
        Task<IEnumerable<CompleteDetalleFormulaModel>> GetItemsByContainsDescription(string value);

        /// <summary>
        /// Gets the pedidos from the Detalle pedido.
        /// </summary>
        /// <param name="pedidoId">the pedido id.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<DetallePedidoModel>> GetPedidoById(int pedidoId);

        /// <summary>
        /// Gets the pedidos from the Detalle pedido.
        /// </summary>
        /// <param name="orderId">the pedido id.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<CompleteDetalleFormulaModel>> GetComponentByBatches(int orderId);

        /// <summary>
        /// Gets the item by code.
        /// </summary>
        /// <param name="itemCode">the item code.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<ProductoModel>> GetProductById(string itemCode);

        /// <summary>
        /// gets the valid batches by item.
        /// </summary>
        /// <param name="itemCode">the item code.</param>
        /// <param name="warehouse">the warehouse.</param>
        /// <returns>the data.</returns>
        Task<IEnumerable<CompleteBatchesJoinModel>> GetValidBatches(string itemCode, string warehouse);
    }
}
