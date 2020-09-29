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
import Resolver

@testable import Omicron
class OrderDetailFormTest: XCTestCase {
    
    // MARK: -VARIABLES
    let disposeBag = DisposeBag()
    @Injected var orderDetailFormViewModel: OrderDetailFormViewModel
    let networkManager = NetworkManager(provider: MoyaProvider<ApiService>(stubClosure: MoyaProvider.immediatelyStub))
    
    // MARCK: - FUNCTION TEST
    
    func testEditTableSuccessValueSuccess() -> Void  {
        // Given
        self.networkManager.getOrdenDetail(orderId: 89900).subscribe(onNext: { res in
            XCTAssertNotNil(res.response?.details)
        }).disposed(by: self.disposeBag)
    }
    
//    func testEditItemOfTableUpdateSuccess() -> Void {
//        // given
//        let expectation = XCTestExpectation()
//
//        let detail = Detail(orderFabID: 89662, productID: "MP-009", detailDescription: "Acido Salicílico (A700)", baseQuantity: 0.002, requiredQuantity: 0.002, pendingQuantity: 0.002, stock: 55.447802000000003, warehouseQuantity: 6.2478020000000001, consumed: 0.0, available: 2.9080119999999998, unit: "KG", warehouse: "MG")
//
//
//        let data = OrderDetail(productionOrderID: 89662, code: "1005   120 ML", productDescription: "Agua de Rosas 50cc  Alcohol 60ª 50cc  Azufre 3.3gr  Resorcina 2.5gr  Salicilico 1.7gr. Loción. (Cantidad proporcional al volumen total)", type: "Estandar", status: "Liberado", plannedQuantity: 1, unit: "Pieza", warehouse: "PT", number: 60222, fabDate: "10/09/2020", dueDate: "20/09/2020", startDate: "10/09/2020", endDate: "10/09/2020", user: "manager", origin: "Manual", baseDocument: 60222, client: "C00007", completeQuantity: 0, realEndDate: "", productLabel:  "Selecciona una...", container: "Selecciona una...", comments: "", isChecked: false, details: [detail])
//
//        self.orderDetailFormViewModel.showAlert.subscribe(onNext: { message in
//            XCTAssertEqual(message, "Se registraron los cambios correctamente")
//            expectation.fulfill()
//        }).disposed(by: self.disposeBag)
//
//        // then
//        self.orderDetailFormViewModel.editItemTable(index: 0, data: data, baseQuantity: 0.002, requiredQuantity: 0.002, werehouse: "MG")
//        
//        wait(for: [expectation], timeout: 1000)
//    }
//
//    func testEditItemUpdateFailed() -> Void {
//
//        let expectation = XCTestExpectation()
//
//        let detail = Detail(orderFabID: 0, productID: "MP-009", detailDescription: "Acido Salicílico (A700)", baseQuantity: 0.002, requiredQuantity: 0.002, pendingQuantity: 0.002, stock: 55.447802000000003, warehouseQuantity: 6.2478020000000001, consumed: 0.0, available: 2.9080119999999998, unit: "KG", warehouse: "MG")
//
//        let data = OrderDetail(productionOrderID: 0, code: "1005   120 ML", productDescription: "Agua de Rosas 50cc  Alcohol 60ª 50cc  Azufre 3.3gr  Resorcina 2.5gr  Salicilico 1.7gr. Loción. (Cantidad proporcional al volumen total)", type: "Estandar", status: "Liberado", plannedQuantity: 1, unit: "Pieza", warehouse: "PT", number: 0, fabDate: "10/09/2020", dueDate: "20/09/2020", startDate: "10/09/2020", endDate: "10/09/2020", user: "manager", origin: "Manual", baseDocument: 0, client: "C00007", completeQuantity: 0, realEndDate: "", productLabel:  "Selecciona una...", container: "Selecciona una...", comments: "", isChecked: false, details: [detail])
//
//        self.orderDetailFormViewModel.response.subscribe(onNext: { message in
//            XCTAssertEqual(message, "{\"0-0\":\"ErrorUpdateFabOrd-OrderNotFound--2028-No existen registros coincidentes (ODBC -2028)\"}")
//            expectation.fulfill()
//        }).disposed(by: self.disposeBag)
//
//        self.orderDetailFormViewModel.editItemTable(index: 0, data: data, baseQuantity: 0.002, requiredQuantity: 0.002, werehouse: "MG")
//
//        wait(for: [expectation], timeout: 400, enforceOrder:  true)
//
//    }
}
