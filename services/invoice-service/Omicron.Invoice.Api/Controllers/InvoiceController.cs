// <summary>
// <copyright file="InvoiceController.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Api.Controllers
{
    /// <summary>
    /// InvoiceController class.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceFacade invoiceFacade;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceController"/> class.
        /// </summary>
        /// <param name="usersFacade">User Facade.</param>
        public InvoiceController(IInvoiceFacade usersFacade)
            => this.invoiceFacade = usersFacade ?? throw new ArgumentNullException(nameof(usersFacade));

        /// <summary>
        /// Method for get all users.
        /// </summary>
        /// <returns>A <see cref="Task{IEnumerable{UserDto}}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
            => this.Ok(await this.invoiceFacade.GetAllAsync());

        /// <summary>
        /// Method for get a user by id.
        /// </summary>
        /// <param name="id">User Id.</param>
        /// <returns>A <see cref="Task{UserDto}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
            => this.Ok(await this.invoiceFacade.GetByIdAsync(id));

        /// <summary>
        /// Method for insert a user.
        /// </summary>
        /// <param name="userRequest">Object to insert.</param>
        /// <returns>A <see cref="Task{UserDto}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        public async Task<IActionResult> InsertAsync([FromBody] CreateUserDto userRequest)
        {
            string user = "userToken";
            var response = await this.invoiceFacade.InsertAsync(user, userRequest);
            return this.Created($"/api/Users/{response.Id}", response);
        }

        /// <summary>
        /// Method for update a user.
        /// </summary>
        /// <param name="id">User Id.</param>
        /// <param name="userRequest">Object to update.</param>
        /// <returns>A <see cref="Task{UserDto}"/> representing the result of the asynchronous operation.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody] UpdateUserDto userRequest)
        {
            string user = "userToken";
            var response = await this.invoiceFacade.UpdateAsync(id, user, userRequest);
            return this.Ok(response);
        }

        /// <summary>
        /// Method to DeleteAsync.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            await this.invoiceFacade.DeleteAsync(id);
            return this.Ok();
        }

        /// <summary>
        /// Method Ping.
        /// </summary>
        /// <returns>Pong.</returns>
        [Route("ping")]
        [HttpGet]
        public IActionResult Ping()
        {
            return this.Ok("Pong");
        }
    }
}
