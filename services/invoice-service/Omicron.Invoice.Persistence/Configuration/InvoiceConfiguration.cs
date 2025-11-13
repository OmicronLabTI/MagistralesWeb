// <summary>
// <copyright file="InvoiceConfiguration.cs" company="Axity">
// Configuration for InvoiceModel
// </copyright>
// </summary>

namespace Omicron.Invoice.Persistence.Configuration
{
    /// <summary>
    /// InvoiceConfiguration class.
    /// </summary>
    public class InvoiceConfiguration : IEntityTypeConfiguration<InvoiceModel>
    {
        /// <inheritdoc/>
        public void Configure(EntityTypeBuilder<InvoiceModel> builder)
        {
            builder.ToTable("invoices");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                   .HasColumnName("id")
                   .IsRequired()
                   .HasColumnType("varchar(100)");

            builder.Property(p => p.DxpOrderId)
                   .HasColumnName("pedidodxp")
                   .HasColumnType("varchar(40)");

            builder.Property(p => p.IdInvoiceError)
                   .HasColumnName("idinvoiceerror")
                   .HasColumnType("integer");

            builder.Property(p => p.CreateDate)
                   .HasColumnName("createdate")
                   .HasColumnType("timestamp");

            builder.Property(p => p.AlmacenUser)
                   .HasColumnName("almacenuser")
                   .HasColumnType("varchar(50)");

            builder.Property(p => p.Status)
                   .HasColumnName("estatus")
                   .HasColumnType("varchar(50)");

            builder.Property(p => p.IdFacturaSap)
                   .HasColumnName("idfacturasap")
                   .HasColumnType("integer");

            builder.Property(p => p.TypeInvoice)
                   .HasColumnName("tipofactura")
                   .HasColumnType("varchar(50)");

            builder.Property(p => p.BillingType)
                   .HasColumnName("formafacturacion")
                   .HasColumnType("varchar(50)");

            builder.Property(p => p.InvoiceCreateDate)
                   .HasColumnName("invoicecreatedate")
                   .HasColumnType("timestamp");

            builder.Property(p => p.ErrorMessage)
                   .HasColumnName("errormessage")
                   .HasColumnType("varchar(255)");

            builder.Property(p => p.UpdateDate)
                   .HasColumnName("updatedate")
                   .HasColumnType("timestamp");

            builder.Property(p => p.RetryNumber)
                   .HasColumnName("retrynumber")
                   .HasColumnType("integer")
                   .HasDefaultValue(0);

            builder.Property(p => p.Type)
                   .HasColumnName("type")
                   .HasColumnType("varchar(50)");

            builder.Property(p => p.ManualChangeApplied)
                   .HasColumnName("manualchangeapplied")
                   .HasColumnType("boolean");

            builder.Property(p => p.IsProcessing)
                   .HasColumnName("isprocessing")
                   .HasColumnType("boolean")
                   .HasDefaultValue(false);

            builder.Property(p => p.Payload)
                    .HasColumnName("payload")
                    .HasColumnType("jsonb")
                    .IsRequired();
            builder.Property(p => p.RetryUser)
                   .HasColumnName("retryuser")
                   .HasColumnType("varchar(50)");
        }
    }
}