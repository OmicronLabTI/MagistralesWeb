// <summary>
// <copyright file="ServiceConstants.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapDiApi.Services.Constants
{
    /// <summary>
    /// the service constants.
    /// </summary>
    public static class ServiceConstants
    {
        /// <summary>
        /// Status Liberado.
        /// </summary>
        public const string StatusLiberado = "R";

        /// <summary>
        /// error while inserting.
        /// </summary>
        public const string ErrorCreateFabOrd = "ErrorCreateFabOrd";

        /// <summary>
        /// error while updating.
        /// </summary>
        public const string ErrorUpdateFabOrd = "ErrorUpdateFabOrd";

        /// <summary>
        /// error while cancel sales order.
        /// </summary>
        public const string ErrorCancelSalesOrder = "ErrorCancelOrd";

        /// <summary>
        /// error the sales order is closed.
        /// </summary>
        public const string ErrorProductionOrderCancelled = "ErrorProductionOrderCancelled";

        /// <summary>
        /// the order was not found.
        /// </summary>
        public const string OrderNotFound = "OrderNotFound";

        /// <summary>
        /// the resource was not found.
        /// </summary>
        public const string NotFound = "NotFound";

        /// <summary>
        /// Unexpected error.
        /// </summary>
        public const string UnexpectedError = "UnexpectedError";

        /// <summary>
        /// Ok.
        /// </summary>
        public const string Ok = "Ok";

        /// <summary>
        /// selects data from WOR1 by DocEntry.
        /// </summary>
        public const string FindWor1ByDocEntry = "SELECT DocEntry, LineNum, BaseQty, IssuedQty, wareHouse, ItemCode, VisOrder FROM WOR1 WHERE DocEntry = {0}";

        /// <summary>
        /// the value to delete the conmponent.
        /// </summary>
        public const string DeleteComponent = "delete";

        /// <summary>
        /// the update value.
        /// </summary>
        public const string UpdateBatch = "update";

        /// <summary>
        /// Insert batch.
        /// </summary>
        public const string InsertBatch = "insert";

        /// <summary>
        /// deletes the batch.
        /// </summary>
        public const string DeleteBatch = "delete";
    }
}
