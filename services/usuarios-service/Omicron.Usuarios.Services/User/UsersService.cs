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
    using Omicron.Usuarios.DataAccess.DAO.User;
    using Omicron.Usuarios.Dtos.User;
    using Omicron.Usuarios.Entities.Model;

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
    }
}
