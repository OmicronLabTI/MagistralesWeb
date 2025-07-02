// <summary>
// <copyright file="KafkaConstants.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Api.Consumers
{
    /// <summary>
    /// the class for kafka constatns.
    /// </summary>
    public static class KafkaConstants
    {
        /// <summary>
        /// Gets KafkaTopicNameFinalizeProductionOrderSap.
        /// </summary>
        /// <value>KafkaTopicNameFinalizeProductionOrderSap.</value>
        public static string KafkaTopicNameFinalizeProductionOrderSap => "finalizeproductionordersap";

        /// <summary>
        /// Gets LogBaseKafkaConsumerFinalizeProductionOrderSap.
        /// </summary>
        /// <value>LogBaseKafkaConsumerFinalizeProductionOrderSap.</value>
        public static string LogBaseKafkaConsumerFinalizeProductionOrderSap => "Pedidos - KafkaConsumerFinalizeProductionOrderSap";

        /// <summary>
        /// Gets KafkaTopicNameFinalizeProductionOrderPostgresql.
        /// </summary>
        /// <value>KafkaTopicNameFinalizeProductionOrderPostgresql.</value>
        public static string KafkaTopicNameFinalizeProductionOrderPostgresql => "finalizeproductionorderpostgresql";

        /// <summary>
        /// Gets LogBaseKafkaConsumerFinalizeProductionOrderPostgresql.
        /// </summary>
        /// <value>LogBaseKafkaConsumerFinalizeProductionOrderPostgresql.</value>
        public static string LogBaseKafkaConsumerFinalizeProductionOrderPostgresql => "Pedidos - KafkaConsumerFinalizeProductionOrderPostgresql";

        /// <summary>
        /// Gets KafkaTopicNameProductionOrderPdfGeneration.
        /// </summary>
        /// <value>KafkaTopicNameProductionOrderPdfGeneration.</value>
        public static string KafkaTopicNameProductionOrderPdfGeneration => "productionorderpdfgeneration";

        /// <summary>
        /// Gets LogBaseKafkaConsumerProductionOrderPdfGeneration.
        /// </summary>
        /// <value>LogBaseKafkaConsumerProductionOrderPdfGeneration.</value>
        public static string LogBaseKafkaConsumerProductionOrderPdfGeneration => "Pedidos - KafkaConsumerProductionOrderPdfGeneration";

        /// <summary>
        /// Gets BackGroundStarting.
        /// </summary>
        /// <value>BackGroundStarting.</value>
        public static string BackGroundStarting => "Background - {LogBase} is starting.";

        /// <summary>
        /// Gets BackGroundStopping.
        /// </summary>
        /// <value>BackGroundStopping.</value>
        public static string BackGroundStopping => "Background - {LogBase} is stopping.";

        /// <summary>
        /// Gets KafkaEhName.
        /// </summary>
        /// <value>KafkaEhName.</value>
        public static string KafkaEhName => "kafka:{0}:EH_NAME";

        /// <summary>
        /// Gets MessageReceived.
        /// </summary>
        /// <value>MessageReceived.</value>
        public static string MessageReceived => "Id: {Id} - Kafka Id: {KafkaProcessId} - {LogBase} - Message Received - Message: {Message}";

        /// <summary>
        /// Gets CreateScope.
        /// </summary>
        /// <value>CreateScope.</value>
        public static string CreateScope => "Id: {Id} - Kafka Id: {KafkaProcessId} - {LogBase} - Create Scope";

        /// <summary>
        /// Gets CommitResult.
        /// </summary>
        /// <value>CommitResult.</value>
        public static string CommitResult => "Id: {Id} - Kafka Id: {KafkaProcessId} - {LogBase} - Commit Result";

        /// <summary>
        /// Gets CallFinalizeProductionOrdersOnSap.
        /// </summary>
        /// <value>CallFinalizeProductionOrdersOnSap.</value>
        public static string CallFinalizeProductionOrdersOnSap => "Id: {Id} - Kafka Id: {KafkaProcessId} - {LogBase} - Call Finalize Production Orders On Sap Async";

        /// <summary>
        /// Gets ProcessCompletedSuccessfully.
        /// </summary>
        /// <value>ProcessCompletedSuccessfully.</value>
        public static string ProcessCompletedSuccessfully => "Id: {Id} - Kafka Id: {KafkaProcessId} - {LogBase} - Process Completed Successfully";

        /// <summary>
        /// Gets ProcessTerminatedWithError.
        /// </summary>
        /// <value>ProcessTerminatedWithError.</value>
        public static string ProcessTerminatedWithError => "Kafka Id: {0} - {1} - Process Terminated With Error - Message: {2} -- StackTrace: {3}";

        /// <summary>
        /// Gets FinallyProcessCompleted.
        /// </summary>
        /// <value>FinallyProcessCompleted.</value>
        public static string FinallyProcessCompleted => "Kafka Id: {KafkaProcessId} - {LogBase} - Process Completed Successfully - End of process -  Elapsed total milliseconds: {TotalMilliseconds}";
    }
}
