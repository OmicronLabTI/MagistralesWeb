// <summary>
// <copyright file="ICatalogsService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Reporting.Services.Clients
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.Reporting.Entities.Model;

    /// <summary>
    /// Contranct catalogs service.
    /// </summary>
    public interface ICatalogsService
    {
        /// <summary>
        /// Method for get parameters by name.
        /// </summary>
        /// <param name="parameterNames">Parameter names.</param>
        /// <returns>Parameters.</returns>
        Task<List<ParametersModel>> GetParams(List<string> parameterNames);

        /// <summary>
        /// Method for get smtp config.
        /// </summary>
        /// <returns>Smtp config.</returns>
        Task<SmtpConfigModel> GetSmtpConfig();

        /// <summary>
        /// Method for get smtp config.
        /// </summary>
        /// <returns>Smtp config.</returns>
        Task<RawMaterialEmailConfigModel> GetRawMaterialEmailConfig();
    }
}
