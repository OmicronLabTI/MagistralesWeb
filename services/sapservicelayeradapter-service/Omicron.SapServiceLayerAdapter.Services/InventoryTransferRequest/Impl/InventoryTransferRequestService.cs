// <summary>
// <copyright file="InventoryTransferRequestService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Services.InventoryTransferRequest.Impl
{
    /// <summary>
    /// Class for inventory transfer request service.
    /// </summary>
    public class InventoryTransferRequestService : IInventoryTransferRequestService
    {
        private readonly IServiceLayerClient serviceLayerClient;

        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryTransferRequestService"/> class.
        /// </summary>
        /// <param name="serviceLayerClient">Service Layer Client.</param>
        /// <param name="logger">Logger.</param>
        public InventoryTransferRequestService(IServiceLayerClient serviceLayerClient, ILogger logger)
        {
            this.serviceLayerClient = serviceLayerClient.ThrowIfNull(nameof(serviceLayerClient));
            this.logger = logger.ThrowIfNull(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<ResultModel> CreateTransferRequest(List<TransferRequestHeaderDto> transferRequestHeader)
        {
            this.logger.Information($"The next transfer requests will be created: {JsonConvert.SerializeObject(transferRequestHeader)}");
            var processResult = new List<InventoryTransferRequestResult>();
            var inventoryTransferRequests = new InventoryTransferRequestsDto();
            foreach (var transferRequest in transferRequestHeader)
            {
                processResult.Add(await this.ProcessToCreateTransferRequest(transferRequest, inventoryTransferRequests));
            }

            return ServiceUtils.CreateResult(true, 200, null, processResult, null);
        }

        private async Task<InventoryTransferRequestResult> ProcessToCreateTransferRequest(TransferRequestHeaderDto transferRequest, InventoryTransferRequestsDto inventoryTransferRequests)
        {
            try
            {
                inventoryTransferRequests = new InventoryTransferRequestsDto
                {
                    JournalMemo = transferRequest.UserInfo,
                    DocumentDate = DateTime.Now,
                    RequestedUserId = transferRequest.UserId,
                    StockTransferLines = transferRequest.TransferRequestDetail.Select(tr =>
                        new StockTransferLinesDto
                        {
                            ItemCode = tr.ItemCode,
                            Quantity = tr.Quantity,
                            FromWarehouseCode = tr.SourceWarehosue,
                            WarehouseCode = tr.TargetWarehosue,
                            ItemDescription = tr.ItemDescription,
                        }).ToList(),
                };

                var resultInventoryTransferRequest = await this.serviceLayerClient.PostAsync(
                    ServiceQuerysConstants.QryPostInventoryTransferRequests, JsonConvert.SerializeObject(inventoryTransferRequests));

                if (!resultInventoryTransferRequest.Success)
                {
                    this.logger.Error($"Sap Service Layer Adapter - InventoryTransferRequestService - The transer request was tried to be created: " +
                    $"{resultInventoryTransferRequest.UserError} - {resultInventoryTransferRequest.ExceptionMessage} - {JsonConvert.SerializeObject(transferRequest)}");
                    return new InventoryTransferRequestResult
                    {
                        UserInfo = transferRequest.UserInfo,
                        Error = string.Format("{0}-{1}-{2}", ServiceConstants.ErrorTransferRequest, resultInventoryTransferRequest.UserError, resultInventoryTransferRequest.ExceptionMessage),
                        IsLabel = transferRequest.IsLabel,
                    };
                }

                var responseInventoryTransferRequest = JsonConvert.DeserializeObject<InventoryTransferRequestsResponseDto>(resultInventoryTransferRequest.Response.ToString());
                this.logger.Information($"The transfer request {responseInventoryTransferRequest.DocEntry} was created {JsonConvert.SerializeObject(transferRequest)}");
                return new InventoryTransferRequestResult
                {
                    UserInfo = transferRequest.UserInfo,
                    TransferRequestId = responseInventoryTransferRequest.DocEntry,
                    IsLabel = transferRequest.IsLabel,
                };
            }
            catch (Exception ex)
            {
                this.logger.Error($"Sap Service Layer Adapter - InventoryTransferRequestService - There was an error while creating the transfer request " +
                    $"{ex.Message} - {ex.StackTrace} - {JsonConvert.SerializeObject(transferRequest)}");
                return new InventoryTransferRequestResult
                {
                    UserInfo = transferRequest.UserInfo,
                    Error = string.Format("{0}-{1}-{2}", ServiceConstants.ErrorTransferRequest, ex.Message, ex.StackTrace),
                    IsLabel = transferRequest.IsLabel,
                };
            }
        }
    }
}
