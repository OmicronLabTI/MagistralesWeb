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
        [SetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: $"TemporalCatalogsInvoiceServiceTestDB_{Guid.NewGuid()}")
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
            this.configuration.Setup(x => x[ServiceConstants.InvoiceErrorsCatalogsFileUrl])
                .Returns("https://test.blob.core.windows.net/test.xlsx");

            // Setup Redis mock
            this.redisService
                .Setup(x => x.WriteToRedis(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()))
                .ReturnsAsync(true);
        }

        /// <summary>
        /// Cleanup after each test.
        /// </summary>
        [TearDown]
        public void Cleanup()
        {
            this.context?.Dispose();
        }

        /// <summary>
        /// Validates successful execution when inserting new invoice error records.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task InvoiceCatalags_InsertNewRecords()
        {
            // Arrange
            var mockExcelStream = this.CreateMockExcelData();

            this.azureService
                .Setup(x => x.GetElementsFromAzure(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Stream>()))
                .Callback<string, string, string, Stream>((acc, key, file, stream) =>
                {
                    mockExcelStream.CopyTo(stream);
                    stream.Position = 0;
                });

            var service = new CatalogsInvoiceService(
                this.configuration.Object,
                this.logger.Object,
                this.redisService.Object,
                this.invoiceService.Object,
                this.invoiceDao,
                this.azureService.Object);

            var initialErrorCount = (await this.invoiceDao.GetAllErrors()).Count();

            // Act
            var result = await service.InvoiceErrorsFromExcel();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.True);
                Assert.That(result.Code, Is.EqualTo(200));
            });

            var finalErrorCount = (await this.invoiceDao.GetAllErrors()).Count();
            Assert.That(finalErrorCount, Is.GreaterThan(initialErrorCount), "inserted new records");

            var codes = new List<string> { "08006", "XX000" };
            var newErrors = await this.invoiceDao.GetExistingErrorsByCodes(codes);
            Assert.That(newErrors.Count, Is.EqualTo(2), "inserted the 2 new error codes");
        }

        /// <summary>
        /// Validates successful execution when updating existing invoice error records.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task InvoiceCatalogs_UpdateExistingRecords()
        {
            // Arrange
            var mockExcelStream = this.CreateMockExcelDataForUpdate();

            this.azureService
                .Setup(x => x.GetElementsFromAzure(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Stream>()))
                .Callback<string, string, string, Stream>((acc, key, file, stream) =>
                {
                    mockExcelStream.CopyTo(stream);
                    stream.Position = 0;
                });

            var service = new CatalogsInvoiceService(
                this.configuration.Object,
                this.logger.Object,
                this.redisService.Object,
                this.invoiceService.Object,
                this.invoiceDao,
                this.azureService.Object);

            var existingErrors = await this.invoiceDao.GetExistingErrorsByCodes(new List<string> { "55P03" });
            var oldMessage = existingErrors.FirstOrDefault()?.ErrorMessage;

            // Act
            var result = await service.InvoiceErrorsFromExcel();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.True);
                Assert.That(result.Code, Is.EqualTo(200));
            });

            var updatedErrors = await this.invoiceDao.GetExistingErrorsByCodes(new List<string> { "55P03" });
            var newMessage = updatedErrors.FirstOrDefault()?.ErrorMessage;

            Assert.That(newMessage, Is.Not.EqualTo(oldMessage));
        }

        /// <summary>
        /// Validates that Redis cache is correctly populated with invoice errors data.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task InvoiceCatalogs_WritesToRedis()
        {
            // Arrange
            var mockExcelStream = this.CreateMockExcelData();

            this.azureService
                .Setup(x => x.GetElementsFromAzure(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Stream>()))
                .Callback<string, string, string, Stream>((acc, key, file, stream) =>
                {
                    mockExcelStream.CopyTo(stream);
                    stream.Position = 0;
                });

            var service = new CatalogsInvoiceService(
                this.configuration.Object,
                this.logger.Object,
                this.redisService.Object,
                this.invoiceService.Object,
                this.invoiceDao,
                this.azureService.Object);

            // Act
            var result = await service.InvoiceErrorsFromExcel();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.True);
            });

            this.redisService.Verify(
                x => x.WriteToRedis(
                    ServiceConstants.InvoiceErrorsCatalogs,
                    It.Is<string>(s => !string.IsNullOrEmpty(s)),
                    TimeSpan.FromHours(12)),
                Times.Once);
        }

        /// <summary>
        /// Validates proper error handling when an exception occurs during execution.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task InvoiceCatalogs_HandlesException()
        {
            // Arrange
            this.azureService
                .Setup(x => x.GetElementsFromAzure(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Stream>()))
                .ThrowsAsync(new Exception("Azure service error"));

            var service = new CatalogsInvoiceService(
                this.configuration.Object,
                this.logger.Object,
                this.redisService.Object,
                this.invoiceService.Object,
                this.invoiceDao,
                this.azureService.Object);

            // Act
            var result = await service.InvoiceErrorsFromExcel();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.False);
                Assert.That(result.Code, Is.EqualTo(500));
                Assert.That(result.UserError, Is.EqualTo(ServiceConstants.InternalServerError));
                Assert.That(result.ExceptionMessage, Is.Null);
            });

            this.logger.Verify(
                x => x.Error(
                    It.IsAny<Exception>(),
                    ServiceConstants.LogsInvoiceErrorsCatalogs),
                Times.Once);
        }

        /// <summary>
        /// Validates that duplicate records are removed before processing.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task InvoiceCatalogs_RemovesDuplicates()
        {
            // Arrange
            var mockExcelStream = this.CreateMockExcelDataWithDuplicates();

            this.azureService
                .Setup(x => x.GetElementsFromAzure(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Stream>()))
                .Callback<string, string, string, Stream>((acc, key, file, stream) =>
                {
                    mockExcelStream.CopyTo(stream);
                    stream.Position = 0;
                });

            var service = new CatalogsInvoiceService(
                this.configuration.Object,
                this.logger.Object,
                this.redisService.Object,
                this.invoiceService.Object,
                this.invoiceDao,
                this.azureService.Object);

            var initialCount = (await this.invoiceDao.GetAllErrors()).Count();

            // Act
            var result = await service.InvoiceErrorsFromExcel();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.True);
            });

            var finalCount = (await this.invoiceDao.GetAllErrors()).Count();
            var duplicateErrors = await this.invoiceDao.GetExistingErrorsByCodes(new List<string> { "55P03" });

            Assert.That(duplicateErrors.Count(), Is.EqualTo(1));
        }

        /// <summary>
        /// Validates that empty or null values are filtered out correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task InvoiceCatalogs_FiltersInvalidRecords()
        {
            // Arrange
            var mockExcelStream = this.CreateMockExcelDataWithInvalidRecords();

            this.azureService
                .Setup(x => x.GetElementsFromAzure(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Stream>()))
                .Callback<string, string, string, Stream>((acc, key, file, stream) =>
                {
                    mockExcelStream.CopyTo(stream);
                    stream.Position = 0;
                });

            var service = new CatalogsInvoiceService(
                this.configuration.Object,
                this.logger.Object,
                this.redisService.Object,
                this.invoiceService.Object,
                this.invoiceDao,
                this.azureService.Object);

            // Act
            var result = await service.InvoiceErrorsFromExcel();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Success, Is.True);
            });

            var allErrors = await this.invoiceDao.GetAllErrors();
            Assert.That(allErrors.All(e => !string.IsNullOrWhiteSpace(e.Code)), Is.True);
            Assert.That(allErrors.All(e => !string.IsNullOrWhiteSpace(e.Error)), Is.True);
            Assert.That(allErrors.All(e => !string.IsNullOrWhiteSpace(e.ErrorMessage)), Is.True);
        }

        /// <summary>
        /// Create mock Excel data with invalid/empty records.
        /// </summary>
        /// <returns>MemoryStream with Excel data containing invalid records.</returns>
        private MemoryStream CreateMockExcelDataWithInvalidRecords()
        {
            var stream = new MemoryStream();

            using (var workbook = new ClosedXML.Excel.XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");

                // Headers
                worksheet.Cell(1, 1).Value = "Code";
                worksheet.Cell(1, 2).Value = "Error";
                worksheet.Cell(1, 3).Value = "ErrorMessage";
                worksheet.Cell(1, 4).Value = "RequireManualChange";

                // Valid record
                worksheet.Cell(2, 1).Value = "VALID1";
                worksheet.Cell(2, 2).Value = "valid_error";
                worksheet.Cell(2, 3).Value = "Valid error message";
                worksheet.Cell(2, 4).Value = "NO";

                // Invalid - empty Code
                worksheet.Cell(3, 1).Value = string.Empty;
                worksheet.Cell(3, 2).Value = "empty_code";
                worksheet.Cell(3, 3).Value = "Error with empty code";
                worksheet.Cell(3, 4).Value = "NO";

                // Invalid - empty Error
                worksheet.Cell(4, 1).Value = "INVALID2";
                worksheet.Cell(4, 2).Value = string.Empty;
                worksheet.Cell(4, 3).Value = "Error with empty error field";
                worksheet.Cell(4, 4).Value = "NO";

                // Invalid - empty ErrorMessage
                worksheet.Cell(5, 1).Value = "INVALID3";
                worksheet.Cell(5, 2).Value = "empty_message";
                worksheet.Cell(5, 3).Value = string.Empty;
                worksheet.Cell(5, 4).Value = "NO";

                workbook.SaveAs(stream);
            }

            stream.Position = 0;
            return stream;
        }

        /// <summary>
        /// Create mock Excel data with sample errors.
        /// </summary>
        /// <returns>MemoryStream with Excel data.</returns>
        private MemoryStream CreateMockExcelData()
        {
            var stream = new MemoryStream();

            using (var workbook = new ClosedXML.Excel.XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");

                // Headers
                worksheet.Cell(1, 1).Value = "Code";
                worksheet.Cell(1, 2).Value = "Error";
                worksheet.Cell(1, 3).Value = "ErrorMessage";
                worksheet.Cell(1, 4).Value = "RequireManualChange";

                // Data - Registro existente (UPDATE)
                worksheet.Cell(2, 1).Value = "55P03";
                worksheet.Cell(2, 2).Value = "lock_not_available";
                worksheet.Cell(2, 3).Value = "Otro proceso está usando esta información";
                worksheet.Cell(2, 4).Value = "NO";

                // Data - Registro nuevo (INSERT)
                worksheet.Cell(3, 1).Value = "08006";
                worksheet.Cell(3, 2).Value = "connection_failure";
                worksheet.Cell(3, 3).Value = "No se pudo conectar con la base de datos";
                worksheet.Cell(3, 4).Value = "NO";

                // Data - Registro nuevo (INSERT)
                worksheet.Cell(4, 1).Value = "XX000";
                worksheet.Cell(4, 2).Value = "internal_error";
                worksheet.Cell(4, 3).Value = "Ocurrió un error interno";
                worksheet.Cell(4, 4).Value = "SI";

                workbook.SaveAs(stream);
            }

            stream.Position = 0;
            return stream;
        }

        /// <summary>
        /// Create mock Excel data specifically for update testing with different messages.
        /// </summary>
        /// <returns>MemoryStream with Excel data.</returns>
        private MemoryStream CreateMockExcelDataForUpdate()
        {
            var stream = new MemoryStream();

            using (var workbook = new ClosedXML.Excel.XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");

                // Headers
                worksheet.Cell(1, 1).Value = "Code";
                worksheet.Cell(1, 2).Value = "Error";
                worksheet.Cell(1, 3).Value = "ErrorMessage";
                worksheet.Cell(1, 4).Value = "RequireManualChange";

                // Data - Registro existente con MENSAJE DIFERENTE (UPDATE)
                worksheet.Cell(2, 1).Value = "55P03";
                worksheet.Cell(2, 2).Value = "lock_not_available";
                worksheet.Cell(2, 3).Value = "MENSAJE ACTUALIZADO - Otro proceso está usando esta información";
                worksheet.Cell(2, 4).Value = "SI";

                workbook.SaveAs(stream);
            }

            stream.Position = 0;
            return stream;
        }

        /// <summary>
        /// Create mock Excel data with duplicate records.
        /// </summary>
        /// <returns>MemoryStream with Excel data containing duplicates.</returns>
        private MemoryStream CreateMockExcelDataWithDuplicates()
        {
            var stream = new MemoryStream();

            using (var workbook = new ClosedXML.Excel.XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");

                // Headers
                worksheet.Cell(1, 1).Value = "Code";
                worksheet.Cell(1, 2).Value = "Error";
                worksheet.Cell(1, 3).Value = "ErrorMessage";
                worksheet.Cell(1, 4).Value = "RequireManualChange";

                // Duplicate records with same code
                worksheet.Cell(2, 1).Value = "55P03";
                worksheet.Cell(2, 2).Value = "lock_not_available";
                worksheet.Cell(2, 3).Value = "Mensaje 1";
                worksheet.Cell(2, 4).Value = "NO";

                worksheet.Cell(3, 1).Value = "55P03";
                worksheet.Cell(3, 2).Value = "lock_not_available";
                worksheet.Cell(3, 3).Value = "Mensaje 2";
                worksheet.Cell(3, 4).Value = "SI";

                workbook.SaveAs(stream);
            }

            stream.Position = 0;
            return stream;
        }
    }
}