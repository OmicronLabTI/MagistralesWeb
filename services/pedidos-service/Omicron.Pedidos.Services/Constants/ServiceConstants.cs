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
        public const string Planificada = "Planificado";

        /// <summary>
        /// orden de venta plan.
        /// </summary>
        public const string OrdenVentaPlan = "Orden de venta planificada";

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
        /// const for error whne inserting fab orde.
        /// </summary>
        public const string ErrorCreateFabOrd = "ErrorCreateFabOrd";

        /// <summary>
        /// if there were error while inserting.
        /// </summary>
        public const string ErrorAlInsertar = "Error al insertar";
    }
}
