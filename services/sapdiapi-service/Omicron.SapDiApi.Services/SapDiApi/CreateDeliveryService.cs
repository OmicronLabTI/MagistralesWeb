// <summary>
// <copyright file="CreateDeliveryService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapDiApi.Services.SapDiApi
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Linq;
    using Newtonsoft.Json;
    using Omicron.SapDiApi.Entities.Context;
    using Omicron.SapDiApi.Entities.Models;
    using Omicron.SapDiApi.Services.Constants;
    using Omicron.SapDiApi.Services.Utils;
    using SAPbobsCOM;
    using Omicron.SapDiApi.Log;
    using Omicron.LeadToCash.Resources.Exceptions;

    public class CreateDeliveryService : ICreateDeliveryService
    {
        private readonly Company company;
        private readonly ILoggerProxy _loggerProxy;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateDeliveryService"/> class.
        /// </summary>   
        public CreateDeliveryService(ILoggerProxy loggerProxy)
        {
            this.company = Connection.Company;
            this._loggerProxy = loggerProxy;
        }

        /// <inheritdoc/>
        public async Task<ResultModel> CreateDelivery(List<CreateDeliveryModel> createDelivery)
        {
            var dictionaryResult = new Dictionary<string, string>();
            var saleOrderId = createDelivery.First().SaleOrderId;
            try
            {
                var saleOrder = (Documents)company.GetBusinessObject(BoObjectTypes.oOrders);
                var saleOrderFound = saleOrder.GetByKey(saleOrderId);

                if (!saleOrderFound)
                {
                    dictionaryResult.Add($"{saleOrderId}-Error", ServiceConstants.OrderNotFound);
                    return ServiceUtils.CreateResult(true, 200, null, dictionaryResult, null);
                }

                var deliveryNote = (Documents)company.GetBusinessObject(BoObjectTypes.oDeliveryNotes);
                deliveryNote.CardCode = saleOrder.CardCode;
                deliveryNote.SalesPersonCode = saleOrder.SalesPersonCode;
                deliveryNote.DocType = BoDocumentTypes.dDocument_Items;
                deliveryNote.DocumentSubType = BoDocumentSubType.bod_None;
                deliveryNote.Address = saleOrder.Address;
                deliveryNote.Address2 = saleOrder.Address2;
                deliveryNote.ShipToCode = saleOrder.ShipToCode;
                deliveryNote.JournalMemo = $"Delivery {saleOrder.CardCode}";
                deliveryNote.Comments = $"Basado en pedido: {saleOrderId}";

                for (var i = 0; i < saleOrder.Lines.Count; i++)
                {
                    saleOrder.Lines.SetCurrentLine(i);
                    var itemCode = saleOrder.Lines.ItemCode;

                    deliveryNote = this.UpdateDelivery(deliveryNote, saleOrder, saleOrderId, i, createDelivery, itemCode);
                    deliveryNote.Lines.Add();
                }

                var update = deliveryNote.Add();
                company.GetLastError(out int errCode, out string errMsg);

                if (update != 0)
                {
                    _loggerProxy.Info($"The saleORder {saleOrderId} was tried to be delivered {errCode} - {errMsg} - {JsonConvert.SerializeObject(createDelivery)}");
                    dictionaryResult.Add($"{saleOrderId}-Error", $"Error- {errMsg}");
                }
                else
                {
                    _loggerProxy.Info($"The saleORder {saleOrderId} was delivered {errCode} - {errMsg}");
                    dictionaryResult.Add($"{saleOrderId}-Ok", "Ok");
                }
            }
            catch(Exception ex)
            {
                _loggerProxy.Info($"Error while Delivery {saleOrderId} {JsonConvert.SerializeObject(createDelivery)} - ex: {ex.Message} - stackTrace: {ex.StackTrace}");
                dictionaryResult.Add($"{saleOrderId}-ErrorHandled", "Error mientras se crea remisión");
            }
            
            return ServiceUtils.CreateResult(true, 200, null, dictionaryResult, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> CreateDeliveryPartial(List<CreateDeliveryModel> createDelivery)
        {
            var dictionaryResult = new Dictionary<string, string>();
            var saleOrderId = createDelivery.First().SaleOrderId;
            var productsIds = createDelivery.Select(x => x.ItemCode).ToList();
            try
            {
                var saleOrder = (Documents)company.GetBusinessObject(BoObjectTypes.oOrders);
                var saleOrderFound = saleOrder.GetByKey(saleOrderId);

                if (!saleOrderFound)
                {
                    dictionaryResult.Add($"{saleOrderId}-Error", ServiceConstants.OrderNotFound);
                    return ServiceUtils.CreateResult(true, 200, null, dictionaryResult, null);
                }

                var deliveryNote = (Documents)company.GetBusinessObject(BoObjectTypes.oDeliveryNotes);
                deliveryNote.CardCode = saleOrder.CardCode;
                deliveryNote.SalesPersonCode = saleOrder.SalesPersonCode;
                deliveryNote.DocType = BoDocumentTypes.dDocument_Items;
                deliveryNote.DocumentSubType = BoDocumentSubType.bod_None;
                deliveryNote.Address = saleOrder.Address;
                deliveryNote.Address2 = saleOrder.Address2;
                deliveryNote.ShipToCode = saleOrder.ShipToCode;
                deliveryNote.JournalMemo = $"Delivery {saleOrder.CardCode}";
                deliveryNote.Comments = $"Basado en pedido: {saleOrderId}";

                for (var i = 0; i < saleOrder.Lines.Count; i++)
                {
                    saleOrder.Lines.SetCurrentLine(i);
                    var itemCode = saleOrder.Lines.ItemCode;

                    if (!productsIds.Contains(itemCode))
                    {
                        continue;
                    }

                    deliveryNote = this.UpdateDelivery(deliveryNote, saleOrder, saleOrderId, i, createDelivery, itemCode);
                    deliveryNote.Lines.Add();
                }

                var update = deliveryNote.Add();
                company.GetLastError(out int errCode, out string errMsg);

                if (update != 0)
                {
                    _loggerProxy.Info($"The saleORder {saleOrderId} was tried to be delivered {errCode} - {errMsg} - {JsonConvert.SerializeObject(createDelivery)}");
                    dictionaryResult.Add($"{saleOrderId}-Error", $"Error- {errMsg}");
                }
                else
                {
                    _loggerProxy.Info($"The saleORder {saleOrderId} was delivered {errCode} - {errMsg}");
                    dictionaryResult.Add($"{saleOrderId}-Ok", "Ok");
                }
            }
            catch (Exception ex)
            {
                _loggerProxy.Info($"Error while Delivery {saleOrderId} {JsonConvert.SerializeObject(createDelivery)} - ex: {ex.Message} - stackTrace: {ex.StackTrace}");
                dictionaryResult.Add($"{saleOrderId}-ErrorHandled", "Error mientras se crea remisión");
            }

            return ServiceUtils.CreateResult(true, 200, null, dictionaryResult, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> CreateDeliveryBatch(List<CreateDeliveryModel> createDelivery)
        {
            var dictionaryResult = new Dictionary<string, string>();
            var saleOrderId = createDelivery.First().SaleOrderId;

            try
            {
                var saleOrder = (Documents)company.GetBusinessObject(BoObjectTypes.oOrders);
                var saleOrderFound = saleOrder.GetByKey(saleOrderId);

                if (!saleOrderFound)
                {
                    dictionaryResult.Add($"{saleOrderId}-Error", ServiceConstants.OrderNotFound);
                    return ServiceUtils.CreateResult(true, 200, null, dictionaryResult, null);
                }

                var ids = JsonConvert.SerializeObject(createDelivery.Select(x => x.SaleOrderId).ToList()).Replace("[", string.Empty).Replace("]", string.Empty);

                var deliveryNote = (Documents)company.GetBusinessObject(BoObjectTypes.oDeliveryNotes);
                deliveryNote.CardCode = saleOrder.CardCode;
                deliveryNote.SalesPersonCode = saleOrder.SalesPersonCode;
                deliveryNote.DocType = BoDocumentTypes.dDocument_Items;
                deliveryNote.DocumentSubType = BoDocumentSubType.bod_None;
                deliveryNote.Address = saleOrder.Address;
                deliveryNote.Address2 = saleOrder.Address2;
                deliveryNote.ShipToCode = saleOrder.ShipToCode;
                deliveryNote.JournalMemo = $"Delivery {saleOrder.CardCode}";
                deliveryNote.Comments = $"Basado en pedido: {ids}"; 

                foreach (var sale in createDelivery.GroupBy(p => p.SaleOrderId).ToList())
                {
                    var saleOrderFoundLocal = saleOrder.GetByKey(sale.FirstOrDefault().SaleOrderId);

                    if (!saleOrderFoundLocal)
                    {
                        _loggerProxy.Info($"The sale Order {sale.FirstOrDefault().SaleOrderId} was not found for creating the delivery");
                        continue;
                    }

                    for (var i = 0; i < saleOrder.Lines.Count; i++)
                    {
                        saleOrder.Lines.SetCurrentLine(i);
                        var itemCode = saleOrder.Lines.ItemCode;

                        deliveryNote = this.UpdateDelivery(deliveryNote, saleOrder, sale.FirstOrDefault().SaleOrderId, i, createDelivery, itemCode);
                        deliveryNote.Lines.Add();
                    }
                }

                var update = deliveryNote.Add();
                company.GetLastError(out int errCode, out string errMsg);

                if (update != 0)
                {
                    _loggerProxy.Info($"The saleORder {saleOrderId} was tried to be delivered {errCode} - {errMsg} - {JsonConvert.SerializeObject(createDelivery)}");
                    dictionaryResult.Add($"{saleOrderId}-Error", $"Error- {errMsg}");
                }
                else
                {
                    _loggerProxy.Info($"The saleORder {saleOrderId} was delivered {errCode} - {errMsg}");
                    dictionaryResult.Add($"{saleOrderId}-Ok", "Ok");
                }
            }
            catch (Exception ex)
            {
                _loggerProxy.Info($"Error while creating the Delivery for multiple sales: {JsonConvert.SerializeObject(createDelivery)} - ex: {ex.Message} - stackTrace: {ex.StackTrace}");
                dictionaryResult.Add($"{saleOrderId}-ErrorHandled", "Error mientras se crea remisión");
            }

            return ServiceUtils.CreateResult(true, 200, null, dictionaryResult, null);
        }

        private Documents UpdateDelivery(Documents deliveryNote, Documents saleOrder, int saleOrderId, int i, List<CreateDeliveryModel> createDelivery, string itemCode)
        {
            deliveryNote.Lines.ItemCode = saleOrder.Lines.ItemCode;
            deliveryNote.Lines.Quantity = saleOrder.Lines.Quantity;
            deliveryNote.Lines.DiscountPercent = saleOrder.Lines.DiscountPercent;
            deliveryNote.Lines.TaxCode = saleOrder.Lines.TaxCode;

            deliveryNote.Lines.LineTotal = saleOrder.Lines.LineTotal;
            deliveryNote.Lines.BaseType = 17;
            deliveryNote.Lines.WarehouseCode = saleOrder.Lines.WarehouseCode;

            deliveryNote.Lines.UserFields.Fields.Item("U_ENVASE").Value = saleOrder.Lines.UserFields.Fields.Item("U_ENVASE").Value;

            deliveryNote.Lines.BaseEntry = saleOrderId;
            deliveryNote.Lines.BaseLine = i;

            var product = createDelivery.FirstOrDefault(x => x.ItemCode.Equals(itemCode) && x.SaleOrderId == saleOrderId);
            product = product ?? new CreateDeliveryModel { OrderType = ServiceConstants.Magistral };

            if (product.OrderType != ServiceConstants.Magistral)
            {
                foreach (var b in product.Batches)
                {
                    double.TryParse(b.BatchQty.ToString(), out var doubleQuantity);
                    deliveryNote.Lines.BatchNumbers.Add();
                    deliveryNote.Lines.BatchNumbers.Quantity = doubleQuantity;
                    deliveryNote.Lines.BatchNumbers.BatchNumber = b.BatchNumber;
                }
            }

            return deliveryNote;
        }
    }
}
