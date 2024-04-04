// <summary>
// <copyright file="IPrescriptionService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapFile.Services.Prescription.Impl
{
    using Omicron.SapFile.Entities.Models;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Omicron.SapFile.Services.Utils;
    using System.Linq;
    using Omicron.SapFile.Services.Constants;
    using Omicron.SapFile.Dtos.Models;
    using System.Web.UI.WebControls;

    /// <summary>
    /// Class to Prescription Service.
    /// </summary>
    public class PrescriptionService : IPrescriptionService
    {
        public async Task<ResultModel> SavePresciptionToServer(List<PrescriptionServerRequestDto> prescriptionUrls)
        {
            var azureKey = ConfigurationManager.AppSettings[ServiceConstants.AzureKey];
            var azureAccountName = ConfigurationManager.AppSettings[ServiceConstants.AzureAccountName];

            var azureObj = new AzureServices(azureAccountName, azureKey);
            List<string> routeArray = new List<string>();
            string fileName = string.Empty;
            var containerRoute = string.Empty;
            var routeFile = string.Empty;
            bool itDownloadCorrectly = false;
            string messageAzure = string.Empty;
            var downloadResult = new List<PrescriptionServerResponseDto>();
            prescriptionUrls.ForEach(async presurl =>
            {
                routeArray = presurl.AzureRecipeUrl.Split(ServiceConstants.CharacterPathSeparator).ToList();
                fileName = routeArray.Last();
                containerRoute = presurl.AzureRecipeUrl.Replace(fileName, string.Empty);
                routeFile = $"{ConfigurationManager.AppSettings[ServiceConstants.PrescriptionFiles]}{fileName}";
                (itDownloadCorrectly, messageAzure) = await azureObj.SaveToPathFromAzure(containerRoute, fileName, routeFile);
                downloadResult.Add(
                    new PrescriptionServerResponseDto
                    {
                        AzureRecipeUrl = presurl.AzureRecipeUrl,
                        ServerRecipeUrl = routeFile,
                        ItDownloadCorrectly = itDownloadCorrectly,
                        Error = messageAzure,
                    });
            });


            return new ResultModel
            {
                Response = downloadResult,
                Code = 200,
                Success = true
            };
        }
    }
}
