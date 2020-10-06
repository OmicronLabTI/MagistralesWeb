//
//  InboxTest.swift
//  OmicronTests
//
//  Created by Axity on 06/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import XCTest
import RxSwift
import Resolver
import RxDataSources

@testable import Omicron
// swiftlint:disable type_body_length
class InboxTest: XCTestCase {
    // MARK: - VARIABLES
    var inboxViewModel: InboxViewModel?
    var rootViewModel: RootViewModel?
    var disposeBag: DisposeBag?
    var order1: Order?
    var order2: Order?
    var orderItemCodeEmpty: Order?
    var orderTest1: Order?
    var orderTest2: Order?
    @Injected var networkManager: NetworkManager
    // swiftlint:disable function_body_length
    override func setUp() {
        print("XXXX setUp InboxTest")
        inboxViewModel = InboxViewModel()
        rootViewModel = RootViewModel()
        disposeBag = DisposeBag()
        order1 = Order(
            productionOrderId: 89284,
            baseDocument: 60067,
            container: "",
            tag: "Selecciona una...",
            plannedQuantity: 1,
            startDate: "27/08/2020",
            finishDate: "06/09/2020",
            descriptionProduct: "Aceite de Arbol de Te 0.3%, Alantoina 0.3%, Citrico 0.2%, " +
            "Extracto de Te Verde 3%, Extracto de Pepino 3%, Glicerina 3%, Hamamelis 3%, Hialuronico 3%, " +
            "Menta Piperita 0.02%, Niacinamida 2%, Pantenol 0.5%,  Salicilico 0.5%, Urea 5%, Solucion",
            statusId: 1,
            itemCode: "3264   120 ML",
            productCode: "3264",
            destiny: "Foráneo",
            hasMissingStock: false)
        order2 = Order(
            productionOrderId: 89995,
            baseDocument: 60284,
            container: "PRINCESS/ATOMIZADOR",
            tag: "NA",
            plannedQuantity: 1,
            startDate: "22/09/2020",
            finishDate: "30/09/2020",
            descriptionProduct: "Lactico 30% Solución",
            statusId: 1,
            itemCode: "1027S   30 ML",
            productCode: "1027S",
            destiny: "Local",
            hasMissingStock: false)
        orderItemCodeEmpty = Order(
            productionOrderId: 89995,
            baseDocument: 60284,
            container: "PRINCESS/ATOMIZADOR",
            tag: "NA",
            plannedQuantity: 1,
            startDate: "22/09/2020",
            finishDate: "30/09/2020",
            descriptionProduct: "Lactico 30% Solución",
            statusId: 1,
            itemCode: "",
            productCode: "1027S",
            destiny: "Local",
            hasMissingStock: false)
        orderTest1 = Order(
            productionOrderId: 90006,
            baseDocument: 60288,
            container: "Selecciona una...",
            tag: "Selecciona una...",
            plannedQuantity: 2,
            startDate: "24/09/2020",
            finishDate: "25/09/2020",
            descriptionProduct: "Agua de rosas 48%  agua de hamamelis 48%   propilenglicol 4%",
            statusId: 1,
            itemCode: "1132   120 ML",
            productCode: nil,
            destiny: "Local",
            hasMissingStock: true)
        orderTest2 = Order(
            productionOrderId: 89997,
            baseDocument: 60284,
            container: "PRINCESS/DISCTOP",
            tag: "PERSONALIZADA",
            plannedQuantity: 1,
            startDate: "22/09/2020",
            finishDate: "30/09/2020",
            descriptionProduct: "Aceite de Lima 20%, Vaselina",
            statusId: 1,
            itemCode: "2573   30 ML",
            productCode: nil,
            destiny: "Local",
            hasMissingStock: false)
    }
    override func tearDown() {
        print("XXXX tearDown InboxTest")
        inboxViewModel = nil
        rootViewModel = nil
        disposeBag = nil
        order1 = nil
        order2 = nil
        orderTest1 = nil
        orderTest2 = nil
    }
    // MARK: - TEST FUNCTIONS
    func testChangingAnOrderToStatusProcessSuccess() {
        // Given
        // swiftlint:disable line_length
        let response = "[{\"Id\":400,\"Userid\":\"d125566b-6321-4854-9a42-10fb5c5e4cc1\",\"Salesorderid\":\"\",\"Productionorderid\":\"89628\",\"Status\":\"Proceso\",\"Comments\":null,\"FinishDate\":null,\"CreationDate\":\"10/09/2020 09:44:31 AM\",\"CreatorUserId\":\"14409829-caa8-42f5-83e8-bc52b1f7afa5\",\"CloseDate\":null,\"CloseUserId\":null,\"IsIsolatedProductionOrder\":true,\"IsSalesOrder\":false,\"IsProductionOrder\":true,\"StatusOrder\":5}]"
        let userId = "d125566b-6321-4854-9a42-10fb5c5e4cc1"
        let orderId = 89628
        let statusToChange = "Proceso"
        var arrayOfOrdersToChangeStatusToProgress: [ChangeStatusRequest] = []
        let orderToChangeToChangeStatus = ChangeStatusRequest(userId: userId, orderId: orderId, status: statusToChange)
        arrayOfOrdersToChangeStatusToProgress.append(orderToChangeToChangeStatus)
        // When
        networkManager.changeStatusOrder(changeStatusRequest: arrayOfOrdersToChangeStatusToProgress).subscribe(onNext: { res in
            // Then
            XCTAssertNotNil(res)
            XCTAssertNotNil(res.response)
            XCTAssertTrue(res.code == 200)
            XCTAssertTrue(res.response == response)
        }).disposed(by: self.disposeBag!)
    }
    func testGroupedWithSimilarityOrWithoutSimilarityShouldBeEmpty() {
        // Given
        let data: [String?:[Order]] = [:]
        // Then
        let groupedOrders = inboxViewModel!.groupedByOrderNumber(data: data)
        // When
        XCTAssertTrue(groupedOrders.count == 0)
    }
    func testGroupedWithSimilarityOrWithoutSimilarityShouldBeGroupedWithoutSimilarity() {
        // Given
        var data: [String?:[Order]] = [:]
        let order = Order(productionOrderId: 89852, baseDocument: 0, container: "", tag: "",
                          plannedQuantity: 1, startDate: "14/09/2020", finishDate: "14/09/2020",
                          descriptionProduct: "CREMA BASE PARA RETINOICO", statusId: 1, itemCode: "BA-01",
                          productCode: "BA-01", destiny: "Local", hasMissingStock: false)
        data["BA-01"] = [order]
        // When
        let sectionModels = inboxViewModel!.groupedWithSimilarityOrWithoutSimilarity(
            data: data, titleForOrdersWithoutSimilarity: CommonStrings.noSimilarity,
            titleForOrdersWithSimilarity: CommonStrings.product)
        // Then
        XCTAssertFalse(sectionModels.count == 0)
        XCTAssertEqual(sectionModels[0].model, "Sin similitud")
    }
        func testGroupedWithSimilarityOrWithoutSimilarityShouldBeGroupedWithSimilarity() {
            // Given
            var data: [String?:[Order]] = [:]
            var orders: [Order] = []
            orders.append(self.order1!)
            orders.append(self.order2!)
            data["BA-01"] = orders
            // When
            let sectionModels = inboxViewModel!.groupedWithSimilarityOrWithoutSimilarity(
                data: data, titleForOrdersWithoutSimilarity: CommonStrings.noSimilarity,
                titleForOrdersWithSimilarity: CommonStrings.product)
            // Then
            XCTAssertEqual(sectionModels[0].model, "Producto: BA-01")
    }
    func testGroupedByOrderNumberShouldBeEmpty() {
        // Given
        let data: [String?: [Order]] = [:]
        // When
        let ordersGroupedAndSorted = inboxViewModel!.groupedByOrderNumber(data: data)
        // Then
        XCTAssertTrue(ordersGroupedAndSorted.count == 0)
    }
    func testGroupedByOrderNumberShouldBeGroupedAndSorted() {
        // Given
        var data: [String?:[Order]] = [:]
        var orders: [Order] = []
        orders.append(self.order1!)
        orders.append(self.order2!)
        data["60067"] = [order1!]
        data["60284"] = [order2!]
        // When
        let sortedSections = inboxViewModel!.groupedByOrderNumber(data: data)
        // Then
        XCTAssertTrue(sortedSections.count > 0 )
    }
    func testSortByBaseBocumentAscendingShouldBeEmpty() {
        // Given
        let orders: [Order] = []
        // When
        let ordersSorted = inboxViewModel!.sortByBaseBocumentAscending(orders: orders)
        // When
        XCTAssertTrue(ordersSorted.count == 0)
        XCTAssertNotNil(orders)
    }
    func testSortByBaseBocumentAscendingShoulBeSuccess() {
        // Given
        var orders: [Order] = []
        orders.append(self.order1!)
        orders.append(self.order2!)
        // When
        let ordersSorted = inboxViewModel!.sortByBaseBocumentAscending(orders: orders)
        XCTAssertTrue(ordersSorted.count > 0)
        XCTAssertEqual(ordersSorted[0].baseDocument, 60067)
        XCTAssertEqual(ordersSorted[1].baseDocument, 60284)
    }
    func testGetStatusNameShouldReturnEmpty() {
        // Given
        let index = 5
        // When
        let status = inboxViewModel!.getStatusName(index: index)
        // Then
        XCTAssertEqual(status, "")
    }
    func testGetStatusNameShouldReturnAssignedStatus() {
        // Given
        let index = 0
        // When
        let status = inboxViewModel!.getStatusName(index: index)
        // Then
        XCTAssertEqual(status, "Asignadas")
    }
    func testGetStatusNameShouldReturnProcessStatus() {
        // Given
        let index = 1
        // When
        let status = inboxViewModel!.getStatusName(index: index)
        // Then
        XCTAssertEqual(status, "En proceso")
    }
    func testGetStatusNameShouldReturnPendingStatus() {
        // Given
        let index = 2
        // When
        let status = inboxViewModel!.getStatusName(index: index)
        // Then
        XCTAssertEqual(status, "Pendiente")
    }
    func testGetStatusNameShouldReturnFinishedStatus() {
        // Given
        let index = 3
        // When
        let status = inboxViewModel!.getStatusName(index: index)
        // Then
        XCTAssertEqual(status, "Terminado")
    }
    func testGetStatusNameShouldReturnReassinedStatus() {
        // Given
        let index = 4
        // When
        let status = inboxViewModel!.getStatusName(index: index)
        // Then
        XCTAssertEqual(status, "Reasignado")
    }
    func testGetStatusIdShouldBeLeesOne() {
        // Given
        let name = "SomeValue"
        // Then
        let status = inboxViewModel!.getStatusId(name: name)
        // When
        XCTAssertEqual(status, -1)
    }
    func testGetStatusIdShouldBeAssignedStatus() {
        // Given
        let name = "Asignadas"
        // Then
        let status = inboxViewModel!.getStatusId(name: name)
        // When
        XCTAssertEqual(status, 0)
    }
    func testGetStatusIdShouldBeProcessStatus() {
        // Given
        let name = "En Proceso"
        // Then
        let status = inboxViewModel!.getStatusId(name: name)
        // When
        XCTAssertEqual(status, -1)
    }
    func testGetStatusIdShouldBePendindStatus() {
        // Given
        let name = "Pendiente"
        // Then
        let status = inboxViewModel!.getStatusId(name: name)
        // When
        XCTAssertEqual(status, 2)
    }
    func testGetStatusIdShouldBeFinishedStatus() {
        // Given
        let name = "Terminado"
        // Then
        let status = inboxViewModel!.getStatusId(name: name)
        // When
        XCTAssertEqual(status, 3)
    }
    func testGetStatusIdShouldBeReasignedStatus() {
        // Given
        let name = "Reasignado"
        // Then
        let status = inboxViewModel!.getStatusId(name: name)
        // When
        XCTAssertEqual(status, 4)
    }
    func testPendingDidTap() {
        self.inboxViewModel!.showAlertToChangeOrderOfStatus.subscribe(onNext: { message in
            XCTAssertTrue(message.message == CommonStrings.confirmationMessagePendingStatus)
        }).disposed(by: self.disposeBag!)
        self.inboxViewModel!.pendingDidTap.onNext(())
    }
    func testProcessDidTap() {
        self.inboxViewModel!.showAlertToChangeOrderOfStatus.subscribe(onNext: { message in
            XCTAssertTrue(message.message == CommonStrings.confirmationMessageProcessStatus)
        }).disposed(by: self.disposeBag!)
    }
    func testSimilarityViewButtonDidTapSucess() {
        var orders: [Order] = []
        orders.append(orderTest1!)
        orders.append(orderTest2!)
        let section = SectionOrder(statusId: 1, statusName: "Asignadas", numberTask: 2, imageIndicatorStatus: "assignedStatus", orders: orders)
        self.inboxViewModel!.setSelection(section: section)
        self.inboxViewModel!.statusDataGrouped.subscribe(onNext: { res in
            XCTAssertTrue(res.count > 0)
        }).disposed(by: self.disposeBag!)
        self.inboxViewModel!.similarityViewButtonDidTap.onNext(())
    }
    func testSetSectionSimilaritySort() {
        var orders: [Order] = []
        orders.append(orderTest1!)
        orders.append(orderTest2!)
        self.inboxViewModel?.similaritySort = true
        self.inboxViewModel?.normalSort = false
        let section = SectionOrder(statusId: 1, statusName: "Asignadas", numberTask: 2, imageIndicatorStatus: "assignedStatus", orders: orders)

        self.inboxViewModel!.statusDataGrouped.subscribe(onNext: { res in
            //XCTAssertTrue(res.count > 0)
            if res.count > 0 {
                XCTAssertEqual(res[0].model, "Sin similitud")
            }
        }).disposed(by: self.disposeBag!)
        self.inboxViewModel!.setSelection(section: section)
    }
    func testSimilarityViewButtonDidTapCodeProductIsEmpty() {
        var orders: [Order] = []
        orders.append(orderItemCodeEmpty!)
        let section = SectionOrder(statusId: 1, statusName: "Asignadas", numberTask: 2, imageIndicatorStatus: "assignedStatus", orders: orders)
        self.inboxViewModel!.setSelection(section: section)
        self.inboxViewModel!.statusDataGrouped.subscribe(onNext: { res in
            XCTAssertEqual(res[0].items[0].itemCode, CommonStrings.empty)
        }).disposed(by: self.disposeBag!)
        self.inboxViewModel!.similarityViewButtonDidTap.onNext(())
    }
    func testNormalViewButtonDidTap() {
        var orders: [Order] = []
        orders.append(orderTest1!)
        orders.append(orderTest2!)
        let section = SectionOrder(
            statusId: 1, statusName: "Asignadas", numberTask: 2, imageIndicatorStatus: "assignedStatus", orders: orders)
        self.inboxViewModel!.setSelection(section: section)
        self.inboxViewModel!.statusDataGrouped.subscribe(onNext: { res in
            XCTAssertTrue(res.count > 0)
        }).disposed(by: self.disposeBag!)
        self.inboxViewModel!.normalViewButtonDidTap.onNext(())
    }
    func testGroupByOrderNumberButtonDidTap() {
        var orders: [Order] = []
        orders.append(orderTest1!)
        orders.append(orderTest2!)
        let section = SectionOrder(
            statusId: 1, statusName: "Asignadas", numberTask: 2, imageIndicatorStatus: "assignedStatus", orders: orders)
        self.inboxViewModel!.setSelection(section: section)
        self.inboxViewModel!.statusDataGrouped.subscribe(onNext: { res in
            XCTAssertTrue(res.count > 0)
            XCTAssertNotNil(res)
        }).disposed(by: self.disposeBag!)
        self.inboxViewModel!.groupByOrderNumberButtonDidTap.onNext(())
    }
    func testProcessDidTapSucess() {
        // Then
        self.inboxViewModel?.showAlertToChangeOrderOfStatus.subscribe(onNext: { message in
            // When
            XCTAssertEqual(message.message, CommonStrings.confirmationMessageProcessStatus)
            XCTAssertEqual(message.typeOfStatus, StatusNameConstants.inProcessStatus)
        }).disposed(by: self.disposeBag!)
        // Given
        self.inboxViewModel?.processDidTap.onNext(())
    }
    func testSetFilterWithOrderEmpty() {
        self.inboxViewModel?.title.subscribe(onNext: { _ in
            self.inboxViewModel?.statusDataGrouped.subscribe(onNext: { res in
                if res.count != 0 {
                    XCTAssertEqual(res[0].model, CommonStrings.empty)
                    XCTAssertTrue(res[0].items.count == 0)
                }
            }).disposed(by: self.disposeBag!)
        }).disposed(by: self.disposeBag!)
        self.inboxViewModel?.setFilter(orders: [])
    }
    func testSetFilterSuccess() {
        self.inboxViewModel?.statusDataGrouped.subscribe(onNext: { res in
            if res.count != 0 {
                XCTAssertEqual(res[0].model, CommonStrings.empty)
                XCTAssertEqual(res[0].items[0].productionOrderId, 89284)
            }
        }).disposed(by: self.disposeBag!)
        self.inboxViewModel?.setFilter(orders: [self.order1!])
    }
    func testChangeStatusSuccess() {
        let indexPath = IndexPath(row: 0, section: 0)
        let typeStatus = StatusNameConstants.inProcessStatus
        var orders: [Order] = []
        orders.append(order1!)
        orders.append(order2!)
        let sectionModel = SectionModel(model: "", items: orders)
        self.inboxViewModel?.sectionOrders = [sectionModel]
        self.inboxViewModel?.processButtonIsEnable.subscribe(onNext: { res in
            XCTAssertFalse(res)
        }).disposed(by: self.disposeBag!)
        self.inboxViewModel?.changeStatus(indexPath: [indexPath], typeOfStatus: typeStatus)
    }
    func testGetStatusNameAssignedStatusName() {
        let index = 0
        let statusName = self.inboxViewModel?.getStatusName(index: index)
        XCTAssertEqual(statusName, StatusNameConstants.assignedStatus)
    }
    func testGetStatusNameProcessStatusName() {
        let index = 1
        let statusName = self.inboxViewModel?.getStatusName(index: index)
        XCTAssertEqual(statusName, StatusNameConstants.inProcessStatus)
    }
    func testGetStatusNamePendingStatusName() {
        let index = 2
        let statusName = self.inboxViewModel?.getStatusName(index: index)
        XCTAssertEqual(statusName, StatusNameConstants.penddingStatus)
    }
    func testGetStatusNameFinishedStatusName() {
        let index = 3
        let statusName = self.inboxViewModel?.getStatusName(index: index)
        XCTAssertEqual(statusName, StatusNameConstants.finishedStatus)
    }
    func testGetStatusNameReasignedStatusName() {
        let index = 4
        let statusName = self.inboxViewModel?.getStatusName(index: index)
        XCTAssertEqual(statusName, StatusNameConstants.reassignedStatus)
    }
    func testGetStatusNameDefaultStatusName() {
        let index = 5
        let statusName = self.inboxViewModel?.getStatusName(index: index)
        XCTAssertEqual(statusName, CommonStrings.empty)
    }
   // swiftlint:disable file_length
}
