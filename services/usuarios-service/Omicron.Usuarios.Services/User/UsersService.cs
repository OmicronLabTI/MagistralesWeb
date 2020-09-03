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
    using Omicron.Usuarios.Services.Pedidos;
    using Omicron.Usuarios.Services.SapAdapter;
    using Omicron.Usuarios.Services.Utils;

    /// <summary>
    /// Class User Service.
    /// </summary>
    public class UsersService : IUsersService
    {
        private readonly IMapper mapper;

        private readonly IUserDao userDao;

        private readonly IPedidosService pedidoService;

        private readonly ISapAdapter sapService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersService"/> class.
        /// </summary>
        /// <param name="mapper">Object to mapper.</param>
        /// <param name="userDao">Object to userDao.</param>
        /// <param name="pedidoService">The pedido service.</param>
        /// <param name="sapAdapter">The sap adapter.</param>
        public UsersService(IMapper mapper, IUserDao userDao, IPedidosService pedidoService, ISapAdapter sapAdapter)
        {
            this.mapper = mapper;
            this.userDao = userDao ?? throw new ArgumentNullException(nameof(userDao));
            this.pedidoService = pedidoService ?? throw new ArgumentNullException(nameof(pedidoService));
            this.sapService = sapAdapter ?? throw new ArgumentException(nameof(sapAdapter));
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

            var usersOrdered = users.OrderBy(x => x.FirstName).ToList();
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
            var response = await this.userDao.DeleteUsers(listIds);
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

            var userExist = await this.userDao.GetUserByUserName(user.UserName);

            if (userExist != null && userExist.Id != user.Id)
            {
                throw new CustomServiceException(ServiceConstants.UserAlreadyExist, HttpStatusCode.BadRequest);
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
            return ServiceUtils.CreateResult(true, (int)HttpStatusCode.OK, null, JsonConvert.SerializeObject(users), null, null);
        }

        /// <summary>
        /// Gets the user with the count of orders.
        /// </summary>
        /// <returns>the data.</returns>
        public async Task<ResultModel> GetActiveQfbWithOrcerCount()
        {
            var roles = (await this.userDao.GetAllRoles()).FirstOrDefault(x => x.Description.ToLower().Contains(ServiceConstants.Qfb));
            var rolId = roles == null ? 0 : roles.Id;
            var users = (await this.userDao.GetUsersByRole(rolId)).Where(x => x.Activo == 1).ToList();

            var userIdList = users.Select(x => x.Id);
            var pedidosResponse = await this.pedidoService.PostPedidos(userIdList, ServiceConstants.QfbOrders);
            var userOrders = JsonConvert.DeserializeObject<List<UserOrderModel>>(pedidosResponse.Response.ToString());
            var userWithCount = await this.GroupUserWithOrderCount(users, userOrders);

            return ServiceUtils.CreateResult(true, 200, null, userWithCount, null, null);
        }

        /// <summary>
        /// gets the relation between orders and the user.
        /// </summary>
        /// <param name="users">the users.</param>
        /// <param name="orders">the orders.</param>
        /// <returns>the relationship.</returns>
        private async Task<List<UserWithOrderCountModel>> GroupUserWithOrderCount(List<UserModel> users, List<UserOrderModel> orders)
        {
            var listToReturn = new List<UserWithOrderCountModel>();

            foreach (var x in users)
            {
                var usersOrders = orders.Where(y => y.Userid.Equals(x.Id) && ServiceConstants.ListStatusOrdenes.Contains(y.Status)).ToList();
                var sapOrders = await this.GetFabOrders(usersOrders);

                listToReturn.Add(new UserWithOrderCountModel
                {
                    UserId = x.Id,
                    UserName = $"{x.FirstName} {x.LastName}",
                    CountTotal = usersOrders.Where(y => !string.IsNullOrEmpty(y.Productionorderid)).ToList().Count,
                    CountTotalOrders = usersOrders.Select(y => y.Salesorderid).Distinct().Count(),
                    CountTotalPieces = sapOrders.Sum(y => (int)y.Quantity),
                });
            }

            return listToReturn.OrderBy(x => x.UserName).ToList();
        }

        /// <summary>
        /// Gets the sap response for the orders.
        /// </summary>
        /// <param name="orders">the orders.</param>
        /// <returns>the data.</returns>
        private async Task<List<FabricacionOrderModel>> GetFabOrders(List<UserOrderModel> orders)
        {
            if (!orders.Any())
            {
                return new List<FabricacionOrderModel>();
            }

            var ids = orders.Where(x => !string.IsNullOrEmpty(x.Productionorderid)).Select(y => int.Parse(y.Productionorderid)).ToList();
            var sapResponse = await this.sapService.PostSapAdapter(ids, ServiceConstants.GetFabOrders);
            return JsonConvert.DeserializeObject<List<FabricacionOrderModel>>(sapResponse.Response.ToString());
        }
    }
}
