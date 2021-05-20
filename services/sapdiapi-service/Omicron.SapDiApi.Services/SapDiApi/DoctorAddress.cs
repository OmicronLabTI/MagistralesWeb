// <summary>
// <copyright file="DoctorAddress.cs" company="Axity">
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
    /// clas for the data to sap.
    /// </summary>
    public class DoctorAddress : IDoctorAddress
    {
        private readonly Company company;
        private readonly ILoggerProxy _loggerProxy;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoctorAddress"/> class.
        /// </summary>   
        public DoctorAddress(ILoggerProxy loggerProxy)
        {
            this.company = Connection.Company;
            this._loggerProxy = loggerProxy;
        }

        /// <inheritdoc/>
        public async Task<ResultModel> UpdateDoctorDeliveryAddress(List<DoctorDeliveryAddressModel> addresses)
        {
            var doctor = addresses.FirstOrDefault().DoctorId;
            var doctorSap = (BusinessPartners)company.GetBusinessObject(BoObjectTypes.oBusinessPartners);
            var doctorFound = doctorSap.GetByKey(doctor);
            if (!doctorFound)
            {
                return ServiceUtils.CreateResult(false, 400, ServiceConstants.DoctorNotFound, ServiceConstants.DoctorNotFound, null);
            }

            var addressToUpdate = addresses.Where(x => x.Action == "update").ToList();
            var addressToInsert = addresses.Where(x => x.Action == "insert").ToList();
            var addressToDelete = addresses.Where(x => x.Action == "delete").ToList();

            doctorSap = this.InsertAddress(addressToInsert, doctor, ServiceConstants.AddresShip, doctorSap);
            doctorSap = this.UpdateAddress(addressToUpdate, doctor, ServiceConstants.AddresShip, doctorSap);
            var updated = doctorSap.Update();

            if (updated != 0)
            {
                company.GetLastError(out int errorCode, out string errMsg);
                _loggerProxy.Info($"The next addresses were tried to be updated to the doctor: {doctor} - {errorCode} - {errMsg} - {JsonConvert.SerializeObject(addresses)}");
                return ServiceUtils.CreateResult(false, 400, errMsg, null, null);
            }

            var responseDelete = this.DeleteAddress(addressToDelete, doctor, ServiceConstants.AddresShip);

            var code = responseDelete.Item1 ? 200 : 400;
            return ServiceUtils.CreateResult(responseDelete.Item1, code, responseDelete.Item2, null, null);
        }

        private BusinessPartners InsertAddress(List<DoctorDeliveryAddressModel> addresses, string doctor, string type, BusinessPartners doctorSap)
        {
            foreach (var address in addresses)
            {
                doctorSap.Addresses.Add();
                doctorSap = this.SetDeliveryFieldData(doctorSap, address, type);
            }

            return doctorSap;
        }

        private BusinessPartners UpdateAddress(List<DoctorDeliveryAddressModel> address, string doctor, string type, BusinessPartners doctorSap)
        {
            if (!address.Any())
            {
                return doctorSap;
            }

            var recordSet = this.ExecuteQuery(ServiceConstants.GetAddressesByDoctor, doctor, type);

            if (recordSet.RecordCount == 0)
            {
                return doctorSap;
            }

            for (var i = 0; i < recordSet.RecordCount; i++)
            {
                var lineNum = recordSet.Fields.Item("LineNum").Value;
                var adressName = recordSet.Fields.Item("Address").Value;
                
                try
                {
                    doctorSap.Addresses.SetCurrentLine(lineNum);
                }
                catch(Exception)
                {
                    _loggerProxy.Info($"Error while trying to update address {adressName}");
                    continue;
                }

                var localAddress = address.FirstOrDefault(x => x.AddressId == adressName);

                if (localAddress == null)
                {
                    recordSet.MoveNext();
                    continue;
                }

                doctorSap = this.SetDeliveryFieldData(doctorSap, localAddress, type);
                recordSet.MoveNext();
            }

            return doctorSap;
        }

        private Tuple<bool, string> DeleteAddress(List<DoctorDeliveryAddressModel> address, string doctor, string type)
        {
            var doctorSap = (BusinessPartners)company.GetBusinessObject(BoObjectTypes.oBusinessPartners);
            doctorSap.GetByKey(doctor);
            if (!address.Any())
            {
                return new Tuple<bool, string>(true, string.Empty);
            }

            var recordSet = this.ExecuteQuery(ServiceConstants.GetAddressesByDoctor, doctor, type);

            if (recordSet.RecordCount == 0)
            {
                return new Tuple<bool, string>(true, string.Empty);
            }

            var listLineNum = new List<int>();

            for (var i = 0; i < recordSet.RecordCount; i++)
            {
                var lineNum = recordSet.Fields.Item("LineNum").Value;
                var adressName = recordSet.Fields.Item("Address").Value;

                var localAddress = address.FirstOrDefault(x => x.AddressId == adressName);

                if (localAddress == null)
                {
                    recordSet.MoveNext();
                    continue;
                }

                listLineNum.Add(lineNum);
                recordSet.MoveNext();
            }

            foreach(var lineNum in listLineNum.OrderByDescending(x => x))
            {
                doctorSap.Addresses.SetCurrentLine(lineNum);
                doctorSap.Addresses.Delete();
            }

            var updated = doctorSap.Update();

            if (updated != 0)
            {
                company.GetLastError(out int errorCode, out string errMsg);
                _loggerProxy.Info($"The next address was tried to be deleted for the doctor: {doctor} - {errorCode} - {errMsg} - {JsonConvert.SerializeObject(address)}");
                return new Tuple<bool, string>(false, errMsg);
            }

            return new Tuple<bool, string>(true, string.Empty);
        }

        private BusinessPartners SetDeliveryFieldData(BusinessPartners doctorSap, DoctorDeliveryAddressModel address, string type)
        {
            doctorSap.Addresses.AddressName = address.AddressId;
            doctorSap.Addresses.ZipCode = address.ZipCode;
            doctorSap.Addresses.Country = "MX";
            doctorSap.Addresses.State = address.State;
            doctorSap.Addresses.City = address.City;
            doctorSap.Addresses.Block = address.Neighborhood;
            doctorSap.Addresses.Street = address.Street;
            doctorSap.Addresses.StreetNo = address.Number;
            doctorSap.Addresses.GlobalLocationNumber = address.Phone;
            doctorSap.Addresses.AddressType = type == ServiceConstants.AddresShip ? BoAddressType.bo_ShipTo : BoAddressType.bo_BillTo;
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
