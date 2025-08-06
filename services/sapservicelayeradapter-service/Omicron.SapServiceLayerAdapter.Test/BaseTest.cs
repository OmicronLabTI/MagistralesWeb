// <summary>
// <copyright file="BaseTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.SapServiceLayerAdapter.Test
{
    /// <summary>
    /// Class Base Test.
    /// </summary>
    public abstract class BaseTest
    {
        /// <summary>
        /// Returns the ressult model.
        /// </summary>
        /// <param name="code">Code.</param>
        /// <param name="success">Is success.</param>
        /// <param name="response">the object for response.</param>
        /// <param name="userError">User error.</param>
        /// <param name="exceptionMessage">Exception Message.</param>
        /// <param name="comments">the comments.</param>
        /// <returns>the data.</returns>
        public static ResultModel GetGenericResponseModel(
            int code,
            bool success,
            object response,
            string userError = null,
            string exceptionMessage = null,
            object comments = null)
            => new ResultModel
            {
                Code = code,
                Success = success,
                Response = JsonConvert.SerializeObject(response),
                UserError = userError,
                ExceptionMessage = exceptionMessage,
                Comments = comments,
            };

        /// <summary>
        /// Get OrderDto.
        /// </summary>
        /// <returns>The OrderDto.</returns>
        public static IEnumerable<OrderDto> GetOrdersDtol()
            => new List<OrderDto>()
            {
                new () { DocumentEntry = 1 },
            };

        /// <summary>
        /// FinalizeProductionOrderInSap.
        /// </summary>
        /// <param name="orderId">Order id.</param>
        /// <param name="batches">Batches.</param>
        /// <returns>CloseProductionOrderDto.</returns>
        public static CloseProductionOrderDto FinalizeProductionOrderInSap(int orderId, bool batches)
        {
            var batch = new BatchesConfigurationDto()
            {
                Quantity = "1.0",
                BatchCode = "Code",
                ManufacturingDate = DateTime.Now,
                ExpirationDate = DateTime.Now,
            };
            var item = new CloseProductionOrderDto()
            {
                ProductionOrderId = orderId,
                Batches = batches ? new List<BatchesConfigurationDto>() { batch } : null,
            };

            return item;
        }

        /// <summary>
        /// GetProductionOrder.
        /// </summary>
        /// <param name="status">status.</param>
        /// <param name="issuedQuantityB">issuedQuantityB.</param>
        /// <param name="issuedQuantityM">issuedQuantityM.</param>
        /// <returns>ProductionOrderDto.</returns>
        public static ProductionOrderDto GetProductionOrder(string status, double issuedQuantityB, double issuedQuantityM)
        {
            var firstProduct = new ProductionOrderLineDto()
            {
                ProductionOrderIssueType = "im_Backflush",
                ItemNo = "EM-123",
                Warehouse = "MN",
                PlannedQuantity = 1,
                IssuedQuantity = issuedQuantityB,
                BatchNumbers = new List<ProductionOrderItemBatchDto>(),
            };

            var batch = new ProductionOrderItemBatchDto()
            {
                BatchNumber = "X-111",
                Quantity = 1,
                ItemCode = "EN-123",
            };

            var secondProduct = new ProductionOrderLineDto()
            {
                ProductionOrderIssueType = "im_Manual",
                ItemNo = "EN-123",
                Warehouse = "MG",
                PlannedQuantity = 1,
                IssuedQuantity = issuedQuantityM,
                BatchNumbers = new List<ProductionOrderItemBatchDto>() { batch },
            };

            var productionOrder = new ProductionOrderDto()
            {
                ProductionOrderStatus = status,
                AbsoluteEntry = 1,
                DocumentNumber = 1,
                Series = 3,
                ProductionOrderLines = new List<ProductionOrderLineDto>() { firstProduct, secondProduct },
                PlannedQuantity = 1,
                Warehouse = "MN",
            };

            return productionOrder;
        }

        /// <summary>
        /// GetProduct.
        /// </summary>
        /// <param name="name">name.</param>
        /// <param name="manageBatchNumbers">manageBatchNumbers.</param>
        /// <param name="stock">stock.</param>
        /// <returns>ItemDto.</returns>
        public static ItemDto GetProduct(string name, string manageBatchNumbers, double stock)
        {
            var itemWareHouse = new ItemWarehouseInfoDto()
            {
                WarehouseCode = "MN",
                InStock = stock,
            };

            var firstItem = new ItemDto()
            {
                ManageBatchNumbers = manageBatchNumbers,
                ItemCode = name,
                ItemWarehouseInfoCollection = new List<ItemWarehouseInfoDto>() { itemWareHouse },
            };

            return firstItem;
        }

        /// <summary>
        /// GetBatchNumber.
        /// </summary>
        /// <param name="hasResults">hasResults.</param>
        /// <returns>BatchNumberResponseDto.</returns>
        public static BatchNumberResponseDto GetBatchNumber(bool hasResults)
        {
            return new BatchNumberResponseDto()
            {
                Results = hasResults ? new List<BatchNumberDetailDto>() { new BatchNumberDetailDto() } : new List<BatchNumberDetailDto>(),
            };
        }

        /// <summary>
        /// GetResult.
        /// </summary>
        /// <param name="success">success.</param>
        /// <param name="data">data.</param>
        /// <returns>ResultModel.</returns>
        public static ResultModel GetResult(bool success, object data)
        {
            return new ResultModel()
            {
                Code = success ? 200 : 400,
                Success = success,
                Response = JsonConvert.SerializeObject(data),
                UserError = success ? null : "Error",
            };
        }

        /// <summary>
        /// GetCloseProductionOrderDto.
        /// </summary>
        /// <param name="orderId">Order Id.</param>
        /// <param name="batches">Batches.</param>
        /// <returns>CloseProductionOrderDto.</returns>
        public static CloseProductionOrderDto GetCloseProductionOrderDto(int orderId, bool batches)
        {
            var batch = new BatchesConfigurationDto()
            {
                Quantity = "1.0",
                BatchCode = "Code",
                ManufacturingDate = DateTime.Now,
                ExpirationDate = DateTime.Now,
            };
            var item = new CloseProductionOrderDto()
            {
                ProductionOrderId = orderId,
                Batches = batches ? new List<BatchesConfigurationDto>() { batch } : null,
            };

            return item;
        }
    }
}
