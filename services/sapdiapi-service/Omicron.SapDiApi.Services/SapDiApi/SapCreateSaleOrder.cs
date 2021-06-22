// <summary>
// <copyright file="SapCreateSaleOrder.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapDiApi.Services.SapDiApi
{
    using Omicron.SapDiApi.Entities.Context;
    using Omicron.SapDiApi.Entities.Models;
    using Omicron.SapDiApi.Entities.Models.Experience;
    using Omicron.SapDiApi.Log;
    using Omicron.SapDiApi.Services.Utils;
    using SAPbobsCOM;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using System.Linq;
    using System.IO;

    /// <summary>
    /// class create order.
    /// </summary>
    public class SapCreateSaleOrder : ISapCreateSaleOrder
    {
        private readonly Company company;
        private readonly ILoggerProxy _loggerProxy;

        /// <summary>
        /// Initializes a new instance of the <see cref="SapCreateSaleOrder"/> class.
        /// </summary>   
        public SapCreateSaleOrder(ILoggerProxy loggerProxy)
        {
            this.company = Connection.Company;
            this._loggerProxy = loggerProxy;
        }

        /// <inheritdoc/>
        public async Task<ResultModel> CreateSaleOrder(CreateSaleOrderModel saleOrderModel)
        {            
            try
            {
                var prescription = await this.SavePresciptionToServer(saleOrderModel.PrescriptionUrl);
                var attachment = string.Empty;
                if (!string.IsNullOrEmpty(prescription))
                {
                    attachment = this.CreateAttachment(prescription);

                    if (string.IsNullOrEmpty(attachment))
                    {
                        return ServiceUtils.CreateResult(false, 400, null, "The attachment could not be created", null);
                    }
                }

                var order = (Documents)company.GetBusinessObject(BoObjectTypes.oOrders);

                order.CardCode = saleOrderModel.CardCode;
                order.DocDate = DateTime.Now;
                order.DocDueDate = DateTime.Now.AddDays(10);
                order.ShipToCode = saleOrderModel.ShippinAddress;
                order.PayToCode = saleOrderModel.BillingAddress;

                if (!string.IsNullOrEmpty(attachment))
                {
                    order.AttachmentEntry = int.Parse(attachment);
                }

                for(var i = 0; i < saleOrderModel.Items.Count; i++)
                {
                    order.Lines.SetCurrentLine(i);
                    order.Lines.ItemCode = saleOrderModel.Items[i].ItemCode;
                    order.Lines.Quantity = saleOrderModel.Items[i].Quantity;
                    order.Lines.UserFields.Fields.Item("U_ENVASE").Value = saleOrderModel.Items[i].Container;
                    order.Lines.UserFields.Fields.Item("U_ETIQUETA").Value = saleOrderModel.Items[i].Label;
                    order.Lines.UserFields.Fields.Item("U_RECETA").Value = saleOrderModel.Items[i].NeedRecipe == "Y" ? "Si" : "No";
                    order.Lines.Add();
                }

                var resultAdd = order.Add();

                if (resultAdd != 0)
                {
                    company.GetLastError(out int code, out string errMSg);
                    _loggerProxy.Info($"The sale order was tried to be created: {code} - {errMSg} - {JsonConvert.SerializeObject(saleOrderModel)}");
                    return ServiceUtils.CreateResult(false, 400, null, errMSg, null);
                }

                company.GetNewObjectCode(out var orderId);
                _loggerProxy.Info($"The sale order {orderId} was created {JsonConvert.SerializeObject(saleOrderModel)}");
                return ServiceUtils.CreateResult(true, 200, null, orderId, null);
            }
            catch(Exception ex)
            {
                _loggerProxy.Info($"There was an error while creating the sale order {ex.Message} - {ex.StackTrace} - {JsonConvert.SerializeObject(saleOrderModel)}");
                return ServiceUtils.CreateResult(false, 400, null, ex.Message, null);
            }
        }

        private async Task<string> SavePresciptionToServer(string recipeUrl)
        {
            if (string.IsNullOrEmpty(recipeUrl))
            {
                return string.Empty;
            }

            var azureKey = ConfigurationManager.AppSettings["AzureKey"];
            var azureAccountName = ConfigurationManager.AppSettings["AzureAccountName"];

            var azureObj = new AzureServices(azureAccountName, azureKey);
            var routeArray = recipeUrl.Split('/').ToList();
            var fileName = routeArray.Last();
            var containerRoute = recipeUrl.Replace(fileName, string.Empty);

            var routeFile = $"{ConfigurationManager.AppSettings["PrescriptionFiles"]}{fileName}";

            await azureObj.SaveToPathFromAzure(containerRoute, fileName, routeFile);

            return routeFile;
        }

        private string CreateAttachment(string pathfile)
        {
            var attachment = (Attachments2)company.GetBusinessObject(BoObjectTypes.oAttachments2);
            attachment.Lines.Add();
            attachment.Lines.FileName = Path.GetFileNameWithoutExtension(pathfile);
            attachment.Lines.FileExtension = Path.GetExtension(pathfile).Substring(1);
            attachment.Lines.SourcePath = Path.GetDirectoryName(pathfile);
            attachment.Lines.Override = BoYesNoEnum.tYES;
            

            if (attachment.Add() != 0)
            {
                company.GetLastError(out int code, out string errMSg);
                _loggerProxy.Info($"The attachement could not be saved {code} - {errMSg}");
                return string.Empty;
            }

            company.GetNewObjectCode(out var orderId);
            _loggerProxy.Info($"The attachmentid is {orderId}");
            return orderId;
        }
    }
}
