// <summary>
// <copyright file="ServiceConstants.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.Constants
{
    /// <summary>
    /// the class for constatns.
    /// </summary>
    public static class ServiceConstants
    {
        /// <summary>
        /// status planificada.
        /// </summary>
        public const string Planificado = "Planificado";

        /// <summary>
        /// status middleware liberado.
        /// </summary>
        public const string Liberado = "Liberado";

        /// <summary>
        /// status middleware asignado.
        /// </summary>
        public const string Asignado = "Asignado";

        /// <summary>
        /// orden de venta plan.
        /// </summary>
        public const string OrdenVentaPlan = "Orden de venta planificada";

        /// <summary>
        /// cuando se asigna un pedido.
        /// </summary>
        public const string AsignarVenta = "Se asigno el pedido a {0}";

        /// <summary>
        /// se asigna la orden.
        /// </summary>
        public const string AsignarOrden = "Se asigno la orden a {0}";

        /// <summary>
        /// orde fab plani.
        /// </summary>
        public const string OrdenFabricacionPlan = "Orden de fabricación planificada";

        /// <summary>
        /// orden fab.
        /// </summary>
        public const string OrdenFab = "OF";

        /// <summary>
        /// orden venta.
        /// </summary>
        public const string OrdenVenta = "OV";

        /// <summary>
        /// the ok value.
        /// </summary>
        public const string Ok = "Ok";

        /// <summary>
        /// the get with orders.
        /// </summary>
        public const string GetOrderWithDetail = "getDetails";

        /// <summary>
        /// gets the order by product item and product order.
        /// </summary>
        public const string GetProdOrderByOrderItem = "getProductionOrderItem";

        /// <summary>
        /// gets the formula for each order.
        /// </summary>
        public const string GetFormula = "formula/";

        /// <summary>
        /// the route to call the details for the details.
        /// </summary>
        public const string GetFabOrdersByPedidoId = "detail/{0}";

        /// <summary>
        /// const for error whne inserting fab orde.
        /// </summary>
        public const string ErrorCreateFabOrd = "ErrorCreateFabOrd";

        /// <summary>
        /// the error when update a order fab.
        /// </summary>
        public const string ErrorUpdateFavOrd = "ErrorUpdateFabOrd";

        /// <summary>
        /// if there were error while inserting.
        /// </summary>
        public const string ErrorAlInsertar = "Error al insertar";

        /// <summary>
        /// error al asignar.
        /// </summary>
        public const string ErroAlAsignar = "Error al asignar";

        /// <summary>
        /// the en proceso status.
        /// </summary>
        public const string ProcesoStatus = "En Proceso";

        /// <summary>
        /// if the type is pedido.
        /// </summary>
        public const string TypePedido = "Pedido";

        /// <summary>
        /// Status liberado.
        /// </summary>
        public const string StatusSapLiberado = "R";

        /// <summary>
        /// route to create orders.
        /// </summary>
        public const string CreateFabOrder = "createFabOrder";

        /// <summary>
        /// route to update faborder.
        /// </summary>
        public const string UpdateFabOrder = "updateFabOrder";
    }
}
