// <summary>
// <copyright file="InvoiceErrorConfiguration.cs" company="Axity">
// Configuration for InvoiceErrorModel
// </copyright>
// </summary>

namespace Omicron.Invoice.Persistence.Configuration
{
       /// <summary>
       /// InvoiceErrorConfiguration class.
       /// </summary>
       public class InvoiceErrorConfiguration : IEntityTypeConfiguration<InvoiceErrorModel>
       {
              /// <inheritdoc/>
              public void Configure(EntityTypeBuilder<InvoiceErrorModel> builder)
              {
                     builder.ToTable("invoiceerrors");

                     builder.HasKey(p => p.Id);

                     builder.Property(p => p.Id)
                            .HasColumnName("id")
                            .HasColumnType("integer")
                            .ValueGeneratedOnAdd()
                            .IsRequired();

                     builder.Property(p => p.Code)
                            .HasColumnName("code")
                            .IsRequired()
                            .HasMaxLength(100)
                            .HasColumnType("varchar(100)");

                     builder.Property(p => p.Error)
                            .HasColumnName("error")
                            .IsRequired()
                            .HasMaxLength(255)
                            .HasColumnType("varchar(255)");

                     builder.Property(p => p.ErrorMessage)
                            .HasColumnName("errormessage")
                            .HasMaxLength(255)
                            .HasColumnType("varchar(255)");

                     builder.Property(p => p.RequireManualChange)
                            .HasColumnName("requiremanualchange")
                            .HasColumnType("boolean")
                            .HasDefaultValue(false);

                     builder.HasMany(p => p.Invoices)
                            .WithOne(i => i.InvoiceError)
                            .HasForeignKey(i => i.IdInvoiceError)
                            .OnDelete(DeleteBehavior.SetNull);
              }
       }
}