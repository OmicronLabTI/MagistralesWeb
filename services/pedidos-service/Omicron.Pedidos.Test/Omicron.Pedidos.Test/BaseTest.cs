// <summary>
// <copyright file="BaseTest.cs" company="Axity">
// This source code is Copyright Axity and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axity (www.axity.com).
// </copyright>
// </summary>

namespace Omicron.Pedidos.Test
{
    /// <summary>
    /// Class Base Test.
    /// </summary>
    public abstract class BaseTest
    {
        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public UserDto GetUserDto()
        {
            return new UserDto
            {
                Id = 10,
                FirstName = "Jorge",
                LastName = "Morales",
                Email = "test@test.com",
                Birthdate = DateTime.Now,
            };
        }

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public List<UserOrderModel> GetUserOrderModel()
        {
            var magistralQr = new MagistralQrModel
            {
                NeedsCooling = "Y",
                ProductionOrder = 100,
                Quantity = 1,
                SaleOrder = 100,
                ItemCode = "ITEM CODE 200",
            };

            var remisionQr = new RemisionQrModel
            {
                RemisionId = 100,
                NeedsCooling = true,
                PedidoId = 300,
                TotalPieces = 5,
            };

            var invoiceQr = new InvoiceQrModel
            {
                InvoiceId = 100,
                NeedsCooling = false,
            };

            var labelMuestra = new RemisionQrModel
            {
                PedidoId = 1,
                Ship = "Pedido Muestra",
            };

            return new List<UserOrderModel>
            {
                new UserOrderModel { Id = 1, Productionorderid = "100", Salesorderid = "100", Status = "Asignado", Userid = "abc", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 1 },
                new UserOrderModel { Id = 2, Productionorderid = "101", Salesorderid = "100", Status = "Proceso", Userid = "abc", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 2 },
                new UserOrderModel { Id = 3, Productionorderid = "102", Salesorderid = "100", Status = "Terminado", Userid = "abc", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 3, },
                new UserOrderModel { Id = 4, Productionorderid = "103", Salesorderid = "100", Status = "Reasignado", Userid = "abc", Comments = null, FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 4 },
                new UserOrderModel { Id = 5, Productionorderid = null, Salesorderid = "100", Status = "Terminado", Userid = "abc", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 5 },
                new UserOrderModel { Id = 6, Productionorderid = null, Salesorderid = "100", Status = "Reasignado", Userid = "abc", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 6 },
                new UserOrderModel { Id = 18, Productionorderid = "200", Salesorderid = "200", Status = "Reasignado", Userid = "abc", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 7 },
                new UserOrderModel { Id = 19, Productionorderid = "301", Salesorderid = "300", Status = "Finalizado", Userid = "abc", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 8 },
                new UserOrderModel { Id = 20, Productionorderid = null, Salesorderid = "300", Status = "Finalizado", Userid = "abc", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 9 },

                // not complet sales log
                new UserOrderModel { Id = 1000, Productionorderid = "501", Salesorderid = "800", Status = "Finalizado", Userid = "abc", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "21/03/2021", CreatorUserId = "abc", Quantity = 10 },
                new UserOrderModel { Id = 1002, Productionorderid = "502", Salesorderid = "800", Status = "Finalizado", Userid = "abc", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "21/03/2021", CreatorUserId = "abc", Quantity = 11 },
                new UserOrderModel { Id = 1003, Productionorderid = null, Salesorderid = "800", Status = "Finalizado", Userid = "abc", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "21/03/2021", CreatorUserId = "abc", Quantity = 12 },

                // Cancelled orders.
                new UserOrderModel { Id = 7, Productionorderid = null, Salesorderid = "100", Status = "Terminado", Userid = "abcd", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), Quantity = 13 },
                new UserOrderModel { Id = 8, Productionorderid = null, Salesorderid = "100", Status = "Reasignado", Userid = "abcd", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), Quantity = 14 },
                new UserOrderModel { Id = 9, Productionorderid = null, Salesorderid = "101", Status = "Asignado", Userid = "abc", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), Quantity = 15 },
                new UserOrderModel { Id = 10, Productionorderid = "104", Salesorderid = "103", Status = "Proceso", Userid = "abc", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), Quantity = 16 },
                new UserOrderModel { Id = 11, Productionorderid = "105", Salesorderid = "103", Status = "Cancelado", Userid = "abc", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), Quantity = 17 },
                new UserOrderModel { Id = 12, Productionorderid = null, Salesorderid = "103", Status = "Finalizado", Userid = "abc", FinishDate = new DateTime(2020, 8, 29), Quantity = 18 },
                new UserOrderModel { Id = 13, Productionorderid = "106", Salesorderid = "103", Status = "Finalizado", Userid = "abc", FinishDate = new DateTime(2020, 8, 29), Quantity = 19 },
                new UserOrderModel { Id = 14, Productionorderid = null, Salesorderid = "104", Status = "Terminado", Userid = "abc", FinishDate = new DateTime(2020, 8, 29), Quantity = 20 },
                new UserOrderModel { Id = 15, Productionorderid = "107", Salesorderid = "104", Status = "Terminado", Userid = "abc", FinishDate = new DateTime(2020, 8, 29), Quantity = 21 },
                new UserOrderModel { Id = 16, Productionorderid = "108", Salesorderid = "104", Status = "Cancelado", Userid = "abc", FinishDate = new DateTime(2020, 8, 29), Quantity = 22 },
                new UserOrderModel { Id = 17, Productionorderid = "109", Salesorderid = "104", Status = "Finalizado", Userid = "abc", FinishDate = new DateTime(2020, 8, 29), Quantity = 23 },

                // orders for almacen
                new UserOrderModel { Id = 98, Salesorderid = "104", Status = "Finalizado", Userid = "abc", FinishDate = new DateTime(2020, 8, 29), FinishedLabel = 1, FinalizedDate = DateTime.Now, MagistralQr = JsonConvert.SerializeObject(magistralQr), RemisionQr = JsonConvert.SerializeObject(remisionQr), Quantity = 24 },
                new UserOrderModel { Id = 99, Productionorderid = "301", Salesorderid = "104", Status = "Finalizado", Userid = "abc", FinishDate = new DateTime(2020, 8, 29), MagistralQr = JsonConvert.SerializeObject(magistralQr), RemisionQr = JsonConvert.SerializeObject(remisionQr), Quantity = 25 },

                // Orders for Qr.
                new UserOrderModel { Id = 100, Productionorderid = "300", Salesorderid = "104", Status = "Finalizado", Userid = "abc", FinishDate = new DateTime(2020, 8, 29), MagistralQr = JsonConvert.SerializeObject(magistralQr), RemisionQr = JsonConvert.SerializeObject(remisionQr), Quantity = 26 },
                new UserOrderModel { Id = 101, Productionorderid = "301", Salesorderid = "104", Status = "Finalizado", Userid = "abc", FinishDate = new DateTime(2020, 8, 29), MagistralQr = JsonConvert.SerializeObject(magistralQr), RemisionQr = JsonConvert.SerializeObject(remisionQr), Quantity = 27 },
                new UserOrderModel { Id = 102, Productionorderid = "302", Salesorderid = "105", Status = "Finalizado", Userid = "abc", FinishDate = new DateTime(2020, 8, 29), MagistralQr = JsonConvert.SerializeObject(magistralQr), RemisionQr = JsonConvert.SerializeObject(remisionQr), Quantity = 28 },
                new UserOrderModel { Id = 103, Productionorderid = "303", Salesorderid = "105", Status = "Finalizado", Userid = "abc", FinishDate = new DateTime(2020, 8, 29), MagistralQr = JsonConvert.SerializeObject(magistralQr), RemisionQr = JsonConvert.SerializeObject(remisionQr), DeliveryId = 105, Quantity = 29 },

                // orders for invoice
                new UserOrderModel { Id = 104, Productionorderid = null, Salesorderid = "106", Status = "Almacenado", Userid = "abc", FinishDate = new DateTime(2020, 8, 29), MagistralQr = JsonConvert.SerializeObject(magistralQr), RemisionQr = JsonConvert.SerializeObject(remisionQr), Quantity = 30 },
                new UserOrderModel { Id = 105, Productionorderid = "2", Salesorderid = "106", Status = "Almacenado", Userid = "abc", FinishDate = new DateTime(2020, 8, 29), MagistralQr = JsonConvert.SerializeObject(magistralQr), RemisionQr = JsonConvert.SerializeObject(remisionQr), Quantity = 31 },

                // order for invoice qr
                new UserOrderModel { Id = 106, Productionorderid = null, Salesorderid = "107", Status = "Almacenado", Userid = "abc", FinishDate = new DateTime(2020, 8, 29), InvoiceId = 100, InvoiceQr = JsonConvert.SerializeObject(invoiceQr), Quantity = 32 },

                // orders for packages
                new UserOrderModel { Id = 107, Productionorderid = null, Salesorderid = "107", Status = "Almacenado", Userid = "abc", FinishDate = new DateTime(2020, 8, 29), InvoiceId = 100, InvoiceQr = JsonConvert.SerializeObject(invoiceQr), StatusInvoice = "Empaquetado", InvoiceType = "local", Quantity = 33 },
                new UserOrderModel { Id = 108, Productionorderid = null, Salesorderid = "107", Status = "Almacenado", Userid = "abc", FinishDate = new DateTime(2020, 8, 29), InvoiceId = 101, InvoiceQr = JsonConvert.SerializeObject(invoiceQr), StatusInvoice = "Empaquetado", InvoiceType = "local", Quantity = 34 },

                // order pending for graph
                new UserOrderModel { Id = 109, Productionorderid = null, Salesorderid = "700", Status = "Liberado", FinalizedDate = DateTime.Now, Quantity = 35 },
                new UserOrderModel { Id = 110, Productionorderid = "300", Salesorderid = "700", Status = "Finalizado", FinalizedDate = DateTime.Now, FinishedLabel = 1, Quantity = 36 },
                new UserOrderModel { Id = 111, Productionorderid = "301", Salesorderid = "700", Status = "Pendiente", FinalizedDate = DateTime.Now, Quantity = 37 },
                new UserOrderModel { Id = 112, Productionorderid = "301", Salesorderid = "201", Status = "Pendiente", FinalizedDate = DateTime.Now, InvoiceId = 1, InvoiceType = "local", StatusInvoice = "Empaquetado", Quantity = 38, InvoiceStoreDate = DateTime.Now, },
                new UserOrderModel { Id = 113, Productionorderid = "301", Salesorderid = "201", Status = "Pendiente", FinalizedDate = DateTime.Now, InvoiceId = 2, InvoiceType = "local", StatusInvoice = "Asignado", Quantity = 39, InvoiceStoreDate = DateTime.Now, },
                new UserOrderModel { Id = 114, Productionorderid = "301", Salesorderid = "201", Status = "Pendiente", FinalizedDate = DateTime.Now, InvoiceId = 3, InvoiceType = "local", StatusInvoice = "En Camino", Quantity = 40 },
                new UserOrderModel { Id = 115, Productionorderid = "301", Salesorderid = "201", Status = "Pendiente", FinalizedDate = DateTime.Now, InvoiceId = 4, InvoiceType = "local", StatusInvoice = "Entregado", Quantity = 41 },
                new UserOrderModel { Id = 116, Productionorderid = "301", Salesorderid = "201", Status = "Pendiente", FinalizedDate = DateTime.Now, InvoiceId = 5, InvoiceType = "local", StatusInvoice = "No Entregado", Quantity = 42 },
                new UserOrderModel { Id = 117, Productionorderid = "301", Salesorderid = "201", Status = "Pendiente", FinalizedDate = DateTime.Now, InvoiceId = 6, InvoiceType = "foraneo", StatusInvoice = "Empaquetado", Quantity = 43 },
                new UserOrderModel { Id = 118, Productionorderid = "301", Salesorderid = "201", Status = "Pendiente", FinalizedDate = DateTime.Now, InvoiceId = 7, InvoiceType = "foraneo", StatusInvoice = "Enviado", Quantity = 44 },
                new UserOrderModel { Id = 119, Productionorderid = null, Salesorderid = "202", Status = "Recibir", StatusAlmacen = "Back Order", FinalizedDate = DateTime.Now, Quantity = 45 },
                new UserOrderModel { Id = 120, Productionorderid = null, Salesorderid = "203", Status = "Almacenado", FinalizedDate = DateTime.Now, Quantity = 46 },

                // orders DXP
                new UserOrderModel { Id = 121, Productionorderid = null, Salesorderid = "204", StatusInvoice = "Enviado", Quantity = 47 },
                new UserOrderModel { Id = 122, Productionorderid = null, Salesorderid = "205", StatusInvoice = "Entregado", Quantity = 48 },

                // orders for almacen By Id
                new UserOrderModel { Id = 123, Productionorderid = null, Salesorderid = "206", Status = "Liberado", Quantity = 47, FinishedLabel = 0 },
                new UserOrderModel { Id = 124, Productionorderid = "2", Salesorderid = "206", Status = "Finalizado", Quantity = 48, FinishedLabel = 1 },
                new UserOrderModel { Id = 125, Productionorderid = "3", Salesorderid = "206", Status = "Almacenado", Quantity = 47, FinishedLabel = 1 },
                new UserOrderModel { Id = 126, Productionorderid = "4", Salesorderid = "206", Status = "Pendiente", Quantity = 48, FinishedLabel = 0 },
                new UserOrderModel { Id = 127, Productionorderid = null, Salesorderid = "207", Status = "Finalizado", Quantity = 47, FinishedLabel = 1 },
                new UserOrderModel { Id = 128, Productionorderid = "2", Salesorderid = "207", Status = "Finalizado", Quantity = 48, FinishedLabel = 1 },

                // orders for sample lable
                new UserOrderModel { Id = 129, Productionorderid = "303", Salesorderid = "208", Status = "Finalizado", Userid = "abc", FinishDate = new DateTime(2020, 8, 29), MagistralQr = JsonConvert.SerializeObject(magistralQr), RemisionQr = JsonConvert.SerializeObject(labelMuestra), DeliveryId = 105, Quantity = 29 },

                // order for manual assign Sale order
                new UserOrderModel { Id = 130, Productionorderid = "100", Salesorderid = "1502", Status = "Asignado", Userid = "abc", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 1 },
                new UserOrderModel { Id = 131, Productionorderid = "101", Salesorderid = "1502", Status = "Planificado", Userid = "abc", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 2 },
                new UserOrderModel { Id = 132, Productionorderid = null, Salesorderid = "1502", Status = "Planificado", Userid = "abc", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 3, },

                // order for autmatic DXP
                new UserOrderModel { Id = 133, Productionorderid = "900", Salesorderid = "900", Status = "Planificado", Userid = null, Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 1 },
                new UserOrderModel { Id = 134, Productionorderid = null, Salesorderid = "900", Status = "Planificado", Userid = null, Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 2 },
                new UserOrderModel { Id = 135, Productionorderid = "901", Salesorderid = "901", Status = "Planificado", Userid = null, Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 1 },
                new UserOrderModel { Id = 136, Productionorderid = null, Salesorderid = "901", Status = "Planificado", Userid = null, Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 2 },

                // Tecnical id
                new UserOrderModel { Id = 137, Productionorderid = null, Salesorderid = "901", Status = "Planificado", Userid = "abc",  TecnicId = "tecnial", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 2 },
                new UserOrderModel { Id = 138, Productionorderid = null, Salesorderid = "902", Status = "Proceso", Userid = "abcquimico",  TecnicId = "tecnicoqfb", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 2, StatusForTecnic = "Asignado" },
                new UserOrderModel { Id = 139, Productionorderid = null, Salesorderid = "903", Status = "Asignado", Userid = "abcquimicocd",  TecnicId = "tecnicoqfb2", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 1, StatusForTecnic = "Asignado" },

                // Test For signed orders
                new UserOrderModel { Id = 140, Productionorderid = "223740", Salesorderid = "901", Status = "Planificado", Userid = "abc",  TecnicId = null, Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 2 },
                new UserOrderModel { Id = 141, Productionorderid = "224212", Salesorderid = "902", Status = "Proceso", Userid = "abcquimico",  TecnicId = "tecnicoqfb", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 2, StatusForTecnic = "Asignado" },
                new UserOrderModel { Id = 142, Productionorderid = "224211", Salesorderid = "903", Status = "Asignado", Userid = "abcquimicocd",  TecnicId = "tecnicoqfb2", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 1, StatusForTecnic = "Asignado" },
                new UserOrderModel { Id = 143, Productionorderid = "224159", Salesorderid = "903", Status = "Asignado", Userid = "abcquimicocd",  TecnicId = "tecnicoqfb2", Comments = "Hello", FinishDate = new DateTime(2020, 8, 29), CloseDate = new DateTime(2020, 8, 28), CloseUserId = "abc", CreationDate = "28/08/2020", CreatorUserId = "abc", Quantity = 1, StatusForTecnic = "Asignado" },

                new UserOrderModel { Id = 144, Salesorderid = "104", Productionorderid = "5599", Status = "Finalizado", Userid = "abc", FinishDate = new DateTime(2020, 8, 29), FinishedLabel = 1, FinalizedDate = DateTime.Now, MagistralQr = JsonConvert.SerializeObject(magistralQr), RemisionQr = JsonConvert.SerializeObject(remisionQr), Quantity = 24, InvoiceId = 1, InvoiceLineNum = 1 },

                // UserOrderModel for fabOrders
                new UserOrderModel { Id = 145, Userid = "8df154e0-5061-4749-b06e-6bd3a1aebef8", Salesorderid = "175623", Productionorderid = "226274",  Status = "Proceso", FinishDate = new DateTime(2025, 5, 28), FinishedLabel = 1, FinalizedDate = new DateTime(2025, 5, 28), MagistralQr = JsonConvert.SerializeObject(magistralQr), RemisionQr = JsonConvert.SerializeObject(remisionQr), Quantity = 24, InvoiceId = 1, InvoiceLineNum = 1 },
                new UserOrderModel { Id = 146, Userid = "8df154e0-5061-4749-b06e-6bd3a1aebef8", Salesorderid = "175624", Productionorderid = "226275",  Status = "Proceso", FinishDate = new DateTime(2025, 5, 28), FinishedLabel = 1, FinalizedDate = new DateTime(2025, 5, 28), MagistralQr = JsonConvert.SerializeObject(magistralQr), RemisionQr = JsonConvert.SerializeObject(remisionQr), Quantity = 24, InvoiceId = 1, InvoiceLineNum = 1 },
                new UserOrderModel { Id = 147, Userid = "8df154e0-5061-4749-b06e-6bd3a1aebef8", Salesorderid = "175625", Productionorderid = "226276",  Status = "Proceso", FinishDate = new DateTime(2025, 5, 28), FinishedLabel = 1, FinalizedDate = new DateTime(2025, 5, 28), MagistralQr = JsonConvert.SerializeObject(magistralQr), RemisionQr = JsonConvert.SerializeObject(remisionQr), Quantity = 24, InvoiceId = 1, InvoiceLineNum = 1 },
                new UserOrderModel { Id = 148, Userid = "8df154e0-5061-4749-b06e-6bd3a1aebef8", Salesorderid = "175626", Productionorderid = "226277",  Status = "Proceso", FinishDate = new DateTime(2025, 5, 28), FinishedLabel = 1, FinalizedDate = new DateTime(2025, 5, 28), MagistralQr = JsonConvert.SerializeObject(magistralQr), RemisionQr = JsonConvert.SerializeObject(remisionQr), Quantity = 24, InvoiceId = 1, InvoiceLineNum = 1 },

                // UserOrderModel for QfbOrders parents
                new UserOrderModel { Id = 149, Userid = "1a663b91-fffa-4298-80c3-aaae35586dc6", Salesorderid = "171904", Productionorderid = "225305",  Status = "Cancelado", FinishDate = new DateTime(2025, 5, 28), FinishedLabel = 1, FinalizedDate = new DateTime(2025, 5, 28), MagistralQr = JsonConvert.SerializeObject(magistralQr), RemisionQr = JsonConvert.SerializeObject(remisionQr), Quantity = 1, InvoiceId = 1, InvoiceLineNum = 1, ReassignmentDate = new DateTime(2025, 5, 28) },
                new UserOrderModel { Id = 150, Userid = "1a663b91-fffa-4298-80c3-aaae35586dc6", Salesorderid = "171905", Productionorderid = "225306",  Status = "Cancelado", FinishDate = new DateTime(2025, 5, 28), FinishedLabel = 1, FinalizedDate = new DateTime(2025, 5, 28), MagistralQr = JsonConvert.SerializeObject(magistralQr), RemisionQr = JsonConvert.SerializeObject(remisionQr), Quantity = 1, InvoiceId = 1, InvoiceLineNum = 1 },
                new UserOrderModel { Id = 151, Userid = "1a663b91-fffa-4298-80c3-aaae35586dc6", Salesorderid = "171906", Productionorderid = "225307",  Status = "Cancelado", FinishDate = new DateTime(2025, 5, 28), FinishedLabel = 1, FinalizedDate = new DateTime(2025, 5, 28), MagistralQr = JsonConvert.SerializeObject(magistralQr), RemisionQr = JsonConvert.SerializeObject(remisionQr), Quantity = 1, InvoiceId = 1, InvoiceLineNum = 1, ReassignmentDate = new DateTime(2025, 5, 28) }, // completamente dividida
                new UserOrderModel { Id = 152, Userid = "1a663b91-fffa-4298-80c3-aaae35586dc6", Salesorderid = "171907", Productionorderid = "225308",  Status = "Cancelado", FinishDate = new DateTime(2025, 5, 28), FinishedLabel = 1, FinalizedDate = new DateTime(2025, 5, 28), MagistralQr = JsonConvert.SerializeObject(magistralQr), RemisionQr = JsonConvert.SerializeObject(remisionQr), Quantity = 1, InvoiceId = 1, InvoiceLineNum = 1 }, // completamente dividida

                // UserOrderModel for QfbOrders parents ordering
                new UserOrderModel { Id = 153, Userid = "bd4b2724-3b13-490e-aed2-5c8bfdd7551a", Salesorderid = "171908", Productionorderid = "225309",  Status = "Proceso", FinishDate = new DateTime(2025, 5, 28), FinishedLabel = 1, FinalizedDate = new DateTime(2025, 5, 28), MagistralQr = JsonConvert.SerializeObject(magistralQr), RemisionQr = JsonConvert.SerializeObject(remisionQr), Quantity = 1, InvoiceId = 1, InvoiceLineNum = 1, AreBatchesComplete = 1 }, // completa con batches
                new UserOrderModel { Id = 154, Userid = "bd4b2724-3b13-490e-aed2-5c8bfdd7551a", Salesorderid = "171909", Productionorderid = "225310",  Status = "Proceso", FinishDate = new DateTime(2025, 5, 28), FinishedLabel = 1, FinalizedDate = new DateTime(2025, 5, 28), MagistralQr = JsonConvert.SerializeObject(magistralQr), RemisionQr = JsonConvert.SerializeObject(remisionQr), Quantity = 1, InvoiceId = 1, InvoiceLineNum = 1, AreBatchesComplete = 1 }, // hija con batches
                new UserOrderModel { Id = 155, Userid = "bd4b2724-3b13-490e-aed2-5c8bfdd7551a", Salesorderid = "171910", Productionorderid = "225311",  Status = "Cancelado", FinishDate = new DateTime(2025, 5, 28), FinishedLabel = 1, FinalizedDate = new DateTime(2025, 5, 28), MagistralQr = JsonConvert.SerializeObject(magistralQr), RemisionQr = JsonConvert.SerializeObject(remisionQr), Quantity = 1, InvoiceId = 1, InvoiceLineNum = 1, AreBatchesComplete = 0 }, // padre
                new UserOrderModel { Id = 156, Userid = "bd4b2724-3b13-490e-aed2-5c8bfdd7551a", Salesorderid = "171911", Productionorderid = "225312",  Status = "Proceso", FinishDate = new DateTime(2025, 5, 28), FinishedLabel = 1, FinalizedDate = new DateTime(2025, 5, 28), MagistralQr = JsonConvert.SerializeObject(magistralQr), RemisionQr = JsonConvert.SerializeObject(remisionQr), Quantity = 1, InvoiceId = 1, InvoiceLineNum = 1, AreBatchesComplete = 0 }, // completa sin batches
                new UserOrderModel { Id = 157, Userid = "bd4b2724-3b13-490e-aed2-5c8bfdd7551a", Salesorderid = "171912", Productionorderid = "225313",  Status = "Proceso", FinishDate = new DateTime(2025, 5, 28), FinishedLabel = 1, FinalizedDate = new DateTime(2025, 5, 28), MagistralQr = JsonConvert.SerializeObject(magistralQr), RemisionQr = JsonConvert.SerializeObject(remisionQr), Quantity = 1, InvoiceId = 1, InvoiceLineNum = 1, AreBatchesComplete = 0 }, // hija sin batches
            };
        }

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public List<UserOrderModel> GetUserModelsForPackangignCancelation()
        {
            var magistralQr = new MagistralQrModel
            {
                NeedsCooling = "Y",
                ProductionOrder = 100,
                Quantity = 1,
                SaleOrder = 100,
                ItemCode = "REVE 14",
            };

            var remisionQr = new RemisionQrModel
            {
                RemisionId = 100,
                NeedsCooling = true,
                PedidoId = 300,
                TotalPieces = 5,
            };

            return new List<UserOrderModel>
            {
                new UserOrderModel { Id = 10008000, Productionorderid = "304", Salesorderid = "701", Status = "Almacenado", Userid = "abc-123", FinishDate = new DateTime(2020, 8, 29), MagistralQr = JsonConvert.SerializeObject(magistralQr), RemisionQr = JsonConvert.SerializeObject(remisionQr), DeliveryId = 15800, Quantity = 29, InvoiceId = 15700 },
            };
        }

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public List<UserOrderModel> GetUserModelsForTotalCancellationInformation()
        {
            var magistralQr = new MagistralQrModel
            {
                NeedsCooling = "Y",
                ProductionOrder = 100,
                Quantity = 1,
                SaleOrder = 100,
                ItemCode = "REVE 14",
            };

            var remisionQr = new RemisionQrModel
            {
                RemisionId = 100,
                NeedsCooling = true,
                PedidoId = 300,
                TotalPieces = 5,
            };

            return new List<UserOrderModel>
            {
                // Remision completa
                new UserOrderModel { Id = 10008001, Productionorderid = "304", Salesorderid = "246", Status = "Almacenado", Userid = "abc-123", FinishDate = new DateTime(2020, 8, 29), MagistralQr = JsonConvert.SerializeObject(magistralQr), RemisionQr = JsonConvert.SerializeObject(remisionQr), DeliveryId = 150158, Quantity = 29, InvoiceId = 15700, StatusAlmacen = "Almacenado" },
                new UserOrderModel { Id = 10008002, Productionorderid = "304", Salesorderid = "246", Status = "Almacenado", Userid = "abc-123", FinishDate = new DateTime(2020, 8, 29), MagistralQr = JsonConvert.SerializeObject(magistralQr), RemisionQr = JsonConvert.SerializeObject(remisionQr), DeliveryId = 150158, Quantity = 29, InvoiceId = 15700, StatusAlmacen = "Almacenado" },
                new UserOrderModel { Id = 10008003, Productionorderid = null, Salesorderid = "246", Status = "Almacenado", Userid = "abc-123", FinishDate = new DateTime(2020, 8, 29), MagistralQr = JsonConvert.SerializeObject(magistralQr), RemisionQr = JsonConvert.SerializeObject(remisionQr), DeliveryId = 150158, Quantity = 29, InvoiceId = 15700, StatusAlmacen = "Almacenado" },

                // Remision parcial
                new UserOrderModel { Id = 10008004, Productionorderid = "304", Salesorderid = "701", Status = "Almacenado", Userid = "abc-123", FinishDate = new DateTime(2020, 8, 29), MagistralQr = JsonConvert.SerializeObject(magistralQr), RemisionQr = JsonConvert.SerializeObject(remisionQr), DeliveryId = 150160, Quantity = 29, InvoiceId = 15700, StatusAlmacen = "Almacenado" },
                new UserOrderModel { Id = 10008005, Productionorderid = "304", Salesorderid = "701", Status = "Almacenado", Userid = "abc-123", FinishDate = new DateTime(2020, 8, 29), MagistralQr = JsonConvert.SerializeObject(magistralQr), RemisionQr = JsonConvert.SerializeObject(remisionQr), DeliveryId = 150160, Quantity = 29, InvoiceId = 15700, StatusAlmacen = "Almacenado" },
                new UserOrderModel { Id = 10008006, Productionorderid = null, Salesorderid = "701", Status = "Almacenado", Userid = "abc-123", FinishDate = new DateTime(2020, 8, 29), MagistralQr = JsonConvert.SerializeObject(magistralQr), RemisionQr = JsonConvert.SerializeObject(remisionQr), DeliveryId = 150159, Quantity = 29, InvoiceId = 15700, StatusAlmacen = "Back Order" },
            };
        }

        /// <summary>
        /// retruns a list od completedetailorder.
        /// </summary>
        /// <returns>the data.</returns>
        public List<OrderWithDetailModel> GetOrderWithDetailModel()
        {
            var listDetalles = new List<CompleteDetailOrderModel>
            {
                new CompleteDetailOrderModel { CodigoProducto = "Aspirina", DescripcionProducto = "dec", FechaOf = "2020/01/01", FechaOfFin = "2020/01/01", IsChecked = false, OrdenFabricacionId = 100, Qfb = "qfb", QtyPlanned = 1, QtyPlannedDetalle = 1, Status = null },
            };

            var listOrders = new List<OrderWithDetailModel>
            {
                new OrderWithDetailModel
                {
                    Detalle = new List<CompleteDetailOrderModel>(listDetalles),
                    Order = new OrderModel { AsesorId = 2, Cliente = "C", Codigo = "C", DocNum = 1, FechaFin = DateTime.Now, FechaInicio = DateTime.Now, Medico = "M", PedidoId = 100, PedidoStatus = null },
                },
                new OrderWithDetailModel
                {
                    Detalle = new List<CompleteDetailOrderModel>(listDetalles),
                    Order = new OrderModel { AsesorId = 2, Cliente = "C", Codigo = "C", DocNum = 100, FechaFin = DateTime.Now, FechaInicio = DateTime.Now, Medico = "M", PedidoId = 100, PedidoStatus = null },
                },
            };

            return listOrders;
        }

        /// <summary>
        /// Gets the signature.
        /// </summary>
        /// <returns>the data.</returns>
        public List<UserOrderSignatureModel> GetSignature()
        {
            return new List<UserOrderSignatureModel>
            {
                new UserOrderSignatureModel { Id = 1000, LogisticSignature = null, TechnicalSignature = null, UserOrderId = 1 },
                new UserOrderSignatureModel { Id = 1, LogisticSignature = null, TechnicalSignature = Convert.FromBase64String("aG9sYQ=="), UserOrderId = 142 },
                new UserOrderSignatureModel { Id = 2, LogisticSignature = null, TechnicalSignature = null, UserOrderId = 143 },
            };
        }

        /// <summary>
        /// Gets the signature.
        /// </summary>
        /// <returns>the data.</returns>
        public List<ProductionOrderSeparationModel> GetProductionOrderSeparation()
        {
            return new List<ProductionOrderSeparationModel>
            {
                new ProductionOrderSeparationModel { Id = 1,  OrderId = 225307, ProductionDetailCount = 2, TotalPieces = 10, AvailablePieces = 0, Status = "Completamente dividida" },
                new ProductionOrderSeparationModel { Id = 2,  OrderId = 225308, ProductionDetailCount = 2, TotalPieces = 10, AvailablePieces = 0, Status = "Completamente dividida" },
            };
        }

        /// <summary>
        /// Gets the ProductionOrderSeparationDetailModel.
        /// </summary>
        /// <returns>the data.</returns>
        public List<ProductionOrderSeparationDetailModel> GetProductionOrderSeparationDetailModel()
        {
            return new List<ProductionOrderSeparationDetailModel>
            {
                new ProductionOrderSeparationDetailModel { Id = 1, DetailOrderId = 226274, OrderId = 226270, UserId = "user", SapOrder = 5 },
            };
        }

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public ResultModel GetResultModelGetFabricacionModel()
        {
            var listOrders = new List<FabricacionOrderModel>
            {
                new FabricacionOrderModel { DataSource = "O", OrdenId = 100, PedidoId = 100, PostDate = DateTime.Now, ProductoId = "Aspirina", Quantity = 10, Status = "R" },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = JsonConvert.SerializeObject(listOrders),
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public ResultModel GetResultModelGetFabricacionModelNoSerealize()
        {
            var listOrders = new List<FabricacionOrderModel>
            {
                new FabricacionOrderModel { DataSource = "O", OrdenId = 100, PedidoId = 100, PostDate = DateTime.Now, ProductoId = "Aspirina", Quantity = 10, Status = "R" },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = listOrders,
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public ResultModel GetResultModelCompleteDetailModel()
        {
            var listDetalles = new List<CompleteDetailOrderModel>
            {
                new CompleteDetailOrderModel
                {
                    CodigoProducto = "Aspirina",
                    DescripcionProducto = "dec",
                    FechaOf = "2020/01/01",
                    FechaOfFin = "2020/01/01",
                    IsChecked = false,
                    OrdenFabricacionId = 100,
                    Qfb = "qfb",
                    QtyPlanned = 1,
                    QtyPlannedDetalle = 1,
                    Status = "L",
                    CreatedDate = DateTime.Now,
                    Label = "Pesonalizada",
                    IsOmigenomics = false,
                    ProductFirmName = string.Empty,
                },
            };

            var listOrders = new List<OrderWithDetailModel>
            {
                new OrderWithDetailModel
                {
                    Detalle = new List<CompleteDetailOrderModel>(listDetalles),
                    Order = new OrderModel { AsesorId = 2, Cliente = "C", Codigo = "C", DocNum = 1, FechaFin = DateTime.Now, FechaInicio = DateTime.Now, Medico = "M", PedidoId = 100, PedidoStatus = "L", OrderType = "MN" },
                },
                new OrderWithDetailModel
                {
                    Detalle = new List<CompleteDetailOrderModel>(listDetalles),
                    Order = new OrderModel { AsesorId = 2, Cliente = "C", Codigo = "C", DocNum = 100, FechaFin = DateTime.Now, FechaInicio = DateTime.Now, Medico = "M", PedidoId = 100, PedidoStatus = "L", OrderType = "MG" },
                },
                new OrderWithDetailModel
                {
                    Detalle = new List<CompleteDetailOrderModel>(listDetalles),
                    Order = new OrderModel { AsesorId = 2, Cliente = "C", Codigo = "C", DocNum = 101, FechaFin = DateTime.Now, FechaInicio = DateTime.Now, Medico = "M", PedidoId = 100, PedidoStatus = "L", OrderType = "MX" },
                },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = listOrders,
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public ResultModel GetResultModelCompleteDetailDZModel()
        {
            var listDetailDZ = Enumerable.Range(1, 1)
                .Select(detail => new CompleteDetailOrderModel
                {
                    CodigoProducto = $"DZ Test {detail}",
                    DescripcionProducto = "dec",
                    FechaOf = "2020/01/01",
                    FechaOfFin = "2020/01/01",
                    IsChecked = false,
                    OrdenFabricacionId = 100,
                    Qfb = "qfb",
                    QtyPlanned = 1,
                    QtyPlannedDetalle = 1,
                    Status = "L",
                    CreatedDate = DateTime.Now,
                    Label = "Pesonalizada",
                    IsOmigenomics = false,
                    ProductFirmName = string.Empty,
                });

            var listOrders = Enumerable.Range(1, 7)
                .Select(x =>
                new OrderWithDetailModel
                {
                    Detalle = new List<CompleteDetailOrderModel>(listDetailDZ),
                    Order = new OrderModel { AsesorId = 2, Cliente = "C", Codigo = "C", DocNum = x, FechaFin = DateTime.Now, FechaInicio = DateTime.Now, Medico = "M", PedidoId = x, PedidoStatus = "L", OrderType = "MG" },
                }).ToList();

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = listOrders,
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// gets the recipes.
        /// </summary>
        /// <returns>the data.</returns>
        public ResultModel GetRecipes()
        {
            var recipes = new List<OrderRecipeModel>
            {
                new OrderRecipeModel { Order = 107, Recipe = "C:aglo" },
                new OrderRecipeModel { Order = 100, Recipe = "C:aglo" },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = recipes,
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// Gets the fabrication orders model.
        /// </summary>
        /// <returns>the data.</returns>
        public ResultModel GetFabricacionOrderModel()
        {
            var listData = new List<FabricacionOrderModel>
            {
                new FabricacionOrderModel { CreatedDate = DateTime.Now, OrdenId = 100, ProductoId = "Aspirina" },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = listData,
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public ResultModel GetResultCreateOrder()
        {
            var listOrders = new Dictionary<string, string>
            {
                { "100-Aspirina-1-100", ServiceConstants.Ok },
                { "200-Aspirina", $"{ServiceConstants.ErrorCreateFabOrd}-404-Error en la cantidad" },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = JsonConvert.SerializeObject(listOrders),
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public ResultModel GetResultFinalizeProductionOrdersAsync()
        {
            var listResult = new FinalizeProductionOrdersResult
            {
                Successful = new List<FinalizeProductionOrderModel>
                {
                    new FinalizeProductionOrderModel { UserId = "Prueba", ProductionOrderId = 123, SourceProcess = "pruebas", Batches = new List<BatchesConfigurationModel>() },
                },
                Failed = new List<ProductionOrderFailedResultModel>
                {
                    new ProductionOrderFailedResultModel { UserId = "Prueba", OrderId = 123, Reason = "prueba falla" },
                },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = listResult,
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <param name="isTecnic">Is tecnic.</param>
        /// <returns>the user.</returns>
        public ResultModel GetResultUserModel(bool isTecnic = false)
        {
            List<UserModel> listUsers;
            if (isTecnic)
            {
                listUsers = new List<UserModel>
                {
                    new UserModel { Activo = 1, FirstName = "Sutano", Id = "tecnic", LastName = "Lope", Password = "as", Role = 9, UserName = "sutan", Piezas = 1000, Asignable = 1 },
                };
            }
            else
            {
                listUsers = new List<UserModel>
                {
                    new UserModel { Activo = 1, FirstName = "Sutano", Id = "abc", LastName = "Lope", Password = "as", Role = 1, UserName = "sutan", Piezas = 1000, Asignable = 1, TechnicalRequire = true },
                };
            }

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = JsonConvert.SerializeObject(listUsers),
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <param name="isTecnic">Is tecnic.</param>
        /// <returns>the user.</returns>
        public ResultModel GetUserModel()
        {
            var listUsers = new List<UserModel>
            {
                new UserModel { Activo = 1, FirstName = "Sutano", Id = "8df154e0-5061-4749-b06e-6bd3a1aebef8", LastName = "Lope", Password = "as", Role = 1, UserName = "sutan", Piezas = 1000, Asignable = 1, TechnicalRequire = true },
                new UserModel { Activo = 1, FirstName = "usurio", Id = "08faf89b-2f56-47bb-83da-895ba965fad4", LastName = "prueba", Password = "as12", Role = 1, UserName = "user123", Piezas = 1000, Asignable = 1, TechnicalRequire = true },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = JsonConvert.SerializeObject(listUsers),
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public ResultModel GetResultAssignBatch()
        {
            var listOrders = new Dictionary<string, string>
            {
                { "100-Aspirina-101", ServiceConstants.Ok },
                { "200-Aspirina-023", ServiceConstants.ErrorUpdateFabOrd },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = JsonConvert.SerializeObject(listOrders),
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public ResultModel GetBatches()
        {
            var assigneBatches = new List<AssignedBatches>
            {
                new AssignedBatches { CantidadSeleccionada = 10, NumeroLote = "asd", SysNumber = 1 },
            };

            var listOrders = new List<BatchesComponentModel>
            {
                new BatchesComponentModel { CodigoProducto = "asd", LotesAsignados = assigneBatches },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = JsonConvert.SerializeObject(listOrders),
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public ResultModel GetFabricacionOrderModelClassifications()
        {
            var listData = new List<FabricacionOrderModel>
            {
                new FabricacionOrderModel { OrdenId = 226274, PedidoId = 175623, Quantity = 20, ProductoId = "567   30 ML", OrderType = "MG", OrderRelationType = "N" },
                new FabricacionOrderModel { OrdenId = 226275, PedidoId = 175624, Quantity = 20, ProductoId = "567   30 ML", OrderType = "MN", OrderRelationType = "N" },
                new FabricacionOrderModel { OrdenId = 226276, PedidoId = 175625, Quantity = 20, ProductoId = "567   30 ML", OrderType = "BE", OrderRelationType = "N" },
                new FabricacionOrderModel { OrdenId = 226277, PedidoId = null, Quantity = 20, ProductoId = "567   30 ML", OrderType = null, OrderRelationType = "Y" },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = JsonConvert.SerializeObject(listData),
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public ResultModel GetFabricacionOrderModelOnlyParentsOrComplete()
        {
            var listData = new List<FabricacionOrderModel>
            {
                new FabricacionOrderModel { OrdenId = 226277, PedidoId = null, Quantity = 20, ProductoId = "567   30 ML", OrderType = null, OrderRelationType = "Y" },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = JsonConvert.SerializeObject(listData),
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public ResultModel GetMissingBatches()
        {
            var listOrders = new List<BatchesComponentModel>
            {
                new BatchesComponentModel { CodigoProducto = "asd", LotesAsignados = new List<AssignedBatches>() },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = JsonConvert.SerializeObject(listOrders),
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public ResultModel GetResultUpdateOrder()
        {
            var listOrders = new Dictionary<string, string>
            {
                { "100-100", ServiceConstants.Ok },
                { "200-200", ServiceConstants.ErrorUpdateFabOrd },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = JsonConvert.SerializeObject(listOrders),
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// the values for the formulas.
        /// </summary>
        /// <returns>the data.</returns>
        public ResultModel GetFormulaDetalle()
        {
            var listFormula = new CompleteFormulaWithDetalle { BaseDocument = 100, Client = "C001", Code = "Aspirina", Container = "container", CompleteQuantity = 10, Details = new List<CompleteDetalleFormulaModel>(), DueDate = "01/01/2020", EndDate = "01/01/2020", FabDate = "01/01/2020", IsChecked = false, Number = 100, Origin = "PT", PlannedQuantity = 100, ProductDescription = "orden", ProductionOrderId = 100, ProductLabel = "label", RealEndDate = "01/01/2020", StartDate = "01/01/2020", Status = "L", Type = "type", Unit = "KG", User = "manager", Warehouse = "MN", DestinyAddress = "Nuevo Le?n, Mexico, CP. 54715", Comments = "Cooments" };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = listFormula,
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// the values for the formulas.
        /// </summary>
        /// <returns>the data.</returns>
        public ResultModel GetListFormulaDetalle()
        {
            var listFormula = new List<CompleteFormulaWithDetalle>
            {
                new CompleteFormulaWithDetalle { BaseDocument = 100, Client = "C001", Code = "Aspirina", Container = "container", CompleteQuantity = 10, Details = new List<CompleteDetalleFormulaModel>(), DueDate = "01/01/2020", EndDate = "01/01/2020", FabDate = "01/01/2020", IsChecked = false, Number = 100, Origin = "PT", PlannedQuantity = 100, ProductDescription = "orden", ProductionOrderId = 100, ProductLabel = "label", RealEndDate = "01/01/2020", StartDate = "01/01/2020", Status = "L", Type = "type", Unit = "KG", User = "manager", Warehouse = "MN", DestinyAddress = "Nuevo Le?n, Mexico, CP. 54715", Comments = "Cooments", HasMissingStock = false },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = listFormula,
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// the values for the formulas.
        /// </summary>
        /// <returns>the data.</returns>
        public ResultModel GetListFormulaDetalleForOrdersParent()
        {
            var listFormula = new List<CompleteFormulaWithDetalle>
            {
                new CompleteFormulaWithDetalle { ProductionOrderId = 225305, BaseDocument = 171904, Client = "C001", Code = "Aspirina", Container = "container", CompleteQuantity = 1, Details = new List<CompleteDetalleFormulaModel>(), DueDate = "01/01/2020", EndDate = "01/01/2020", FabDate = "01/01/2020", IsChecked = false, Number = 100, Origin = "PT", PlannedQuantity = 1, ProductDescription = "orden", ProductLabel = "label", RealEndDate = "01/01/2020", StartDate = "01/01/2020", Status = "L", Type = "type", Unit = "KG", User = "manager", Warehouse = "MN", DestinyAddress = "Nuevo Le?n, Mexico, CP. 54715", Comments = "Cooments", HasMissingStock = false, OrderRelationType = "Padre" },
                new CompleteFormulaWithDetalle { ProductionOrderId = 225306, BaseDocument = 171905, Client = "C001", Code = "Aspirina", Container = "container", CompleteQuantity = 1, Details = new List<CompleteDetalleFormulaModel>(), DueDate = "01/01/2020", EndDate = "01/01/2020", FabDate = "01/01/2020", IsChecked = false, Number = 100, Origin = "PT", PlannedQuantity = 1, ProductDescription = "orden", ProductLabel = "label", RealEndDate = "01/01/2020", StartDate = "01/01/2020", Status = "L", Type = "type", Unit = "KG", User = "manager", Warehouse = "MN", DestinyAddress = "Nuevo Le?n, Mexico, CP. 54715", Comments = "Cooments", HasMissingStock = false, OrderRelationType = "Padre" },
                new CompleteFormulaWithDetalle { ProductionOrderId = 225307, BaseDocument = 171906, Client = "C001", Code = "Aspirina", Container = "container", CompleteQuantity = 1, Details = new List<CompleteDetalleFormulaModel>(), DueDate = "01/01/2020", EndDate = "01/01/2020", FabDate = "01/01/2020", IsChecked = false, Number = 100, Origin = "PT", PlannedQuantity = 1, ProductDescription = "orden", ProductLabel = "label", RealEndDate = "01/01/2020", StartDate = "01/01/2020", Status = "L", Type = "type", Unit = "KG", User = "manager", Warehouse = "MN", DestinyAddress = "Nuevo Le?n, Mexico, CP. 54715", Comments = "Cooments", HasMissingStock = false, OrderRelationType = "Padre" },
                new CompleteFormulaWithDetalle { ProductionOrderId = 225308, BaseDocument = 171907, Client = "C001", Code = "Aspirina", Container = "container", CompleteQuantity = 1, Details = new List<CompleteDetalleFormulaModel>(), DueDate = "01/01/2020", EndDate = "01/01/2020", FabDate = "01/01/2020", IsChecked = false, Number = 100, Origin = "PT", PlannedQuantity = 1, ProductDescription = "orden", ProductLabel = "label", RealEndDate = "01/01/2020", StartDate = "01/01/2020", Status = "L", Type = "type", Unit = "KG", User = "manager", Warehouse = "MN", DestinyAddress = "Nuevo Le?n, Mexico, CP. 54715", Comments = "Cooments", HasMissingStock = false, OrderRelationType = "Padre" },

                // for ordering
                new CompleteFormulaWithDetalle { ProductionOrderId = 225309, BaseDocument = 171908, Client = "C001", Code = "Aspirina", Container = "container", CompleteQuantity = 1, Details = new List<CompleteDetalleFormulaModel>(), DueDate = "01/01/2020", EndDate = "01/01/2020", FabDate = "01/01/2020", IsChecked = false, Number = 100, Origin = "PT", PlannedQuantity = 1, ProductDescription = "orden", ProductLabel = "label", RealEndDate = "01/01/2020", StartDate = "01/01/2020", Status = "L", Type = "type", Unit = "KG", User = "manager", Warehouse = "MN", DestinyAddress = "Nuevo Le?n, Mexico, CP. 54715", Comments = "Cooments", HasMissingStock = false, OrderRelationType = "Completa" },
                new CompleteFormulaWithDetalle { ProductionOrderId = 225310, BaseDocument = 171909, Client = "C001", Code = "Aspirina", Container = "container", CompleteQuantity = 1, Details = new List<CompleteDetalleFormulaModel>(), DueDate = "01/01/2020", EndDate = "01/01/2020", FabDate = "01/01/2020", IsChecked = false, Number = 100, Origin = "PT", PlannedQuantity = 1, ProductDescription = "orden", ProductLabel = "label", RealEndDate = "01/01/2020", StartDate = "01/01/2020", Status = "L", Type = "type", Unit = "KG", User = "manager", Warehouse = "MN", DestinyAddress = "Nuevo Le?n, Mexico, CP. 54715", Comments = "Cooments", HasMissingStock = false, OrderRelationType = "Hija" },
                new CompleteFormulaWithDetalle { ProductionOrderId = 225311, BaseDocument = 171910, Client = "C001", Code = "Aspirina", Container = "container", CompleteQuantity = 1, Details = new List<CompleteDetalleFormulaModel>(), DueDate = "01/01/2020", EndDate = "01/01/2020", FabDate = "01/01/2020", IsChecked = false, Number = 100, Origin = "PT", PlannedQuantity = 1, ProductDescription = "orden", ProductLabel = "label", RealEndDate = "01/01/2020", StartDate = "01/01/2020", Status = "L", Type = "type", Unit = "KG", User = "manager", Warehouse = "MN", DestinyAddress = "Nuevo Le?n, Mexico, CP. 54715", Comments = "Cooments", HasMissingStock = false, OrderRelationType = "Padre" },
                new CompleteFormulaWithDetalle { ProductionOrderId = 225312, BaseDocument = 171911, Client = "C001", Code = "Aspirina", Container = "container", CompleteQuantity = 1, Details = new List<CompleteDetalleFormulaModel>(), DueDate = "01/01/2020", EndDate = "01/01/2020", FabDate = "01/01/2020", IsChecked = false, Number = 100, Origin = "PT", PlannedQuantity = 1, ProductDescription = "orden", ProductLabel = "label", RealEndDate = "01/01/2020", StartDate = "01/01/2020", Status = "L", Type = "type", Unit = "KG", User = "manager", Warehouse = "MN", DestinyAddress = "Nuevo Le?n, Mexico, CP. 54715", Comments = "Cooments", HasMissingStock = false, OrderRelationType = "Completa" },
                new CompleteFormulaWithDetalle { ProductionOrderId = 225313, BaseDocument = 171912, Client = "C001", Code = "Aspirina", Container = "container", CompleteQuantity = 1, Details = new List<CompleteDetalleFormulaModel>(), DueDate = "01/01/2020", EndDate = "01/01/2020", FabDate = "01/01/2020", IsChecked = false, Number = 100, Origin = "PT", PlannedQuantity = 1, ProductDescription = "orden", ProductLabel = "label", RealEndDate = "01/01/2020", StartDate = "01/01/2020", Status = "L", Type = "type", Unit = "KG", User = "manager", Warehouse = "MN", DestinyAddress = "Nuevo Le?n, Mexico, CP. 54715", Comments = "Cooments", HasMissingStock = false, OrderRelationType = "Hija" },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = listFormula,
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// gets the result from detail orde rmodel.
        /// </summary>
        /// <returns>the data.</returns>
        public ResultModel GetListCompleteDetailOrderModel()
        {
            var listDetails = new List<CompleteDetailOrderModel>
            {
                new CompleteDetailOrderModel { CodigoProducto = "CA", DescripcionProducto = "desc", FechaOf = "20/01/2020", FechaOfFin = "01/01/2020", IsChecked = false, OrdenFabricacionId = 100, Qfb = "qfb", QtyPlanned = 100, QtyPlannedDetalle = 100, Status = "Planificado" },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = JsonConvert.SerializeObject(listDetails),
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// gets the users by role.
        /// </summary>
        /// <returns>the users.</returns>
        public ResultModel GetUsersByRole()
        {
            var users = new List<UserModel>
            {
                new UserModel { Id = "abc", Activo = 1, FirstName = "Gustavo", LastName = "Ramirez", Password = "pass", Role = 2, UserName = "gus1", Piezas = 1000, Asignable = 1, Classification = "MN" },
                new UserModel { Id = "abcd", Activo = 1, FirstName = "Hugo", LastName = "Ramirez", Password = "pass", Role = 2, UserName = "gus1", Piezas = 1000, Asignable = 1, Classification = "BE" },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = users,
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// gets the users by role.
        /// </summary>
        /// <returns>the users.</returns>
        public ResultModel GetUser()
        {
            var users = new List<UserModel>
            {
                new UserModel { Id = "1a663b91-fffa-4298-80c3-aaae35586dc6", Activo = 1, FirstName = "Victor", LastName = "Sanchez", Password = "pass", Role = 2, UserName = "vicQFB", Piezas = 1000, Asignable = 1, Classification = "MN" },
                new UserModel { Id = "bd4b2724-3b13-490e-aed2-5c8bfdd7551a", Activo = 1, FirstName = "Victor", LastName = "quimico2", Password = "pass", Role = 2, UserName = "quimico2", Piezas = 1000, Asignable = 1, Classification = "MN" },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = JsonConvert.SerializeObject(users),
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// gets the users by role.
        /// </summary>
        /// <param name="technicalRequire">Is Technical Require.</param>
        /// <param name="tecnicId">Tecnic id.</param>
        /// <returns>the users.</returns>
        public ResultModel GetUsersByRoleWithDZ(bool technicalRequire = false, string tecnicId = "8c426e2d-0615-4516-a94c-a79e5c11ae4d")
        {
            var users = new List<UserModel>
            {
                new UserModel { Id = "abc", Activo = 1, FirstName = "Gustavo", LastName = "Ramirez", Password = "pass", Role = 2, UserName = "gus1", Piezas = 1000, Asignable = 1, Classification = "MN", TechnicalRequire = true, TecnicId = "8c426e2d-0615-4516-a94c-a79e5c11ae4d" },
                new UserModel { Id = "abcd", Activo = 1, FirstName = "Hugo", LastName = "Ramirez", Password = "pass", Role = 2, UserName = "gus1", Piezas = 1000, Asignable = 1, Classification = "BE", TechnicalRequire = true, TecnicId = "8c426e2d-0615-4516-a94c-a79e5c11ae4d" },
                new UserModel { Id = "abcde", Activo = 1, FirstName = "Magistrales", LastName = "Magistrales", Password = "pass", Role = 2, UserName = "gus1", Piezas = 1000, Asignable = 1, Classification = "MG", TechnicalRequire = technicalRequire },
                new UserModel { Id = "abcdef", Activo = 1, FirstName = "Test DZ 1", LastName = "Test DZ 1", Password = "pass", Role = 2, UserName = "gus1", Piezas = 0, Asignable = 1, Classification = "DZ", TechnicalRequire = technicalRequire, TecnicId = tecnicId },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = users,
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// Generates a result model.
        /// </summary>
        /// <param name="dataToSend">the data.</param>
        /// <returns>the dta.</returns>
        public ResultModel GenerateResultModel(object dataToSend)
        {
            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = JsonConvert.SerializeObject(dataToSend),
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// gets the users by role.
        /// </summary>
        /// <param name="isValidtecnic">Is valid tecnic.</param>
        /// <returns>the users.</returns>
        public ResultModel GetQfbInfoDto(bool isValidtecnic)
        {
            var qfbValidatedInfo = new List<QfbTecnicInfoDto>
            {
                new QfbTecnicInfoDto
                {
                    IsTecnicRequired = true,
                    IsValidTecnic = isValidtecnic,
                    QfbFirstName = "Juan",
                    QfbLastName = "P�rez",
                    QfbId = "abc",
                    TecnicId = "6bc7f8a8-8617-43ac-a804-79cf9667b801",
                    IsValidQfbConfiguration = true,
                },
            };

            return new ResultModel
            {
                Code = 200,
                ExceptionMessage = string.Empty,
                Response = qfbValidatedInfo,
                Success = true,
                UserError = string.Empty,
            };
        }

        /// <summary>
        /// GetProductionOrderProcessingStatusModel.
        /// </summary>
        /// <returns>List ProductionOrderProcessingStatusModel.</returns>
        public List<ProductionOrderProcessingStatusModel> GetProductionOrderProcessingStatusModel()
        {
            return new List<ProductionOrderProcessingStatusModel>
            {
                new ProductionOrderProcessingStatusModel
                {
                    Id = "9e7ea1ba-5950-4a94-a34e-5b7a5db112a4",
                    ProductionOrderId = 100001,
                    LastStep = "Primary Validations",
                    Status = "In Progress",
                    Payload = "{\"FinalizeProductionOrder\":{\"UserId\":\"35642b3a-9471-4b89-9862-8bee6d98c361\",\"ProductionOrderId\":226357,\"SourceProcess\":null,\"Batches\":null}}",
                    CreatedAt = DateTime.Now.AddDays(-1),
                    LastUpdated = DateTime.Now,
                },
                new ProductionOrderProcessingStatusModel
                {
                    Id = "656feb73-35ad-46ae-9e4c-4e708c05205d",
                    ProductionOrderId = 223580,
                    LastStep = "Update UsersOrders in postgres",
                    Status = "In Progress",
                    Payload = "{\"FinalizeProductionOrder\":{\"UserId\":\"7ac2db83-3c31-4042-9d1b-5531753694b4\",\"ProductionOrderId\":223580,\"SourceProcess\":null,\"Batches\":null}}",
                    CreatedAt = DateTime.Now.AddDays(-1),
                    LastUpdated = DateTime.Now,
                },
                new ProductionOrderProcessingStatusModel
                {
                    Id = "6fe84f79-c724-45a3-9588-f3cf947322de",
                    ProductionOrderId = 223581,
                    LastStep = "Update UsersOrders in postgres",
                    Status = "In Progress",
                    Payload = "{\"FinalizeProductionOrder\":{\"UserId\":\"7ac2db83-3c31-4042-9d1b-5531753694b4\",\"ProductionOrderId\":223581,\"SourceProcess\":null,\"Batches\":null}}",
                    CreatedAt = DateTime.Now.AddDays(-1),
                    LastUpdated = DateTime.Now,
                },
                new ProductionOrderProcessingStatusModel
                {
                    Id = "741baf58-b87a-4235-b724-35f966229fd8",
                    ProductionOrderId = 224896,
                    LastStep = "Update UsersOrders in postgres",
                    Status = "In Progress",
                    Payload = "{\"FinalizeProductionOrder\":{\"UserId\":\"7ac2db83-3c31-4042-9d1b-5531753694b4\",\"ProductionOrderId\":224896,\"SourceProcess\":null,\"Batches\":null}}",
                    CreatedAt = DateTime.Now.AddDays(-1),
                    LastUpdated = DateTime.Now,
                },
            };
        }

        /// <summary>
        /// GetProductionOrderProcessingStatusModel.
        /// </summary>
        /// <returns>List ProductionOrderProcessingStatusModel.</returns>
        public IEnumerable<ProductionOrderProcessingStatusModel> GetProductionOrderProcessingStatusModelForGetFailedProductionOrders()
        {
            return new List<ProductionOrderProcessingStatusModel>
            {
                // GetFailedProductionOrders test
                new ProductionOrderProcessingStatusModel
                {
                    Id = "fe6e0eea-c279-4a07-b0fb-923dce1b5e31",
                    ProductionOrderId = 123,
                    LastStep = "Primary Validations",
                    Status = "Failed",
                    Payload = "{\"FinalizeProductionOrder\":{\"UserId\":\"5c9700ba-92e1-40ae-91ab-d6d833eb03de\",\"ProductionOrderId\":123,\"SourceProcess\":null,\"Batches\":null}}",
                    CreatedAt = DateTime.Now.AddDays(-1),
                    LastUpdated = DateTime.Now,
                },
                new ProductionOrderProcessingStatusModel
                {
                    Id = "b02abfd6-7c31-420d-bcf5-db99aef35a65",
                    ProductionOrderId = 456,
                    LastStep = "Update UsersOrders in postgres",
                    Status = "Failed",
                    Payload = "{\"FinalizeProductionOrder\":{\"UserId\":\"5c9700ba-92e1-40ae-91ab-d6d833eb03de\",\"ProductionOrderId\":456,\"SourceProcess\":null,\"Batches\":null}}",
                    CreatedAt = DateTime.Now.AddDays(-1),
                    LastUpdated = DateTime.Now,
                },
                new ProductionOrderProcessingStatusModel
                {
                    Id = "7773b512-9eb2-495d-abcf-16de3ac616db",
                    ProductionOrderId = 789,
                    LastStep = "PDF Creation",
                    Status = "Failed",
                    Payload = "{\"FinalizeProductionOrder\":{\"UserId\":\"5c9700ba-92e1-40ae-91ab-d6d833eb03de\",\"ProductionOrderId\":789,\"SourceProcess\":null,\"Batches\":null}}",
                    CreatedAt = DateTime.Now.AddDays(-1),
                    LastUpdated = DateTime.Now,
                },
            };
        }

        /// <summary>
        /// GetProductionOrderProcessingStatusModel.
        /// </summary>
        /// <returns>List ProductionOrderProcessingStatusModel.</returns>
        public IEnumerable<ProductionOrderSeparationDetailLogsModel> GetProductionOrderSeparationDetailLogsModel()
        {
            return new List<ProductionOrderSeparationDetailLogsModel>
            {
                // GetFailedProductionOrders test
                new ProductionOrderSeparationDetailLogsModel
                {
                    Id = "fe6e0eea-c279-4a07-b0fb-923dce1b5e31",
                    ParentProductionOrderId = 123,
                    LastStep = "CancelPostgres",
                    IsSuccessful = false,
                    Payload = "{\"FinalizeProductionOrder\":{\"UserId\":\"5c9700ba-92e1-40ae-91ab-d6d833eb03de\",\"ProductionOrderId\":123,\"SourceProcess\":null,\"Batches\":null}}",
                    CreatedAt = DateTime.Now.AddDays(-1),
                    LastUpdated = DateTime.Now,
                },
                new ProductionOrderSeparationDetailLogsModel
                {
                    Id = "b02abfd6-7c31-420d-bcf5-db99aef35a65",
                    ParentProductionOrderId = 456,
                    LastStep = "CancelPostgres",
                    IsSuccessful = false,
                    Payload = "{\"FinalizeProductionOrder\":{\"UserId\":\"5c9700ba-92e1-40ae-91ab-d6d833eb03de\",\"ProductionOrderId\":456,\"SourceProcess\":null,\"Batches\":null}}",
                    CreatedAt = DateTime.Now.AddDays(-1),
                    LastUpdated = DateTime.Now,
                },
                new ProductionOrderSeparationDetailLogsModel
                {
                    Id = "7773b512-9eb2-495d-abcf-16de3ac616db",
                    ParentProductionOrderId = 789,
                    LastStep = "StartCancelParentOrderProcess",
                    IsSuccessful = false,
                    Payload = "{\"FinalizeProductionOrder\":{\"UserId\":\"5c9700ba-92e1-40ae-91ab-d6d833eb03de\",\"ProductionOrderId\":789,\"SourceProcess\":null,\"Batches\":null}}",
                    CreatedAt = DateTime.Now.AddDays(-1),
                    LastUpdated = DateTime.Now,
                },
            };
        }

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public List<UserOrderModel> GetUserModelsForFinalizeProductionOrdersOnPostgresqlAsync()
        {
            return new List<UserOrderModel>
            {
                // UPDATE POSTGRES
                new UserOrderModel { Id = 50, Productionorderid = null, Salesorderid = "165062", Status = "Terminado", Userid = "abc", FinishDate = new DateTime(2025, 4, 22), Quantity = 0 },
                new UserOrderModel { Id = 51, Productionorderid = "223580", Salesorderid = "165062", Status = "Terminado", Userid = "abc", FinishDate = new DateTime(2025, 4, 22), Quantity = 1 },
                new UserOrderModel { Id = 52, Productionorderid = "223581", Salesorderid = "165062", Status = "Terminado", Userid = "abc", FinishDate = new DateTime(2025, 4, 22), Quantity = 1 },
                new UserOrderModel { Id = 53, Productionorderid = "223582", Salesorderid = "165062", Status = "Terminado", Userid = "abc", FinishDate = new DateTime(2025, 4, 22), Quantity = 1 },

                // CREATE PDF
                new UserOrderModel { Id = 55, Productionorderid = "224896", Salesorderid = null, Status = "Finalizado", Userid = "abc", FinishDate = new DateTime(2025, 4, 22), Quantity = 1 },

                // Separate
                new UserOrderModel { Id = 56, Productionorderid = "220001", Salesorderid = null, Status = "Almacenado", Userid = "abc", Quantity = 10, MagistralQr = "{\"SaleOrder\":1234,\"ProductionOrder\":220001,\"Quantity\":1.0,\"NeedsCooling\":\"Y\",\"ItemCode\":\"ItemCode\",\"DocNumDxp\":\"DocNumDxp\"}" },
                new UserOrderModel { Id = 57, Productionorderid = "220002", Salesorderid = null, Status = "Cancelado", Userid = "abc", Quantity = 10, MagistralQr = "{\"SaleOrder\":1234,\"ProductionOrder\":220002,\"Quantity\":1.0,\"NeedsCooling\":\"Y\",\"ItemCode\":\"ItemCode\",\"DocNumDxp\":\"DocNumDxp\"}" },
                new UserOrderModel { Id = 58, Productionorderid = "220003", Salesorderid = null, Status = "Almacenado", Userid = "abc", Quantity = 10, MagistralQr = "{\"SaleOrder\":1234,\"ProductionOrder\":220003,\"Quantity\":1.0,\"NeedsCooling\":\"Y\",\"ItemCode\":\"ItemCode\",\"DocNumDxp\":\"DocNumDxp\"}" },
                new UserOrderModel { Id = 59, Productionorderid = "220004", Salesorderid = null, Status = "Almacenado", Userid = "abc", Quantity = 10, MagistralQr = "{\"SaleOrder\":12345,\"ProductionOrder\":220004,\"Quantity\":1.0,\"NeedsCooling\":\"Y\",\"ItemCode\":\"ItemCode\",\"DocNumDxp\":\"DocNumDxp\"}" },
                new UserOrderModel { Id = 60, Productionorderid = "220005", Salesorderid = null, Status = "Almacenado", Userid = "abc", Quantity = 10, MagistralQr = "{\"SaleOrder\":12345,\"ProductionOrder\":220005,\"Quantity\":1.0,\"NeedsCooling\":\"Y\",\"ItemCode\":\"ItemCode\",\"DocNumDxp\":\"DocNumDxp\"}" },
                new UserOrderModel { Id = 62, Productionorderid = "220006", Salesorderid = null, Status = "Almacenado", Userid = "abc", Quantity = 10, MagistralQr = "{\"SaleOrder\":12345,\"ProductionOrder\":220006,\"Quantity\":1.0,\"NeedsCooling\":\"Y\",\"ItemCode\":\"ItemCode\",\"DocNumDxp\":\"DocNumDxp\"}" },
                new UserOrderModel { Id = 63, Productionorderid = "220007", Salesorderid = null, Status = "Almacenado", Userid = "abc", Quantity = 10, MagistralQr = "{\"SaleOrder\":12345,\"ProductionOrder\":220007,\"Quantity\":1.0,\"NeedsCooling\":\"Y\",\"ItemCode\":\"ItemCode\",\"DocNumDxp\":\"DocNumDxp\"}" },

                // Create Child Order
                new UserOrderModel { Id = 61, Productionorderid = "226744", Salesorderid = null, Status = "Almacenado", Userid = "abc", Quantity = 10, MagistralQr = "{\"SaleOrder\":1234,\"ProductionOrder\":226744,\"Quantity\":1.0,\"NeedsCooling\":\"Y\",\"ItemCode\":\"ItemCode\",\"DocNumDxp\":\"DocNumDxp\"}" },
            };
        }

        /// <summary>
        /// Gets user Dto.
        /// </summary>
        /// <returns>the user.</returns>
        public List<ParentOrderDetailModel> GetParentOrderDetailModel()
        {
            return new List<ParentOrderDetailModel>
            {
                new ParentOrderDetailModel { DocNum = "176693", FabOrderId = 227323, Quantity = 3, Batch = null, CreateDate = "07/09/2025 23:25:31", FinishDate = "08/09/2025",  UserCreate = "08faf89b-2f56-47bb-83da-895ba965fad4", Qfb = "08faf89b-2f56-47bb-83da-895ba965fad4", Status = "Terminado" },
                new ParentOrderDetailModel { DocNum = "176693", FabOrderId = 227324, Quantity = 3, Batch = null, CreateDate = "08/09/2025 00:07:23", FinishDate = "08/09/2025",  UserCreate = "08faf89b-2f56-47bb-83da-895ba965fad4", Qfb = "08faf89b-2f56-47bb-83da-895ba965fad4", Status = "Terminado" },
                new ParentOrderDetailModel { DocNum = "176693", FabOrderId = 227322, Quantity = 3, Batch = null, CreateDate = "07/09/2025 23:24:58", FinishDate = "08/09/2025",  UserCreate = "08faf89b-2f56-47bb-83da-895ba965fad4", Qfb = "08faf89b-2f56-47bb-83da-895ba965fad4", Status = "Terminado" },
            };
        }

        /// <summary>
        /// GetRedisFinalizeProductionOrderString.
        /// </summary>
        /// <param name="hasRedisValue">Has Redis Value.</param>
        /// <returns>the user.</returns>
        public string GetRedisFinalizeProductionOrderString(bool hasRedisValue)
        {
            var redisValue = new List<FinalizeProductionOrderModel>();
            if (hasRedisValue)
            {
                redisValue.Add(new FinalizeProductionOrderModel { ProductionOrderId = 100002, UserId = "2b8211b7-30a0-4841-ad79-d01c5d3ff71e", SourceProcess = "SaleOrder" });
            }

            return JsonConvert.SerializeObject(redisValue);
        }

        /// <summary>
        /// Creates a resultModle.
        /// </summary>
        /// <param name="response">the object to send.</param>
        /// <param name="isOk">flag to define if the service returns an error.</param>
        /// <param name="comments">the comments.</param>
        /// <param name="exceptionMessage">Exception Message.</param>
        /// <returns>the data.</returns>
        public ResultModel GetResultModelCompl(object response, bool isOk = true, string comments = "", string exceptionMessage = null)
        {
            return new ResultModel
            {
                Code = isOk ? 200 : 400,
                Response = JsonConvert.SerializeObject(response),
                Success = isOk,
                Comments = comments,
                ExceptionMessage = exceptionMessage,
            };
        }

        /// <summary>
        /// Gets the resultDto.
        /// </summary>
        /// <param name="dataToSend">the data to send.</param>
        /// <returns>the object.</returns>
        public ResultModel GetResulModel(object dataToSend)
        {
            return new ResultModel
            {
                Code = 200,
                Comments = null,
                Response = JsonConvert.SerializeObject(dataToSend),
            };
        }

        /// <summary>
        /// Get new db context for in memory database.
        /// </summary>
        /// <param name="dbname">Data base name.</param>
        /// <returns>New context options.</returns>
        internal static DbContextOptions<DatabaseContext> CreateNewContextOptions(string dbname)
        {
            // Create a fresh service provider, and therefore a fresh.
            // InMemory database instance.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Create a new options instance telling the context to use an
            // InMemory database and the new service provider.
            var builder = new DbContextOptionsBuilder<DatabaseContext>();
            builder.UseInMemoryDatabase(dbname)
                   .UseInternalServiceProvider(serviceProvider);

            return builder.Options;
        }
    }
}
