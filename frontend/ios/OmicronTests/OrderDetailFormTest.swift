//
//  OrderDetailFormTest.swift
//  OmicronTests
//
//  Created by Axity on 24/09/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import XCTest
import RxSwift
import RxCocoa
import Moya

@testable import Omicron
class OrderDetailFormTest: XCTestCase {
    
    // MARK: -VARIABLES
    var disposeBag: DisposeBag?
    var expectation: XCTestExpectation?
    var orderDetailFormViewModel:OrderDetailFormViewModel?
    var networkManager: NetworkManager?
    
    override func setUp() {
        print("XXXX setUp OrderDetailFormTest")
        orderDetailFormViewModel = OrderDetailFormViewModel()
        networkManager = NetworkManager(provider: MoyaProvider<ApiService>(stubClosure: MoyaProvider.immediatelyStub))
        expectation = XCTestExpectation()
        disposeBag = DisposeBag()
    }
    
    override func tearDown() {
        print("XXXX tearDown OrderDetailFormTest")
        orderDetailFormViewModel = nil
        expectation = nil
    }
    
    // MARCK: - FUNCTION TEST
    
    func testEditTableSuccessValueSuccess() -> Void  {
        // Given
        self.networkManager!.getOrdenDetail(orderId: 89900).subscribe(onNext: { res in
            XCTAssertNotNil(res.response?.details)
        }).disposed(by: self.disposeBag!)
    }
    
    func testEditItemOfTableUpdateSuccess() -> Void {
        // given

        let detail = Detail(orderFabID: 89662, productID: "MP-009", detailDescription: "Acido Salicílico (A700)", baseQuantity: 0.002, requiredQuantity: 0.002, pendingQuantity: 0.002, stock: 55.447802000000003, warehouseQuantity: 6.2478020000000001, consumed: 0.0, available: 2.9080119999999998, unit: "KG", warehouse: "MG")


        let data = OrderDetail(productionOrderID: 89662, code: "1005   120 ML", productDescription: "Agua de Rosas 50cc  Alcohol 60ª 50cc  Azufre 3.3gr  Resorcina 2.5gr  Salicilico 1.7gr. Loción. (Cantidad proporcional al volumen total)", type: "Estandar", status: "Liberado", plannedQuantity: 1, unit: "Pieza", warehouse: "PT", number: 60222, fabDate: "10/09/2020", dueDate: "20/09/2020", startDate: "10/09/2020", endDate: "10/09/2020", user: "manager", origin: "Manual", baseDocument: 60222, client: "C00007", completeQuantity: 0, realEndDate: "", productLabel:  "Selecciona una...", container: "Selecciona una...", comments: "", isChecked: false, details: [detail])

        self.orderDetailFormViewModel!.success.subscribe(onNext: { res in
            if(res != 0) {
                XCTAssertEqual(89662, res)
                self.expectation!.fulfill()
                print("PASO testEditItemOfTableUpdateSuccess")
            }
        }).disposed(by: self.disposeBag!)

        // then
        self.orderDetailFormViewModel!.editItemTable(index: 0, data: data, baseQuantity: 0.002, requiredQuantity: 0.002, werehouse: "MG")
        wait(for: [self.expectation!], timeout: 1000)
    }

    func testEditItemUpdateFailed() -> Void {

        let detail = Detail(orderFabID: 0, productID: "MP-009", detailDescription: "Acido Salicílico (A700)", baseQuantity: 0.002, requiredQuantity: 0.002, pendingQuantity: 0.002, stock: 55.447802000000003, warehouseQuantity: 6.2478020000000001, consumed: 0.0, available: 2.9080119999999998, unit: "KG", warehouse: "MG")

        let data = OrderDetail(productionOrderID: 0, code: "1005   120 ML", productDescription: "Agua de Rosas 50cc  Alcohol 60ª 50cc  Azufre 3.3gr  Resorcina 2.5gr  Salicilico 1.7gr. Loción. (Cantidad proporcional al volumen total)", type: "Estandar", status: "Liberado", plannedQuantity: 1, unit: "Pieza", warehouse: "PT", number: 0, fabDate: "10/09/2020", dueDate: "20/09/2020", startDate: "10/09/2020", endDate: "10/09/2020", user: "manager", origin: "Manual", baseDocument: 0, client: "C00007", completeQuantity: 0, realEndDate: "", productLabel:  "Selecciona una...", container: "Selecciona una...", comments: "", isChecked: false, details: [detail])

        self.orderDetailFormViewModel!.response.subscribe(onNext: { message in
            if (message != "") {
                XCTAssertEqual(message, "{\"0-0\":\"ErrorUpdateFabOrd-OrderNotFound--2028-No existen registros coincidentes (ODBC -2028)\"}")
                self.expectation!.fulfill()
                print("PASO testEditItemUpdateFailed")
            }
        }).disposed(by: self.disposeBag!)

        self.orderDetailFormViewModel!.editItemTable(index: 0, data: data, baseQuantity: 0.002, requiredQuantity: 0.002, werehouse: "MG")
        wait(for: [self.expectation! ], timeout: 1000)

    }
}
