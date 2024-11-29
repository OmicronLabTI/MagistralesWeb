// <summary>
// <copyright file="UtcDateTimeInterceptor.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Catalogos.Entities.Interceptor
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Diagnostics;

    /// <summary>
    /// Class UtcDateTimeInterceptor.
    /// </summary>
    public class UtcDateTimeInterceptor : SaveChangesInterceptor
    {
        private static readonly TimeZoneInfo MexicoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Mexico_City");

        /// <inheritdoc/>
        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;
            if (context == null)
            {
                return await base.SavingChangesAsync(eventData, result, cancellationToken);
            }

            foreach (var entry in context.ChangeTracker.Entries())
            {
                NormalizeDateTimeProperties(entry);
            }

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        /// <inheritdoc/>
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            var context = eventData.Context;
            if (context == null)
            {
                return base.SavingChanges(eventData, result);
            }

            foreach (var entry in context.ChangeTracker.Entries())
            {
                NormalizeDateTimeProperties(entry);
            }

            return base.SavingChanges(eventData, result);
        }

        private static void NormalizeDateTimeProperties(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
        {
            foreach (var property in entry.Properties)
            {
                if (property.CurrentValue is DateTime dateTime)
                {
                    property.CurrentValue = InterceptorCommon.ConvertToUtc(dateTime);
                }
            }
        }
    }
}