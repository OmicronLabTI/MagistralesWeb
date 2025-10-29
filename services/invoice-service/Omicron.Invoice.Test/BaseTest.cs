// <summary>
// <copyright file="BaseTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.Invoice.Test
{
    using System.Collections.Generic;

    /// <summary>
    /// Class Base Test.
    /// </summary>
    public abstract class BaseTest
    {
        /// <summary>
        /// Get UserModel.
        /// </summary>
        /// <returns>The UserModel.</returns>
        public IEnumerable<UserModel> GetAllUserModel()
            => new List<UserModel>()
            {
                new UserModel { Id = 1, Name = "User 1", UserName = "user1", Email = "user1@yopmail.com",  Active = true },
                new UserModel { Id = 2, Name = "User 2", UserName = "user2", Email = "user2@yopmail.com",  Active = true },
                new UserModel { Id = 3, Name = "User 3", UserName = "user3", Email = "user3@yopmail.com",  Active = true },
                new UserModel { Id = 4, Name = "User 4", UserName = "user4", Email = "user4@yopmail.com",  Active = true },
                new UserModel { Id = 5, Name = "User 5", UserName = "user5", Email = "user5@yopmail.com",  Active = true },
                new UserModel { Id = 6, Name = "User 6", UserName = "user6", Email = "user6@yopmail.com",  Active = true },
                new UserModel { Id = 7, Name = "User 7", UserName = "user7", Email = "user7@yopmail.com",  Active = true },
                new UserModel { Id = 8, Name = "User 8", UserName = "user8", Email = "user8@yopmail.com",  Active = true },
                new UserModel { Id = 9, Name = "User 9", UserName = "user9", Email = "user9@yopmail.com",  Active = true },
            };
    }
}
