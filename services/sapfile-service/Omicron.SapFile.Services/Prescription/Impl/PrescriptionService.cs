// <summary>
// <copyright file="IPrescriptionService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapFile.Services.Prescription.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.SapFile.Dtos.Models;
    using Omicron.SapFile.Entities.Models;
    using Omicron.SapFile.Log;
    using Omicron.SapFile.Services.Constants;
    using Omicron.SapFile.Services.Utils;

    /// <summary>
    /// Class to Prescription Service.
    /// </summary>
    public class PrescriptionService : IPrescriptionService
    {
        private readonly ILoggerProxy _loggerProxy;

        /// <summary>
        /// Prescription Service.
        /// </summary>
        /// <param name="loggerProxy">Logger Proxy.</param>
        public PrescriptionService(ILoggerProxy loggerProxy)
        {
            this._loggerProxy = loggerProxy;
        }

        public async Task<ResultModel> SavePresciptionToServer(List<PrescriptionServerRequestDto> prescriptionUrls)
        {
            try
            {
                this._loggerProxy.Info($"Omicron.SapFile.Prescription Service - The following medical prescriptions will be downloaded {JsonConvert.SerializeObject(prescriptionUrls)}");
                var azureKey = ConfigurationManager.AppSettings[ServiceConstants.AzureKey];
                var azureAccountName = ConfigurationManager.AppSettings[ServiceConstants.AzureAccountName];
                var azureObj = new AzureServices(azureAccountName, azureKey);
                List<string> routeArray = new List<string>();
                string fileName = string.Empty;
                var containerRoute = string.Empty;
                var routeFile = string.Empty;
                var downloadResult = new List<PrescriptionServerResponseDto>();

                foreach (var presurl in prescriptionUrls)
                {
                    routeArray = presurl.AzurePrescriptionUrl.Split(ServiceConstants.CharacterPathSeparator).ToList();
                    fileName = routeArray.Last();
                    containerRoute = presurl.AzurePrescriptionUrl.Replace(fileName, string.Empty);
                    routeFile = $"{ConfigurationManager.AppSettings[ServiceConstants.PrescriptionFiles]}{fileName}";
                    await azureObj.SaveToPathFromAzure(containerRoute, fileName, routeFile);
                    downloadResult.Add(
                        new PrescriptionServerResponseDto
                        {
                            AzurePrescriptionUrl = presurl.AzurePrescriptionUrl,
                            ServerSourcePath = ConfigurationManager.AppSettings[ServiceConstants.PrescriptionFiles],
                            PrescriptionFileName = fileName.Replace(".pdf", ""),
                            PrescriptionFileExtension = Path.GetExtension(fileName).Substring(1),
                });
                }

                this._loggerProxy.Info($"Omicron.SapFile.Prescription Service - Prescriptions to return {JsonConvert.SerializeObject(downloadResult)}");
                return new ResultModel
                {
                    Response = downloadResult,
                    Code = 200,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                this._loggerProxy.Error(
                   $"Omicron.SapFile.Prescription Service - Error to download the recipe to server {JsonConvert.SerializeObject(prescriptionUrls)}. Error: {ex.Message} - {ex.StackTrace}");
                return new ResultModel
                {
                    Code = 400,
                    Success = false,
                    ExceptionMessage = ex.StackTrace,
                    UserError = ex.Message
                };
            }

        }
    }
}
