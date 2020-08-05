// ------------------------------------------------------------------------------------------------
// <copyright file="LayoutElementsController.cs" company="Ann Inc">
//  Copyright (c) 2017 Ann Inc, All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace Omicron.SapDiApi.Api.Controllers
{    
    using System.Threading.Tasks;
    using System.Web.Http;

    /// <summary>
    /// Layout Element controller for any layout element which belongs to a specific layout
    /// </summary>
    [RoutePrefix("SapDiApi")]
    public class LayoutElementsController : BaseController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutElementsController"/> class.
        /// It creates the mapping object for internal types with its corresponding model.
        /// </summary>
        public LayoutElementsController()
        {
        }

        /// <summary>
        /// the ping pong.
        /// </summary>
        /// <returns>rturn pong.</returns>
        [HttpGet]
        [Route("ping")]
        public async Task<IHttpActionResult> Get()
        {
            return this.Ok("pong");
        }
    }
}