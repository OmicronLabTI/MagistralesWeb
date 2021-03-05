//
//  ExtensionLoginTest.swift
//  OmicronTests
//
//  Created by Sergio Flores Ramirez on 18/02/21.
//  Copyright © 2021 Diego Cárcamo. All rights reserved.
//

import XCTest
import RxSwift
import RxDataSources

@testable import OmicronLab
class ExtensionInboxTest: XCTestCase {

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

    func testGetFabOrderIDsSuccess() {
        inboxViewModel?.sectionOrders = [SectionModel(model: CommonStrings.empty, items: [order1!])]
        let indexPath = IndexPath(row: 0, section: 0)
        let result = inboxViewModel?.getFabOrderIDs(indexPathOfOrdersSelected: [indexPath])
        XCTAssertEqual(result?.count, 1)
        XCTAssertEqual(result?[0], 89284)
    }

    func testGetFabOrderIDsFailure() {
        let result = inboxViewModel?.getFabOrderIDs(indexPathOfOrdersSelected: [])
        XCTAssertEqual(result?.count, 0)
        XCTAssertTrue(result!.isEmpty)
    }
}
