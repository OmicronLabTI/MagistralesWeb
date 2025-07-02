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
    }
}
