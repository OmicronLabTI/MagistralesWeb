// <summary>
// <copyright file="UserFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Usuarios.Facade.Catalogs.Users
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using Omicron.Usuarios.Dtos.Models;
    using Omicron.Usuarios.Dtos.User;
    using Omicron.Usuarios.Entities.Model;
    using Omicron.Usuarios.Services.User;

    /// <summary>
    /// Class User Facade.
    /// </summary>
    public class UserFacade : IUserFacade
    {
        /// <summary>
        /// Mapper Object.
        /// </summary>
        private readonly IMapper mapper;

        private readonly IUsersService usersService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserFacade"/> class.
        /// </summary>
        /// <param name="usersService">Interface User Service.</param>
        /// <param name="mapper">The mapper.</param>
        public UserFacade(IUsersService usersService, IMapper mapper)
        {
            this.mapper = mapper;
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

        /// <summary>
        /// Validate the user credentials.
        /// </summary>
        /// <param name="loginDto">the loginDto.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<ResultDto> ValidateCredentials(LoginDto loginDto)
        {
            return this.mapper.Map<ResultDto>(await this.usersService.ValidateCredentials(this.mapper.Map<LoginModel>(loginDto)));
        }

        /// <summary>
        /// The create user method.
        /// </summary>
        /// <param name="userDto">The user Dto.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<ResultDto> CreateUser(UserDto userDto)
        {
            return this.mapper.Map<ResultDto>(await this.usersService.CreateUser(this.mapper.Map<UserModel>(userDto)));
        }

        /// <summary>
        /// Gets all the user with offset and limit.
        /// </summary>
        /// <param name="parameters">the params.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<ResultDto> GetUsers(Dictionary<string, string> parameters)
        {
            return this.mapper.Map<ResultDto>(await this.usersService.GetUsers(parameters));
        }

        /// <summary>
        /// deletes the user.
        /// </summary>
        /// <param name="listIds">the list of ids.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<ResultDto> DeleteUser(List<string> listIds)
        {
            return this.mapper.Map<ResultDto>(await this.usersService.DeleteUser(listIds));
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="user">the user to update.</param>
        /// <returns>the user updated.</returns>
        public async Task<ResultDto> UpdateUser(UserDto user)
        {
            return this.mapper.Map<ResultDto>(await this.usersService.UpdateUser(this.mapper.Map<UserModel>(user)));
        }

        /// <summary>
        /// Get the user.
        /// </summary>
        /// <param name="userName">the username.</param>
        /// <returns>the user.</returns>
        public async Task<ResultDto> GetUser(string userName)
        {
            return this.mapper.Map<ResultDto>(await this.usersService.GetUser(userName));
        }
    }
}
