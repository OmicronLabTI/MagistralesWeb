// <summary>
// <copyright file="ICancelService.cs" company="Axity">
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

    /// <summary>
    /// class for cancelling.
    /// </summary>
    public class CancelService : ICancelService
    {
        private readonly Company company;
        private readonly ILoggerProxy _loggerProxy;

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelService"/> class.
        /// </summary>   
        public CancelService(ILoggerProxy loggerProxy)
        {
            this.company = Connection.Company;
            this._loggerProxy = loggerProxy;
        }

        /// <inheritdoc/>
        public async Task<ResultModel> CancelDelivery(string type, List<CancelDeliveryModel> deliveryIds)
        {
            _loggerProxy.Debug($"Deliveries to cancel: {JsonConvert.SerializeObject(deliveryIds)}.");
            var dictionaryResult = new Dictionary<string, string>();
            var delivery = (Documents)company.GetBusinessObject(BoObjectTypes.oDeliveryNotes);

            try
            {
                for (var i = 0; i < deliveryIds.Count; i++)
                {
                    if (!delivery.GetByKey(deliveryIds[i].Delivery))
                    {
                        dictionaryResult.Add($"{deliveryIds[i]}-Error", ServiceConstants.OrderNotFound);
                        return ServiceUtils.CreateResult(true, 200, null, dictionaryResult, null);
                    }

                    var cancelDoc = delivery.CreateCancellationDocument();
                    cancelDoc.DocDate = DateTime.Now;
                    var cancellation = cancelDoc.Add();
                    company.GetLastError(out int errCode, out string errMsg);
                    if (cancellation != 0)
                    {
                        _loggerProxy.Info($"The delivery {deliveryIds[i]} was triend to be CANCELLED. {errCode} - {errMsg}");
                        dictionaryResult.Add($"{deliveryIds[i]}-Error", $"Error- {errMsg}");
                        continue;
                    }
                    else
                    {
                        _loggerProxy.Info($"The saleORder {deliveryIds[i]} was Cancelled {errCode} - {errMsg}");
                        dictionaryResult.Add($"{deliveryIds[i]}-Ok", "Ok");
                    }

                    if (type == ServiceConstants.Total)
                    {
                        dictionaryResult = CreateTransfer(deliveryIds[i], dictionaryResult, delivery);
                    }
                    
                }
            }
            catch(Exception ex)
            {
                _loggerProxy.Info($"There was an error while cancelling {JsonConvert.SerializeObject(deliveryIds)}. {ex.Message}-{ex.StackTrace}");
                dictionaryResult.Add($"{deliveryIds.FirstOrDefault()}-Ok", "Ok");
            }
            
            return ServiceUtils.CreateResult(true, 200, null, dictionaryResult, null);
        }

        private Dictionary<string, string> CreateTransfer(CancelDeliveryModel deliveryIds, Dictionary<string, string> dictionaryResult, Documents delivery)
        {
            var deliveryType = delivery.UserFields.Fields.Item("U_TipoPedido").Value;
            var finalWhs = ServiceConstants.DictWhs.ContainsKey(deliveryType) ? ServiceConstants.DictWhs[deliveryType] : "MG";

            var transfer = (StockTransfer)company.GetBusinessObject(BoObjectTypes.oStockTransfer);
            transfer.DocDate = DateTime.Today;
            transfer.FromWarehouse = "PT";
            transfer.ToWarehouse = finalWhs;
            transfer.JournalMemo = $"Traspaso por Cancelación: {deliveryIds.SaleOrderId}";

            for (var i = 0; i < deliveryIds.MagistralProducts.Count; i++)
            {
                transfer.Lines.SetCurrentLine(i);
                transfer.Lines.ItemCode = deliveryIds.MagistralProducts[i].ItemCode;
                transfer.Lines.FromWarehouseCode = "PT";
                transfer.Lines.WarehouseCode = finalWhs;
                transfer.Lines.Quantity = deliveryIds.MagistralProducts[i].Pieces;
                transfer.Lines.Add();
            }

            var transferResult = transfer.Add();
            company.GetLastError(out int errCode, out string errMsg);

            if (transferResult != 0)
            {
                _loggerProxy.Info($"The transfer for delivery: {deliveryIds.Delivery} was tried to be made. {errCode} - {errMsg}");
                dictionaryResult.Add($"{deliveryIds.Delivery}-Transfer-Error", $"Error- {errMsg}");
            }
            else
            {
                _loggerProxy.Info($"The transfer for delivery: {deliveryIds.Delivery} was done. {errCode} - {errMsg}");
                dictionaryResult.Add($"{deliveryIds.Delivery}-Transfer-Ok", "Ok");
            }

            return dictionaryResult;
        }
    }
}
