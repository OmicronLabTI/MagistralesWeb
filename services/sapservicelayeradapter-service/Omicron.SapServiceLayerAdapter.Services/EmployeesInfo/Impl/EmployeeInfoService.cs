// <summary>
// <copyright file="EmployeeInfoService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Services.EmployeesInfo.Impl
{
    /// <summary>
    /// Class to Employee Info Service.
    /// </summary>
    public class EmployeeInfoService : IEmployeeInfoService
    {
        private readonly IServiceLayerClient serviceLayerClient;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeInfoService"/> class.
        /// </summary>
        /// <param name="serviceLayerClient">Service Layer Client.</param>
        /// <param name="logger">Logger.</param>
        public EmployeeInfoService(IServiceLayerClient serviceLayerClient, ILogger logger)
        {
            this.serviceLayerClient = serviceLayerClient.ThrowIfNull(nameof(serviceLayerClient));
            this.logger = logger.ThrowIfNull(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<ResultModel> UpdateAdviserProfileInfo(AdviserProfileInfoDto adviserProfileInfo)
        {
            try
            {
                if (!int.TryParse(adviserProfileInfo.AdviserId, out var adviserId))
                {
                    return ServiceUtils.CreateResult(false, 400, ServiceConstants.InvalidAdviserId, ServiceConstants.InvalidAdviserId, null);
                }

                var adviserRequest = new EmployeeInfoDto
                {
                    PhoneNumber = adviserProfileInfo.PhoneNumber,
                };

                if (adviserProfileInfo.BirthDate.HasValue)
                {
                    adviserRequest.BirthDate = (DateTime)adviserProfileInfo.BirthDate;
                }

                var adviserUpdateResult = await this.serviceLayerClient.PatchAsync(
                    string.Format(ServiceQuerysConstants.QryEmployeesInfoByDocEntry, adviserId), JsonConvert.SerializeObject(adviserRequest));

                if (!adviserUpdateResult.Success)
                {
                    this.logger.Error(
                        $"The next addresses were tried to be updated to the doctor: {adviserProfileInfo.AdviserId} - {adviserUpdateResult.UserError} - {JsonConvert.SerializeObject(adviserProfileInfo)}");
                    return ServiceUtils.CreateResult(false, 400, adviserUpdateResult.UserError, null, null);
                }

                return ServiceUtils.CreateResult(true, 200, null, null, null);
            }
            catch (Exception ex)
            {
                return ServiceUtils.CreateResult(false, 400, ex.Message, ex.Message, null);
            }
        }
    }
}
