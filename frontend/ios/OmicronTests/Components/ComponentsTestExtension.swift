//
//  OrderDetailTestExtension.swift
//  OmicronTests
//
//  Created by Sergio Flores Ramirez on 28/03/22.
//  Copyright © 2022 Diego Cárcamo. All rights reserved.
//

import XCTest
import RxSwift
import Resolver
import Moya
@testable import OmicronLab
class ComponentsTestExtension: XCTestCase {
    // MARK: - VARIABLES
    var disposeBag: DisposeBag?
    var componentsViewModel: ComponentsViewModel?
    var provider: MoyaProvider<ApiService>!
    var statusCode = 200
    var testData = Data()
    var order1: Order?

    @Injected var networkManager: NetworkManager
    @Injected var inboxViewModel: InboxViewModel

    override func setUpWithError() throws {
        disposeBag = DisposeBag()
        componentsViewModel = ComponentsViewModel()
        order1 = Order(
            areBatchesComplete: true, productionOrderId: 89284, baseDocument: 60067, container: "",
            tag: "Selecciona una...", plannedQuantity: 1, startDate: "27/08/2020",
            finishDate: "06/09/2020",
            descriptionProduct: "Aceite de Arbol de Te 0.3%, Alantoina 0.3%, Citrico 0.2%, " +
            "Extracto de Te Verde 3%, Extracto de Pepino 3%, Glicerina 3%, Hamamelis 3%, Hialuronico 3%, " +
            "Menta Piperita 0.02%, Niacinamida 2%, Pantenol 0.5%,  Salicilico 0.5%, Urea 5%, Solucion",
            statusId: 1, itemCode: "3264   120 ML", productCode: "3264", destiny: "Foráneo",
            hasMissingStock: false, finishedLabel: false, patientName: "Ejemplo",
            clientDxp: "clientDxp", shopTransaction: "", qfbName: "", technicalSign: true)
        provider = MoyaProvider<ApiService>(
            endpointClosure: customEndpointClosure,
            stubClosure: MoyaProvider.immediatelyStub)
    }

    override func tearDownWithError() throws {
        disposeBag = nil
        componentsViewModel = nil
    }

    func customEndpointClosure(_ target: ApiService) -> Endpoint {
        return Endpoint(url: URL(target: target).absoluteString,
                        sampleResponseClosure: { .networkResponse(self.statusCode, self.testData) },
                        method: target.method,
                        task: target.task,
                        httpHeaderFields: target.headers)
    }

    func testSaveDidTapOrderIsNil() {
        let values = ComponentFormValues(baseQuantity: 2,
                                         requiredQuantity: 1,
                                         warehouse: "MG")
        let componentSelected = ComponentO()
        componentsViewModel?.saveSuccess.subscribe(onNext: { _ in
            XCTAssertTrue(true)
        }).disposed(by: disposeBag!)
        order1?.productionOrderId = nil
        inboxViewModel.selectedOrder = order1
        componentsViewModel?.selectedComponent.onNext(componentSelected)
        componentsViewModel?.saveDidTap.onNext(values)
    }

    func testGetMostComponetService() {
        componentsViewModel?.bindingData.subscribe(onNext: { res in
            if res.count == 2 {
                XCTAssertEqual(res.count, 2)
                XCTAssertEqual(res[0].productId, "EN-002")
                XCTAssertEqual(res[0].description, "Airless AcrÝlico con Tapa y Base Plateada 30 ml")
                XCTAssertEqual(res[1].productId, "EN-006")
                XCTAssertEqual(res[1].description, "Airless Pump Star 150 ml Blanco")
            }
        }).disposed(by: disposeBag!)
        componentsViewModel?.getMostCommonComponentsService()
    }

    func testGetMostComponetServiceWhenCodeIs500() {
        componentsViewModel?.dataError.subscribe(onNext: { res in
            XCTAssertEqual(res, Constants.Errors.errorData.rawValue)
        }).disposed(by: disposeBag!)
        statusCode = 500
        componentsViewModel?.networkManager = NetworkManager(provider: provider)
        componentsViewModel?.getMostCommonComponentsService()
    }

}
