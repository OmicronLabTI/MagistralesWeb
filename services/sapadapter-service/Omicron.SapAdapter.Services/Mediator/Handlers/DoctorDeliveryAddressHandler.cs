// <summary>
// <copyright file="DoctorDeliveryAddressHandler.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Services.Mediator.Handlers
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Omicron.SapAdapter.Entities.Model.BusinessModels;
    using Omicron.SapAdapter.Services.Doctors;
    using Omicron.SapAdapter.Services.Mediator.Commands;
    using Omicron.SapAdapter.Services.Utils;

    /// <summary>
    /// DoctorServiceByTransactionHandler class.
    /// </summary>
    public class DoctorDeliveryAddressHandler : IRequestHandler<DoctorDeliveryAddressCommand, List<DoctorDeliveryAddressModel>>
    {
        private readonly IDoctorService doctorService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoctorDeliveryAddressHandler"/> class.
        /// </summary>
        /// <param name="doctorService">The Doctor Service.</param>
        public DoctorDeliveryAddressHandler(IDoctorService doctorService)
        {
            this.doctorService = doctorService.ThrowIfNull(nameof(doctorService));
        }

        /// <inheritdoc/>
        public async Task<List<DoctorDeliveryAddressModel>> Handle(DoctorDeliveryAddressCommand request, CancellationToken cancellationToken)
        {
            return await ServiceUtils.GetDoctorDeliveryAddressData(this.doctorService, request.DoctorAddress);
        }
    }
}
