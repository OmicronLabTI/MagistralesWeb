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
        /// Cancelled status.
        /// </summary>
        public const string Cancelled = "Cancelado";

        /// <summary>
        /// status middleware liberado.
        /// </summary>
        public const string Liberado = "Liberado";

        /// <summary>
        /// status middleware asignado.
        /// </summary>
        public const string Asignado = "Asignado";

        /// <summary>
        /// the proceso status.
        /// </summary>
        public const string Proceso = "Proceso";

        /// <summary>
        /// the en proceso status.
        /// </summary>
        public const string ProcesoStatus = "En Proceso";

        /// <summary>
        /// the finalizado status.
        /// </summary>
        public const string Finalizado = "Finalizado";

        /// <summary>
        /// the pendiente status.
        /// </summary>
        public const string Pendiente = "Pendiente";

        /// <summary>
        /// orden de venta plan.
        /// </summary>
        public const string OrdenVentaPlan = "Orden de venta planificada";

        /// <summary>
        /// when the order goes to Proceso.
        /// </summary>
        public const string OrdenProceso = "La orden {0} paso a Proceso";

        /// <summary>
        /// when the order goes to Cancelled.
        /// </summary>
        public const string OrderCancelled = "La orden {0} paso a Cancelado";

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
        /// Error the sales order is cancelled.
        /// </summary>
        public const string ErrorProductionOrderCancelled = "ErrorProductionOrderCancelled";

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
        /// error no user available.
        /// </summary>
        public const string ErrorQfbAutomatico = "Todos los QFB han rebasado el número máximo de piezas a elaborar, intenta con la asignación manual";

        /// <summary>
        /// if the type is pedido.
        /// </summary>
        public const string TypePedido = "Pedido";

        /// <summary>
        /// if the type is fabrication order.
        /// </summary>
        public const string TypeFabOrder = "Orden de Fabricación";

        /// <summary>
        /// Status liberado.
        /// </summary>
        public const string StatusSapLiberado = "R";

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
        /// route to create orders.
        /// </summary>
        public const string CreateFabOrder = "createFabOrder";

        /// <summary>
        /// route to update faborder.
        /// </summary>
        public const string UpdateFabOrder = "updateFabOrder";

        /// <summary>
        /// route to updates orders.
        /// </summary>
        public const string UpdateFormula = "updateFormula";

        /// <summary>
        /// route to cancel orders.
        /// </summary>
        public const string CancelFabOrder = "cancelProductionOrder";

        /// <summary>
        /// the connect to sap di api.
        /// </summary>
        public const string ConnectSapDiApi = "connect";

        /// <summary>
        /// Gets the users by role from user service.
        /// </summary>
        public const string GetUsersByRole = "role/{0}";

        /// <summary>
        /// the id for qfb role.
        /// </summary>
        public const int QfbRoleId = 2;

        /// <summary>
        /// Reason not found.
        /// </summary>
        public const string ReasonNotExistsOrder = "No existe la orden.";

        /// <summary>
        /// Reason finsihed order.
        /// </summary>
        public const string ReasonOrderFinished = "La orden ya esta finalizada.";

        /// <summary>
        /// Reason finsihed order.
        /// </summary>
        public const string ReasonProductionOrderFinished = "Una orden de fabricación se encuentra finalizada.";

        /// <summary>
        /// Reason SAP error.
        /// </summary>
        public const string ReasonSapError = "Ocurrió un error al actualizar en SAP.";
    }
}
