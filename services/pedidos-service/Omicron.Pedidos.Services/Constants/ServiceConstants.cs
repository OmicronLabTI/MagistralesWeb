// <summary>
// <copyright file="ServiceConstants.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.Constants
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// the class for constatns.
    /// </summary>
    public static class ServiceConstants
    {
        /// <summary>
        /// the update batches.
        /// </summary>
        public const string UpdateBatches = "batches";

        /// <summary>
        /// status planificada.
        /// </summary>
        public const string Planificado = "Planificado";

        /// <summary>
        /// Cancelled status.
        /// </summary>
        public const string Cancelled = "Cancelado";

        /// <summary>
        /// close salesOrders.
        /// </summary>
        public const string SalesOrders = "SalesOrders";

        /// <summary>
        /// close fabOrders.
        /// </summary>
        public const string FabOrders = "FabOrders";

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
        /// Almacenado status.
        /// </summary>
        public const string Almacenado = "Almacenado";

        /// <summary>
        /// value for empaquetado.
        /// </summary>
        public const string Empaquetado = "Empaquetado";

        /// <summary>
        /// value for empaquetado.
        /// </summary>
        public const string Enviado = "Enviado";

        /// <summary>
        /// The enviado status.
        /// </summary>
        public const string Camino = "En Camino";

        /// <summary>
        /// value for empaquetado.
        /// </summary>
        public const string Entregado = "Entregado";

        /// <summary>
        /// value for empaquetado.
        /// </summary>
        public const string NoEntregado = "No Entregado";

        /// <summary>
        /// status back order.
        /// </summary>
        public const string BackOrder = "Back Order";

        /// <summary>
        /// recibir value.
        /// </summary>
        public const string Recibir = "Recibir";

        /// <summary>
        /// status entregado.
        /// </summary>
        public const string Rechazado = "Rechazado";

        /// <summary>
        /// orden de venta plan.
        /// </summary>
        public const string OrdenVentaPlan = "Orden de venta planificada";

        /// <summary>
        /// status planificada.
        /// </summary>
        public const string SignedStatus = "Firmado";

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
        public const string OrderNotRejectedBecauseExits = "El pedido {0} ya tiene Órdenes de Fabricación";

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
        /// error no user available.
        /// </summary>
        public const string ErrirQfbAutomaticoParcial = "No se pudieron asignar los pedidos {0}se ha exedido el número de piezas por laboratorio, intenta con la asignación manual";

        /// <summary>
        /// error almacenar.
        /// </summary>
        public const string ErrorDesempaquetar = "No existe un registro que coincidan con estos datos ItemCode: {0}, DeliveryId: {1}, InvoiceId: {2} o el producto ya se encuentra en distribución";

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
        /// the get with orders.
        /// </summary>
        public const string GetOrderWithDetailDxp = "getDetails/dxpDetails";

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
        /// Update Production Order Batches Service Layer.
        /// </summary>
        public const string UpdateProductionOrderBatchesServiceLayer = "productionorders/batches";

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
        /// Gets the delivery.
        /// </summary>
        public const string GetDelivery = "delivery/orderids";

        /// <summary>
        /// Gets the line orders from almacen.
        /// </summary>
        public const string AlmacenGetOrders = "orders";

        /// <summary>
        /// Get the line products by invoice id.
        /// </summary>
        public const string AlmacenGetOrderByInvoice = "getline/invoiceId";

        /// <summary>
        /// Get the line products by sale order id.
        /// </summary>
        public const string AlmacenGetOrderBySaleOrder = "getline/saleorder";

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
        public const string ProductUnit = "products/units";

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
        /// invoice pdf.
        /// </summary>
        public const string CreatePdfByType = "create/{0}/pdfs";

        /// <summary>
        /// send emails to rejected orders.
        /// </summary>
        public const string SendEmailToRejectedOrders = "rejection/order/email";

        /// <summary>
        /// deletes the files.
        /// </summary>
        public const string DeleteFiles = "delete/files";

        /// <summary>
        /// deletes the files.
        /// </summary>
        public const string ProductNoLabel = "ProductNoLabel";

        /// <summary>
        /// the id for qfb role.
        /// </summary>
        public const int QfbRoleId = 2;

        /// <summary>
        /// Const for the all classifications.
        /// </summary>
        public const string AllClassifications = "Todas";

        /// <summary>
        /// Const for the classifications.
        /// </summary>
        public const string Classifications = "classifications";

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
        /// Reason non complete production orders.
        /// </summary>
        public const string ReasonProductionOrdersNonCompleted = "La órdenes de fabricación: {0} no se encuentran terminadas.";

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
        public const string Type = "type";

        /// <summary>
        /// the filter for orders.
        /// </summary>
        public const string Chips = "chips";

        /// <summary>
        /// the filter for orders.
        /// </summary>
        public const string Qfb = "qfb";

        /// <summary>
        /// the sale order id key.
        /// </summary>
        public const string SaleOrderId = "saleorderid";

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
        /// Const for the qfbId.
        /// </summary>
        public const string QfbId = "qfbId";

        /// <summary>
        /// Const for the orders.
        /// </summary>
        public const string Orders = "orders";

        /// <summary>
        /// Const for the StatusCancelado.
        /// </summary>
        public const string StatusCancelado = "Cancelado";

        /// <summary>
        /// delivery contants.
        /// </summary>
        public const string Delivery = "delivery";

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
        /// the foraneo status.
        /// </summary>
        public const string ForaneoDb = "Foraneo";

        /// <summary>
        /// Get users by id.
        /// </summary>
        public const string Personalizado = "Personalizada";

        /// <summary>
        /// Gets the magistrgal qr.
        /// </summary>
        public const string MagistralQr = "QrMagistral";

        /// <summary>
        /// Gets the magistrgal qr.
        /// </summary>
        public const string MagistralQrHeight = "QrMagistralHeight";

        /// <summary>
        /// Gets the magistrgal qr.
        /// </summary>
        public const string MagistralQrWidth = "QrMagistralWidth";

        /// <summary>
        /// Gets the magistrgal qr.
        /// </summary>
        public const string MagistralQrMargin = "QrMagistralMargin";

        /// <summary>
        /// Gets the magistrgal qr.
        /// </summary>
        public const string QrMagistralRectx = "QrMagistralRectx";

        /// <summary>
        /// Gets the magistrgal qr.
        /// </summary>
        public const string QrMagistralRecty = "QrMagistralRecty";

        /// <summary>
        /// Gets the magistrgal qr.
        /// </summary>
        public const string QrMagistralRectWidth = "QrMagistralRectWidth";

        /// <summary>
        /// Gets the magistrgal qr.
        /// </summary>
        public const string QrMagistralRectHeight = "QrMagistralRectHeight";

        /// <summary>
        /// gets the size of text.
        /// </summary>
        public const string QrMagistralBottomTextSize = "QrMagistralBottomTextSize";

        /// <summary>
        /// Gets the magistrgal qr.
        /// </summary>
        public const string QrMagistralRectxTop = "QrMagistralRectxTop";

        /// <summary>
        /// Gets the magistrgal qr.
        /// </summary>
        public const string QrMagistralRectyTop = "QrMagistralRectyTop";

        /// <summary>
        /// Gets the magistrgal qr.
        /// </summary>
        public const string QrMagistralAngleRotTop = "QrMagistralAngleRotTop";

        /// <summary>
        /// Field for the max day to look.
        /// </summary>
        public const string AlmacenMaxDayToLook = "AlmacenMaxDayToLook";

        /// <summary>
        /// const for the bottom temxt.
        /// </summary>
        public const string QrBottomTextOrden = "OR: {0} {1}";

        /// <summary>
        /// const for the bottom temxt.
        /// </summary>
        public const string QrBottomTextRemision = ": {0} {1}";

        /// <summary>
        /// const for the bottom temxt.
        /// </summary>
        public const string QrBottomTextFactura = "F: {0} {1}";

        /// <summary>
        /// const for the cooling.
        /// </summary>
        public const string NeedsCooling = "\nRefrigerado";

        /// <summary>
        /// the insert value.
        /// </summary>
        public const string Insert = "insert";

        /// <summary>
        /// the insert value.
        /// </summary>
        public const string RedisComponents = "redisComponents";

        /// <summary>
        /// the insert value.
        /// </summary>
        public const string Name = "name";

        /// <summary>
        /// Get users by id.
        /// </summary>
        public const string ProductId = "productId";

        /// <summary>
        /// Get users by id.
        /// </summary>
        public const string Mix = "MX";

        /// <summary>
        /// for total cancelation.
        /// </summary>
        public const string Total = "total";

        /// <summary>
        /// for total cancelation.
        /// </summary>
        public const string PedidoRedisPlanificado = "SaleOrderPlan{0}";

        /// <summary>
        /// for total cancelation.
        /// </summary>
        public const string ErrorWhenPlnanningIntro = "Error al planificar, ";

        /// <summary>
        /// for total cancelation.
        /// </summary>
        public const string ErrorWhenPlanning = "El pedido {0} se encuentra en proceso de planificación, favor de intentar más tarde";

        /// <summary>
        /// for total cancelation.
        /// </summary>
        public const bool IsProductionOrder = true;

        /// <summary>
        /// azure account.
        /// </summary>
        public const string AzureAccountName = "AzureAccountName";

        /// <summary>
        /// azure account.
        /// </summary>
        public const string AzureAccountKey = "AzureAccountKey";

        /// <summary>
        /// azure account.
        /// </summary>
        public const string BlobUrlTemplate = "https://{0}.blob.core.windows.net/{1}/{2}";

        /// <summary>
        /// the order containe.
        /// </summary>
        public const string OrderQrContainer = "OrderQrContainer";

        /// <summary>
        /// the order containe.
        /// </summary>
        public const string DeliveryQrContainer = "DeliveryQrContainer";

        /// <summary>
        /// the order containe.
        /// </summary>
        public const string InvoiceQrContainer = "InvoiceQrContainer";

        /// <summary>
        /// const for the bottom temxt.
        /// </summary>
        public const string QrTopTextOrden = "{0}\nSKU: {1}";

        /// <summary>
        /// Inidicates the font type in the QR text.
        /// </summary>
        public const string QrTextFontType = "Tahoma";

        /// <summary>
        /// Inidicates the font type in the QR text.
        /// </summary>
        public const double QrSize = 0.65;

        /// <summary>
        /// gets the size of text.
        /// </summary>
        public const string QrDeliveryBottomTextSize = "QrDeliveryBottomTextSize";

        /// <summary>
        /// gets the size of text.
        /// </summary>
        public const string QrDeliveryHeight = "QrDeliveryHeight";

        /// <summary>
        /// gets the size of text.
        /// </summary>
        public const string QrDeliveryWidth = "QrDeliveryWidth";

        /// <summary>
        /// gets the size of text.
        /// </summary>
        public const string QrDeliveryMargin = "QrDeliveryMargin";

        /// <summary>
        /// gets the size of text.
        /// </summary>
        public const string QrDeliveryRectx = "QrDeliveryRectx";

        /// <summary>
        /// gets the size of text.
        /// </summary>
        public const string QrDeliveryRecty = "QrDeliveryRecty";

        /// <summary>
        /// gets the size of text.
        /// </summary>
        public const string QrDeliveryRectWidth = "QrDeliveryRectWidth";

        /// <summary>
        /// gets the size of text.
        /// </summary>
        public const string QrDeliveryRectHeight = "QrDeliveryRectHeight";

        /// <summary>
        /// gets the size of text.
        /// </summary>
        public const string QrDeliveryRectxTop = "QrDeliveryRectxTop";

        /// <summary>
        /// gets the size of text.
        /// </summary>
        public const string QrDeliveryRectyTop = "QrDeliveryRectyTop";

        /// <summary>
        /// gets the size of text.
        /// </summary>
        public const string QrDeliveryLabelRectx = "QrDeliveryLabelRectx";

        /// <summary>
        /// gets the size of text.
        /// </summary>
        public const string QrDeliveryLabelRecty = "QrDeliveryLabelRecty";

        /// <summary>
        /// gets the size of text.
        /// </summary>
        public const string QrDeliveryLabelSaleRectx = "QrDeliveryLabelSaleRectx";

        /// <summary>
        /// gets the size of text.
        /// </summary>
        public const string QrDeliveryLabelSaleRecty = "QrDeliveryLabelSaleRecty";

        /// <summary>
        /// gets the size of text.
        /// </summary>
        public const string QrDeliveryLabelSaleFontSize = "QrDeliveryLabelSaleFontSize";

        /// <summary>
        /// const for the bottom temxt.
        /// </summary>
        public const string QrTopTextRemision = "\n{0}";

        /// <summary>
        /// const for the bottom temxt.
        /// </summary>
        public const string LabelMuestraText = "{0}:\n{1}";

        /// <summary>
        /// Gets the delivery qr.
        /// </summary>
        public const string DeliveryQr = "QrDelivery";

        /// <summary>
        /// Gets the delivery qr.
        /// </summary>
        public const string LocalShip = "Local";

        /// <summary>
        /// Gets the delivery qr.
        /// </summary>
        public const string ForeignShip = "Foráneo";

        /// <summary>
        /// Gets the delivery qr.
        /// </summary>
        public const string RemisionType = "R";

        /// <summary>
        /// Gets the delivery qr.
        /// </summary>
        public const string RemisionOmiType = "R-O";

        /// <summary>
        /// Gets the delivery qr.
        /// </summary>
        public const string LocalShipAbr = "L";

        /// <summary>
        /// impresapor cliente.
        /// </summary>
        public const string LabelImpresaPorCliente = "IMPRESA POR CLIENTE";

        /// <summary>
        /// Pedido muestra.
        /// </summary>
        public const string PedidoMuestra = "Pedido Muestra";

        /// <summary>
        /// the order containe.
        /// </summary>
        public const string EndPointToValidateQuatitiesOrdersFormula = "formula/validate/quantities";

        /// <summary>
        /// the order containe.
        /// </summary>
        public const string FailConsumedQuantity = "No coincide la cantidad requerida con la cantidad consumida de la siguiente orden {0}";

        /// <summary>
        /// Gets the users by role from user service.
        /// </summary>
        public const string GetQfbInfoById = "getqfb/info/byids";

        /// <summary>
        /// Reason unexpected error.
        /// </summary>
        public const string QfbWithoutTecnic = "El químico {0} no tiene un técnico activo asignado, favor de realizar la asignación para poder continuar";

        /// <summary>
        /// Reason unexpected error.
        /// </summary>
        public const string OrderWithoutTecnicSign = "No es posible terminar, falta la firma del técnico asignado";

        /// <summary>
        /// the insert value.
        /// </summary>
        public const string RedisBulkOrderKey = "redisComponentsBulkOrder";

        /// <summary>
        /// gets the bold of label sale text.
        /// </summary>
        public const string QrDeliveryTextIsBold = "QrDeliveryTextIsBold";

        /// <summary>
        /// gets the bold of bootom text magistral text.
        /// </summary>
        public const string QrMagistralTextIsBold = "QrMagistralTextIsBold";

        /// <summary>
        /// Gets the value of X position for the magistral qr.
        /// </summary>
        public const string QrMagistralCodePositionX = "QrMagistralCodePositionX";

        /// <summary>
        /// Gets the value of Y position for the magistral qr.
        /// </summary>
        public const string QrMagistralCodePositionY = "QrMagistralCodePositionY";

        /// <summary>
        /// Gets the value of widht and height value for the magistral qr code.
        /// </summary>
        public const string QrMagistralCodeSize = "QrMagistralCodeSize";

        /// <summary>
        /// Gets the value of X position for the delivery qr.
        /// </summary>
        public const string QrDeliveryCodePositionX = "QrDeliveryCodePositionX";

        /// <summary>
        /// Gets the value of Y position for the delivery qr.
        /// </summary>
        public const string QrDeliveryCodePositionY = "QrDeliveryCodePositionY";

        /// <summary>
        /// Gets the value of widht value for the delivery qr code.
        /// </summary>
        public const string QrDeliveryCodeSize = "QrDeliveryCodeSize";

        /// <summary>
        /// Gets the value for hex white color.
        /// </summary>
        public const string HexWhiteColor = "FFFFFF";

        /// <summary>
        /// Gets the value for hex black color.
        /// </summary>
        public const string HexBlackColor = "000000";

        /// <summary>
        /// Get the value for png file format.
        /// </summary>
        public const string PngFileFormat = "png";

        /// <summary>
        /// Get the value for format.
        /// </summary>
        public const string QrPng = "qr.png";

        /// <summary>
        /// const for OrdersToExcludeKey.
        /// </summary>
        public const string OrdersToExcludeKey = "OrdersToExclude";

        /// <summary>
        /// StepPrimaryValidations.
        /// </summary>
        public const string StepPrimaryValidations = "Primary Validations";

        /// <summary>
        /// StepCreateInventoryStep.
        /// </summary>
        public const string StepCreateInventoryStep = "Create Inventory";

        /// <summary>
        /// StepCreateReceiptStep.
        /// </summary>
        public const string StepCreateReceiptStep = "Create Receipt";

        /// <summary>
        /// StepSuccessfullyClosedInSapStep.
        /// </summary>
        public const string StepSuccessfullyClosedInSapStep = "Successfully Closed In SAP";

        /// <summary>
        /// Gets StepPrimaryValidations.
        /// </summary>
        /// <value>
        /// StepPrimaryValidations.
        /// </value>
        public const string StepUpdatePostgres = "Successfully Closed In Postgresql";

        /// <summary>
        /// Gets StepPrimaryValidations.
        /// </summary>
        /// <value>
        /// StepPrimaryValidations.
        /// </value>
        public const string StepCreatePdf = "PDF Created";

        /// <summary>
        /// Gets ParentOrder.
        /// </summary>
        /// <value>
        /// ParentOrder.
        /// </value>
        public const string ParentOrder = "Padre";

        /// <summary>
        /// CompletelyDivided.
        /// </summary>
        public const string CompletelyDivided = "Completamente dividida";

        /// <summary>
        /// PartiallyDivided.
        /// </summary>
        public const string PartiallyDivided = "Parcialmente dividida";

        /// <summary>
        /// CancelSapStep.
        /// </summary>
        public const string StartStep = "StartCancelParentOrderProcess";

        /// <summary>
        /// CancelSapStep.
        /// </summary>
        public const string UpdateCancelParentOrderStep = "UpdateCancelParentOrderProcess";

        /// <summary>
        /// CancelSapStep.
        /// </summary>
        public const string CancelSapStep = "CancelSAP";

        /// <summary>
        /// CancelPostgresStep.
        /// </summary>
        public const string CancelPostgresStep = "CancelPostgres";

        /// <summary>
        /// SaveHistoryStep.
        /// </summary>
        public const string SaveHistoryStep = "SaveHistory";

        /// <summary>
        /// StepCreateChildOrderSap.
        /// </summary>
        public const string StepCreateChildOrderSap = "CreateChildOrderSap";

        /// <summary>
        /// StepCreateChildOrderSap.
        /// </summary>
        public const string StepCreateChildOrderWithComponentsSap = "CreateChildOrderSapWithComponents";

        /// <summary>
        /// StepCreateChildOrderPostgres.
        /// </summary>
        public const string StepCreateChildOrderPostgres = "CreateChildOrderPostgres";

        /// <summary>
        /// StepCreateChildOrderPostgres.
        /// </summary>
        public const string StepSaveChildOrderHistory = "SaveChildOrderHistory";

        /// <summary>
        /// EmptyValue.
        /// </summary>
        public const string EmptyValue = "";

        /// <summary>
        /// Gets ProductionDetailCount.
        /// </summary>
        /// <value>
        /// ProductionDetailCount.
        /// </value>
        public const int ProductionDetailCount = 1;

        /// <summary>
        /// Gets DefaultRedisValueTimeToLive.
        /// </summary>
        /// <value>
        /// DefaultRedisValueTimeToLive.
        /// </value>
        public static TimeSpan DefaultRedisValueTimeToLive => new TimeSpan(8, 0, 0);

        /// <summary>
        /// Gets ProductionOrderIsAlreadyBeignProcessed.
        /// </summary>
        /// <value>
        /// ProductionOrderIsAlreadyBeignProcessed.
        /// </value>
        public static string ProductionOrderIsAlreadyBeignProcessed => "La orden de producción ya está siendo procesada.";

        /// <summary>
        /// Gets SapValidationFinalizationEndpoint.
        /// </summary>
        /// <value>
        /// SapValidationFinalizationEndpoint.
        /// </value>
        public static string SapValidationFinalizationEndpoint => "validation/productionorders/finalization";

        /// <summary>
        /// Gets SapFinalizeProductionOrdersEndpoint.
        /// </summary>
        /// <value>
        /// SapFinalizeProductionOrdersEndpoint.
        /// </value>
        public static string SapFinalizeProductionOrdersEndpoint => "finalize/productionorders";

        /// <summary>
        /// Gets FinalizeProcessInProgressStatus.
        /// </summary>
        /// <value>
        /// FinalizeProcessInProgressStatus.
        /// </value>
        public static string FinalizeProcessInProgressStatus => "In Progress";

        /// <summary>
        /// Gets FinalizeProcessFailedStatus.
        /// </summary>
        /// <value>
        /// FinalizeProcessFailedStatus.
        /// </value>
        public static string FinalizeProcessFailedStatus => "Failed";

        /// <summary>
        /// Gets FinalizeProcessInSuccessStatus.
        /// </summary>
        /// <value>
        /// FinalizeProcessInSuccessStatus.
        /// </value>
        public static string FinalizeProcessInSuccessStatus => "Success";

        /// <summary>
        /// Gets ProductionOrderFinalizingKey.
        /// </summary>
        /// <value>
        /// ProductionOrderFinalizingKey.
        /// </value>
        public static string ProductionOrderFinalizingKey => "production-order-finalizing:{0}";

        /// <summary>
        /// Gets ProductionOrderFinalizingToProcessKey.
        /// </summary>
        /// <value>
        /// ProductionOrderFinalizingToProcessKey.
        /// </value>
        public static string ProductionOrderFinalizingToProcessKey => "production-order-finalizing-to-process";

        /// <summary>
        /// Gets ProductionOrderFinalizingRetryCronjob.
        /// </summary>
        /// <value>
        /// ProductionOrderFinalizingRetryCronjob.
        /// </value>
        public static string ProductionOrderFinalizingRetryCronjob => "production-order-finalizing-retry-cronjob";

        /// <summary>
        /// Gets KafkaInsertLogsConfigName.
        /// </summary>
        /// <value>
        /// KafkaInsertLogsConfigName.
        /// </value>
        public static string KafkaInsertLogsConfigName => "insertlogs";

        /// <summary>
        /// Gets KafkaFinalizeProductionOrderSapConfigName.
        /// </summary>
        /// <value>
        /// KafkaFinalizeProductionOrderSapConfigName.
        /// </value>
        public static string KafkaFinalizeProductionOrderSapConfigName => "finalizeproductionordersap";

        /// <summary>
        /// Gets KafkaFinalizeProductionOrderPostgresqlConfigName.
        /// </summary>
        /// <value>
        /// KafkaFinalizeProductionOrderPostgresqlConfigName.
        /// </value>
        public static string KafkaFinalizeProductionOrderPostgresqlConfigName => "finalizeproductionorderpostgresql";

        /// <summary>
        /// Gets KafkaProductionOrderPdfGenerationConfigName.
        /// </summary>
        /// <value>
        /// KafkaProductionOrderPdfGenerationConfigName.
        /// </value>
        public static string KafkaProductionOrderPdfGenerationConfigName => "productionorderpdfgeneration";

        /// <summary>
        /// Gets ErrorOccurredWhileCommunicatingWithServiceLayerAdapter.
        /// </summary>
        /// <value>
        /// ErrorOccurredWhileCommunicatingWithServiceLayerAdapter.
        /// </value>
        public static string ErrorOccurredWhileCommunicatingWithServiceLayerAdapter => "An error occurred while communicating with the Service Layer Adapter.";

        /// <summary>
        /// Gets MinutesToRetrySeparationProductionOrder.
        /// </summary>
        /// <value>
        /// MinutesToRetrySeparationProductionOrder.
        /// </value>
        public static int MinutesToRetrySeparationProductionOrder => 5;

        /// <summary>
        /// Gets CreateChildOrderSapUrl.
        /// </summary>
        /// <value>
        /// CreateChildOrderSapUrl.
        /// </value>
        public static string CreateChildOrderSapUrl => "child/order";

        /// <summary>
        /// Gets ProductionOrderSeparationProcessKey.
        /// </summary>
        /// <value>
        /// ProductionOrderSeparationProcessKey.
        /// </value>
        public static string ProductionOrderSeparationProcessKey => "production-order-separation-process:{0}";

        /// <summary>
        /// Gets ProductionOrderSeparationProcessMessage.
        /// </summary>
        /// <value>
        /// ProductionOrderSeparationProcessMessage.
        /// </value>
        public static string ProductionOrderSeparationProcessMessage => "La orden de fabricación seleccionada ya tiene un proceso de división en curso. Por favor espera a que finalice antes de intentar dividirla nuevamente.";

        /// <summary>
        /// Gets DivisionOrdersToProcessKey.
        /// </summary>
        /// <value>
        /// DivisionOrdersToProcessKey.
        /// </value>
        public static string DivisionOrdersToProcessKey => "DivisionOrders:ToProcess";

        /// <summary>
        /// Gets DivisionProcessingKey.
        /// </summary>
        /// <value>
        /// DivisionProcessingKey.
        /// </value>
        public static string DivisionProcessingKey => "DivisionProcessing:{0}:{1}";

        /// <summary>
        /// Gets the status of the order.
        /// </summary>
        /// <value>
        /// the status.
        /// </value>
        public static List<string> StatusLocal { get; } = new List<string>
        {
            Empaquetado,
        };

        /// <summary>
        /// Gets the status of the order.
        /// </summary>
        /// <value>
        /// the status.
        /// </value>
        public static List<string> StatusDelivered { get; } = new List<string>
        {
            Enviado,
            Entregado,
        };

        /// <summary>
        /// Gets the status of the order.
        /// </summary>
        /// <value>
        /// the status.
        /// </value>
        public static List<string> StatusAvoidReasignar { get; } = new List<string>
        {
            Finalizado,
            Almacenado,
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
            Entregado,
            Almacenado,
        };

        /// <summary>
        /// Gets the status of the order.
        /// </summary>
        /// <value>
        /// the status.
        /// </value>
        public static List<string> ValidStatusFinalizar { get; } = new List<string>
        {
            Finalizado,
            Entregado,
            Almacenado,
        };

        /// <summary>
        /// Gets the status of the order.
        /// </summary>
        /// <value>
        /// the status.
        /// </value>
        public static List<string> ValidStatusToFillFinalizedate { get; } = new List<string>
        {
            Finalizado,
            Almacenado,
            BackOrder,
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
        };

        /// <summary>
        /// Gets the status of the order.
        /// </summary>
        /// <value>
        /// the status.
        /// </value>
        public static List<string> AllStatusWorkload { get; } = new List<string>
        {
            Asignado,
            Proceso,
            Pendiente,
            Terminado,
            Finalizado,
            Entregado,
            Almacenado,
            Reasignado,
        };

        /// <summary>
        /// Gets the status of the order.
        /// </summary>
        /// <value>
        /// the status.
        /// </value>
        public static List<string> StatuPendingAlmacen { get; } = new List<string>
        {
            Pendiente,
            Finalizado,
            Almacenado,
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

        /// <summary>
        /// Gets the status of the order.
        /// </summary>
        /// <value>
        /// the status.
        /// </value>
        public static List<string> StatusIgnoreWorkLoad { get; } = new List<string>
        {
            Cancelled,
            Finalizado,
            Entregado,
            Almacenado,
        };

        /// <summary>
        /// Gets list of thw status for the orders.
        /// </summary>
        /// <value>
        /// List of thw status for the orders.
        /// </value>
        public static List<string> ListStatusOrdenesForQfbCount
            => new List<string>
            {
                Asignado,
                Proceso,
                Pendiente,
                Reasignado,
            };

        /// <summary>
        /// Gets the clasification user DZ.
        /// </summary>
        /// <value>
        /// String UserTypeDZ.
        /// </value>
        public static string UserClassificationDZ => "DZ";

        /// <summary>
        /// Gets error users dz automatico.
        /// </summary>
        /// <value>
        /// String ErrorUsersDZAutomatico.
        /// </value>
        public static string ErrorUsersDZAutomatico
            => "No hay químico asignado para atender los productos Dermazone, favor de configurar un usuario o asignarlos manualmente";

        /// <summary>
        /// Gets StartDateParam.
        /// </summary>
        /// <value>
        /// String StartDateParam.
        /// </value>
        public static string StartDateParam => "startdate";

        /// <summary>
        /// Gets EndDateParam.
        /// </summary>
        /// <value>
        /// String EndDateParam.
        /// </value>
        public static string EndDateParam => "enddate";

        /// <summary>
        /// Gets DateTimeFormatddMMyyyy.
        /// </summary>
        /// <value>
        /// String DateTimeFormatddMMyyyy.
        /// </value>
        public static string DateTimeFormatddMMyyyy => "dd/MM/yyyy";

        /// <summary>
        /// Gets SeparationProcessCancelProductionOrderEndPoint.
        /// </summary>
        /// <value>
        /// String SeparationProcessCancelProductionOrderEndPoint.
        /// </value>
        public static string SeparationProcessCancelProductionOrderEndPoint => "separationprocess/cancelproductionorder";

        /// <summary>
        /// Gets list of signatures to assign products DZ in lowercase.
        /// </summary>
        /// <value>
        /// String SignaturesToAssignProductsDZ.
        /// </value>
        public static List<string> SignaturesToAssignProductsDZ
            => new List<string>
            {
                "dermazone",
            };

        /// <summary>
        /// Gets list of signatures to assign products DZ in lowercase.
        /// </summary>
        /// <value>
        /// String SignaturesToAssignProductsDZ.
        /// </value>
        public static Dictionary<(bool, string), int> OrderPriorities { get; } = new Dictionary<(bool, string), int>
        {
            { (true, "Completa"), 1 },
            { (true, "Hija"), 2 },
            { (false, "Padre"), 3 },
            { (false, "Completa"), 4 },
            { (false, "Hija"), 5 },
        };
    }
}