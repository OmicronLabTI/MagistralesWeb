// <summary>
// <copyright file="InvoiceSapOrderConfiguration.cs" company="Axity">
// Configuration for InvoiceSapOrderModel
// </copyright>
// </summary>

namespace Omicron.Invoice.Persistence.Configuration
{
    /// <summary>
    /// Configuration class for InvoiceSapOrderModel.
    /// </summary>
    public class InvoiceSapOrderConfiguration : IEntityTypeConfiguration<InvoiceSapOrderModel>
    {
        /// <inheritdoc/>
        public void Configure(EntityTypeBuilder<InvoiceSapOrderModel> builder)
        {
            builder.ToTable("saporders");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                   .HasColumnName("id")
                   .HasColumnType("bigint")
                   .IsRequired();

            builder.Property(p => p.SapOrderId)
                   .HasColumnName("idpedidosap")
                   .HasColumnType("varchar(100)")
                   .IsRequired();

            builder.Property(p => p.IdInvoice)
                   .HasColumnName("idinvoice")
                   .HasColumnType("varchar(100)")
                   .IsRequired();

            builder.HasOne(p => p.Invoice)
                   .WithMany(i => i.SapOrders)
                   .HasForeignKey(p => p.IdInvoice)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
