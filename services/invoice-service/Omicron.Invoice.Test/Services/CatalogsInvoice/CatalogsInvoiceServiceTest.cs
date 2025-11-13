// <summary>
// <copyright file="CatalogsInvoiceServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Invoice.Test.Services.CatalogsInvoice
{
    /// <summary>
    /// Class CatalogsInvoiceServiceTest.
    /// </summary>
    [TestFixture]
    public class CatalogsInvoiceServiceTest : BaseTest
    {
        private IInvoiceDao invoiceDao;
        private Mock<ILogger> logger;
        private Mock<IRedisService> redisService;
        private Mock<IInvoiceService> invoiceService;
        private Mock<IAzureService> azureService;
        private Mock<IConfiguration> configuration;
        private DatabaseContext context;

        /// <summary>
        /// Init configuration.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TemporalCatalogsInvoiceServiceTestDB")
                .Options;

            this.context = new DatabaseContext(options);
            this.context.InvoiceError.AddRange(this.GetInvoiceErrorModel());
            this.context.SaveChanges();

            this.logger = new Mock<ILogger>();
            this.redisService = new Mock<IRedisService>();
            this.invoiceService = new Mock<IInvoiceService>();
            this.azureService = new Mock<IAzureService>();
            this.configuration = new Mock<IConfiguration>();
            this.invoiceDao = new InvoiceDao(this.context);

            // Setup configuration mock
            this.configuration.Setup(x => x["AzureAccountKey"]).Returns("test-key");
            this.configuration.Setup(x => x["AzureAccountName"]).Returns("test-account");
            this.configuration.Setup(x => x["InvoiceErrorsCatalogsFileUrl"]).Returns("https://test.blob.core.windows.net/test.xlsx");

            // Setup AzureService mock to simulate Excel download
            this.azureService
            .Setup(x => x.GetElementsFromAzure(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
            .Returns(Task.CompletedTask);

            // Setup Redis mock
            this.redisService
                .Setup(x => x.WriteToRedis(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()))
                .Returns((Task<bool>)Task.CompletedTask);
        }

        /// <summary>
        /// Create mock Excel data with sample errors.
        /// </summary>
        /// <returns>MemoryStream with Excel data.</returns>
        private MemoryStream CreateMockExcelData()
        {
            var stream = new MemoryStream();

            // Crear un Excel simple con ClosedXML
            using (var workbook = new ClosedXML.Excel.XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Errors");

                // Headers
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Code";
                worksheet.Cell(1, 3).Value = "Error";
                worksheet.Cell(1, 4).Value = "ErrorMessage";
                worksheet.Cell(1, 5).Value = "RequireManualChange";

                // Data - Registro existente (UPDATE)
                worksheet.Cell(2, 1).Value = 1;
                worksheet.Cell(2, 2).Value = "55P03";
                worksheet.Cell(2, 3).Value = "lock_not_available";
                worksheet.Cell(2, 4).Value = "Otro proceso está usando esta información";
                worksheet.Cell(2, 5).Value = "0";

                // Data - Registro nuevo (INSERT)
                worksheet.Cell(3, 1).Value = 100;
                worksheet.Cell(3, 2).Value = "08006";
                worksheet.Cell(3, 3).Value = "connection_failure";
                worksheet.Cell(3, 4).Value = "No se pudo conectar con la base de datos";
                worksheet.Cell(3, 5).Value = "0";

                // Data - Registro nuevo (INSERT)
                worksheet.Cell(4, 1).Value = 101;
                worksheet.Cell(4, 2).Value = "XX000";
                worksheet.Cell(4, 3).Value = "internal_error";
                worksheet.Cell(4, 4).Value = "Ocurrió un error interno";
                worksheet.Cell(4, 5).Value = "1";

                workbook.SaveAs(stream);
            }

            stream.Position = 0;
            return stream;
        }
    }
}