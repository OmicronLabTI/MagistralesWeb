// <summary>
// <copyright file="UsersFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Facade.Users.Impl
{
    /// <summary>
    /// Class UsersFacade.
    /// </summary>
    public class UsersFacade : IUsersFacade
    {
        private readonly IUsersService usersService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersFacade"/> class.
        /// </summary>
        /// <param name="usersService">Interface UsersService.</param>
        public UsersFacade(IUsersService usersService) =>
            this.usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));

        /// <inheritdoc/>
        public async Task<IEnumerable<UserDto>> GetAllAsync()
            => await this.usersService.GetAllAsync();

        /// <inheritdoc/>
        public async Task<UserDto> GetByIdAsync(int id)
            => await this.usersService.GetByIdAsync(id);

        /// <inheritdoc/>
        public async Task<UserDto> InsertAsync(string user, CreateUserDto userRequest)
            => await this.usersService.InsertAsync(user, userRequest);

        /// <inheritdoc/>
        public async Task<UserDto> UpdateAsync(
            int id, string user, UpdateUserDto userRequest)
            => await this.usersService.UpdateAsync(id, user, userRequest);

        /// <inheritdoc/>
        public async Task DeleteAsync(int id)
            => await this.usersService.DeleteAsync(id);
    }
}