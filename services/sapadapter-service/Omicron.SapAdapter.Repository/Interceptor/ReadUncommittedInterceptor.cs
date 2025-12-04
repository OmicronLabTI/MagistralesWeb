// <summary>
// <copyright file="ReadUncommittedInterceptor.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapAdapter.Entities.Interceptor
{
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore.Diagnostics;

    /// <summary>
    /// ReadUncommittedInterceptor.
    /// </summary>
    public class ReadUncommittedInterceptor : DbConnectionInterceptor
    {
        /// <summary>
        /// ConnectionOpenedAsync.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="eventData">EventData.</param>
        /// <param name="cancellationToken">Cancellation Token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public override Task ConnectionOpenedAsync(
        DbConnection connection,
        ConnectionEndEventData eventData,
        CancellationToken cancellationToken = default)
        {
            using var command = connection.CreateCommand();
            command.CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;";
            command.ExecuteNonQuery();

            return Task.CompletedTask;
        }
    }
}