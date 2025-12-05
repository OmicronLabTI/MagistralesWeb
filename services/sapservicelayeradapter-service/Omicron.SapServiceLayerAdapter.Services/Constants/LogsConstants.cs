// <summary>
// <copyright file="LogsConstants.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Services.Constants
{
    /// <summary>
    /// the class for logs constatns.
    /// </summary>
    public static class LogsConstants
    {
        /// <summary>
        /// Gets PrimaryValidationForProductionOrderFinalization.
        /// </summary>
        /// <value>
        /// PrimaryValidationForProductionOrderFinalization.
        /// </value>
        public static string PrimaryValidationForProductionOrderFinalization => "{0} - SapServiceLayerAdapter - Primary Validation For Production Order Finalization";

        /// <summary>
        /// Gets FinalizeProductionOrderInSap.
        /// </summary>
        /// <value>
        /// FinalizeProductionOrderInSap.
        /// </value>
        public static string FinalizeProductionOrderInSap => "{0} - SapServiceLayerAdapter - Finalize Production Order In Sap";

        /// <summary>
        /// Gets SearchProductionOrder.
        /// </summary>
        /// <value>
        /// SearchProductionOrder.
        /// </value>
        public static string SearchProductionOrder => "{LogBase} - Search production order {ProductionOrderId} on SAP";

        /// <summary>
        /// Gets NotFoundProductionOrder.
        /// </summary>
        /// <value>
        /// NotFoundProductionOrder.
        /// </value>
        public static string NotFoundProductionOrder => "La orden de produción {0} no existe.";

        /// <summary>
        /// Gets ProductionOrderIsClosed.
        /// </summary>
        /// <value>
        /// ProductionOrderIsClosed.
        /// </value>
        public static string ProductionOrderIsClosed => "{LogBase} - Production order is closed - {ProductionOrderId}";

        /// <summary>
        /// Gets ProductionOrderNotReleased.
        /// </summary>
        /// <value>
        /// ProductionOrderNotReleased.
        /// </value>
        public static string ProductionOrderNotReleased => "{LogBase} - Production order not released - {ProductionOrderId}";

        /// <summary>
        /// Gets ValidateRequiredQuantityForRetroactiveIssue.
        /// </summary>
        /// <value>
        /// ValidateRequiredQuantityForRetroactiveIssue.
        /// </value>
        public static string ValidateRequiredQuantityForRetroactiveIssue => "{LogBase} - Validate required quantity for retroactive issues - {ProductionOrderId}";

        /// <summary>
        /// Gets ValidatingNewBatches.
        /// </summary>
        /// <value>
        /// ValidatingNewBatches.
        /// </value>
        public static string ValidatingNewBatches => "{LogBase} - Validating new batches - {ProductionOrderId}";

        /// <summary>
        /// Gets ExecuteFinalizationSteps.
        /// </summary>
        /// <value>
        /// ExecuteFinalizationSteps.
        /// </value>
        public static string ExecuteFinalizationSteps => "{LogBase} - Execute Finalization Steps {ProductionOrderId}";

        /// <summary>
        /// Gets AlreadyBeingProcessedByAnotherPod.
        /// </summary>
        /// <value>
        /// AlreadyBeingProcessedByAnotherPod.
        /// </value>
        public static string AlreadyBeingProcessedByAnotherPod => "{LogBase} - ProductionOrder {ProductionOrderId} Already being processed by another pod. LockKey={LockKey} Pod={PodName} ";

        /// <summary>
        /// Gets StepNotRecognized.
        /// </summary>
        /// <value>
        /// StepNotRecognized.
        /// </value>
        public static string StepNotRecognized => "{LogBase} - Step: '{LastStep}' is not recognized. Skipping.";

        /// <summary>
        /// Gets CreateInventoryGenExit.
        /// </summary>
        /// <value>
        /// CreateInventoryGenExit.
        /// </value>
        public static string CreateInventoryGenExit => "{LogBase} - Create Inventory GenExit {ProductionOrderId}";

        /// <summary>
        /// Gets CreateReceiptFromProductionOrderId.
        /// </summary>
        /// <value>
        /// CreateReceiptFromProductionOrderId.
        /// </value>
        public static string CreateReceiptFromProductionOrderId => "{LogBase} - Create Receipt From Production Order Id {ProductionOrderId}";

        /// <summary>
        /// Gets CloseProductionOrder.
        /// </summary>
        /// <value>
        /// CloseProductionOrder.
        /// </value>
        public static string CloseProductionOrder => "{LogBase} - Close Production Order {ProductionOrderId}";

        /// <summary>
        /// Gets ComponentAlreadyHasConsumedQuantity.
        /// </summary>
        /// <value>
        /// ComponentAlreadyHasConsumedQuantity.
        /// </value>
        public static string ComponentAlreadyHasConsumedQuantity => "{LogBase} - [VALIDATE CONSUMED QUANTITY] The Component Already Has Consumed Quantity: {ItemNo}, Required  {PlannedQuantity}, Consumed: {IssuedQuantity}.";

        /// <summary>
        /// Gets ErrorOcurredOnCreateInventoryGenExit.
        /// </summary>
        /// <value>
        /// ErrorOcurredOnCreateInventoryGenExit.
        /// </value>
        public static string ErrorOcurredOnCreateInventoryGenExit => "{LogBase} - An Error Has Ocurred On Create oInventoryGenExit {Code} - {UserError}.";

        /// <summary>
        /// Gets LogBatchesQuantityWithDecimalSeparator.
        /// </summary>
        /// <value>
        /// LogBatchesQuantityWithDecimalSeparator.
        /// </value>
        public static string LogBatchesQuantityWithDecimalSeparator => "{LogBase} - Log Batches Quantity With Decimal Separator {Separator}.";

        /// <summary>
        /// Gets LogBatchesSum.
        /// </summary>
        /// <value>
        /// LogBatchesSum.
        /// </value>
        public static string LogBatchesSum => "{LogBase} - Sum {BatchesSum}.";

        /// <summary>
        /// Gets AnErrorOcurredOnSaveReceiptProduction.
        /// </summary>
        /// <value>
        /// AnErrorOcurredOnSaveReceiptProduction.
        /// </value>
        public static string AnErrorOcurredOnSaveReceiptProduction => "{LogBase} - An Error Has Ocurred On Save Receipt Production {Code} - {UserError}.";

        /// <summary>
        /// Gets AnErrorOcurredOnUpdateProductionOrderStatus.
        /// </summary>
        /// <value>
        /// AnErrorOcurredOnUpdateProductionOrderStatus.
        /// </value>
        public static string AnErrorOcurredOnUpdateProductionOrderStatus => "{LogBase} - An Error Has Ocurred On Update Production Order Status {Code} - {UserError}.";

        /// <summary>
        /// Gets ProcessLogTwoParts.
        /// </summary>
        /// <value>
        /// ProcessLogTwoParts.
        /// </value>
        public static string ProcessLogTwoParts => "{0} - {1}";

        /// <summary>
        /// Gets ProcessLogThreeParts.
        /// </summary>
        /// <value>
        /// ProcessLogThreeParts.
        /// </value>
        public static string ProcessLogThreeParts => "{0} - {1} - {2}";

        /// <summary>
        /// Gets CancelProductionOrderLogBase.
        /// </summary>
        /// <value>
        /// CancelProductionOrderLogBase.
        /// </value>
        public static string CancelProductionOrderLogBase => "{0} - Sap Service Layer - ProductionOrder: {1} - Cancel Order In SAP";

        /// <summary>
        /// Gets SeparateProductionOrderStart.
        /// </summary>
        /// <value>
        /// SeparateProductionOrderStart.
        /// </value>
        public static string CancelProductionOrderStart => "{LogBase} - Start Proccess";

        /// <summary>
        /// Gets GetProductionOrderToCancel.
        /// </summary>
        /// <value>
        /// GetProductionOrderToCancel.
        /// </value>
        public static string GetProductionOrderToCancel => "{LogBase} - Get Production Order";

        /// <summary>
        /// Gets ProductionOrderNotFound.
        /// </summary>
        /// <value>
        /// ProductionOrderNotFound.
        /// </value>
        public static string ProductionOrderNotFound => "Production Order NotFound";

        /// <summary>
        /// Gets ProductionOrderIsAlreadyCancelled.
        /// </summary>
        /// <value>
        /// ProductionOrderIsAlreadyCancelled.
        /// </value>
        public static string ProductionOrderIsAlreadyCancelled => "{LogBase} - Production Order Is Already Cancelled";

        /// <summary>
        /// Gets ErrorToUpdateProductionOrder.
        /// </summary>
        /// <value>
        /// ErrorToUpdateProductionOrder.
        /// </value>
        public static string ErrorToUpdateProductionOrder => "{LogBase} - Error To Update Production Order - Error: {Error} - {Message}";

        /// <summary>
        /// Gets ErrorToCancelProductionOrder.
        /// </summary>
        /// <value>
        /// ErrorToCancelProductionOrder.
        /// </value>
        public static string ErrorToCancelProductionOrder => "{LogBase} - Error To Cancel Production Order - Error: {Error} - {Message}";

        /// <summary>
        /// Gets UpdateProductionOrderToCancel.
        /// </summary>
        /// <value>
        /// UpdateProductionOrderToCancel.
        /// </value>
        public static string UpdateProductionOrderToCancel => "{LogBase} - Update Production Order";

        /// <summary>
        /// Gets CancelProductionOrderToCancel.
        /// </summary>
        /// <value>
        /// CancelProductionOrderToCancel.
        /// </value>
        public static string CancelProductionOrderToCancel => "{LogBase} - Cancel Production Order";

        /// <summary>
        /// Gets CreateInvoiceInSapLogBase.
        /// </summary>
        /// <value>
        /// CreateInvoiceInSapLogBase.
        /// </value>
        public static string CreateInvoiceInSapLogBase => "{0} - SapServiceLayerAdapter - Create Invoice";

        /// <summary>
        /// Gets GetDeliveriesForInvoice.
        /// </summary>
        /// <value>
        /// GetDeliveriesForInvoice.
        /// </value>
        public static string GetDeliveriesForInvoice => "{LogBase} - Get Deliveries For Invoice {DeliveryObject}";

        /// <summary>
        /// Gets CreateInvoiceOnSap.
        /// </summary>
        /// <value>
        /// CreateInvoiceOnSap.
        /// </value>
        public static string CreateInvoiceOnSap => "{LogBase} - Create Invoice On SAP - {InvoiceRequest}";

        /// <summary>
        /// Gets InvoiceCreatedSuccessfully.
        /// </summary>
        /// <value>
        /// InvoiceCreatedSuccessfully.
        /// </value>
        public static string InvoiceCreatedSuccessfully => "{LogBase} - Invoice Created Successfully";

        /// <summary>
        /// Gets ServiceLayerErrorToCreateInvoice.
        /// </summary>
        /// <value>
        /// ServiceLayerErrorToCreateInvoice.
        /// </value>
        public static string ServiceLayerErrorToCreateInvoice => "{LogBase} - Service Layer Error To Create Invoice";

        /// <summary>
        /// Gets InvoiceServiceLayerError.
        /// </summary>
        /// <value>
        /// InvoiceServiceLayerError.
        /// </value>
        public static string InvoiceServiceLayerError => "{LogBase} - Service Layer Error: {UserError}";
    }
}
