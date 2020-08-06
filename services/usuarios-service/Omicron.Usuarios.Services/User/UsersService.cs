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
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using AutoMapper;
    using Newtonsoft.Json;
    using Omicron.LeadToCash.Resources.Exceptions;
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
                return ServiceUtils.CreateResult(false, ServiceConstants.LogicError, ServiceConstants.UserDontExist, null, null, null);
            }

            if (!user.Password.Equals(login.Password))
            {
                return ServiceUtils.CreateResult(false, ServiceConstants.LogicError, ServiceConstants.IncorrectPass, null, null, null);
            }

            return ServiceUtils.CreateResult(true, ServiceConstants.StatusOk, null, JsonConvert.SerializeObject(user), null, null);
        }

        /// <summary>
        /// Method to create a user.
        /// </summary>
        /// <param name="userModel">the user model.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<ResultModel> CreateUser(UserModel userModel)
        {
            var user = await this.userDao.GetUserByUserName(userModel.UserName);

            if (user != null)
            {
                throw new CustomServiceException(ServiceConstants.UserAlreadyExist, HttpStatusCode.BadRequest);
            }

            userModel.Id = Guid.NewGuid().ToString("D");
            userModel.Password = ServiceUtils.ConvertToBase64(userModel.Password);
            var dataBaseResponse = await this.userDao.InsertUser(userModel);

            if (!dataBaseResponse)
            {
                throw new CustomServiceException(ServiceConstants.ErrorWhileInsertingUser, HttpStatusCode.InternalServerError);
            }

            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, JsonConvert.SerializeObject(userModel), null, null);
        }

        /// <summary>
        /// Get all users.
        /// </summary>
        /// <param name="parameters">the parameters.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<ResultModel> GetUsers(Dictionary<string, string> parameters)
        {
            var users = await this.userDao.GetAllUsersAsync();

            var offset = parameters.ContainsKey(ServiceConstants.Offset) ? parameters[ServiceConstants.Offset] : "0";
            var limit = parameters.ContainsKey(ServiceConstants.Limit) ? parameters[ServiceConstants.Limit] : "1";

            int.TryParse(offset, out int offsetNumber);
            int.TryParse(limit, out int limitNumber);

            var usersOrdered = users.Where(x => x.Activo == 1).OrderBy(x => x.FirstName).ToList();
            var listUsers = usersOrdered.Skip(offsetNumber).Take(limitNumber).ToList();

            listUsers.ForEach(x => x.Password = ServiceUtils.ConvertFromBase64(x.Password));

            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, listUsers, null, users.Count());
        }

        /// <summary>
        /// Deletes the user logically.
        /// </summary>
        /// <param name="listIds">the list ids.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<ResultModel> DeleteUser(List<string> listIds)
        {
            var listUserToUpdate = new List<UserModel>();
            foreach (var i in listIds)
            {
                var user = await this.userDao.GetUserById(i);

                if (user != null)
                {
                    user.Activo = 0;
                    listUserToUpdate.Add(user);
                }
            }

            var response = await this.userDao.UpdateUsers(listUserToUpdate);
            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, response, null, null);
        }

        /// <summary>
        /// update the user.
        /// </summary>
        /// <param name="user">the user.</param>
        /// <returns>the user updaterd.</returns>
        public async Task<ResultModel> UpdateUser(UserModel user)
        {
            var usertoUpdate = await this.userDao.GetUserById(user.Id);

            if (usertoUpdate == null)
            {
                throw new CustomServiceException(ServiceConstants.UserDontExist, HttpStatusCode.BadRequest);
            }

            usertoUpdate.UserName = user.UserName;
            usertoUpdate.FirstName = user.FirstName;
            usertoUpdate.LastName = user.LastName;
            usertoUpdate.Password = ServiceUtils.ConvertToBase64(user.Password);
            usertoUpdate.Role = user.Role;
            usertoUpdate.Activo = user.Activo;

            var response = await this.userDao.UpdateUser(usertoUpdate);
            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, response, null, null);
        }

        /// <summary>
        /// gets the user.
        /// </summary>
        /// <param name="userName">the user.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<ResultModel> GetUser(string userName)
        {
            var user = await this.userDao.GetUserByUserName(userName);
            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, user, null, null);
        }

        /// <summary>
        /// gets the qfb.
        /// </summary>
        /// <param name="roleId">The roleid.</param>
        /// <returns>the list of qfb.</returns>
        public async Task<ResultModel> GetUsersByRole(string roleId)
        {
            int.TryParse(roleId, out int roleInt);
            var users = await this.userDao.GetUsersByRole(roleInt);
            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, users, null, users.Count());
        }

        /// <summary>
        /// returns user by id.
        /// </summary>
        /// <param name="listIds">the ids.</param>
        /// <returns>the data.</returns>
        public async Task<ResultModel> GetUsersById(List<string> listIds)
        {
            var users = await this.userDao.GetUsersById(listIds);
            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, users, null, null);
        }
    }
}
