// <summary>
// <copyright file="DoctorService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Services.Doctors
{
    /// <summary>
    /// Class for Doctor Service.
    /// </summary>
    public class DoctorService : IDoctorService
    {
        private readonly IServiceLayerClient serviceLayerClient;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoctorService"/> class.
        /// </summary>
        /// <param name="serviceLayerClient">Service Layer Client.</param>
        /// <param name="logger">Logger.</param>
        public DoctorService(IServiceLayerClient serviceLayerClient, ILogger logger)
        {
            this.serviceLayerClient = serviceLayerClient.ThrowIfNull(nameof(serviceLayerClient));
            this.logger = logger.ThrowIfNull(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<ResultModel> UpdateDoctorDeliveryAddress(List<DoctorDeliveryAddressDto> addresses)
        {
            try
            {
                var doctor = addresses.FirstOrDefault().DoctorId;
                var doctorSap = await this.GetDoctorById(doctor);

                var addressToInsert = addresses.Where(x => x.Action == ServiceConstants.ActionInsert).ToList();
                var addressToUpdate = addresses.Where(x => x.Action == ServiceConstants.ActionUpdate).ToList();
                var addressToDelete = addresses.Where(x => x.Action == ServiceConstants.ActionDelete).Select(y => y.AddressId).ToList();

                doctorSap = this.AddLocalItems(addressToInsert, new List<DoctorInvoiceAddressDto>(), doctorSap);
                doctorSap = this.UpdateLocalItems(doctorSap, addressToUpdate, new List<DoctorInvoiceAddressDto>());
                doctorSap = this.DeleteLocalItems(doctorSap, addressToDelete, ServiceConstants.DeliveryAddress);

                await this.SaveChanges(doctorSap);
            }
            catch (Exception ex)
            {
                return ServiceUtils.CreateResult(false, 400, ex.Message, ex.Message, null);
            }

            return ServiceUtils.CreateResult(true, 200, string.Empty, string.Empty, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> UpdateDoctorDeliveryAddress(List<DoctorInvoiceAddressDto> addresses)
        {
            try
            {
                var doctor = addresses.FirstOrDefault().DoctorId;
                var doctorSap = await this.GetDoctorById(doctor);

                var addressToInsert = addresses.Where(x => x.Action == ServiceConstants.ActionInsert).ToList();
                var addressToUpdate = addresses.Where(x => x.Action == ServiceConstants.ActionUpdate).ToList();
                var addressToDelete = addresses.Where(x => x.Action == ServiceConstants.ActionDelete).Select(y => y.NickName).ToList();

                doctorSap = this.AddLocalItems(new List<DoctorDeliveryAddressDto>(), addressToInsert, doctorSap);
                doctorSap = this.UpdateLocalItems(doctorSap, new List<DoctorDeliveryAddressDto>(), addressToUpdate);
                doctorSap = this.DeleteLocalItems(doctorSap, addressToDelete, ServiceConstants.InvoiceAddress);

                await this.SaveChanges(doctorSap);
            }
            catch (Exception ex)
            {
                return ServiceUtils.CreateResult(false, 400, ex.Message, ex.Message, null);
            }

            return ServiceUtils.CreateResult(true, 200, string.Empty, string.Empty, null);
        }

        /// <inheritdoc/>
        public async Task<ResultModel> UpdateDoctorProfileInfo(DoctorProfileInfoDto doctorProfileInfo)
        {
            this.logger.Information(
                $"Sap Service Layer Adapter - Update doctor profile info - {JsonConvert.SerializeObject(doctorProfileInfo)}");
            var doctorProfileInfoRequest = new BusinessParterProfileInfoDto
            {
                PhoneNumber = doctorProfileInfo.PhoneNumber,
            };

            if (doctorProfileInfo.BirthDate.HasValue)
            {
                doctorProfileInfoRequest.BirthDate = (DateTime)doctorProfileInfo.BirthDate;
            }

            var doctorProfileInfoResponse = await this.serviceLayerClient.PatchAsync(
                string.Format(ServiceQuerysConstants.QryDoctorbyId, doctorProfileInfo.DoctorId), JsonConvert.SerializeObject(doctorProfileInfoRequest));

            if (!doctorProfileInfoResponse.Success)
            {
                this.logger.Error(
                    $"Sap Service Layer Adapter - Update doctor profile info - ERROR: {doctorProfileInfoResponse.UserError} - {JsonConvert.SerializeObject(doctorProfileInfo)}");
                return ServiceUtils.CreateResult(false, 400, doctorProfileInfoResponse.UserError, null, null);
            }

            return ServiceUtils.CreateResult(true, 200, null, null, null);

        }

        private async Task<DoctorDto> GetDoctorById(string cardCode)
        {
            var response = await this.serviceLayerClient.GetAsync(string.Format(ServiceQuerysConstants.QryDoctorbyId, cardCode));
            if (!response.Success)
            {
                throw new CustomServiceException(response.UserError, (HttpStatusCode)response.Code);
            }

            return JsonConvert.DeserializeObject<DoctorDto>(response.Response.ToString());
        }

        private DoctorDto AddLocalItems(List<DoctorDeliveryAddressDto> addressesDelivery, List<DoctorInvoiceAddressDto> addressInvoice, DoctorDto doctorSap)
        {
            foreach (var address in addressesDelivery)
            {
                doctorSap = this.SetDeliveryFieldData(doctorSap, address);
            }

            foreach (var address in addressInvoice)
            {
                doctorSap = this.SetInvoiceFieldData(doctorSap, address);
            }

            return doctorSap;
        }

        private DoctorDto UpdateLocalItems(DoctorDto doctorSap, List<DoctorDeliveryAddressDto> deliveryAddresses, List<DoctorInvoiceAddressDto> invoiceAddresses)
        {
            doctorSap.Addresses.ForEach(item =>
            {
                var updatedAddres = deliveryAddresses
                    .Where(x => x.AddressId.Equals(item.AddressName))
                    .FirstOrDefault();
                if (updatedAddres != null && item.AddressType == ServiceConstants.DeliveryAddress)
                {
                    item.AddressName = updatedAddres.AddressId;
                    item.ZipCode = updatedAddres.ZipCode;
                    item.Country = "MX";
                    item.State = updatedAddres.State;
                    item.City = updatedAddres.City;
                    item.Block = updatedAddres.Neighborhood;
                    item.Street = updatedAddres.Street;
                    item.StreetNo = updatedAddres.Number;
                    item.GlobalLocationNumber = updatedAddres.Phone;
                    item.AddressType = ServiceConstants.DeliveryAddress;
                    item.AddressName2 = updatedAddres.Contact;
                }
            });

            doctorSap.Addresses.ForEach(item =>
            {
                var updatedAddres = invoiceAddresses
                    .Where(x => x.NickName.Equals(item.AddressName))
                    .FirstOrDefault();
                if (updatedAddres != null && item.AddressType == ServiceConstants.InvoiceAddress)
                {
                    item.AddressName = updatedAddres.NickName;
                    item.Reason = updatedAddres.NickName;
                    item.RFC = updatedAddres.Rfc;
                    item.FederalTaxID = updatedAddres.Rfc;
                    item.ZipCode = updatedAddres.ZipCode;
                    item.Country = "MX";
                    item.State = updatedAddres.State;
                    item.Block = updatedAddres.Suburb;
                    item.Street = updatedAddres.Street;
                    item.StreetNo = updatedAddres.Number;
                    item.City = updatedAddres.City;
                    item.GlobalLocationNumber = updatedAddres.Email;
                    item.AddressType = ServiceConstants.InvoiceAddress;
                    item.AddressName3 = updatedAddres.TaxRegimeCode;
                }
            });

            return doctorSap;
        }

        private DoctorDto DeleteLocalItems(DoctorDto doctorSap, List<string> address, string type)
        {
            doctorSap.Addresses = doctorSap.Addresses
                .Where(x => !address.Contains(x.AddressName) || (address.Contains(x.AddressName) && x.AddressType != type))
                .ToList();
            return doctorSap;
        }

        private DoctorDto SetInvoiceFieldData(DoctorDto doctorSap, DoctorInvoiceAddressDto address)
        {
            var newAddres = new DoctorAddressDto();
            newAddres.AddressName = address.NickName;
            newAddres.Reason = address.NickName;
            newAddres.RFC = address.Rfc;
            newAddres.FederalTaxID = address.Rfc;
            newAddres.ZipCode = address.ZipCode;
            newAddres.Country = "MX";
            newAddres.State = address.State;
            newAddres.Block = address.Suburb;
            newAddres.Street = address.Street;
            newAddres.StreetNo = address.Number;
            newAddres.City = address.City;
            newAddres.GlobalLocationNumber = address.Email;
            newAddres.AddressType = ServiceConstants.InvoiceAddress;
            newAddres.AddressName3 = address.TaxRegimeCode;

            doctorSap.Addresses.Add(newAddres);
            return doctorSap;
        }

        private DoctorDto SetDeliveryFieldData(DoctorDto doctorSap, DoctorDeliveryAddressDto address)
        {
            var newAddres = new DoctorAddressDto();
            newAddres.AddressName = address.AddressId;
            newAddres.ZipCode = address.ZipCode;
            newAddres.Country = "MX";
            newAddres.State = address.State;
            newAddres.City = address.City;
            newAddres.Block = address.Neighborhood;
            newAddres.Street = address.Street;
            newAddres.StreetNo = address.Number;
            newAddres.GlobalLocationNumber = address.Phone;
            newAddres.AddressType = ServiceConstants.DeliveryAddress;
            newAddres.AddressName2 = address.Contact;

            doctorSap.Addresses.Add(newAddres);
            return doctorSap;
        }

        private async Task SaveChanges(DoctorDto doctor)
        {
            var body = JsonConvert.SerializeObject(doctor);
            var result = await this.serviceLayerClient.PutAsync(string.Format(ServiceQuerysConstants.QryDoctorbyId, doctor.DoctorId), body);

            if (!result.Success)
            {
                throw new CustomServiceException(result.UserError, (HttpStatusCode)result.Code);
            }
        }
    }
}