// <summary>
// <copyright file="ServiceConstants.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.Constants
{
    using System.Collections.Generic;

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
        /// Completed status.
        /// </summary>
        public const string Completed = "Terminado";

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
        /// The terminado status.
        /// </summary>
        public const string Terminado = "Terminado";

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
        /// abierto status.
        /// </summary>
        public const string Abierto = "Abierto";

        /// <summary>
        /// abierto status.
        /// </summary>
        public const string Reasignado = "Reasignado";

        /// <summary>
        /// status entregado.
        /// </summary>
        public const string Entregado = "Entregado";

        /// <summary>
        /// status entregado.
        /// </summary>
        public const string Rechazado = "Rechazado";

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
        /// when the order goes to Finished.
        /// </summary>
        public const string OrderFinished = "La orden {0} paso a Finalizado";

        /// <summary>
        /// when the order not goes to Rejected.
        /// </summary>
        public const string OrderNotRejectedBecauseExits = "El pedido {0} ya tiene ordenes de fabricación";

        /// <summary>
        /// cuando se asigna un pedido.
        /// </summary>
        public const string AsignarVenta = "Se asigno el pedido a {0}";

        /// <summary>
        /// se asigna la orden.
        /// </summary>
        public const string AsignarOrden = "Se asigno la orden a {0}";

        /// <summary>
        /// se termino la orden.
        /// </summary>
        public const string OrdenTerminada = "Se termino la orden por el usuario";

        /// <summary>
        /// orde fab plani.
        /// </summary>
        public const string OrdenFabricacionPlan = "Orden de fabricación planificada";

        /// <summary>
        /// when the Pedido es reassigned.
        /// </summary>
        public const string ReasignarPedido = "Se reasigno el pedido a {0}";

        /// <summary>
        /// se asigna la orden.
        /// </summary>
        public const string ReasignarOrden = "Se reasigno la orden a {0}";

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
        /// const for error whne inserting fab orde.
        /// </summary>
        public const string ErrorCreatePdf = "ErrorCreatePdf";

        /// <summary>
        /// the error when update a order fab.
        /// </summary>
        public const string ErrorUpdateFabOrd = "ErrorUpdateFabOrd";

        /// <summary>
        /// if there were error while inserting.
        /// </summary>
        public const string ErrorAlInsertar = "Error al insertar";

        /// <summary>
        /// if there were error while inserting.
        /// </summary>
        public const string ErrorCrearPdf = "Error al crear PDF";

        /// <summary>
        /// error al asignar.
        /// </summary>
        public const string ErroAlAsignar = "Error al asignar";

        /// <summary>
        /// error al asignar.
        /// </summary>
        public const string ErrorToRejectedAnOrder = "Hubo un error al enviar emails";

        /// <summary>
        /// error no user available.
        /// </summary>
        public const string ErrorQfbAutomatico = "Todos los QFB han rebasado el número máximo de piezas a elaborar, intenta con la asignación manual";

        /// <summary>
        /// if the type is pedido.
        /// </summary>
        public const string TypePedido = "Pedido";

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
        public const string GetFormula = "qfb/formula";

        /// <summary>
        /// gets the asesors email.
        /// </summary>
        public const string GetAsesorsMail = "asesors";

        /// <summary>
        /// gets the last isolated production order id.
        /// </summary>
        public const string GetLastIsolatedProductionOrderId = "fabOrder/isolated/last";

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
        /// the update batches.
        /// </summary>
        public const string UpdateBatches = "batches";

        /// <summary>
        /// route to cancel orders.
        /// </summary>
        public const string CancelFabOrder = "cancelProductionOrder";

        /// <summary>
        /// route to create isolated fabrication orders.
        /// </summary>
        public const string CreateIsolatedFabOrder = "isolatedProductionOrder";

        /// <summary>
        /// route to finish orders.
        /// </summary>
        public const string FinishFabOrder = "finishProducionOrders";

        /// <summary>
        /// the connect to sap di api.
        /// </summary>
        public const string ConnectSapDiApi = "connect";

        /// <summary>
        /// gets the recipes of a group of orders.
        /// </summary>
        public const string GetRecipes = "recipes/orders";

        /// <summary>
        /// Gets the users by role from user service.
        /// </summary>
        public const string GetUsersByRole = "role/{0}";

        /// <summary>
        /// Gets the components with the data.
        /// </summary>
        public const string GetComponentsWithBatches = "componentes/lotes/{0}";

        /// <summary>
        /// gets the data by the filters.
        /// </summary>
        public const string GetFabOrdersByFilter = "fabOrder/filters";

        /// <summary>
        /// the route to get the users by ids.
        /// </summary>
        public const string GetUsersById = "getUsersById";

        /// <summary>
        /// the route to get the users by ids.
        /// </summary>
        public const string GetUsersByOrdersById = "fabOrderId";

        /// <summary>
        /// creates the pdfs.
        /// </summary>
        public const string CreatePdf = "create/pdf";

        /// <summary>
        /// creates the pdfs.
        /// </summary>
        public const string CreateSalePdf = "create/sale/pdf";

        /// <summary>
        /// send emails to rejected orders.
        /// </summary>
        public const string SendEmailToRejectedOrders = "rejection/order/email";

        /// <summary>
        /// deletes the files.
        /// </summary>
        public const string DeleteFiles = "delete/files";

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
        public const string ReasonSalesOrderFinished = "El pedido ya esta finalizado.";

        /// <summary>
        /// Reason finsihed production order.
        /// </summary>
        public const string ReasonProductionOrderFinished = "La orden de fabricación {0} se encuentra finalizada.";

        /// <summary>
        /// Reason non complete production orders.
        /// </summary>
        public const string ReasonProductionOrderNonCompleted = "La orden de fabricación {0} no se encuentra terminada.";

        /// <summary>
        /// Reason not found.
        /// </summary>
        public const string ReasonProductionOrderNotExists = "La orden de fabricación {0} no existe.";

        /// <summary>
        /// Reason non complete sales orders.
        /// </summary>
        public const string ReasonOrderNonCompleted = "La orden no se encuentra terminada.";

        /// <summary>
        /// Reason pre-production orders in SAP.
        /// </summary>
        public const string ReasonPreProductionOrdersInSap = "El pedido aun contiene órdenes sin procesar en SAP.";

        /// <summary>
        /// Reason SAP error.
        /// </summary>
        public const string ReasonSapError = "Ocurrió un error al actualizar en SAP.";

        /// <summary>
        /// Reason SAP error.
        /// </summary>
        public const string ReasonSapConnectionError = "Ocurrió un error al actualizar en SAP.";

        /// <summary>
        /// error when batche are missing.
        /// </summary>
        public const string BatchesAreMissingError = "La orden no puede ser Terminada, revisa que todos los artículos tengan un lote asignado";

        /// <summary>
        /// when the isolated order is created.
        /// </summary>
        public const string IsolatedProductionOrderCreated = "La orden de fabricación {0} ha sido creada.";

        /// <summary>
        /// Reason unexpected error.
        /// </summary>
        public const string ReasonUnexpectedError = "Ocurrió un error inesperado.";

        /// <summary>
        /// Reason custom formula already exists.
        /// </summary>
        public const string ReasonCustomListAlreadyExists = "La lista {0} ya existe para el producto {1}.";

        /// <summary>
        /// the filter for orders.
        /// </summary>
        public const string FechaInicio = "fini";

        /// <summary>
        /// the filter for orders.
        /// </summary>
        public const string FechaFin = "ffin";

        /// <summary>
        /// the filter for orders.
        /// </summary>
        public const string DocNum = "docNum";

        /// <summary>
        /// the filter for orders.
        /// </summary>
        public const string Status = "status";

        /// <summary>
        /// the filter for orders.
        /// </summary>
        public const string Qfb = "qfb";

        /// <summary>
        /// if needs the large description.
        /// </summary>
        public const string NeedsLargeDsc = "Ldsc";

        /// <summary>
        /// const for offset.
        /// </summary>
        public const string Offset = "offset";

        /// <summary>
        /// Const for the limit.
        /// </summary>
        public const string Limit = "limit";

        /// <summary>
        /// the nvo leon state.
        /// </summary>
        public const string NuevoLeon = "Nuevo León";

        /// <summary>
        /// the foreign value.
        /// </summary>
        public const string Foraneo = "Foráneo";

        /// <summary>
        /// the local status.
        /// </summary>
        public const string Local = "Local";

        /// <summary>
        /// Get users by id.
        /// </summary>
        public const string Personalizado = "Personalizada";

        /// <summary>
        /// the insert value.
        /// </summary>
        public const string Insert = "insert";

        /// <summary>
        /// the insert value.
        /// </summary>
        public const string RedisComponents = "redisComponents";

        /// <summary>
        /// Gets the status of the order.
        /// </summary>
        /// <value>
        /// the status.
        /// </value>
        public static List<string> StatusAvoidReasignar { get; } = new List<string>
        {
            "Finalizado",
        };

        /// <summary>
        /// Gets the status of the order.
        /// </summary>
        /// <value>
        /// the status.
        /// </value>
        public static List<string> ValidStatusTerminar { get; } = new List<string>
        {
            Terminado,
            Finalizado,
            Cancelled,
        };

        /// <summary>
        /// Gets the status of the order.
        /// </summary>
        /// <value>
        /// the status.
        /// </value>
        public static List<string> ValidStatusLiberado { get; } = new List<string>
        {
            Asignado,
            Reasignado,
            Pendiente,
            Proceso,
        };

        /// <summary>
        /// Gets the status of the order.
        /// </summary>
        /// <value>
        /// the status.
        /// </value>
        public static List<string> StatusWorkload { get; } = new List<string>
        {
            Asignado,
            Proceso,
            Pendiente,
            Terminado,
            Finalizado,
            Reasignado,
            Entregado,
        };

        /// <summary>
        /// Gets the status of the order.
        /// </summary>
        /// <value>
        /// the status.
        /// </value>
        public static List<string> ListComponentsMostAssigned { get; } = new List<string>
        {
            "EN",
            "EM",
        };
    }
}
