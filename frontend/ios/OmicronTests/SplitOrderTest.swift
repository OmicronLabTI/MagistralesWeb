//
//  SplitOrderTest.swift
//  Omicron
//
//  Created by Josue Castillo on 04/08/25.
//  Copyright © 2025 Diego Cárcamo. All rights reserved.
//

import XCTest
import RxSwift
import Resolver
import Moya

@testable import Magistrales

class SplitOrderTest: XCTestCase {
    
    // MARK: - Variables
    var splitOrderModel: SplitOrderViewModel?
    var disposeBag: DisposeBag?
    var provider: MoyaProvider<ApiService>!
    var statusCode = 200
    var testData = Data()
    var dataRequest: SplitOrderRequest?
    var section = String()
    
    @Injected var networkManager: NetworkManager
    override func setUp() {
        splitOrderModel = SplitOrderViewModel()
        disposeBag = DisposeBag()
        provider = MoyaProvider<ApiService>(
            endpointClosure: customEndpointClosure,
            stubClosure: MoyaProvider.immediatelyStub)
        dataRequest = SplitOrderRequest(
            productionOrderId: 190028,
            pieces: 20,
            userId: "d125566b-6321-4854-9a42-10fb5c5e4cc1",
            dxpOrder: "d125566b-6321-4854-9a42-10fb5c5edcc1",
            sapOrder: 280028,
            totalPieces: 30)
        section = "Reasignado"
    }
    
    func customEndpointClosure(_ target: ApiService) -> Endpoint {
        return Endpoint(url: URL(target: target).absoluteString,
                        sampleResponseClosure: { .networkResponse(self.statusCode, self.testData) },
                        method: target.method,
                        task: target.task,
                        httpHeaderFields: target.headers)
    }
    
    override func tearDown() {
        splitOrderModel = nil
        disposeBag = nil
        dataRequest = nil
        section = String()
    }
    
    // MARK: - TEST FUNCTIONS
    func saveChangesTest() {
        splitOrderModel?.closeModal.subscribe(onNext: { mssg in
            XCTAssertTrue(mssg == CommonStrings.succesSplitOrder)
        }).disposed(by: disposeBag!)
        
        splitOrderModel?.networkManager = NetworkManager(provider: provider)
        splitOrderModel?.saveChanges(dataRequest!, section: section)
    }
    
}
