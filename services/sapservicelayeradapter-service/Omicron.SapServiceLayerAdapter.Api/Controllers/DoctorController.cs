// <summary>
// <copyright file="DoctorController.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Api.Controllers
{
    /// <summary>
    /// DoctorController class.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorFacade doctorFacade;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoctorController"/> class.
        /// </summary>
        /// <param name="doctorFacade">Doctor Facade.</param>
        public DoctorController(IDoctorFacade doctorFacade)
            => this.doctorFacade = doctorFacade ?? throw new ArgumentNullException(nameof(doctorFacade));

        /// <summary>
        /// Create new isolated production order.
        /// </summary>
        /// <param name="addresses">Isolated production order.</param>
        /// <returns>Operation result.</returns>
        [HttpPost]
        [Route("/doctor/delivery/address")]
        public async Task<IActionResult> UpdateDoctorAddress([FromBody] List<DoctorDeliveryAddressDto> addresses)
        {
            var result = await this.doctorFacade.UpdateDoctorAddress(addresses);
            return this.Ok(result);
        }

        /// <summary>
        /// Create new isolated production order.
        /// </summary>
        /// <param name="addresses">Isolated production order.</param>
        /// <returns>Operation result.</returns>
        [HttpPost]
        [Route("/doctor/invoice/address")]
        public async Task<IActionResult> UpdateDoctorAddress([FromBody] List<DoctorInvoiceAddressDto> addresses)
        {
            var result = await this.doctorFacade.UpdateDoctorAddress(addresses);
            return this.Ok(result);
        }

        /// <summary>
        /// Update doctor profile info.
        /// </summary>
        /// <param name="doctorProfileInfo">Adviser profile info.</param>
        /// <returns>Result.</returns>
        [HttpPatch("/doctor/profileinfo")]
        public async Task<IActionResult> UpdateAdviserProfileInfo([FromBody] DoctorProfileInfoDto doctorProfileInfo)
            => this.Ok(await this.doctorFacade.UpdateDoctorProfileInfo(doctorProfileInfo));
    }
}