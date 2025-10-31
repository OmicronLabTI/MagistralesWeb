// <summary>
// <copyright file="OrderServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.SapServiceLayerAdapter.Test.Services
{
    /// <summary>
    /// Class OrdersServiceTest.
    /// </summary>
    [TestFixture]
    public class OrderServiceTest : BaseTest
    {
        private Mock<ILogger> logger;

        /// <summary>
        /// Init configuration.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            this.logger = new Mock<ILogger>();
        }

        /// <summary>
        /// Method to GetOrdersHeaderStatus for dxp project.
        /// </summary>
        /// <param name="isSuccess">Is Success.</param>
        /// <param name="userError">User Error.</param>
        /// <param name="code">Code.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        [TestCase(true, null, 200)]
        [TestCase(false, "Invalid session or session already timeout.", 401)]
        public async Task GetLastGeneratedOrder(bool isSuccess, string userError, int code)
        {
            // arrange
            var resultServiceLayer = new ResultModel
            {
                Success = isSuccess,
                Response = isSuccess ?
                JsonConvert.SerializeObject(new ServiceLayerResponseDto
                {
                    Metadata = "metadata",
                    Value = JsonConvert.SerializeObject(new List<OrderDto>()),
                }) : JsonConvert.SerializeObject(new ServiceLayerErrorDetailDto()),
                UserError = userError,
                ExceptionMessage = null,
                Code = code,
                Comments = null,
            };

            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            mockServiceLayerClient
               .Setup(x => x.GetAsync(It.IsAny<string>()))
               .Returns(Task.FromResult(resultServiceLayer));

            var mockConfiguration = new Mock<IConfiguration>();
            var mockSapFile = new Mock<ISapFileService>();
            var orderServiceMock = new OrderService(mockServiceLayerClient.Object, this.logger.Object, mockConfiguration.Object, mockSapFile.Object);

            // act
            var result = await orderServiceMock.GetLastGeneratedOrder();

            // assert
            if (isSuccess)
            {
                Assert.That(result.Success);
                Assert.That(result.Code == 200);
                Assert.That(result.Response, Is.Not.Null);
                Assert.That(result.UserError, Is.Null);
                Assert.That(result.Response, Is.InstanceOf<List<OrderDto>>());
            }
            else
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.Code == 401);
                Assert.That(result.Response, Is.Not.Null);
                Assert.That(result.UserError, Is.Not.Null);
                Assert.That(result.UserError, Is.EqualTo("Invalid session or session already timeout."));
                Assert.That(result.Response, Is.InstanceOf<ServiceLayerErrorResponseDto>());
            }

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ExceptionMessage, Is.Null);
            Assert.That(result.Comments, Is.Null);
        }

        /// <summary>
        /// Close sample orders.
        /// </summary>
        /// <param name="isResponseOrderSuccess">Is Response Order Success.</param>
        /// <param name="isResponseInventoryGenExitSuccess">Is Response Inventory Gen Exit Success.</param>
        /// <param name="isCloseOrderSuccess">Is Close Order Success.</param>
        /// <param name="userError">User Error.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Test]
        [TestCase(false, true, true, "No existen registros coincidentes (ODBC -2028)")]
        [TestCase(true, false, true, "Error al crear el Inventory Gen Exit")]
        [TestCase(true, true, false, "Error al cerrar la orden")]
        [TestCase(true, true, true, null)]
        public async Task CloseSampleOrders(bool isResponseOrderSuccess, bool isResponseInventoryGenExitSuccess, bool isCloseOrderSuccess, string userError)
        {
            var sampleOrders = new List<CloseSampleOrderDto>
            {
                new ()
                {
                    SaleOrderId = 1,
                    ItemsList = new List<CreateDeliveryDto>
                    {
                        new ()
                        {
                            OrderType = "linea",
                            ItemCode = "Item Code 23",
                            Batches = new List<AlmacenBatchDto>
                            {
                                new () { BatchNumber = "BATCH1", BatchQty = 1, WarehouseCode = "PT" },
                                new () { BatchNumber = "BATCH2", BatchQty = 2.5M, WarehouseCode = "PT" },
                            },
                        },
                        new ()
                        {
                            OrderType = "magistral",
                            ItemCode = "DZ 50",
                            Batches = new List<AlmacenBatchDto>(),
                        },
                        new ()
                        {
                            OrderType = "magistral",
                            ItemCode = "FL 1",
                            Batches = new List<AlmacenBatchDto>(),
                        },
                    },
                    CloseOrder = true,
                },
            };

            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            var orderDtoMock = new OrderDto();
            if (isResponseOrderSuccess)
            {
                orderDtoMock = new OrderDto
                {
                    DocumentEntry = 3,
                    OrderLines = new List<OrderLineDto>
                    {
                        new () { ItemCode = "Item Code 23", LineNum = 0, Quantity = 1, BaseLine = 1 },
                        new () { ItemCode = "DZ 50", LineNum = 1, Quantity = 1, BaseLine = 2 },
                        new () { ItemCode = "FL 1", LineNum = 2, Quantity = 1, BaseLine = 3 },
                    },
                };
            }

            var responseOrder = GetGenericResponseModel(400, isResponseOrderSuccess, orderDtoMock, userError, null, null);
            mockServiceLayerClient
                .Setup(sl => sl.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(responseOrder));

            var closeOrderResponse = new InventoryGenExitDto() { DocEntry = 1 };
            var responseInventoryGenExit = GetGenericResponseModel(400, isResponseInventoryGenExitSuccess, closeOrderResponse, userError, null, null);
            var responseCloseOrder = GetGenericResponseModel(400, isCloseOrderSuccess, null, userError, null, null);

            mockServiceLayerClient
                .SetupSequence(sl => sl.PostAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(responseInventoryGenExit))
                .Returns(Task.FromResult(responseCloseOrder));

            var mockConfiguration = new Mock<IConfiguration>();
            var mockSapFile = new Mock<ISapFileService>();
            var orderServiceMock = new OrderService(mockServiceLayerClient.Object, this.logger.Object, mockConfiguration.Object, mockSapFile.Object);

            // act
            var result = await orderServiceMock.CloseSampleOrders(sampleOrders);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ResultModel>());
            if (!isResponseOrderSuccess)
            {
                KeyValuePair<string, string> resultDict = ((Dictionary<string, string>)result.Response).First();
                Assert.That(resultDict.Value, Is.EqualTo("Error-No se encontró la factura."));
            }
            else if (!isResponseInventoryGenExitSuccess)
            {
                KeyValuePair<string, string> resultDict = ((Dictionary<string, string>)result.Response).First();
                Assert.That(resultDict.Value, Is.EqualTo("Error-Error al crear el Inventory Gen Exit"));
            }
            else if (!isCloseOrderSuccess)
            {
                KeyValuePair<string, string> resultDict = ((Dictionary<string, string>)result.Response).First();
                Assert.That(resultDict.Value, Is.EqualTo("Error-Error al cerrar la orden"));
            }
            else
            {
                KeyValuePair<string, string> resultDict = ((Dictionary<string, string>)result.Response).First();
                Assert.That(resultDict.Value, Is.EqualTo("Ok"));
            }

            Assert.That(result.UserError, Is.Null);
            Assert.That(result.Success, Is.True);
            Assert.That(result.Response, Is.Not.Null);
            Assert.That(result.Comments, Is.Null);
        }

        /// <summary>
        /// Method to create sale order with error.
        /// </summary>
        /// <param name="isSuccess">Is Success.</param>
        /// <param name="userError">User Error.</param>
        /// <param name="code">Code.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task CreateSaleOrderWithSapFileError()
        {
            var resultSapFile = new ResultModel
            {
                Success = false,
                Response = string.Empty,
                UserError = "Error",
                ExceptionMessage = null,
                Code = 400,
                Comments = null,
            };

            var mockSapFileService = new Mock<ISapFileService>();
            mockSapFileService.
                Setup(x => x.PostAsync(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(resultSapFile));

            var mockConfiguration = new Mock<IConfiguration>();
            var mockSapFile = new Mock<ISapFileService>();
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();

            var orderServiceMock = new OrderService(mockServiceLayerClient.Object, this.logger.Object, mockConfiguration.Object, mockSapFileService.Object);

            // act
            var request = new CreateSaleOrderDto();
            request.PrescriptionUrl = "https://localhost:9090/myfile.pdf";
            var result = await orderServiceMock.CreateSaleOrder(request);

            Assert.That(result.Code, Is.EqualTo(400));
        }

        /*
        /// <summary>
        /// Method to create sale order.
        /// </summary>
        /// <param name="isSuccess">Is Success.</param>
        /// <param name="userError">User Error.</param>
        /// <param name="code">Code.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task CreateSaleOrderWithAttachmentError()
        {
            var prescriptionsUrl = new List<PrescriptionServerResponseDto>();
            var prescription = new PrescriptionServerResponseDto
            {
                ServerPrescriptionUrl = "C:\\Users\\TuUsuario\\Documentos\\documento.pdf",
                AzurePrescriptionUrl = "http://localhost:9090/server/docs/public/documento.pdf",
            };
            prescriptionsUrl.Add(prescription);

            var resultSapFile = new ResultModel
            {
                Success = true,
                Response = JsonConvert.SerializeObject(prescriptionsUrl),
                UserError = null,
                ExceptionMessage = null,
                Code = 200,
                Comments = null,
            };

            var mockSapFileService = new Mock<ISapFileService>();
            mockSapFileService
                .Setup(x => x.PostAsync(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(resultSapFile));

            var mockConfiguration = new Mock<IConfiguration>();
            var mockSapFile = new Mock<ISapFileService>();
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();

            var resultServiceLayerClient = new ResultModel
            {
                Success = false,
                Response = null,
                UserError = "Error no se pudo crear el attachment",
                ExceptionMessage = "Error no se pudo crear el attachment",
                Code = 400,
                Comments = null,
            };
            mockServiceLayerClient
                .Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(resultServiceLayerClient));

            var orderServiceMock = new OrderService(mockServiceLayerClient.Object, this.logger.Object, mockConfiguration.Object, mockSapFileService.Object);

            // act
            var request = new CreateSaleOrderDto();
            request.PrescriptionUrl = "http://localhost:9090/server/docs/public/documento.pdf";
            var result = await orderServiceMock.CreateSaleOrder(request);

            Assert.That(result.Code, Is.EqualTo(400));
            Assert.That("The attachment could not be created", Is.EqualsTo(result.UserError));
        }
        */

        /// <summary>
        /// Method to create sale order.
        /// </summary>
        /// <param name="success">Is Success.</param>
        /// <param name="code">Code.</param>
        /// <param name="userError">userError.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        [TestCase(true, 200, null)]
        [TestCase(false, 400, "Error al crear la orden")]
        public async Task CreateSaleOrder(bool success, int code, string userError)
        {
            var prescriptionsUrl = new List<PrescriptionServerResponseDto>();
            var prescription = new PrescriptionServerResponseDto
            {
                ServerSourcePath = "C:\\Users\\TuUsuario\\Documentos\\documento.pdf",
                AzurePrescriptionUrl = "http://localhost:9090/server/docs/public/documento.pdf",
                PrescriptionFileExtension = "pdf",
                PrescriptionFileName = "documento",
            };
            prescriptionsUrl.Add(prescription);

            var resultSapFile = new ResultModel
            {
                Success = true,
                Response = JsonConvert.SerializeObject(prescriptionsUrl),
                UserError = null,
                ExceptionMessage = null,
                Code = 200,
                Comments = null,
            };

            var mockSapFileService = new Mock<ISapFileService>();
            mockSapFileService
                .Setup(x => x.PostAsync(It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.FromResult(resultSapFile));

            var mockConfiguration = new Mock<IConfiguration>();
            var mockSapFile = new Mock<ISapFileService>();
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();

            var attachment = new CreateAttachmentResponseDto
            {
                AbsoluteEntry = 12,
                AttachmentLines = new List<AttachmentDto>(),
            };
            var resultServiceLayerClient = new ResultModel
            {
                Success = true,
                Response = JsonConvert.SerializeObject(attachment),
                UserError = null,
                ExceptionMessage = null,
                Code = 200,
                Comments = null,
            };

            var orderCreated = new OrderDto
            {
                DocumentEntry = 169869,
                DocumentNumber = 169869,
                DocumentDate = DateTime.Now,
                CardCode = "C03865",
                DocumentType = "dDocument_Items",
                DocumentCurrency = "MXP",
                Comments = "Comentarios sobre la orden",
                ShippingAddress = "CALLE NUMERO\rXALAPA ENRÍQUEZ CENTRO, XALAPA ,\rVeracruz, Mexico , C.P. 91000",
                Series = 9,
                TaxDate = "2024-04-04",
                ReferenceNumber = "11535444",
                BillingAddress = "ESTA ES LA CALLE 102\rXALAPA ENRÍQUEZ CENTRO, XALAPA ,\rVeracruz, Mexico , C.P. 91000",
                DueDate = DateTime.Now,
                ContactPerson = 0,
                SalesPersonCode = 36,
                Branch = null,
                CustomShippingCode = null,
                IsOmigenomics = "N",
                DocumentsOwner = 20,
                DocumentSubType = "bod_None",
                ShippingCode = "PACIENTE FORANEA",
                JournalMemo = "Pedidos de cliente - C03865",
                TypeOrder = "LN",
                PayToCode = "PRUEBA S.A DE C.V",
                TaxId = "VAL2108166I0",
                DiscountPercent = 0,
                DxpOrder = "b2de8f3b-6e7d-4435-8b66-fdaadebde990",
                EcommerceComments = string.Empty,
                BXPPaymentMethod = "PUE",
                BXPWayToPay = "04",
                OrderPackage = "N",
                DXPNeedsShipCost = "1-265.672414",
                SampleOrder = "No",
                AttachmentEntry = null,
                CFDIProvisional = "G03",
            };
            var createdOrderResult = new ResultModel
            {
                Success = success,
                Response = success ? JsonConvert.SerializeObject(orderCreated) : userError,
                UserError = userError,
                Code = code,
            };

            mockServiceLayerClient
                .SetupSequence(x => x.PostAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(resultServiceLayerClient))
                .Returns(Task.FromResult(createdOrderResult));

            mockConfiguration.SetupGet(x => x[It.Is<string>(s => s == ServiceConstants.CustomPropertyNameCFDI)]).Returns("U_CFDI_Provisional");
            var orderServiceMock = new OrderService(mockServiceLayerClient.Object, this.logger.Object, mockConfiguration.Object, mockSapFileService.Object);

            // act
            var item = new ShoppingCartItemDto
            {
                ItemCode = "REVE 14",
                Description = "Neudermic. Jabon Liquido Neutro 240 ml",
                Label = "NA",
                Container = "NA",
                Quantity = 1,
                IsLine = "N",
                NeedRecipe = "N",
                CardCode = string.Empty,
                CostPerPiece = 174.000000,
                DiscountPercentage = 0,
            };
            var request = new CreateSaleOrderDto
            {
                PrescriptionUrl = "http://localhost:9090/server/docs/public/documento.pdf",
                ShippinAddress = "CALLE NUMERO XALAPA ENRÍQUEZ CENTRO, XALAPA , Mexico , C.P. 91000",
                BillingAddress = "ESTA ES LA CALLE 102 XALAPA ENRÍQUEZ CENTRO, XALAPA ,Veracruz, Mexico , C.P. 91000",
                CardCode = "C03865",
                TransactionId = "b2de8f3b-6e7d-4435-8b66-fdaadebde990",
                PatientName = string.Empty,
                IsNamePrinted = 0,
                ShippingCost = "1-265.672414",
                CfdiValue = "G03",
                PaymentMethodSapCode = "PUE",
                WayToPaySapCode = "04",
                DiscountSpecial = 0,
                IsPackage = false,
                UserRfc = "VAL2108166I0",
                ProfecionalLicense = "11535444",
                IsSample = false,
                IsSecondary = false,
                SlpCode = 1,
                OrderComments = "Comentarios sobre la orden",
                EmployeeId = 1,
                Items = new List<ShoppingCartItemDto>(),
                ManufacturerClassificationCode = "LN",
                NeedInvoice = 1,
            };
            request.Items.Add(item);

            var result = await orderServiceMock.CreateSaleOrder(request);

            if (success)
            {
                Assert.That(result.Code, Is.EqualTo(200));
                Assert.That(result.Success, Is.True);
            }
            else
            {
                Assert.That(result.Code, Is.EqualTo(400));
                Assert.That(result.Success, Is.False);
                Assert.That(result.UserError, Is.EqualTo(userError));
            }
        }
    }
}
