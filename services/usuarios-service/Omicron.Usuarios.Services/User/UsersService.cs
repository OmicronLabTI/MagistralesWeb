// <summary>
// <copyright file="UsersService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Usuarios.Services.User
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using Newtonsoft.Json;
    using Omicron.Usuarios.DataAccess.DAO.User;
    using Omicron.Usuarios.Dtos.User;
    using Omicron.Usuarios.Entities.Model;
    using Omicron.Usuarios.Services.Constants;
    using Omicron.Usuarios.Services.Utils;

    /// <summary>
    /// Class User Service.
    /// </summary>
    public class UsersService : IUsersService
    {
        private readonly IMapper mapper;

        private readonly IUserDao userDao;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersService"/> class.
        /// </summary>
        /// <param name="mapper">Object to mapper.</param>
        /// <param name="userDao">Object to userDao.</param>
        public UsersService(IMapper mapper, IUserDao userDao)
        {
            this.mapper = mapper;
            this.userDao = userDao ?? throw new ArgumentNullException(nameof(userDao));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            return this.mapper.Map<List<UserDto>>(await this.userDao.GetAllUsersAsync());
        }

        /// <inheritdoc/>
        public async Task<UserDto> GetUserAsync(int userId)
        {
            return this.mapper.Map<UserDto>(await this.userDao.GetUserAsync(userId));
        }

        /// <inheritdoc/>
        public async Task<bool> InsertUser(UserDto user)
        {
            return await this.userDao.InsertUser(this.mapper.Map<UserModel>(user));
        }

        /// <summary>
        /// Method for validating the login.
        /// </summary>
        /// <param name="login">the login object.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<ResultModel> ValidateCredentials(LoginModel login)
        {
            var user = await this.userDao.GetUserByUserName(login.Username);

            if (user == null || user.UserName == null)
            {
                return ServiceUtils.CreateResult(false, ServiceConstants.LogicError, ServiceConstants.UserDontExist, null, null);
            }

            if (!user.Password.Equals(login.Password))
            {
                return ServiceUtils.CreateResult(false, ServiceConstants.LogicError, ServiceConstants.IncorrectPassword, null, null);
            }

            return ServiceUtils.CreateResult(true, ServiceConstants.StatusOk, null, JsonConvert.SerializeObject(user), null);
        }
    }
}
