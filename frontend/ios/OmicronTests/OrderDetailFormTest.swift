//
//  OrderDetailFormTest.swift
//  OmicronTests
//
//  Created by Axity on 24/09/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import XCTest
import RxSwift
import Resolver
import Moya

@testable import Magistrales
class OrderDetailFormTest: XCTestCase {
    // MARK: - VARIABLES
    var disposeBag: DisposeBag?
    var orderDetailFormViewModel: OrderDetailFormViewModel?
    var provider: MoyaProvider<ApiService>!
    var statusCode = 200
    var testData = Data()

    @Injected var networkManager: NetworkManager
    override func setUp() {
        print("XXXX setUp OrderDetailFormTest")
        orderDetailFormViewModel = OrderDetailFormViewModel()
        disposeBag = DisposeBag()
        provider = MoyaProvider<ApiService>(
            endpointClosure: customEndpointClosure,
            stubClosure: MoyaProvider.immediatelyStub)
    }
    override func tearDown() {
        print("XXXX tearDown OrderDetailFormTest")
        orderDetailFormViewModel = nil
    }

    func customEndpointClosure(_ target: ApiService) -> Endpoint {
        return Endpoint(url: URL(target: target).absoluteString,
                        sampleResponseClosure: { .networkResponse(self.statusCode, self.testData) },
                        method: target.method,
                        task: target.task,
                        httpHeaderFields: target.headers)
    }

    // MARK: - FUNCTION TEST
    func testEditTableSuccessValueSuccess() {
        // Given
        self.networkManager.getOrdenDetail(89900).subscribe(onNext: { res in
            XCTAssertNotNil(res.response?.details)
        }).disposed(by: self.disposeBag!)
    }
    func testEditItemOfTableUpdateSuccess() {
        // given
        let detail = Detail(orderFabID: 89662, productID: "MP-009",
            detailDescription: "Acido Salicílico (A700)",
            baseQuantity: 0.002, requiredQuantity: 0.002, pendingQuantity: 0.002,
            stock: 55.447802000000003, warehouseQuantity: 6.2478020000000001,
            consumed: 0.0, available: 2.9080119999999998, unit: "KG", warehouse: "MG",
            hasBatches: true, orderCreateDate: "10/09/2020")
        let data = OrderDetail(
            productionOrderID: 89662, code: "1005   120 ML",
            productDescription: "Agua de Rosas 50cc  Alcohol 60ª 50cc  Azufre 3.3gr",
            type: "Estandar", status: "Liberado", plannedQuantity: 1,
            unit: "Pieza", warehouse: "PT", number: 60222, fabDate: "10/09/2020",
            dueDate: "20/09/2020", startDate: "10/09/2020", endDate: "10/09/2020",
            user: "manager", origin: "Manual", baseDocument: 60222, client: "C00007",
            completeQuantity: 0, realEndDate: "", productLabel: "Selecciona una...",
            container: "Selecciona una...", comments: "", isChecked: false,
            details: [detail],
            catalogGroupName: "MG", orderCreateDate: "10/09/2020")
        self.orderDetailFormViewModel!.success.subscribe(onNext: { res in
            if res != 0 {
                XCTAssertEqual(89662, res)
            }
        }).disposed(by: self.disposeBag!)
        // then
        self.orderDetailFormViewModel!
            .editItemTable(index: 0, data: data, baseQuantity: 0.002, requiredQuantity: 0.002, werehouse: "MG")
    }

    func testEditItemOfTableUpdateWhenDataIsEmpty() {
        let data = OrderDetail()
        orderDetailFormViewModel?.editItemTable(
            index: 0, data: data, baseQuantity: 0.002, requiredQuantity: 0.002, werehouse: "MG")
    }

    func testUpdateDeleteItemOfTableServiceFailed() {
        orderDetailFormViewModel?
            .showAlert
            .subscribe(onNext: { res in
                XCTAssertNotNil(res)
                XCTAssertEqual(res, "Hubo un error al editar el elemento,  intente de nuevo")
            }).disposed(by: disposeBag!)
        statusCode = 500
        orderDetailFormViewModel?.networkManager = NetworkManager(provider: provider)
        let order = OrderDetailRequest(
            fabOrderID: 1354, plannedQuantity: 5, fechaFin: "12/12/21", comments: "", warehouse: "", components: [])
        let data = OrderDetail()
        orderDetailFormViewModel?.updateDeleteItemOfTableService(order, data, 5)
    }
}
