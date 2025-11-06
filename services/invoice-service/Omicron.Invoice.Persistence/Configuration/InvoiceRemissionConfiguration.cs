// <summary>
// <copyright file="InvoiceRemissionConfiguration.cs" company="Axity">
// Configuration for InvoiceRemissionModel
// </copyright>
// </summary>

namespace Omicron.Invoice.Persistence.Configuration
{
       /// <summary>
       /// Configuration class for InvoiceRemissionModel.
       /// </summary>
       public class InvoiceRemissionConfiguration : IEntityTypeConfiguration<InvoiceRemissionModel>
       {
              /// <inheritdoc/>
              public void Configure(EntityTypeBuilder<InvoiceRemissionModel> builder)
              {
                     builder.ToTable("remissions");

                     builder.HasKey(p => p.Id);

                     builder.Property(p => p.Id)
                            .HasColumnName("id")
                            .HasColumnType("bigint")
                            .ValueGeneratedOnAdd()
                            .IsRequired();

                     builder.Property(p => p.RemissionId)
                            .HasColumnName("idremission")
                            .HasColumnType("int")
                            .IsRequired();

                     builder.Property(p => p.IdInvoice)
                            .HasColumnName("idinvoice")
                            .HasColumnType("varchar(100)")
                            .IsRequired();

                     builder.HasOne(p => p.Invoice)
                            .WithMany(i => i.Remissions)
                            .HasForeignKey(p => p.IdInvoice)
                            .OnDelete(DeleteBehavior.Cascade);
              }
       }
}
