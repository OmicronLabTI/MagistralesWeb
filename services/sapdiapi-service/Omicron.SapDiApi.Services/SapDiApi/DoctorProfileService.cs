// <summary>
// <copyright file="IDoctorProfileService.cs" company="Axity">
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
    /// Service for doctor profile
    /// </summary>
    public class DoctorProfileService : IDoctorProfileService
    {

        private readonly Company company;
        private readonly ILoggerProxy _loggerProxy;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoctorAddress"/> class.
        /// </summary>   
        public DoctorProfileService(ILoggerProxy loggerProxy)
        {
            this.company = Connection.Company;
            this._loggerProxy = loggerProxy;
        }

        /// <inheritdoc />
        public async Task<ResultModel> UpdateDoctorProfileInfo(DoctorProfileModel profileModel)
        {
            var doctorSap = (BusinessPartners)company.GetBusinessObject(BoObjectTypes.oBusinessPartners);
            var doctorFound = doctorSap.GetByKey(profileModel.DoctorId);

            if (!doctorFound)
            {
                return ServiceUtils.CreateResult(false, 400, ServiceConstants.DoctorNotFound, ServiceConstants.DoctorNotFound, null);
            }

            doctorSap = this.SetProfileInfo(profileModel.BirthDate, profileModel.DoctorId, doctorSap);

            var updated = doctorSap.Update();

            if (updated != 0)
            {
                company.GetLastError(out int errorCode, out string errMsg);
                _loggerProxy.Info($"The next addresses were tried to be updated to the doctor: {profileModel.DoctorId} - {errorCode} - {errMsg} - {JsonConvert.SerializeObject(profileModel)}");
                return ServiceUtils.CreateResult(false, 400, errMsg, null, null);
            }

            return ServiceUtils.CreateResult(true, 200, string.Empty, doctorSap, null);
        }


        private BusinessPartners SetProfileInfo(DateTime? birthDate, string doctorId, BusinessPartners doctorSap)
        {
            if (birthDate == null)
            {
                return doctorSap;
            }

            var recordSet = this.ExecuteQuery(ServiceConstants.GetDoctorInfo, doctorId);

            if (recordSet.RecordCount == 0)
            {
                return doctorSap;
            }

            for (var i = 0; i < recordSet.RecordCount; i++)
            {
                var sapDate = recordSet.Fields.Item("U_Fecha_Nacimiento").Value;

                if (sapDate == birthDate)
                {
                    recordSet.MoveNext();
                    continue;
                }

                doctorSap.UserFields.Fields.Item("U_Fecha_Nacimiento").Value = birthDate;

                recordSet.MoveNext();
            }

            return doctorSap;
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
