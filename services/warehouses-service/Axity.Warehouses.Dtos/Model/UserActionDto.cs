// <summary>
// <copyright file="UserActionDto.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Axity.Warehouses.Dtos.Model
{
    /// <summary>
    /// class for the order operation.
    /// </summary>
    /// <typeparam name="T">Action model type.</typeparam>
    public class UserActionDto<T>
        where T : class
    {
        /// <summary>
        /// Gets or sets action model.
        /// </summary>
        /// <value>The model.</value>
        public T Data { get; set; }

        /// <summary>
        /// Gets or sets user id.
        /// </summary>
        /// <value>The user id.</value>
        public string UserId { get; set; }
    }
}
