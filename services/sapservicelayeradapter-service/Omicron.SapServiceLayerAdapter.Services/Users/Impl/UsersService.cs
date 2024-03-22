// <summary>
// <copyright file="UsersService.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Services.Users.Impl
{
    /// <summary>
    /// UsersService class.
    /// </summary>
    public class UsersService : IUsersService
    {
        private readonly IUsersDao usersDao;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersService"/> class.
        /// </summary>
        /// <param name="mapper">Mapper.</param>
        /// <param name="usersDao">Users dao.</param>
        public UsersService(IMapper mapper, IUsersDao usersDao)
            => (this.mapper, this.usersDao) = (mapper, usersDao);

        /// <inheritdoc/>
        public async Task<IEnumerable<UserDto>> GetAllAsync()
            => this.mapper.Map<IEnumerable<UserDto>>(
                await this.usersDao.GetAllAsync());

        /// <inheritdoc/>
        public async Task<UserDto> GetByIdAsync(int id)
        {
            var model = await this.usersDao.GetByIdAsync(id);
            model.ThrowExceptionIfNull<NotFoundException>(
                string.Format(ErrorMessages.NotFoundIdFormat, id));
            return this.mapper.Map<UserDto>(model);
        }

        /// <inheritdoc/>
        public async Task<UserDto> InsertAsync(string user, CreateUserDto userRequest)
        {
            var model = this.mapper.Map<UserModel>(userRequest);
            model.Active = true;
            await this.usersDao.InsertAsync(model);
            return this.mapper.Map<UserDto>(model);
        }

        /// <inheritdoc/>
        public async Task<UserDto> UpdateAsync(
            int id, string user, UpdateUserDto usersRequest)
        {
            var model = await this.usersDao.GetByIdAsync(id);
            model.ThrowExceptionIfNull<NotFoundException>(
                string.Format(ErrorMessages.NotFoundIdFormat, id));

            model.Name = usersRequest.Name;
            model.UserName = usersRequest.UserName;
            model.Email = usersRequest.Email;
            model.Active = usersRequest.Active;
            this.usersDao.Update(model);
            return this.mapper.Map<UserDto>(model);
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(int id)
        {
            var model = await this.usersDao.GetByIdAsync(id);
            model.ThrowExceptionIfNull<NotFoundException>(
                string.Format(ErrorMessages.NotFoundIdFormat, id));
            this.usersDao.Delete(model);
            await this.usersDao.SaveChangesAsync();
        }
    }
}
