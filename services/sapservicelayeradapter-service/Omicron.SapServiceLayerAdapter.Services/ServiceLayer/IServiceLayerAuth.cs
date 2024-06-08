// <summary>
// <copyright file="IServiceLayerAuth.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.SapServiceLayerAdapter.Services.ServiceLayer
{
    /// <summary>
    /// Interface for providing authentication-related functionality to service layer clients.
    /// </summary>
    public interface IServiceLayerAuth
    {
        /// <summary>
        /// Asynchronously retrieves a session ID for authentication purposes.
        /// </summary>
        /// <returns>A task representing the asynchronous operation. The task result contains the session ID.</returns>
        Task<string> GetSessionIdAsync();

        /// <summary>
        /// Method to refresh session.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<string> RefreshSession();
    }
}
