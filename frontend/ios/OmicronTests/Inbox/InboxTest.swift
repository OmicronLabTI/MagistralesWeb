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
import Moya

@testable import OmicronLab

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

    var provider: MoyaProvider<ApiService>!
    var statusCode = 200
    var testData = Data()
    @Injected var networkManager: NetworkManager
    @Injected var inboxVM: InboxViewModel

    override func setUp() {
        print("XXXX setUp InboxTest")
        inboxViewModel = InboxViewModel()
        rootViewModel = RootViewModel()
        disposeBag = DisposeBag()
        order1 = Order( areBatchesComplete: true, productionOrderId: 89284, baseDocument: 60067, container: "",
            tag: "Selecciona una...", plannedQuantity: 1, startDate: "27/08/2020", finishDate: "06/09/2020",
            descriptionProduct: "Aceite de Arbol de Te 0.3%, Alantoina 0.3%, Citrico 0.2%, " +
            "Extracto de Te Verde 3%, Extracto de Pepino 3%, Glicerina 3%, Hamamelis 3%, Hialuronico 3%, " +
            "Menta Piperita 0.02%, Niacinamida 2%, Pantenol 0.5%,  Salicilico 0.5%, Urea 5%, Solucion",
            statusId: 1, itemCode: "3264   120 ML", productCode: "3264", destiny: "Foráneo",
            hasMissingStock: false, finishedLabel: false, patientName: "NamePatient",
            clientDxp: "clientDxp", shopTransaction: "emnjkd", qfbName: "")
        order2 = Order( areBatchesComplete: true, productionOrderId: 89995, baseDocument: 60284,
            container: "PRINCESS/ATOMIZADOR", tag: "NA", plannedQuantity: 1, startDate: "22/09/2020",
            finishDate: "30/09/2020", descriptionProduct: "Lactico 30% Solución", statusId: 1,
            itemCode: "1027S   30 ML", productCode: "1027S", destiny: "Local", hasMissingStock: false,
            finishedLabel: false, patientName: "NamePatient 1", clientDxp: "clientDxp 1", shopTransaction: "emnjkd", qfbName: "")
        orderItemCodeEmpty = Order( areBatchesComplete: true, productionOrderId: 89995, baseDocument: 60284,
            container: "PRINCESS/ATOMIZADOR", tag: "NA", plannedQuantity: 1, startDate: "22/09/2020",
            finishDate: "30/09/2020", descriptionProduct: "Lactico 30% Solución", statusId: 1, itemCode: "",
            productCode: "1027S", destiny: "Local", hasMissingStock: false, finishedLabel: false,
            patientName: "NamePatient 2", clientDxp: "clientDxp 3", shopTransaction: "emnjkd", qfbName: "")
        orderTest1 = Order(
            areBatchesComplete: true, productionOrderId: 90006, baseDocument: 60288, container: "Selecciona una...",
            tag: "Selecciona una...", plannedQuantity: 2, startDate: "24/09/2020", finishDate: "25/09/2020",
            descriptionProduct: "Agua de rosas 48%  agua de hamamelis 48%   propilenglicol 4%", statusId: 1,
            itemCode: "1132   120 ML", productCode: nil, destiny: "Local", hasMissingStock: true,
            finishedLabel: false, patientName: "NamePatient 3", clientDxp: "clientDxp 3", shopTransaction: "emnjkd", qfbName: "")
        orderTest2 = Order(
            areBatchesComplete: true, productionOrderId: 89997, baseDocument: 60284, container: "PRINCESS/DISCTOP",
            tag: "PERSONALIZADA", plannedQuantity: 1, startDate: "22/09/2020", finishDate: "30/09/2020",
            descriptionProduct: "Aceite de Lima 20%, Vaselina", statusId: 1, itemCode: "2573   30 ML",
            productCode: nil, destiny: "Local", hasMissingStock: false, finishedLabel: false,
            patientName: "NamePatient4", clientDxp: "clientDxp4", shopTransaction: "emnjkd", qfbName: "")
        provider = MoyaProvider<ApiService>(
            endpointClosure: customEndpointClosure, stubClosure: MoyaProvider.immediatelyStub)
        statusCode = 200
        testData = Data()
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
    func customEndpointClosure(_ target: ApiService) -> Endpoint {
        return Endpoint(url: URL(target: target).absoluteString,
                        sampleResponseClosure: { .networkResponse(self.statusCode, self.testData) },
                        method: target.method,
                        task: target.task,
                        httpHeaderFields: target.headers)
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
        let orderToChangeToChangeStatus = ChangeStatusRequest(userId: userId, orderId: orderId, status: statusToChange, userType: 2)
        arrayOfOrdersToChangeStatusToProgress.append(orderToChangeToChangeStatus)
        // When
        networkManager.changeStatusOrder(arrayOfOrdersToChangeStatusToProgress).subscribe(onNext: { res in
            // Then
            XCTAssertNotNil(res)
            XCTAssertNotNil(res.response)
            XCTAssertTrue(res.code == 200)
            XCTAssertTrue(res.response == response)
        }).disposed(by: self.disposeBag!)
    }
    func testGroupedWithSimilarityOrWithoutSimilarityShouldBeEmpty() {
        // Given
        let data: [String?: [Order]] = [:]
        // Then
        let groupedOrders = inboxViewModel!.groupedByOrderNumber(data: data)
        // When
        XCTAssertTrue(groupedOrders.count == 0)
    }
    func testGroupedWithSimilarityOrWithoutSimilarityShouldBeGroupedWithoutSimilarity() {
        // Given
        var data: [String?: [Order]] = [:]
        let order = Order(areBatchesComplete: true, productionOrderId: 89852, baseDocument: 0, container: "",
                          tag: "", plannedQuantity: 1, startDate: "14/09/2020", finishDate: "14/09/2020",
                          descriptionProduct: "CREMA BASE PARA RETINOICO", statusId: 1, itemCode: "BA-01",
                          productCode: "BA-01", destiny: "Local", hasMissingStock: false, finishedLabel: false,
                          patientName: "NamePatient", clientDxp: "clientDxp", shopTransaction: "emnjkd", qfbName: "")
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
            var data: [String?: [Order]] = [:]
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
        var data: [String?: [Order]] = [:]
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

    func testChangeStatusWhenCodeIs500() {
        let indexPath = IndexPath(row: 0, section: 0)
        let typeStatus = StatusNameConstants.inProcessStatus
        inboxViewModel?.sectionOrders = [SectionModel(model: CommonStrings.empty, items: [order1!])]
        inboxViewModel?.showAlert.subscribe(onNext: { res in
            XCTAssertEqual(res, CommonStrings.errorToChangeStatus)
        }).disposed(by: disposeBag!)
        statusCode = 500
        inboxViewModel?.networkManager = NetworkManager(provider: provider)
        inboxViewModel!.changeStatus(indexPath: [indexPath], typeOfStatus: typeStatus)
    }

}
