// <summary>
// <copyright file="UtcDateTimeQueryInterceptor.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Usuarios.Entities.Interceptor
{
    using System;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore.Diagnostics;

    /// <summary>
    /// Utc Date Time Query Interceptor.
    /// </summary>
    public class UtcDateTimeQueryInterceptor : DbCommandInterceptor
    {
        /// <inheritdoc/>
        public override async ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
        {
            NormalizeDateTimeQueryParameters(command);

            return await base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
        }

        /// <inheritdoc/>
        public override InterceptionResult<DbDataReader> ReaderExecuting(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result)
        {
            NormalizeDateTimeQueryParameters(command);

            return base.ReaderExecuting(command, eventData, result);
        }

        private static void NormalizeDateTimeQueryParameters(DbCommand command)
        {
            // Recorre los parámetros de la consulta SQL y verifica si algún parámetro es un DateTime
            foreach (DbParameter parameter in command.Parameters)
            {
                if (parameter.Value is DateTime dateTime)
                {
                    // Convierte la fecha en función de su tipo
                    parameter.Value = InterceptorCommon.ConvertToUtc(dateTime);
                }
            }
        }
    }
}
