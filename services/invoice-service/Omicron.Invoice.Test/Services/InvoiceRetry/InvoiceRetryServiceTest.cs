// <summary>
// <copyright file="InvoiceServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

using Omicron.Invoice.Common.DTOs.Requests.Invoices;
using Serilog.Core;

namespace Omicron.Invoice.Test.Services.InvoiceRetry
{
    /// <summary>
    /// Class InvoiceRetryServiceTest.
    /// </summary>
    [TestFixture]
    public class InvoiceRetryServiceTest : BaseTest
    {
        private IInvoiceDao invoiceDao;
        private Mock<ILogger> logger;
        private DatabaseContext context;

        /// <summary>
        /// Init configuration.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TemporalOrderValidationDBTest")
                .Options;

            this.context = new DatabaseContext(options);
            this.context.Invoices.AddRange(this.GetInvoiceModel());
            this.context.InvoiceError.AddRange(this.GetInvoiceErrorModel());
            this.context.SaveChanges();
            this.logger = new Mock<ILogger>();
            this.invoiceDao = new InvoiceDao(this.context);
        }

        /// <summary>
        /// GetDataToRetryCreateInvoicesAsyncTest.
        /// </summary>
        /// <param name="redisLock">Is Redis Lock.</param>
        /// <param name="expectedInvoices">Expected Invoices.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Test]
        [TestCase(false, 0)]
        [TestCase(true, 2)]
        public async Task GetDataToRetryCreateInvoicesAsyncTest(bool redisLock, int expectedInvoices)
        {
            // arrange
            var redisService = new Mock<IRedisService>();
            var invoiceService = new Mock<IInvoiceService>();

            redisService.Setup(x => x.SetKeyIfNotExists(
                ServiceConstants.InvoiceLockAutomaticRetryKey,
                It.IsAny<string>(),
                It.IsAny<TimeSpan>())).ReturnsAsync(redisLock);

            var sut = new InvoiceRetryService(this.logger.Object, this.invoiceDao, redisService.Object, invoiceService.Object);

            // act
            var result = await sut.GetDataToRetryCreateInvoicesAsync();
            var count = (int)result.Response;

            // assert
            Assert.That(result.Success, Is.True);
            Assert.That(count, Is.EqualTo(expectedInvoices));
        }

        /// <summary>
        /// RetryCreateInvoicesAsyncTest.
        /// </summary>
        /// <param name="executionType">Execution Type.</param>
        /// <param name="expectedProcessed">Expected Processed.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Test]
        [TestCase("manual", 1, true)]
        [TestCase("automatic", 3, true)]
        [TestCase("automatic", 0, false)]
        public async Task RetryCreateInvoicesAsyncTest(string executionType, int expectedProcessed, bool hasRecords)
        {
            // arrange
            var redis = new Mock<IRedisService>();
            var invoiceService = new Mock<IInvoiceService>();

            var invoiceRetry = new InvoiceRetryRequestDto
            {
                InvoiceIds = executionType == "manual"
                    ? new List<string> { "bc261af6-682b-4f29-ac3d-74a1b69129fd" }
                    : new List<string>(),
                Offset = 0,
                Limit = 10,
                RequestingUser = "User Test",
            };

            redis.Setup(x => x.SetKeyIfNotExists(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()))
                .ReturnsAsync(true);

            var redisResult = new List<string>();

            if (hasRecords)
            {
                redisResult.Add("bc261af6-682b-4f29-ac3d-74a1b69129fd");
                redisResult.Add("eb3aa587-775f-43ce-ac3d-e09dd0f4bdc2");
                redisResult.Add("28d8520c-c4f7-4c2a-8df5-586adb7c0c94");
            }

            redis.Setup(x => x.ReadListAsync<string>(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(redisResult);

            var sut = new InvoiceRetryService(this.logger.Object, this.invoiceDao, redis.Object, invoiceService.Object);

            // act
            var result = await sut.RetryCreateInvoicesAsync(invoiceRetry, executionType);

            // assert
            Assert.That(result.Success, Is.True);

            if (expectedProcessed == 1)
            {
                invoiceService.Verify(x => x.PublishProcessToMediatR(It.IsAny<CreateInvoiceDto>()), Times.Once);
            }
            else if (expectedProcessed == 3)
            {
                invoiceService.Verify(x => x.PublishProcessToMediatR(It.IsAny<CreateInvoiceDto>()), Times.Exactly(3));
            }
            else
            {
                invoiceService.Verify(x => x.PublishProcessToMediatR(It.IsAny<CreateInvoiceDto>()), Times.Never);
            }
        }
    }
}
