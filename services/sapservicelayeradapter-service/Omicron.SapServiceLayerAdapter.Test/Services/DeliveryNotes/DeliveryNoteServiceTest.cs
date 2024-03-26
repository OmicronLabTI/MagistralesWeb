// <summary>
// <copyright file="DeliveryNoteServiceTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>
namespace Omicron.SapServiceLayerAdapter.Test.Services.DeliveryNotes
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using Omicron.SapServiceLayerAdapter.Common.DTOs.Batches;
    using Omicron.SapServiceLayerAdapter.Common.DTOs.DeliveryNotes;
    using Omicron.SapServiceLayerAdapter.Services.DeliveryNotes;
    using Serilog;

    /// <summary>
    /// Class DeliveryNoteServiceTest.
    /// </summary>
    public class DeliveryNoteServiceTest
    {
        private IServiceLayerClient serviceLayerClient;
        private ILogger logger;
        private IDeliveryNoteService deliveryNoteService;

        /// <summary>
        /// Init configuration.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            var mockLogger = new Mock<ILogger>();
            this.deliveryNoteService = new DeliveryNoteService(mockServiceLayerClient.Object, mockLogger.Object);
        }

        /// <summary>
        /// Method to create delivery with invalid order.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task CreateDeliveryWithNotFoundOrder()
        {
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            var mockLogger = new Mock<ILogger>();
            var service = new DeliveryNoteService(mockServiceLayerClient.Object, mockLogger.Object);

            var serviceLayerClientResult = new ResultModel()
            {
                Success = false,
                UserError = "Error 401",
            };

            mockServiceLayerClient
              .Setup(x => x.GetAsync(It.IsAny<string>()))
              .Returns(Task.FromResult(serviceLayerClientResult));

            var firstDelivery = new CreateDeliveryNoteDto()
            {
                SaleOrderId = 12,
                ShippingCostOrderId = 0,
                ItemCode = "REVE 14",
                OrderType = string.Empty,
                Batches = new List<AlmacenBatchDto>(),
                IsPackage = "N",
                IsOmigenomics = false,
            };

            List<CreateDeliveryNoteDto> createDelivery = new List<CreateDeliveryNoteDto>();
            createDelivery.Add(firstDelivery);

            var result = await service.CreateDelivery(createDelivery);
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.Code, 200);
        }

        /// <summary>
        /// Method to verify Get All Almacens.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task CreateDeliveryWithShipping()
        {
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            var mockLogger = new Mock<ILogger>();
            var service = new DeliveryNoteService(mockServiceLayerClient.Object, mockLogger.Object);
            var orderLines = new List<OrderLineDto>();

            var orderLine = new OrderLineDto()
            {
                ItemCode = "REVE 14",
                Quantity = 1,
                DiscountPercent = 0,
                TaxCode = string.Empty,
                LineTotal = 150,
                WarehouseCode = string.Empty,
                Container = "NA",
                Label = "NA",
                LineNum = 10,
                UnitPrice = 150,
                SalesPersonCode = 30,
            };
            var orderLineShipping = new OrderLineDto()
            {
                ItemCode = "FL 1",
                Quantity = 1,
                DiscountPercent = 0,
                TaxCode = string.Empty,
                LineTotal = 300,
                WarehouseCode = string.Empty,
                Container = "NA",
                Label = "NA",
                LineNum = 10,
                UnitPrice = 150,
                SalesPersonCode = 30,
                OwnerCode = 30,
            };
            orderLines.Add(orderLine);
            orderLines.Add(orderLineShipping);

            var saleOrder = new OrderDto()
            {
                IsOmigenomics = "N",
                CardCode = "C03580",
                SalesPersonCode = 30,
                DocumentsOwner = 0,
                BillingAddress = string.Empty,
                ShippingAddress = string.Empty,
                ShippingCode = string.Empty,
                JournalMemo = string.Empty,
                Comments = "Comentarios",
                OrderLines = orderLines,
            };

            var serviceLayerClientResult = new ResultModel()
            {
                Success = true,
                UserError = string.Empty,
                Response = JsonConvert.SerializeObject(saleOrder),
                Code = 200,
            };

            mockServiceLayerClient
              .Setup(x => x.GetAsync(It.IsAny<string>()))
              .Returns(Task.FromResult(serviceLayerClientResult));

            var patchResul = new ResultModel()
            {
                Success = true,
                UserError = string.Empty,
                Response = string.Empty,
                Code = 201,
            };
            mockServiceLayerClient.Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(patchResul));

            var createResult = new ResultModel()
            {
                Success = true,
                UserError = string.Empty,
                Response = string.Empty,
                Code = 201,
            };
            mockServiceLayerClient.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(createResult));

            var firstDelivery = new CreateDeliveryNoteDto()
            {
                SaleOrderId = 12,
                ShippingCostOrderId = 0,
                ItemCode = "REVE 14",
                OrderType = "linea",
                Batches = new List<AlmacenBatchDto>()
                {
                    new AlmacenBatchDto()
                    {
                        BatchNumber = "Axity.10",
                        BatchQty = 1,
                    },
                },
                IsPackage = "N",
                IsOmigenomics = false,
            };

            var shippingItem = new CreateDeliveryNoteDto()
            {
                SaleOrderId = 12,
                ShippingCostOrderId = 0,
                ItemCode = "FL 1",
                OrderType = "300",
                Batches = new List<AlmacenBatchDto>(),
                IsPackage = "N",
                IsOmigenomics = false,
            };

            List<CreateDeliveryNoteDto> createDelivery = new List<CreateDeliveryNoteDto>();
            createDelivery.Add(firstDelivery);
            createDelivery.Add(shippingItem);

            var result = await service.CreateDelivery(createDelivery);
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.Code, 200);
        }

        /// <summary>
        /// Method to create delivery partial with invalid order.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task CreateDeliveryPartialWithNotFoundOrder()
        {
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            var mockLogger = new Mock<ILogger>();
            var service = new DeliveryNoteService(mockServiceLayerClient.Object, mockLogger.Object);

            var serviceLayerClientResult = new ResultModel()
            {
                Success = false,
                UserError = "Error 401",
            };

            mockServiceLayerClient
              .Setup(x => x.GetAsync(It.IsAny<string>()))
              .Returns(Task.FromResult(serviceLayerClientResult));

            var firstDelivery = new CreateDeliveryNoteDto()
            {
                SaleOrderId = 12,
                ShippingCostOrderId = 0,
                ItemCode = "REVE 14",
                OrderType = string.Empty,
                Batches = new List<AlmacenBatchDto>(),
                IsPackage = "N",
                IsOmigenomics = false,
            };

            List<CreateDeliveryNoteDto> createDelivery = new List<CreateDeliveryNoteDto>();
            createDelivery.Add(firstDelivery);

            var result = await service.CreateDeliveryPartial(createDelivery);
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.Code, 200);
        }

        /// <summary>
        /// Method to verify Get All Almacens.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task CreateDeliveryPartialWithShipping()
        {
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            var mockLogger = new Mock<ILogger>();
            var service = new DeliveryNoteService(mockServiceLayerClient.Object, mockLogger.Object);
            var orderLines = new List<OrderLineDto>();

            var orderLine = new OrderLineDto()
            {
                ItemCode = "REVE 14",
                Quantity = 1,
                DiscountPercent = 0,
                TaxCode = string.Empty,
                LineTotal = 150,
                WarehouseCode = string.Empty,
                Container = "NA",
                Label = "NA",
                LineNum = 10,
                UnitPrice = 150,
                SalesPersonCode = 30,
            };
            var orderLineShipping = new OrderLineDto()
            {
                ItemCode = "FL 1",
                Quantity = 1,
                DiscountPercent = 0,
                TaxCode = string.Empty,
                LineTotal = 300,
                WarehouseCode = string.Empty,
                Container = "NA",
                Label = "NA",
                LineNum = 10,
                UnitPrice = 150,
                SalesPersonCode = 30,
                OwnerCode = 30,
            };
            orderLines.Add(orderLine);
            orderLines.Add(orderLineShipping);

            var saleOrder = new OrderDto()
            {
                IsOmigenomics = "N",
                CardCode = "C03580",
                SalesPersonCode = 30,
                DocumentsOwner = 0,
                BillingAddress = string.Empty,
                ShippingAddress = string.Empty,
                ShippingCode = string.Empty,
                JournalMemo = string.Empty,
                Comments = "Comentarios",
                OrderLines = orderLines,
            };

            var serviceLayerClientResult = new ResultModel()
            {
                Success = true,
                UserError = string.Empty,
                Response = JsonConvert.SerializeObject(saleOrder),
                Code = 200,
            };

            mockServiceLayerClient
              .Setup(x => x.GetAsync(It.IsAny<string>()))
              .Returns(Task.FromResult(serviceLayerClientResult));

            var patchResul = new ResultModel()
            {
                Success = true,
                UserError = string.Empty,
                Response = string.Empty,
                Code = 201,
            };
            mockServiceLayerClient.Setup(x => x.PatchAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(patchResul));

            var createResult = new ResultModel()
            {
                Success = true,
                UserError = string.Empty,
                Response = string.Empty,
                Code = 201,
            };
            mockServiceLayerClient.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(createResult));

            var firstDelivery = new CreateDeliveryNoteDto()
            {
                SaleOrderId = 12,
                ShippingCostOrderId = 0,
                ItemCode = "REVE 14",
                OrderType = "linea",
                Batches = new List<AlmacenBatchDto>()
                {
                    new AlmacenBatchDto()
                    {
                        BatchNumber = "Axity.10",
                        BatchQty = 1,
                    },
                },
                IsPackage = "N",
                IsOmigenomics = false,
            };

            var shippingItem = new CreateDeliveryNoteDto()
            {
                SaleOrderId = 12,
                ShippingCostOrderId = 0,
                ItemCode = "FL 1",
                OrderType = "300",
                Batches = new List<AlmacenBatchDto>(),
                IsPackage = "N",
                IsOmigenomics = false,
            };

            List<CreateDeliveryNoteDto> createDelivery = new List<CreateDeliveryNoteDto>();
            createDelivery.Add(firstDelivery);
            createDelivery.Add(shippingItem);

            var result = await service.CreateDeliveryPartial(createDelivery);
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.Code, 200);
        }

        /// <summary>
        /// Method to create delivery partial with invalid order.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task CreateDeliveryBatchWithNotFoundOrder()
        {
            var mockServiceLayerClient = new Mock<IServiceLayerClient>();
            var mockLogger = new Mock<ILogger>();
            var service = new DeliveryNoteService(mockServiceLayerClient.Object, mockLogger.Object);

            var serviceLayerClientResult = new ResultModel()
            {
                Success = false,
                UserError = "Error 401",
            };

            mockServiceLayerClient
              .Setup(x => x.GetAsync(It.IsAny<string>()))
              .Returns(Task.FromResult(serviceLayerClientResult));

            var firstDelivery = new CreateDeliveryNoteDto()
            {
                SaleOrderId = 12,
                ShippingCostOrderId = 0,
                ItemCode = "REVE 14",
                OrderType = string.Empty,
                Batches = new List<AlmacenBatchDto>(),
                IsPackage = "N",
                IsOmigenomics = false,
            };

            List<CreateDeliveryNoteDto> createDelivery = new List<CreateDeliveryNoteDto>();
            createDelivery.Add(firstDelivery);

            var result = await service.CreateDeliveryBatch(createDelivery);
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.Code, 200);
        }
    }
}