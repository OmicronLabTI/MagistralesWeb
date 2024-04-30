// <summary>
// <copyright file="ServiceQuerysConstants.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Services.Constants
{
        /// <summary>
        /// class for service querys constants.
        /// </summary>
        public static class ServiceQuerysConstants
        {
                /// <summary>
                /// Query to get the last generated order.
                /// </summary>
                public const string QryGetLastGeneratedOrder = "Orders?$orderby=DocEntry desc&$top=1";

                /// <summary>
                /// Query to get invoice document by doc entry.
                /// </summary>
                public const string QryInvoiceDocumentByDocEntry = "Invoices({0})";

                /// <summary>
                /// Query to get invoice document by doc entry.
                /// </summary>
                public const string QryGetShippingTypesByName = "ShippingTypes?$filter=Name eq '{0}'";

                /// <summary>
                /// Query to get invoice document by doc entry.
                /// </summary>
                public const string QryOrdersDocumentByDocEntry = "Orders({0})";

                /// <summary>
                /// Query Post Inventory Gen Exists.
                /// </summary>
                public const string QryPostInventoryGenExists = "InventoryGenExits";

                /// <summary>
                /// Query Post Inventory Gen Entries.
                /// </summary>
                public const string QryPostInventoryGenEntries = "InventoryGenEntries";

                /// <summary>
                /// Query Post Close Order By Id.
                /// </summary>
                public const string QryPostCloseOrderById = "Orders({0})/Close";

                /// <summary>
                /// Query Get Delivery Note By Id.
                /// </summary>
                public const string QryGetDeliveryNoteById = "DeliveryNotes({0})";

                /// <summary>
                /// Query To Create Delivery Note Cancel Document By Id.
                /// </summary>
                public const string QryToCreateDeliveryNoteCancelDocumentById = "DeliveryNotes({0})/Cancel";

                /// <summary>
                /// Query Qry Post Stock Transfers.
                /// </summary>
                public const string QryPostStockTransfers = "StockTransfers";

                /// <summary>
                /// Query Delivery Notes.
                /// </summary>
                public const string QryDeliveryNotes = "DeliveryNotes";

                /// <summary>
                /// Query Cancel Orders.
                /// </summary>
                public const string QryCancelOrders = "Orders({0})/Cancel";

                /// <summary>
                /// Query Delivery Notes.
                /// </summary>
                public const string QryAttachments2 = "Attachments2";

                /// <summary>
                /// Query Create Order.
                /// </summary>
                public const string QryPostOrders = "Orders";

                /// <summary>
                /// Query to get update or delete the employee info by employee id.
                /// </summary>
                public const string QryEmployeesInfoByDocEntry = "EmployeesInfo({0})";

                /// <summary>
                /// Query Doctor By Id.
                /// </summary>
                public const string QryDoctorbyId = "BusinessPartners('{0}')";

                /// <summary>
                /// Query to get the production order by id.
                /// </summary>
                public const string QryProductionOrderById = "ProductionOrders({0})";

                /// <summary>
                /// Query to get the production order by id.
                /// </summary>
                public const string QryProductionOrder = "ProductionOrders";

                /// <summary>
                /// Query to get the product by id.
                /// </summary>
                public const string QryProductById = "Items('{0}')";

                /// <summary>
                /// Query Post Inventory Transfer Requests.
                /// </summary>
                public const string QryPostInventoryTransferRequests = "InventoryTransferRequests";

                /// <summary>
                /// Query to get the product by id.
                /// </summary>
                public const string QryBatchNumbers = "BatchNumberDetails?$filter=ItemCode eq '{0}' and Batch eq '{1}'";

                /// <summary>
                /// Query to get the production order by id to cancel.
                /// </summary>
                public const string QryProductionOrderByIdCancel = "ProductionOrders({0})/Cancel";
    }
}
