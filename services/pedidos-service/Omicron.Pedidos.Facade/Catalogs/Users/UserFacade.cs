// <summary>
// <copyright file="UserFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Facade.Catalogs.Users
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Omicron.Pedidos.Dtos.User;
    using Omicron.Pedidos.Services.User;

    /// <summary>
    /// Class User Facade.
    /// </summary>
    public class UserFacade : IUserFacade
    {
        private readonly IUsersService usersService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserFacade"/> class.
        /// </summary>
        /// <param name="usersService">Interface User Service.</param>
        public UserFacade(IUsersService usersService)
        {
            this.usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<UserDto>> GetListUsersActive()
        {
            return await this.usersService.GetAllUsersAsync();
        }

        /// <inheritdoc/>
        public async Task<UserDto> GetListUserActive(int id)
        {
            return await this.usersService.GetUserAsync(id);
        }

        /// <inheritdoc/>
        public async Task<bool> InsertUser(UserDto user)
        {
            return await this.usersService.InsertUser(user);
        }
    }
}
