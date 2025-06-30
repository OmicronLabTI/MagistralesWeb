// <summary>
// <copyright file="ServiceConstants.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Services.Constants
{
    /// <summary>
    /// class for cosntants.
    /// </summary>
    public static class ServiceConstants
    {
        /// <summary>
        /// Fail reason.
        /// </summary>
        public const string FailReasonProductCodeNotExists = "El producto con código {0} no existe.";

        /// <summary>
        /// Unexpected error.
        /// </summary>
        public const string UnexpectedError = "UnexpectedError";

        /// <summary>
        /// the resource was not found.
        /// </summary>
        public const string NotFound = "NotFound";

        /// <summary>
        /// Status Liberado.
        /// </summary>
        public const string StatusLiberado = "R";

        /// <summary>
        /// Custom property cfdi from environment.
        /// </summary>
        public const string CustomPropertyNameCFDI = "CustomPropertiesName:CFDI";

        /// <summary>
        /// Custom property cfdi from environment.
        /// </summary>
        public const string OrdersCFDIProperty = "U_CFDI_Provisional";

        /// <summary>
        /// The service layer user env.
        /// </summary>
        public const string SAPServiceLayerUserEnvName = "SAPServiceLayer:User";

        /// <summary>
        /// The service layer password env.
        /// </summary>
        public const string SAPServiceLayerPwEnvName = "SAPServiceLayer:Pwd";

        /// <summary>
        /// The service layer companyDb env.
        /// </summary>
        public const string SAPServiceLayerDatabaseName = "SAPServiceLayer:CompanyDb";

        /// <summary>
        /// Query to get the Dictionary Key Error Generic Format.
        /// </summary>
        public const string DictionaryKeyErrorGenericFormat = "{0}-Error";

        /// <summary>
        /// Query to get the Dictionary Key Error Generic Format.
        /// </summary>
        public const string DictionaryValueErrorGenericFormat = "Error-{0}";

        /// <summary>
        /// Query to get the Dictionary Key Ok Generic Format.
        /// </summary>
        public const string DictionaryKeyOkGenericFormat = "{0}-Ok";

        /// <summary>
        /// Query to get the Dictionary Key Ok Generic Format.
        /// </summary>
        public const string DictionaryKeyOkIdResult = "{0}-{1}-Ok";

        /// <summary>
        /// Query to get the Ok Label response.
        /// </summary>
        public const string OkLabelResponse = "Ok";

        /// <summary>
        /// Update Tracking Update Invoice Error.
        /// </summary>
        public const string UpdateTrackingUpdateInvoiceError = "Omicron.SapServiceLayerAdapter Service-Update tracking-Service Layer-The invoice {0} was tried to be updated {1}-{2}";

        /// <summary>
        /// Update Tracking Invoice Not Found Error.
        /// </summary>
        public const string UpdateTrackingInvoiceNotFoundError = "Omicron.SapServiceLayerAdapter Service-Update tracking-Service Layer-The invoice {0} not found.";

        /// <summary>
        /// Update Tracking Invoice Updated Log.
        /// </summary>
        public const string UpdateTrackingInvoiceUpdatedLog = "Omicron.SapServiceLayerAdapter Service-Update tracking-Service Layer-The invoice {0} was updated.";

        /// <summary>
        /// Close Sample Order Not Found Error.
        /// </summary>
        public const string CloseSampleOrderNotFoundError = "Omicron.SapServiceLayerAdapter Service-Close Sample Order-Service Layer-The Order {0} not found.";

        /// <summary>
        /// Update Tracking Invoice Error.
        /// </summary>
        public const string UpdateTrackingInvoiceError = "Error while updating tracking invoice {0} {1} - ex: {2} - stackTrace: {3}";

        /// <summary>
        /// Service Layer Error Handled.
        /// </summary>
        public const string ServiceLayerErrorHandled = "{0}-ErrorHandled";

        /// <summary>
        /// Close Sample Order Comment.
        /// </summary>
        public const string CloseSampleOrderComment = "Pedido muestra basado en {0}";

        /// <summary>
        /// shippingcost item code.
        /// </summary>
        public const string ShippingCostItemCode = "FL 1";

        /// <summary>
        /// Si es pedido paquete.
        /// </summary>
        public const string IsPackage = "Y";

        /// <summary>
        /// Si a package.
        /// </summary>
        public const string IsNotPackage = "N";

        /// <summary>
        /// Gets the user orders.
        /// </summary>
        public const string Magistral = "magistral";

        /// <summary>
        /// Gets the user orders.
        /// </summary>
        public const string MagistralUpper = "Magistral";

        /// <summary>
        /// Gets the user orders.
        /// </summary>
        public const string Mixto = "mixto";

        /// <summary>
        /// The line products.
        /// </summary>
        public const string Linea = "linea";

        /// <summary>
        /// The line products.
        /// </summary>
        public const string LineaUpper = "Línea";

        /// <summary>
        /// Close Sample Order Comment.
        /// </summary>
        public const string CloseSampleOrderLogInfo = "Closing sample with {0}";

        /// <summary>
        /// Close Sample Order An Inventory Error.
        /// </summary>
        public const string CloseSampleOrderAnInventoryError = "Error while Closing and create inventory exit for {0} - ex: {1} - stackTrace: {2}";

        /// <summary>
        /// Close Sample Order Error To Close Order.
        /// </summary>
        public const string CloseSampleOrderErrorToCloseOrder = "An error has ocurred while closing sale order {0} {1}.";

        /// <summary>
        /// Close Sample Order Error To Create InventoryGenExit.
        /// </summary>
        public const string CloseSampleOrderErrorToCreateInventoryGenExit = "An error has ocurred on create oInventoryGenExit {0}.";

        /// <summary>
        /// Close Sample Order Inventory Error.
        /// </summary>
        public const string CloseSampleOrderInventoryError = "{0}-Inventory-Error";

        /// <summary>
        /// Cancel Delivery Note Not Found Error.
        /// </summary>
        public const string CancelDeliveryNoteNotFoundError = "Omicron.SapServiceLayerAdapter Service-Cancel Delivery Note-Service Layer-The Delivery Note {0} not found.";

        /// <summary>
        /// Cancel Delivery Error To Create Document.
        /// </summary>
        public const string CancelDeliveryErrorToCreateDocument = "The delivery {0} was triend to be CANCELLED. {1}.";

        /// <summary>
        /// Cancel Delivery Error To Create Document.
        /// </summary>
        public const string CancelDeliveryLogToCancelDelivery = "The delivery note {0} was cancelled.";

        /// <summary>
        /// the total values.
        /// </summary>
        public const string Total = "total";

        /// <summary>
        /// Close Sample Order Error To Create InventoryGenExit.
        /// </summary>
        public const string CancelDeliveryErrorToCreateStockTransfer = "The transfer for delivery: {0} was tried to be made. {1}";

        /// <summary>
        /// Cancel Delivery Transfer Error.
        /// </summary>
        public const string CancelDeliveryTransferError = "{0}-Transfer-Error";

        /// <summary>
        /// Cancel Delivery Transfer Error.
        /// </summary>
        public const string TransferRequestForDeliveryDone = "The transfer for delivery: {0} was done.";

        /// <summary>
        /// Cancel Delivery Transfer Error.
        /// </summary>
        public const string TransferRequestForDeliveryOk = "{0}-Transfer-Ok";

        /// <summary>
        /// Cancel Delivery Error To Cancel Order.
        /// </summary>
        public const string CancelDeliveryErrorToCancelOrder = "The order: {0} was tried to cancel. {1}";

        /// <summary>
        /// Sap Files Save Prescription To Server Url.
        /// </summary>
        public const string SavePrescriptionToServer = "save/prescriptiontoserver";

        /// <summary>
        /// Invalid adviser id message.
        /// </summary>
        public const string InvalidAdviserId = "El identificador del asesor no es valido";

        /// <summary>
        /// Doctor not found.
        /// </summary>
        public const string DoctorNotFound = "El doctor no se encontró en SAP";

        /// <summary>
        /// the total values.
        /// </summary>
        public const string ActionUpdate = "u";

        /// <summary>
        /// the total values.
        /// </summary>
        public const string ActionInsert = "i";

        /// <summary>
        /// the total values.
        /// </summary>
        public const string ActionDelete = "d";

        /// <summary>
        /// the Delivery Address type.
        /// </summary>
        public const string DeliveryAddress = "bo_ShipTo";

        /// <summary>
        /// the Invoice Address type.
        /// </summary>
        public const string InvoiceAddress = "bo_BillTo";

        /// <summary>
        /// Get the address bill type.
        /// </summary>
        public const string AddresBillType = "B";

        /// <summary>
        /// Get the address ship type.
        /// </summary>
        public const string AddresShipType = "S";

        /// <summary>
        /// Production Order status closed.
        /// </summary>
        public const string ProductionOrderClosed = "boposClosed";

        /// <summary>
        /// Production Order status released.
        /// </summary>
        public const string ProductionOrderReleased = "boposReleased";

        /// <summary>
        /// Production Order type B.
        /// </summary>
        public const string ProductionOrderTypeB = "im_Backflush";

        /// <summary>
        /// Production Order type B.
        /// </summary>
        public const string ProductionOrderTypeM = "im_Manual";

        /// <summary>
        /// Fail reason.
        /// </summary>
        public const string FailReasonNotReleasedProductionOrder = "La orden de producción {0} no esta liberada.";

        /// <summary>
        /// Fail reason.
        /// </summary>
        public const string FailConsumedQuantity = "No es posible finalizar la siguiente orden {0}, la cantidad a consumir ya cuenta con un registro";

        /// <summary>
        /// Fail reason.
        /// </summary>
        public const string FailGetProduct = "No fue posible obtener la información del producto {0}";

        /// <summary>
        /// Fail reason.
        /// </summary>
        public const string FailReasonBatchAlreadyExists = "El lote {0} ya existe para el producto {1}.";

        /// <summary>
        /// Fail reason.
        /// </summary>
        public const string FailReasonNotAvailableRequiredQuantity = "No se ha podido generar recibo de producción para la orden de fabricación {0}, no hay componentes disponibles ({1}).";

        /// <summary>
        /// Fail reason.
        /// </summary>
        public const string FailReasonNotGetExitCreated = "No se ha podido generar la entrega de componentes a producción para la orden de fabricación {0}.";

        /// <summary>
        /// Fail reason.
        /// </summary>
        public const string FailReasonNotReceipProductionCreated = "No se ha podido crear recepción de producción para la orden de fabricación {0}.";

        /// <summary>
        /// Fail reason.
        /// </summary>
        public const string FailReasonNotProductionStatusClosed = "La orden de producción {0} no se ha podido cerrar.";

        /// <summary>
        /// Fail reason.
        /// </summary>
        public const string FailReasonUnexpectedError = "Ocurrió un error inesperado en SAP.";

        /// <summary>
        /// error while create transfer request.
        /// </summary>
        public const string ErrorTransferRequest = "ErrorTransferRequest";

        /// <summary>
        /// deletes the batch.
        /// </summary>
        public const string DeleteBatch = "delete";

        /// <summary>
        /// error while updating.
        /// </summary>
        public const string ErrorUpdateFabOrd = "ErrorUpdateFabOrd";

        /// <summary>
        /// the order was not found.
        /// </summary>
        public const string OrderNotFound = "OrderNotFound";

        /// <summary>
        /// the value to delete the conmponent.
        /// </summary>
        public const string DeleteComponent = "delete";

        /// <summary>
        /// error while inserting.
        /// </summary>
        public const string ErrorCreateFabOrd = "ErrorCreateFabOrd";

        /// <summary>
        /// status cancelled.
        /// </summary>
        public const string ProductionOrderCancelled = "boposCancelled";

        /// <summary>
        /// error the sales order is closed.
        /// </summary>
        public const string ErrorProductionOrderCancelled = "ErrorProductionOrderCancelled";

        /// <summary>
        /// Fail reason.
        /// </summary>
        public const string FailReasonUnexpectedErrorToCreateIsolatedProductionOrder = "Ocurrió un problema inesperado al crear la orden de fabricación para el producto {0} - {1}.";

        /// <summary>
        /// ClientTypeInstitucional.
        /// </summary>
        public const string ClientTypeInstitucional = "institucional";

        /// <summary>
        /// yes.
        /// </summary>
        public const string Yes = "1";

        /// <summary>
        /// no.
        /// </summary>
        public const string No = "2";

        /// <summary>
        /// Gets DictionaryValueFormat.
        /// </summary>
        /// <value>
        /// String DictionaryValueFormat.
        /// </value>
        public static string DictionaryValueFormat => "{0}, {1}";

        /// <summary>
        /// Gets maxSapTrackingLength.
        /// </summary>
        public static int MaxSapTrackingLength => 30;

        /// <summary>
        /// Gets CommaChar.
        /// </summary>
        public static char CommaChar => ',';

        /// <summary>
        /// Gets HypenChar.
        /// </summary>
        public static char HypenChar => '-';

        /// <summary>
        /// Gets the status of the order.
        /// </summary>
        /// <value>
        /// the status.
        /// </value>
        public static Dictionary<string, string> RelationMessagesServiceLayer { get; } = new Dictionary<string, string>
        {
            { "No existen registros coincidentes (ODBC -2028)", "Error-No se encontró la factura." },
            { "Invalid session or session already timeout.", "Error-Sesión no válida o la sesión ya ha expirado." },
        };

        /// <summary>
        /// Gets the status of the order.
        /// </summary>
        /// <value>
        /// the status.
        /// </value>
        public static Dictionary<string, string> DictWhs { get; } = new Dictionary<string, string>
        {
            { "MN", "MN" },
            { "MG", "MG" },
            { "BE", "BE" },
            { "MX", "MG" },
        };
    }
}
