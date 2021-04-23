// <summary>
// <copyright file="DatabaseContext.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.Entities.Context
{
    using Microsoft.EntityFrameworkCore;
    using Omicron.Catalogos.Entities.Model;

    /// <summary>
    /// Class DBcontext.
    /// </summary>
    public class DatabaseContext : DbContext, IDatabaseContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseContext"/> class.
        /// </summary>
        /// <param name="options">Connection Options.</param>
        public DatabaseContext(DbContextOptions options)
            : base(options)
        {
        }

        /// <inheritdoc/>
        public virtual DbSet<UserModel> CatUser { get; set; }

        /// <summary>
        /// Gets or sets the roles tables.
        /// </summary>
        /// <value>
        /// The roles tables.
        /// </value>
        public virtual DbSet<RoleModel> RoleModel { get; set; }

        /// <summary>
        /// Gets or sets parameters model.
        /// </summary>
        /// <value>
        /// Object parameters model.
        /// </value>
        public virtual DbSet<ParametersModel> ParametersModel { get; set; }
    }
}
