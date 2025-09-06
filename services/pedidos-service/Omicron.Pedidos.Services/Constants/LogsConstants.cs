// <summary>
// <copyright file="LogsConstants.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Services.Constants
{
    /// <summary>
    /// the class for logs constatns.
    /// </summary>
    public static class LogsConstants
    {
        /// <summary>
        /// Gets CloseInSapNextStep.
        /// </summary>
        /// <value>
        /// CloseInSapNextStep.
        /// </value>
        public static string CloseInSapNextStep => "Close In Sap";

        /// <summary>
        /// Gets CloseInPostgresqlNextStep.
        /// </summary>
        /// <value>
        /// CloseInPostgresqlNextStep.
        /// </value>
        public static string CloseInPostgresqlNextStep => "Close In Postgresql";

        /// <summary>
        /// Gets GeneratePdfNextStep.
        /// </summary>
        /// <value>
        /// GeneratePdfNextStep.
        /// </value>
        public static string GeneratePdfNextStep => "Generate PDF";

        /// <summary>
        /// Gets StartFinalizeAllProductionOrders.
        /// </summary>
        /// <value>
        /// StartFinalizeAllProductionOrders.
        /// </value>
        public static string StartFinalizeAllProductionOrders => "Pedidos Service - Start Finalize All Production Orders - {ProductionOrdersToFinalize}";

        /// <summary>
        /// Gets FinalizeProductionOrdersAsync.
        /// </summary>
        /// <value>
        /// FinalizeProductionOrdersAsync.
        /// </value>
        public static string FinalizeProductionOrdersAsync => "{0} - Pedidos Service - Finalize Production Orders Async";

        /// <summary>
        /// Gets StartValidatePrimaryRules.
        /// </summary>
        /// <value>
        /// StartValidatePrimaryRules.
        /// </value>
        public static string StartValidatePrimaryRules => "{LogBase} - Start Validate Primary Rules - {ProductionOrderId}";

        /// <summary>
        /// Gets SendKafkaMessageFinalizeProductionOrderSap.
        /// </summary>
        /// <value>
        /// SendKafkaMessageFinalizeProductionOrderSap.
        /// </value>
        public static string SendKafkaMessageFinalizeProductionOrderSap => "{LogBase} - Send Kafka Message Finalize Production Order Sap - {ProductionOrderProcessing}";

        /// <summary>
        /// Gets InsertAllProductionOrderProcessingStatus.
        /// </summary>
        /// <value>
        /// InsertAllProductionOrderProcessingStatus.
        /// </value>
        public static string InsertAllProductionOrderProcessingStatus => "Pedidos Service - Insert All ProductionOrder Processing Status - {ProductionOrderProcessingStatus}";

        /// <summary>
        /// Gets EndFinalizeAllProductionOrders.
        /// </summary>
        /// <value>
        /// EndFinalizeAllProductionOrders.
        /// </value>
        public static string EndFinalizeAllProductionOrders => "Pedidos Service - End Finalize All Production Orders - {ProductionOrdersToFinalize}";

        /// <summary>
        /// Gets EndFinalizeAllProductionOrdersWithError.
        /// </summary>
        /// <value>
        /// EndFinalizeAllProductionOrdersWithError.
        /// </value>
        public static string EndFinalizeAllProductionOrdersWithError => "Pedidos Service - End Finalize All Production Orders With Error - Message Exception: {0} - Inner Exception: {1}";

        /// <summary>
        /// Gets EndFinalizeProductionOrderOnSapWithError.
        /// </summary>
        /// <value>
        /// EndFinalizeProductionOrderOnSapWithError.
        /// </value>
        public static string EndFinalizeProductionOrderOnSapWithError => "Pedidos Service - End Finalize Production Order On Sap With Error - Message Exception: {0} - Inner Exception: {1}";

        /// <summary>
        /// Gets EndFinalizeProductionOrderOnPostgresWithError.
        /// </summary>
        /// <value>
        /// EndFinalizeProductionOrderOnPostgresWithError.
        /// </value>
        public static string EndFinalizeProductionOrderOnPostgresWithError => "Pedidos Service - End Update Order On Postgres With Error - Message Exception: {0} - Inner Exception: {1}";

        /// <summary>
        /// Gets EndFinalizeProductionOrderOnPostgresWithError.
        /// </summary>
        /// <value>
        /// EndFinalizeProductionOrderOnPostgresWithError.
        /// </value>
        public static string EndCreatePdfWithError => "Pedidos Service - End Generate Pdf of Production Orders  With Error - Message Exception: {0} - Inner Exception: {1}";

        /// <summary>
        /// Gets AnUnexpectedErrorOccurred.
        /// </summary>
        /// <value>
        /// AnUnexpectedErrorOccurred.
        /// </value>
        public static string AnUnexpectedErrorOccurred => "An unexpected error occurred.";

        /// <summary>
        /// Gets ProductionOrderIsAlreadyBeignProcessed.
        /// </summary>
        /// <value>
        /// ProductionOrderIsAlreadyBeignProcessed.
        /// </value>
        public static string ProductionOrderIsAlreadyBeignProcessed => "Production Order Is Already Beign Processed - {ProductionOrderId}";

        /// <summary>
        /// Gets PostSapValidationFinalization.
        /// </summary>
        /// <value>
        /// PostSapValidationFinalization.
        /// </value>
        public static string PostSapValidationFinalization => "{LogBase} - Post Sap Validation Finalization - {OrderToFinish}";

        /// <summary>
        /// Gets ErrorOnSAP.
        /// </summary>
        /// <value>
        /// ErrorOnSAP.
        /// </value>
        public static string ErrorOnSAP => "{LogBase} - Error On SAP - {ProductionOrderId}";

        /// <summary>
        /// Gets ErrorOnSAPResponseNull.
        /// </summary>
        /// <value>
        /// ErrorOnSAPResponseNull.
        /// </value>
        public static string ErrorOnSAPResponseNull => "{LogBase} - Error On SAP Response Null - {ProductionOrderId}";

        /// <summary>
        /// Gets ValidationErrorOnSAP.
        /// </summary>
        /// <value>
        /// ValidationErrorOnSAP.
        /// </value>
        public static string ValidationErrorOnSAP => "{LogBase} - Validation Error On SAP - {ProductionOrderId}";

        /// <summary>
        /// Gets FinalizeProductionOrdersOnSapAsync.
        /// </summary>
        /// <value>
        /// FinalizeProductionOrdersOnSapAsync.
        /// </value>
        public static string FinalizeProductionOrdersOnSapAsync => "{0} - Pedidos Service - Finalize Production Orders Async On SAP";

        /// <summary>
        /// Gets FinalizeProductionOrdersOnSapAsync.
        /// </summary>
        /// <value>
        /// FinalizeProductionOrdersOnSapAsync.
        /// </value>
        public static string UpdateProductionOrdersOnPostgresAsync => "{0} - Pedidos Service - Update Production Orders Async On Postgres";

        /// <summary>
        /// Gets FinalizeProductionOrdersOnSapAsync.
        /// </summary>
        /// <value>
        /// FinalizeProductionOrdersOnSapAsync.
        /// </value>
        public static string GeneratePdfOfProductionOrdersAsync => "{0} - Pedidos Service - Generate PDF of Production Orders Async";

        /// <summary>
        /// Gets PostSapFinalizeProductionOrderProcess.
        /// </summary>
        /// <value>
        /// PostSapFinalizeProductionOrderProcess.
        /// </value>
        public static string PostSapFinalizeProductionOrderProcess => "{LogBase} - Post Sap Finalize Production Order Process - {OrderToFinish}";

        /// <summary>
        /// Gets UpdateProductionOrdersOnPostgres.
        /// </summary>
        /// <value>
        /// UpdateProductionOrdersOnPostgres.
        /// </value>
        public static string UpdateProductionOrdersOnPostgres => "{LogBase} - Updating Production and Sales Orders in Postgres - Orders to finalize: {OrderToFinish}";

        /// <summary>
        /// Gets StartGeneratePdfProcess.
        /// </summary>
        /// <value>
        /// StartGeneratePdfProcess.
        /// </value>
        public static string StartGeneratePdfProcess => "{LogBase} - Starting PDF generation process for ProductionOrderId: {OrderToFinish}";

        /// <summary>
        /// Gets UpdateProductionOrdersOnPostgres.
        /// </summary>
        /// <value>
        /// UpdateProductionOrdersOnPostgres.
        /// </value>
        public static string FinalizingFabOrder => "{LogBase} - Finalizing Fab Order: {Productionorderid}";

        /// <summary>
        /// Gets UpdateProductionOrdersOnPostgres.
        /// </summary>
        /// <value>
        /// UpdateProductionOrdersOnPostgres.
        /// </value>
        public static string IfFinalizingFabOrder => "La orden de fabricacion ya esta finalizada, tiene status: {ProductionorderStatus}";

        /// <summary>
        /// Gets UpdateProductionOrdersOnPostgres.
        /// </summary>
        /// <value>
        /// UpdateProductionOrdersOnPostgres.
        /// </value>
        public static string ListToUpdate => "Lista de UserOrderModel por actualizar: {UserOrderModel}";

        /// <summary>
        /// Gets UpdateProductionOrdersOnPostgres.
        /// </summary>
        /// <value>
        /// UpdateProductionOrdersOnPostgres.
        /// </value>
        public static string FinalizingSalesOrder => "{LogBase} - Finalizing Sale Order: {Salesorderid}";

        /// <summary>
        /// Gets GeneratingPdf.
        /// </summary>
        /// <value>
        /// GeneratingPdf.
        /// </value>
        public static string GeneratingPdf => "{LogBase} - Generating PDF for SalesOrderId: {SalesOrderId}, IsolatedProductionOrderId: {IsolatedProductionOrderId}";

        /// <summary>
        /// Gets ErrorInSAPWhileFinalizingTheOrder.
        /// </summary>
        /// <value>
        /// ErrorInSAPWhileFinalizingTheOrder.
        /// </value>
        public static string ErrorInSAPWhileFinalizingTheOrder => "{LogBase} - Error In SAP While Finalizing The Order - {ProductionOrderId}";

        /// <summary>
        /// Gets StartFinalizeProductionOrderInSap.
        /// </summary>
        /// <value>
        /// StartFinalizeProductionOrderInSap.
        /// </value>
        public static string StartFinalizeProductionOrderInSap => "Pedidos Service - Start Finalize Production Order On SAP - {ProductionOrdersToFinalize}";

        /// <summary>
        /// Gets StartFinalizeProductionOrderInSap.
        /// </summary>
        /// <value>
        /// StartFinalizeProductionOrderInSap.
        /// </value>
        public static string StartFinalizeProductionOrderInPostgres => "Pedidos Service - Start Finalize Production Order On Postgres - {ProductionOrdersToFinalize}";

        /// <summary>
        /// Gets StartFinalizeProductionOrderInSap.
        /// </summary>
        /// <value>
        /// StartFinalizeProductionOrderInSap.
        /// </value>
        public static string CreateChildOrdersLogBase => "Pedidos Service - Start Create Child Production Order - {ProductionOrdersToCreateO}";

        /// <summary>
        /// Gets SeparateProductionOrderStart.
        /// </summary>
        /// <value>
        /// SeparateProductionOrderStart.
        /// </value>
        public static string CreateChildOrdersLogBaseStart => "{0} - Start Proccess - Pieces: {1} - SeparationID: {2}";

        /// <summary>
        /// Gets StartFinalizeProductionOrderInSap.
        /// </summary>
        /// <value>
        /// StartFinalizeProductionOrderInSap.
        /// </value>
        public static string StartCreationPdf => "Pedidos Service - Start Create PDF - {ProductionOrdersToFinalize}";

        /// <summary>
        /// Gets EndFinalizeProductionOrderInSap.
        /// </summary>
        /// <value>
        /// EndFinalizeProductionOrderInSap.
        /// </value>
        public static string EndFinalizeProductionOrderInSap => "Pedidos Service - End Finalize Production Order On SAP - {ProductionOrdersToFinalize}";

        /// <summary>
        /// Gets SendKafkaMessageFinalizeProductionOrderPostgresql.
        /// </summary>
        /// <value>
        /// SendKafkaMessageFinalizeProductionOrderPostgresql.
        /// </value>
        public static string SendKafkaMessageFinalizeProductionOrderPostgresql => "{LogBase} - Send Kafka Message Finalize Production Order Postgresql - {ProductionOrderProcessing}";

        /// <summary>
        /// Gets UpdateProductionOrderProcessingStatus.
        /// </summary>
        /// <value>
        /// UpdateProductionOrderProcessingStatus.
        /// </value>
        public static string UpdateProductionOrderProcessingStatus => "{LogBase} - Update ProductionOrder Processing Status - {ProductionOrderProcessingStatus}";

        /// <summary>
        /// Gets GenericErrorLog.
        /// </summary>
        /// <value>
        /// GenericErrorLog.
        /// </value>
        public static string GenericErrorLog => "Internal Error: {0} - {1}";

        /// <summary>
        /// Gets RetryFinalizingProductionOrder.
        /// </summary>
        /// <value>
        /// RetryFinalizingProductionOrder.
        /// </value>
        public static string RetryFinalizingProductionOrder => "{LogBase} - Retry Finalizing Production Order - Id: {Id} - Production Order: {ProductionOrderId} - Next Step: {NextStep}";

        /// <summary>
        /// Gets RetryFinalizingProductionOrderEndWithError.
        /// </summary>
        /// <value>
        /// RetryFinalizingProductionOrderEndWithError.
        /// </value>
        public static string RetryFinalizingProductionOrderEndWithError => "{0} - Retry Finalizing Production Order End Process With Error - Id: {1} - Production Order: {2} - Message {3} - Inner Exception - {4}";

        /// <summary>
        /// Gets StepNotRecognized.
        /// </summary>
        /// <value>
        /// StepNotRecognized.
        /// </value>
        public static string StepNotRecognized => "{LogBase} - Id: {Id} - Production Order: {ProductionOrderId} - Step: '{LastStep}' is not recognized. Skipping.";

        /// <summary>
        /// Gets RetryFailedProductionOrderFinalization.
        /// </summary>
        /// <value>
        /// RetryFailedProductionOrderFinalization.
        /// </value>
        public static string RetryFailedProductionOrderFinalization => "{0} - Retry Failed Production Order Finalization";

        /// <summary>
        /// Gets RetryFailedDivisionOrder.
        /// </summary>
        /// <value>
        /// RetryFailedDivisionOrder.
        /// </value>
        public static string RetryFailedDivisionOrder => "{0} - Retry Failed Production Order Division";

        /// <summary>
        /// Gets UpdateTaskAddedToQueue.
        /// </summary>
        /// <value>
        /// UpdateTaskAddedToQueue.
        /// </value>
        public static string UpdateTaskAddedToQueue => "Pedidos - BackgroundTaskQueue - Update Task Added To Queue";

        /// <summary>
        /// Gets StartingBackgroundProductionOrderSplit.
        /// </summary>
        /// <value>
        /// StartingBackgroundProductionOrderSplit.
        /// </value>
        public static string StartingBackgroundProductionOrderSplit => "{SeparationId} - Pedidos Service - Starting Background Production Order Separation - ProductionOrderId: {ProductionOrderId}";

        /// <summary>
        /// Gets SeparateProductionOrderLogBase.
        /// </summary>
        /// <value>
        /// SeparateProductionOrderLogBase.
        /// </value>
        public static string SeparateProductionOrderLogBase => "{0} - Pedidos Service - ProductionOrder: {1} - Separate Production Order";

        /// <summary>
        /// Gets SeparateProductionOrderStart.
        /// </summary>
        /// <value>
        /// SeparateProductionOrderStart.
        /// </value>
        public static string SeparateProductionOrderStart => "{LogBase} - Start Proccess - Pieces: {Pieces}";

        /// <summary>
        /// Gets SeparateProductionOrderEndSuccessfuly.
        /// </summary>
        /// <value>
        /// SeparateProductionOrderEndSuccessfuly.
        /// </value>
        public static string SeparateProductionOrderEndSuccessfuly => "{LogBase} - End Proccess Successfuly";

        /// <summary>
        /// Gets SeparateProductionOrderEndSuccessfuly.
        /// </summary>
        /// <value>
        /// SeparateProductionOrderEndSuccessfuly.
        /// </value>
        public static string SeparateProductionOrderEndWithError => "{0} - End Proccess With Error";

        /// <summary>
        /// Gets MaximumNumberOfRetriesReached.
        /// </summary>
        /// <value>
        /// MaximumNumberOfRetriesReached.
        /// </value>
        public static string MaximumNumberOfRetriesReached => "{0} - End Proccess With Error - Maximum Number Of Retries Reached";

        /// <summary>
        /// Gets RetryScheduledLog.
        /// </summary>
        /// <value>
        /// RetryScheduledLog.
        /// </value>
        public static string RetryScheduledLog => "{LogBase} - Retry scheduled in {Minutes} minutes - RetryAttempt: {RetryAttempt}";

        /// <summary>
        /// Gets ProductionOrderNotFound.
        /// </summary>
        /// <value>
        /// ProductionOrderNotFound.
        /// </value>
        public static string ProductionOrderNotFound => "Production Order Not Found";

        /// <summary>
        /// Gets ProductionOrderIsAlreadyCancelled.
        /// </summary>
        /// <value>
        /// ProductionOrderIsAlreadyCancelled.
        /// </value>
        public static string ProductionOrderIsAlreadyCancelled => "{LogBase} - Production Order Is Already Cancelled";

        /// <summary>
        /// Gets ProductionOrderCancelledSuccessfully.
        /// </summary>
        /// <value>
        /// ProductionOrderCancelledSuccessfully.
        /// </value>
        public static string ProductionOrderCancelledSuccessfully => "{LogBase} - Production Order Cancelled Successfully";

        /// <summary>
        /// Gets CancellingProductionOrderInSAP.
        /// </summary>
        /// <value>
        /// CancellingProductionOrderInSAP.
        /// </value>
        public static string CancellingProductionOrderInSAP => "{LogBase} - Cancelling Production Order In SAP";

        /// <summary>
        /// Gets FailedToCancelProductionOrderInSAP.
        /// </summary>
        /// <value>
        /// FailedToCancelProductionOrderInSAP.
        /// </value>
        public static string FailedToCancelProductionOrderInSAP => "{0} - Failed To Cancel Production Order In SAP";

        /// <summary>
        /// Gets CancellingProductionOrderInPostgreSQL.
        /// </summary>
        /// <value>
        /// CancellingProductionOrderInPostgreSQL.
        /// </value>
        public static string CancellingProductionOrderInPostgreSQL => "{LogBase} - Cancelling Production Order In PostgreSQL";

        /// <summary>
        /// Gets SaveHistoryOrdersFabLogBase.
        /// </summary>
        /// <value>
        /// SaveHistoryOrdersFabLogBase.
        /// </value>
        public static string SaveHistoryOrdersFabLogBase => "Register Order History Service - ChildOrder: {0} - ParentOrder: {1} - Save History Orders Fab";

        /// <summary>
        /// Gets SaveHistoryOrdersFabLogBase.
        /// </summary>
        /// <value>
        /// SaveHistoryOrdersFabLogBase.
        /// </value>
        public static string SaveHistoryParentOrdersFabLogBase => "Register Order History Service - ParentOrder: {0} - Save History Orders Fab";

        /// <summary>
        /// Gets SaveHistoryOrdersFabStart.
        /// </summary>
        /// <value>
        /// SaveHistoryOrdersFabStart.
        /// </value>
        public static string SaveHistoryOrdersFabStart => "{LogBase} - Start Process - Pieces: {Pieces} - User: {User} - SapOrder: {SapOrder}";

        /// <summary>
        /// Gets SaveHistoryOrdersFabChildExists.
        /// </summary>
        /// <value>
        /// SaveHistoryOrdersFabChildExists.
        /// </value>
        public static string SaveHistoryOrdersFabChildExists => "{LogBase} - Child Order Already Exists - Skip Process";

        /// <summary>
        /// Gets SaveHistoryOrdersFabEndWithError.
        /// </summary>
        /// <value>
        /// SaveHistoryOrdersFabEndWithError.
        /// </value>
        public static string SaveHistoryOrdersFabEndWithError => "{0} - End Process With Error";
    }
}
