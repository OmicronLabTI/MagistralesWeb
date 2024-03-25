// <summary>
// <copyright file="DeliveryNoteService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Services.DeliveryNotes
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Net;
    using System.Security.AccessControl;
    using System.Threading.Tasks;
    using Azure;
    using Newtonsoft.Json;
    using Omicron.SapServiceLayerAdapter.Common.DTOs.DeliveryNotes;
    using Omicron.SapServiceLayerAdapter.Services.Constants;
    using Serilog;
    using static System.Runtime.CompilerServices.RuntimeHelpers;
    using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

    /// <summary>
    /// Class representing a generic service of create delivery.
    /// </summary>
    public class DeliveryNoteService : IDeliveryNoteService
    {
        private readonly IServiceLayerClient serviceLayerClient;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeliveryNoteService"/> class.
        /// </summary>
        /// <param name="serviceLayerClient">The Service layer client instance to use for sending requests.</param>
        /// <param name="logger">The logger.</param>
        public DeliveryNoteService(IServiceLayerClient serviceLayerClient, ILogger logger)
        {
            this.serviceLayerClient = serviceLayerClient.ThrowIfNull(nameof(serviceLayerClient));
            this.logger = logger.ThrowIfNull(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<ResultModel> CreateDelivery(List<CreateDeliveryNoteDto> createDelivery)
        {
            this.logger.Information($"order to be delivered {JsonConvert.SerializeObject(createDelivery)}");
            var dictionaryResult = new Dictionary<string, string>();
            var createDeliveryFirst = createDelivery.First();
            var saleOrderId = createDeliveryFirst.SaleOrderId;
            try
            {
                var response = await this.serviceLayerClient.GetAsync($"Orders({saleOrderId})");
                if (!response.Success)
                {
                    this.logger.Information($"Error to get the order {saleOrderId}, {response.UserError}");
                    dictionaryResult.Add($"{saleOrderId}-Error", response.UserError);
                    return ServiceUtils.CreateResult(true, 200, null, dictionaryResult, null);
                }

                var saleOrder = JsonConvert.DeserializeObject<OrderDto>(response.Response.ToString());
                var isOmigenomics = saleOrder.IsOmigenomics != null && saleOrder.IsOmigenomics == "Y";

                var deliveryNote = new DeliveryNoteDto();
                deliveryNote.CustomerCode = saleOrder.CardCode;
                deliveryNote.SalesPersonCode = saleOrder.SalesPersonCode;
                deliveryNote.DocumentsOwner = saleOrder.DocumentsOwner;
                deliveryNote.DocumentType = "dDocument_Items";
                deliveryNote.DocumentSubType = "bod_None";
                deliveryNote.BillingAddress = saleOrder.BillingAddress;
                deliveryNote.ShippingAddress = saleOrder.ShippingAddress;
                deliveryNote.ShippingCode = saleOrder.ShippingCode;
                deliveryNote.JournalMemo = $"Delivery {saleOrder.CardCode}";
                deliveryNote.Comments = saleOrder.Comments;
                deliveryNote.RemissionComment = $"Basado en pedido: {saleOrderId}";
                deliveryNote.OrderPackage = createDelivery.Any(x => x.IsPackage == ServiceConstants.IsPackage) ? ServiceConstants.IsPackage : ServiceConstants.IsNotPackage;
                deliveryNote.IsOmigenomics = saleOrder.IsOmigenomics;
                deliveryNote.DeliveryNoteLines = new List<DeliveryNoteLineDto>();

                for (var i = 0; i < saleOrder.OrderLines.Count; i++)
                {
                    var itemCode = saleOrder.OrderLines[i].ItemCode;
                    deliveryNote = this.UpdateDelivery(deliveryNote, saleOrder, saleOrderId, i, createDelivery, itemCode);
                }

                if (createDelivery.Any(x => x.ItemCode == ServiceConstants.ShippingCostItemCode))
                {
                    var shippingCost = createDelivery.FirstOrDefault(x => x.ItemCode == ServiceConstants.ShippingCostItemCode);
                    var correctBaseLineId = await this.GetShippingCostBaseLine(shippingCost.ShippingCostOrderId);

                    double.TryParse(shippingCost.OrderType, out var price);

                    var newDeliveryNote = new DeliveryNoteLineDto()
                    {
                        ItemCode = shippingCost.ItemCode,
                        Quantity = 1,
                        BaseType = 17,
                        BaseEntry = shippingCost.ShippingCostOrderId,
                        UnitPrice = price,
                        BaseLine = correctBaseLineId,
                        SalesPersonCode = saleOrder.SalesPersonCode,
                        Price = price,
                    };
                    deliveryNote.DeliveryNoteLines.Add(newDeliveryNote);
                    await this.UpdateShippingCostBaseLine(shippingCost.ShippingCostOrderId, saleOrder.IsOmigenomics, saleOrder.SalesPersonCode, saleOrder.DocumentsOwner);
                }

                var deliveryNotesStg = JsonConvert.SerializeObject(deliveryNote);
                var result = await this.serviceLayerClient.PostAsync("DeliveryNotes", deliveryNotesStg);

                if (!result.Success)
                {
                    this.logger.Error($"The saleORder {saleOrderId} was tried to be delivered {result.Code} - {result.UserError} - {JsonConvert.SerializeObject(createDelivery)}");
                    dictionaryResult.Add($"{saleOrderId}-Error", $"Error- {result.UserError}");
                }
                else
                {
                    this.logger.Information($"The saleORder {saleOrderId} was delivered - {result.Code}");
                    dictionaryResult.Add($"{saleOrderId}-Ok", "Ok");
                }
            }
            catch (Exception ex)
            {
                this.logger.Error($"Error while Delivery {saleOrderId} {JsonConvert.SerializeObject(createDelivery)} - ex: {ex.Message} - stackTrace: {ex.StackTrace}");
                dictionaryResult.Add($"{saleOrderId}-ErrorHandled", "Error mientras se crea remisión");
            }

            return ServiceUtils.CreateResult(true, 200, null, JsonConvert.SerializeObject(dictionaryResult), null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> CreateDeliveryPartial(List<CreateDeliveryNoteDto> createDelivery)
        {
            this.logger.Information($"order to be delivered partial {JsonConvert.SerializeObject(createDelivery)}");
            var dictionaryResult = new Dictionary<string, string>();
            var createDeliveryFirst = createDelivery.First();
            var saleOrderId = createDeliveryFirst.SaleOrderId;
            var productsIds = createDelivery.Where(x => x.ItemCode != ServiceConstants.ShippingCostItemCode).Select(x => x.ItemCode).ToList(); // ["REVE 14"]

            try
            {
                var response = await this.serviceLayerClient.GetAsync($"Orders({saleOrderId})");
                if (!response.Success)
                {
                    this.logger.Information($"Error to get the order {saleOrderId}, {response.UserError}");
                    dictionaryResult.Add($"{saleOrderId}-Error", response.UserError);
                    return ServiceUtils.CreateResult(true, 200, null, dictionaryResult, null);
                }

                var saleOrder = JsonConvert.DeserializeObject<OrderDto>(response.Response.ToString());
                var deliveryNote = new DeliveryNoteDto();
                deliveryNote.DocumentsOwner = saleOrder.DocumentsOwner;
                deliveryNote.CustomerCode = saleOrder.CardCode;
                deliveryNote.SalesPersonCode = saleOrder.SalesPersonCode;
                deliveryNote.DocumentType = "dDocument_Items";
                deliveryNote.DocumentSubType = "bod_None";
                deliveryNote.BillingAddress = saleOrder.BillingAddress;
                deliveryNote.ShippingAddress = saleOrder.ShippingAddress;
                deliveryNote.ShippingCode = saleOrder.ShippingCode;
                deliveryNote.JournalMemo = $"Delivery {saleOrder.CardCode}";
                deliveryNote.Comments = saleOrder.Comments;
                deliveryNote.RemissionComment = $"Basado en pedido: {saleOrderId}";
                deliveryNote.IsOmigenomics = saleOrder.IsOmigenomics;
                deliveryNote.DeliveryNoteLines = new List<DeliveryNoteLineDto>();

                for (var i = 0; i < saleOrder.OrderLines.Count; i++)
                {
                    var itemCode = saleOrder.OrderLines[i].ItemCode;

                    if (!productsIds.Contains(itemCode))
                    {
                        continue;
                    }

                    deliveryNote = this.UpdateDelivery(deliveryNote, saleOrder, saleOrderId, i, createDelivery, itemCode);
                }

                if (createDelivery.Any(x => x.ItemCode == ServiceConstants.ShippingCostItemCode))
                {
                    var shippingCost = createDelivery.FirstOrDefault(x => x.ItemCode == ServiceConstants.ShippingCostItemCode);
                    var shippingOrder = saleOrder.OrderLines.FirstOrDefault(x => x.ItemCode == ServiceConstants.ShippingCostItemCode);

                    double.TryParse(shippingCost.OrderType, out var price);

                    var newDeliveryNote = new DeliveryNoteLineDto()
                    {
                        ItemCode = shippingCost.ItemCode,
                        Quantity = 1,
                        DiscountPercent = shippingOrder.DiscountPercent,
                        TaxCode = shippingOrder.TaxCode,
                        BaseType = -1,
                        WarehouseCode = shippingOrder.WarehouseCode,
                        Price = price,
                        Currency = shippingOrder.Currency,
                        UnitPrice = price,
                        LineTotal = shippingOrder.LineTotal,
                        Container = shippingOrder.Container,
                        Label = shippingOrder.Label,
                        SalesPersonCode = shippingOrder.SalesPersonCode,
                        BaseEntry = null,
                    };

                    deliveryNote.DeliveryNoteLines.Add(newDeliveryNote);
                }

                var deliveryNotesStg = JsonConvert.SerializeObject(deliveryNote);
                var result = await this.serviceLayerClient.PostAsync("DeliveryNotes", deliveryNotesStg);

                if (!result.Success)
                {
                    this.logger.Error($"The saleORder {saleOrderId} was tried to be delivered {result.Code} - {result.UserError} - {JsonConvert.SerializeObject(createDelivery)}");
                    dictionaryResult.Add($"{saleOrderId}-Error", $"Error- {result.UserError}");
                }
                else
                {
                    this.logger.Information($"The saleORder {saleOrderId} was delivered - {result.Code}");
                    dictionaryResult.Add($"{saleOrderId}-Ok", "Ok");
                }
            }
            catch (Exception ex)
            {
                this.logger.Error($"Error while Delivery {saleOrderId} {JsonConvert.SerializeObject(createDelivery)} - ex: {ex.Message} - stackTrace: {ex.StackTrace}");
                dictionaryResult.Add($"{saleOrderId}-ErrorHandled", "Error mientras se crea remisión");
            }

            return ServiceUtils.CreateResult(true, 200, null, JsonConvert.SerializeObject(dictionaryResult), null);
        }

        private async Task UpdateShippingCostBaseLine(int saleOrderId, string isOmigenomics, int salesPersonCode, int documentsOwner)
        {
            var response = await this.serviceLayerClient.GetAsync($"Orders({saleOrderId})");
            if (response.Code == (int)HttpStatusCode.NotFound)
            {
                throw new CustomServiceException($"Almacen - Create Delivery Service - SaleOrderId {saleOrderId} Not found", HttpStatusCode.NotFound);
            }

            var saleOrderShipping = JsonConvert.DeserializeObject<OrderDto>(response.Response.ToString());

            saleOrderShipping.DocumentsOwner = documentsOwner;
            saleOrderShipping.SalesPersonCode = salesPersonCode;
            saleOrderShipping.IsOmigenomics = isOmigenomics;

            for (var i = 0; i < saleOrderShipping.OrderLines.Count; i++)
            {
                var orderLine = saleOrderShipping.OrderLines[i];
                if (orderLine.ItemCode == ServiceConstants.ShippingCostItemCode)
                {
                    orderLine.OwnerCode = documentsOwner;
                    orderLine.SalesPersonCode = salesPersonCode;
                    break;
                }
            }

            await this.serviceLayerClient.PatchAsync("Orders", JsonConvert.SerializeObject(saleOrderShipping));
        }

        private DeliveryNoteDto UpdateDelivery(DeliveryNoteDto deliveryNote, OrderDto saleOrder, int saleOrderId, int i, List<CreateDeliveryNoteDto> createDelivery, string itemCode)
        {
            var orderLine = saleOrder.OrderLines[i];
            var newDeliveryNote = new DeliveryNoteLineDto()
            {
                ItemCode = orderLine.ItemCode,
                Quantity = orderLine.Quantity,
                DiscountPercent = orderLine.DiscountPercent,
                TaxCode = orderLine.TaxCode,
                LineTotal = orderLine.LineTotal,
                BaseType = 17,
                WarehouseCode = orderLine.WarehouseCode,
                Container = orderLine.Container,
                Label = orderLine.Label,
                BaseEntry = saleOrderId,
                BaseLine = orderLine.LineNum,
                BatchNumbers = new List<DeliveryNoteBatchNumbersDto>(),
                UnitPrice = orderLine.UnitPrice,
                SalesPersonCode = orderLine.SalesPersonCode,
            };

            var product = createDelivery.FirstOrDefault(x => x.ItemCode.Equals(itemCode) && x.SaleOrderId == saleOrderId);
            product = product ?? new CreateDeliveryNoteDto { OrderType = ServiceConstants.Magistral };

            if (product.OrderType != ServiceConstants.Magistral)
            {
                foreach (var b in product.Batches)
                {
                    double.TryParse(b.BatchQty.ToString(), out var doubleQuantity);
                    var batch = new DeliveryNoteBatchNumbersDto();
                    batch.Quantity = doubleQuantity;
                    batch.BatchNumber = b.BatchNumber;
                    newDeliveryNote.BatchNumbers.Add(batch);
                }
            }

            deliveryNote.DeliveryNoteLines.Add(newDeliveryNote);
            return deliveryNote;
        }

        private async Task<int> GetShippingCostBaseLine(int saleOrderId)
        {
            var saleOrderShipping = await this.serviceLayerClient.GetAsync($"Orders({saleOrderId})");
            if (saleOrderShipping.Code == (int)HttpStatusCode.NotFound)
            {
                throw new CustomServiceException($"Almacen - Create Delivery Service - SaleOrderId {saleOrderId} Not found", HttpStatusCode.NotFound);
            }

            var saleOrder = JsonConvert.DeserializeObject<OrderDto>(saleOrderShipping.Response.ToString());
            var correctBaseLineId = 0;
            for (var i = 0; i < saleOrder.OrderLines.Count; i++)
            {
                var saleOrderLine = saleOrder.OrderLines[i];
                var itemCode = saleOrderLine.ItemCode;
                if (itemCode == ServiceConstants.ShippingCostItemCode)
                {
                    correctBaseLineId = saleOrderLine.LineNum;
                    break;
                }
            }

            return correctBaseLineId;
        }
    }
}