// <summary>
// <copyright file="SqlConnectionHealthCheck.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Api.Filters
{
    using System;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Omicron.SapAdapter.Entities.Context;

    /// <summary>
    /// class for sql connection check.
    /// </summary>
    public class SqlConnectionHealthCheck : IHealthCheck
    {
        private static readonly string DefaultTestQuery = "Select 1";

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlConnectionHealthCheck"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public SqlConnectionHealthCheck(string connectionString)
            : this(connectionString, testQuery: DefaultTestQuery)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlConnectionHealthCheck"/> class.
        /// </summary>
        /// <param name="connectionString">The connection.</param>
        /// <param name="testQuery">The tes query.</param>
        public SqlConnectionHealthCheck(string connectionString, string testQuery)
        {
            this.ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            this.TestQuery = testQuery;
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>
        /// The connections string.
        /// </value>
        public string ConnectionString { get; }

        /// <summary>
        /// Gets the test query.
        /// </summary>
        /// <value>
        /// The tests query.
        /// </value>
        public string TestQuery { get; }

        /// <summary>
        /// Check the health.
        /// </summary>
        /// <param name="context">the context.</param>
        /// <param name="cancellationToken">the cancellatio token.</param>
        /// <returns>the result.</returns>
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var connection = new SqlConnection(this.ConnectionString))
            {
                try
                {
                    await connection.OpenAsync(cancellationToken);

                    if (this.TestQuery != null)
                    {
                        var command = connection.CreateCommand();
                        command.CommandText = this.TestQuery;

                        await command.ExecuteNonQueryAsync(cancellationToken);
                    }
                }
                catch (DbException ex)
                {
                    return new HealthCheckResult(status: context.Registration.FailureStatus, exception: ex);
                }
            }

            return HealthCheckResult.Healthy();
        }
    }
}
