// <summary>
// <copyright file="DatabaseContext.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.KafkaConsumer.Entities.DbContext
{
    using System.Configuration;
    using Microsoft.EntityFrameworkCore;
    using Omicron.KafkaConsumer.Entities.Model;

    /// <summary>
    /// Class for the context.
    /// </summary>
    public class DatabaseContext : DbContext
    {
        public DbSet<InsertLogModel> InsertLogModel { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseNpgsql(ConfigurationManager.AppSettings["DbConnection"]);
    }
}
