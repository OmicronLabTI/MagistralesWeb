// <summary>
// <copyright file="UsersDao.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Persistence.DAO.Users.Impl
{
    /// <summary>
    /// Class UsersDao.
    /// </summary>
    public class UsersDao : IUsersDao
    {
        private readonly DatabaseContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersDao"/> class.
        /// </summary>
        /// <param name="context">DataBase Context.</param>
        public UsersDao(DatabaseContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            this.context = context;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<UserModel>> GetAllAsync()
            => await this.context.Users.ToListAsync();

        /// <inheritdoc/>
        public async Task<UserModel> GetByIdAsync(int id)
            => await this.context.Users.FindAsync(id);

        /// <inheritdoc/>
        public async Task InsertAsync(UserModel model)
            => await this.context.AddAsync(model);

        /// <inheritdoc/>
        public UserModel Update(UserModel model)
            => this.context.Update(model).Entity;

        /// <inheritdoc/>
        public void Delete(UserModel model)
            => this.context.Remove(model);

        /// <inheritdoc/>
        public async Task<int> SaveChangesAsync() => await this.context.SaveChangesAsync();
    }
}
