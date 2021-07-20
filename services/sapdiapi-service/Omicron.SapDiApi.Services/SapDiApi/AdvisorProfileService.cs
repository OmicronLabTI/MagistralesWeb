// <summary>
// <copyright file="AdvisorProfileService.cs" company="Axity">
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
    using Omicron.SapDiApi.Entities.Models.Experience;

    /// <summary>
    /// Advisor profile service.
    /// </summary>
    public class AdvisorProfileService : IAdvisorProfileService
    {
        private readonly Company company;
        private readonly ILoggerProxy _loggerProxy;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoctorAddress"/> class.
        /// </summary>   
        public AdvisorProfileService(ILoggerProxy loggerProxy)
        {
            this.company = Connection.Company;
            this._loggerProxy = loggerProxy;
        }

        /// <inheritdoc />
        public async Task<ResultModel> UpdateAdvisorProfileInfo(AdvisorProfileModel profileModel)
        {
            if (!int.TryParse(profileModel.AdvisorId, out var advisorId))
            {
                return ServiceUtils.CreateResult(false, 400, ServiceConstants.InvalidAdvisorId, ServiceConstants.InvalidAdvisorId, null);
            }

            var advisorSap = (EmployeesInfo)company.GetBusinessObject(BoObjectTypes.oEmployeesInfo);
            var advisorFound = advisorSap.GetByKey(advisorId);

            if (!advisorFound)
            {
                return ServiceUtils.CreateResult(false, 400, ServiceConstants.AdvisorNotFound, ServiceConstants.AdvisorNotFound, advisorId.ToString());
            }

            advisorSap = this.SetProfileInfo(profileModel.BirthDate, profileModel.PhoneNumber, profileModel.AdvisorId, advisorSap);
            var updated = advisorSap.Update();

            if (updated != 0)
            {
                company.GetLastError(out int errorCode, out string errMsg);
                _loggerProxy.Info($"The next addresses were tried to be updated to the doctor: {profileModel.AdvisorId} - {errorCode} - {errMsg} - {JsonConvert.SerializeObject(profileModel)}");
                return ServiceUtils.CreateResult(false, 400, errMsg, null, null);
            }

            return ServiceUtils.CreateResult(true, 200, null, advisorSap.FirstName, null);
        }


        private EmployeesInfo SetProfileInfo(DateTime? birthDate, string phoneNumber, string advisorId, EmployeesInfo advisorSap)
        {
            var recordSet = this.ExecuteQuery(ServiceConstants.GetAdvisorInfo, advisorId);

            if (recordSet.RecordCount == 0)
            {
                return advisorSap;
            }

            for (var i = 0; i < recordSet.RecordCount; i++)
            {
                if (birthDate != null)
                {
                    advisorSap.DateOfBirth = (DateTime)birthDate;
                }

                advisorSap.OfficePhone = phoneNumber;
                recordSet.MoveNext();
            }

            return advisorSap;
        }

        private Recordset ExecuteQuery(string query, params object[] parameters)
        {
            query = string.Format(query, parameters);
            var recordSet = (Recordset)company.GetBusinessObject(BoObjectTypes.BoRecordset);
            recordSet.DoQuery(query);
            return recordSet;
        }

    }
}
