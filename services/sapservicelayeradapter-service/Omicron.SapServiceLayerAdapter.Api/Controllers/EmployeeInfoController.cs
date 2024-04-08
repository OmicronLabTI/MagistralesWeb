// <summary>
// <copyright file="EmployeeInfoController.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

using Omicron.SapServiceLayerAdapter.Facade.EmployeeInfo;

namespace Omicron.SapServiceLayerAdapter.Api.Controllers
{
    /// <summary>
    /// InvoiceController class.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeInfoController : ControllerBase
    {
        private readonly IEmployeeInfoFacade employeeInfoFacade;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeInfoController"/> class.
        /// </summary>
        /// <param name="employeeInfoFacade">Employee info facade.</param>
        public EmployeeInfoController(IEmployeeInfoFacade employeeInfoFacade)
            => this.employeeInfoFacade = employeeInfoFacade ?? throw new ArgumentNullException(nameof(employeeInfoFacade));

        /// <summary>
        /// Update adviser profile info.
        /// </summary>
        /// <param name="adviserProfileInfo">Adviser profile info.</param>
        /// <returns>Result.</returns>
        [HttpPatch("/adviser/profileinfo")]
        public async Task<IActionResult> UpdateAdviserProfileInfo([FromBody] AdviserProfileInfoDto adviserProfileInfo)
            => this.Ok(await this.employeeInfoFacade.UpdateAdviserProfileInfo(adviserProfileInfo));
    }
}
