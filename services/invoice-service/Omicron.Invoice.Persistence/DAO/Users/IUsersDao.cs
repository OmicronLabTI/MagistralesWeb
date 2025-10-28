// <summary>
// <copyright file="IUsersDao.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Persistence.DAO.Users
{
    /// <summary>
    /// Interface IUsersDao.
    /// </summary>
    public interface IUsersDao
    {
        /// <summary>
        /// Method for GetAllAsync.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<IEnumerable<UserModel>> GetAllAsync();

        /// <summary>
        /// Method for GetByIdAsync.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<UserModel> GetByIdAsync(int id);

        /// <summary>
        /// Method for InsertAsync.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task InsertAsync(UserModel model);

        /// <summary>
        /// Method for UpdateAsync.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>A <see cref="UserModel"/> representing the result of the operation.</returns>
        UserModel Update(UserModel model);

        /// <summary>
        /// Method for DeleteAsync.
        /// </summary>
        /// <param name="model">The model.</param>
        void Delete(UserModel model);

        /// <summary>
        /// Method for SaveChangesAsync.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the operation.</returns>
        Task<int> SaveChangesAsync();
    }
}
