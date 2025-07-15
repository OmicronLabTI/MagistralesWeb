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
        public static string ProductionOrderIsAlreadyBeignProcessed => "{LogBase} - Production Order Is Already Beign Processed - {ProductionOrderId}";

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
        /// Gets PostSapFinalizeProductionOrderProcess.
        /// </summary>
        /// <value>
        /// PostSapFinalizeProductionOrderProcess.
        /// </value>
        public static string PostSapFinalizeProductionOrderProcess => "{LogBase} - Post Sap Finalize Production Order Process - {OrderToFinish}";

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
        public static string RetryFinalizingProductionOrderEndWithError => "{LogBase} - Retry Finalizing Production Order End Process With Error - Id: {Id} - Production Order: {ProductionOrderId} - Message {Message} - Inner Exception - {InnerException}";

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
    }
}
