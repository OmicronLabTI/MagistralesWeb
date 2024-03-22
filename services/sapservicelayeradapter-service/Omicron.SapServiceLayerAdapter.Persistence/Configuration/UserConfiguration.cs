// <summary>
// <copyright file="UserConfiguration.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Persistence.Configuration
{
    /// <summary>
    /// UserConfiguration class.
    /// </summary>
    public class UserConfiguration : IEntityTypeConfiguration<UserModel>
    {
        /// <inheritdoc/>
        public void Configure(EntityTypeBuilder<UserModel> builder)
        {
            builder.ToTable("user").HasKey(p => p.Id);

            builder.Property(s => s.Name).HasColumnName("name").IsRequired().HasMaxLength(100);

            builder.Property(s => s.UserName).HasColumnName("user_name").IsRequired().HasMaxLength(50);

            builder.Property(s => s.Email).HasColumnName("email").IsRequired().HasMaxLength(100);

            builder.Property(s => s.Active).HasColumnName("active").IsRequired();

            builder.Property(s => s.Active)
                .IsRequired();
        }
    }
}
