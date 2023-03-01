// <summary>
// <copyright file="DoctorDeliveryAddressCommand.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Services.Mediator.Commands
{
    using System.Collections.Generic;
    using MediatR;
    using Omicron.SapAdapter.Entities.Model.BusinessModels;
    using Omicron.SapAdapter.Services.Utils;

    /// <summary>
    /// DoctorServiceByTransactionCommand class.
    /// </summary>
    public class DoctorDeliveryAddressCommand : IRequest<List<DoctorDeliveryAddressModel>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoctorDeliveryAddressCommand"/> class.
        /// </summary>
        /// <param name="doctorAddress">Doctor Address.</param>
        public DoctorDeliveryAddressCommand(List<GetDoctorAddressModel> doctorAddress)
        {
            this.DoctorAddress = doctorAddress.ThrowIfNull(nameof(doctorAddress));
        }

        /// <summary>
        /// Gets or sets Doctor Address.
        /// </summary>
        /// <value>
        /// <see cref="List{GetDoctorAddressModel}"/> Doctor Address.
        /// </value>
        public List<GetDoctorAddressModel> DoctorAddress { get; set; }
    }
}
