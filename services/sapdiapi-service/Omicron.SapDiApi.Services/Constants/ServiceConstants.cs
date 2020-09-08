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
        /// Select existing batch code
        /// </summary>
        public const string FindBatchCodeForItem = "SELECT TOP 1 AbsEntry FROM OBTN WHERE DistNumber = '{0}' AND ItemCode = '{1}'";

        /// <summary>
        /// Select manual exit 
        /// </summary>
        public const string FindManualExit = "SELECT ItemCode, LineNum, PlannedQty, warehouse FROM wor1 where docentry={0} and issuetype='M'";

        /// <summary>
        /// Select batch code by item code, warehouse and quantity.
        /// </summary>
        public const string FindBatchCodeForIssueForProduction = "SELECT TOP 1 BatchNum FROM OIBT WHERE ItemCode = '{0}' AND ExpDate > getdate() AND Quantity > {1} AND WhsCode ='{2}' ORDER BY Quantity DESC";

        /// <summary>
        /// Select last invetory log entry for item code in a document.
        /// </summary>
        public const string FindLastInventoryLogEntry = "SELECT ISNULL(MAX(LogEntry), 0) LogEntry FROM OITL WHERE ItemCode = '{0}' AND DocNum = {1}";

        /// <summary>
        /// Select batch code by item code, warehouse code and sys number.
        /// </summary>
        public const string FindBatchCode = "SELECT B.DistNumber FROM OBTQ A INNER JOIN OBTN B ON A.ItemCode = B.ItemCode AND A.SysNumber = B.SysNumber WHERE A.ItemCode = '{0}' AND A.WhsCode = '{1}' AND A.Quantity > 0 AND A.SysNumber = {2}";

        /// <summary>
        /// Select assigned batches by log entry.
        /// </summary>
        public const string FindAssignedBatchesByLogEntry = "SELECT SysNumber, AllocQty FROM  ITL1 WHERE LogEntry = {0} ";

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

        /// <summary>
        /// Fail reason.
        /// </summary>
        public const string FailReasonNotExistsProductionOrder = "La orden de produción {0} no existe.";

        /// <summary>
        /// Fail reason.
        /// </summary>
        public const string FailReasonNotReleasedProductionOrder = "La orden de producción {0} no esta liberada.";

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
        public const string FailReasonNotGetExitCreated = "No se ha podido generar la entrega de componentes a producción para la orden de fabricación {0}.";

        /// <summary>
        /// Fail reason.
        /// </summary>
        public const string FailReasonProductCodeNotExists = "El producto con código {0} no existe.";

        /// <summary>
        /// Fail reason.
        /// </summary>
        public const string FailReasonUnexpectedErrorToCreateIsolatedProductionOrder = "Ocurrió un problema inesperado al crear la orden de fabricación para el producto {0}.";

        /// <summary>
        /// Fail reason.
        /// </summary>
        public const string FailReasonBatchAlreadyExists = "El lote {0} ya existe para el producto {1}.";

        /// <summary>
        /// Fail reason.
        /// </summary>
        public const string FailReasonUnexpectedError = "Ocurrió un error inesperado en SAP.";
    }
}
