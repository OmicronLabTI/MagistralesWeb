// <summary>
// <copyright file="IUsersFacade.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Facade.Users
{
    /// <summary>
    /// Interface IUsersFacade.
    /// </summary>
    public interface IUsersFacade
    {
        /// <summary>
        /// Method for get all users.
        /// </summary>
        /// <returns>A <see cref="Task{IEnumerable{UserDto}}"/> representing the result of the asynchronous operation.</returns>
        Task<IEnumerable<UserDto>> GetAllAsync();

        /// <summary>
        /// Method for get a user by id.
        /// </summary>
        /// <param name="id">User Id.</param>
        /// <returns>A <see cref="Task{UserDto}"/> representing the result of the asynchronous operation.</returns>
        Task<UserDto> GetByIdAsync(int id);

        /// <summary>
        /// Method for insert a user.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="userRequest">Object to insert.</param>
        /// <returns>A <see cref="Task{UserDto}"/> representing the result of the asynchronous operation.</returns>
        Task<UserDto> InsertAsync(string user, CreateUserDto userRequest);

        /// <summary>
        /// Method for update a user.
        /// </summary>
        /// <param name="id">User Id.</param>
        /// <param name="user">User name.</param>
        /// <param name="userRequest">Object to update.</param>
        /// <returns>A <see cref="Task{UserDto}"/> representing the result of the asynchronous operation.</returns>
        Task<UserDto> UpdateAsync(int id, string user, UpdateUserDto userRequest);

        /// <summary>
        /// Method for delete a user.
        /// </summary>
        /// <param name="id">User  Id.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task DeleteAsync(int id);
    }
}