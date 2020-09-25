// <summary>
// <copyright file="CatalogsService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Reporting.Services.Clients
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Omicron.Reporting.Entities.Model;
    using Omicron.Reporting.Services.Constants;

    /// <summary>
    /// Class catalogs service implementation.
    /// </summary>
    public class CatalogsService : BaseClientService, ICatalogsService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogsService" /> class.
        /// </summary>
        /// <param name="httpClient">Client Http.</param>
        public CatalogsService(HttpClient httpClient)
            : base(httpClient)
        {
        }

        /// <summary>
        /// Method for get parameters by name.
        /// </summary>
        /// <param name="parameterNames">Parameter names.</param>
        /// <returns>Parameters.</returns>
        public async Task<List<ParametersModel>> GetParams(List<string> parameterNames)
        {
            var query = $"{string.Join("=v&", parameterNames)}=v";
            var route = $"{EndPointConstants.CatalogsGetParameters}?{query}";
            var resultModel = await this.GetAsync(route);
            return JsonConvert.DeserializeObject<List<ParametersModel>>(resultModel.Response.ToString());
        }

        /// <summary>
        /// Method for get smtp config.
        /// </summary>
        /// <returns>Smtp config.</returns>
        public async Task<SmtpConfigModel> GetSmtpConfig()
        {
            var parameterNames = new List<string>
            {
                "EmailMiddleware",
                "EmailMiddlewarePassword",
                "SmtpServer",
                "SmtpPort",
            };

            var parameters = await this.GetParams(parameterNames);

            return new SmtpConfigModel
            {
                SmtpServer = parameters.FirstOrDefault(x => x.Field.Equals("SmtpServer")).Value,
                SmtpPort = int.Parse(parameters.FirstOrDefault(x => x.Field.Equals("SmtpPort")).Value),
                SmtpDefaultPassword = parameters.FirstOrDefault(x => x.Field.Equals("EmailMiddlewarePassword")).Value,
                SmtpDefaultUser = parameters.FirstOrDefault(x => x.Field.Equals("EmailMiddleware")).Value,
            };
        }

        /// <summary>
        /// Method for get smtp config.
        /// </summary>
        /// <returns>Smtp config.</returns>
        public async Task<RawMaterialEmailConfigModel> GetRawMaterialEmailConfig()
        {
            var parameterNames = new List<string>
            {
                "EmailAlmacen",
                "EmailLogisticaCc1",
                "EmailLogisticaCc2",
            };

            var parameters = await this.GetParams(parameterNames);
            var emailLogistica1 = parameters.FirstOrDefault(x => x.Field.Equals("EmailLogisticaCc1")).Value;
            var emailLogistica2 = parameters.FirstOrDefault(x => x.Field.Equals("EmailLogisticaCc2")).Value;

            return new RawMaterialEmailConfigModel
            {
                Addressee = parameters.FirstOrDefault(x => x.Field.Equals("EmailAlmacen")).Value,
                CopyTo = $"{emailLogistica1};{emailLogistica2}",
            };
        }
    }
}
