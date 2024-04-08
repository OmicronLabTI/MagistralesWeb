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

        /// <summary>
        /// Custom property cfdi from environment.
        /// </summary>
        public const string CustomPropertyNameCFDI = "CustomPropertiesName:CFDI";

        /// <summary>
        /// Custom property cfdi from environment.
        /// </summary>
        public const string OrdersCFDIProperty = "U_CFDI_Provisional";
    }
}
