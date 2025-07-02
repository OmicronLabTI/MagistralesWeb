// <summary>
// <copyright file="LogsConstants.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

using Pipelines.Sockets.Unofficial.Arenas;

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
    }
}
