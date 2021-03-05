//
//  ExtensionInbox2Test.swift
//  OmicronTests
//
//  Created by Sergio Flores Ramirez on 18/02/21.
//  Copyright © 2021 Diego Cárcamo. All rights reserved.
//

import XCTest
import RxSwift
import RxDataSources
@testable import OmicronLab

class ExtensionInbox2Test: XCTestCase {

    var inboxViewModel: InboxViewModel?
    var rootViewModel: RootViewModel?
    var disposeBag: DisposeBag?
    var order1: Order?
    var order2: Order?
    var orderItemCodeEmpty: Order?
    var orderTest1: Order?
    var orderTest2: Order?

    override func setUpWithError() throws {
        inboxViewModel = InboxViewModel()
        rootViewModel = RootViewModel()
        disposeBag = DisposeBag()
        order1 = Order(
            productionOrderId: 89284, baseDocument: 60067, container: "",
            tag: "Selecciona una...", plannedQuantity: 1, startDate: "27/08/2020",
            finishDate: "06/09/2020",
            descriptionProduct: "Aceite de Arbol de Te 0.3%, Alantoina 0.3%, Citrico 0.2%, " +
            "Extracto de Te Verde 3%, Extracto de Pepino 3%, Glicerina 3%, Hamamelis 3%, Hialuronico 3%, " +
            "Menta Piperita 0.02%, Niacinamida 2%, Pantenol 0.5%,  Salicilico 0.5%, Urea 5%, Solucion",
            statusId: 1, itemCode: "3264   120 ML", productCode: "3264", destiny: "Foráneo",
            hasMissingStock: false, finishedLabel: false)
        order2 = Order(
            productionOrderId: 89995, baseDocument: 60284, container: "PRINCESS/ATOMIZADOR",
            tag: "NA", plannedQuantity: 1, startDate: "22/09/2020", finishDate: "30/09/2020",
            descriptionProduct: "Lactico 30% Solución", statusId: 1, itemCode: "1027S   30 ML",
            productCode: "1027S", destiny: "Local", hasMissingStock: false, finishedLabel: false)
        orderItemCodeEmpty = Order(
            productionOrderId: 89995, baseDocument: 60284, container: "PRINCESS/ATOMIZADOR", tag: "NA",
            plannedQuantity: 1, startDate: "22/09/2020",
            finishDate: "30/09/2020", descriptionProduct: "Lactico 30% Solución",
            statusId: 1, itemCode: "", productCode: "1027S", destiny: "Local",
            hasMissingStock: false, finishedLabel: false)
        orderTest1 = Order(
            productionOrderId: 90006, baseDocument: 60288, container: "Selecciona una...",
            tag: "Selecciona una...", plannedQuantity: 2, startDate: "24/09/2020", finishDate: "25/09/2020",
            descriptionProduct: "Agua de rosas 48%  agua de hamamelis 48%   propilenglicol 4%",
            statusId: 1, itemCode: "1132   120 ML", productCode: nil, destiny: "Local",
            hasMissingStock: true, finishedLabel: false)
        orderTest2 = Order(
            productionOrderId: 89997, baseDocument: 60284, container: "PRINCESS/DISCTOP",
            tag: "PERSONALIZADA", plannedQuantity: 1, startDate: "22/09/2020", finishDate: "30/09/2020",
            descriptionProduct: "Aceite de Lima 20%, Vaselina", statusId: 1, itemCode: "2573   30 ML",
            productCode: nil, destiny: "Local", hasMissingStock: false, finishedLabel: false)
    }

    override func tearDownWithError() throws {
        // Put teardown code here. This method is called after the invocation of each test method in the class.
        inboxViewModel = nil
        rootViewModel = nil
        disposeBag = nil
        order1 = nil
        order2 = nil
        orderTest1 = nil
        orderTest2 = nil
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
        let section = SectionOrder(
            statusId: 1, statusName: "Asignadas", numberTask: 2, imageIndicatorStatus: "assignedStatus", orders: orders)
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
        let section = SectionOrder(
            statusId: 1, statusName: "Asignadas", numberTask: 2, imageIndicatorStatus: "assignedStatus", orders: orders)

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
        let section = SectionOrder(
            statusId: 1, statusName: "Asignadas", numberTask: 2, imageIndicatorStatus: "assignedStatus", orders: orders)
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
        self.inboxViewModel?.changeStatus(indexPath: [indexPath], typeOfStatus: typeStatus, needsError: false)
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
}
