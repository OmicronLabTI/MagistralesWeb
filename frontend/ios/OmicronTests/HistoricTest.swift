//
//  HistoricTest.swift
//  Omicron
//
//  Created by Josue Castillo on 24/09/25.
//  Copyright © 2025 Diego Cárcamo. All rights reserved.
//

import XCTest
import RxSwift
import Resolver
import Moya

@testable import Magistrales

class HistoricTest: XCTestCase {
    var historicViewModel: HistoricViewModel?
    
    var provider: MoyaProvider<ApiService>!
    var disposeBag: DisposeBag?
    
    var testData = Data()
    var statusCode = 200
    var dataRequest: HistoricRequestModel?
    
    @Injected var networkManager: NetworkManager
    
    override func setUp() {
        historicViewModel = HistoricViewModel()
        disposeBag = DisposeBag()
        provider = MoyaProvider<ApiService>(
            endpointClosure: customEndpointClosure,
            stubClosure: MoyaProvider.immediatelyStub)
        dataRequest = HistoricRequestModel(
            qfbId: "d125566b-6321-4854-9a42-10fb5c5e4cc1",
            orders: "102920",
            offset: 0,
            limit: 10
        )
    }
    
    func customEndpointClosure(_ target: ApiService) -> Endpoint {
        return Endpoint(url: URL(target: target).absoluteString,
                        sampleResponseClosure: { .networkResponse(self.statusCode, self.testData) },
                        method: target.method,
                        task: target.task,
                        httpHeaderFields: target.headers)
    }
    
    
    
    override func tearDown() {
        historicViewModel = nil
        disposeBag = nil
        dataRequest = nil
    }
    
    func testSearchDidTapBinding() {
        let expectation = XCTestExpectation(description: "Se introdujo ordenes que cumplen con lo pedido")
        var textToSearch: String?
        
        historicViewModel?.searchDidTap.withLatestFrom(historicViewModel!.searchFilter).subscribe(onNext: { text in
            textToSearch = text
            expectation.fulfill()
        }).disposed(by: disposeBag!)
        
        historicViewModel?.searchFilter.onNext("101010")
        historicViewModel?.searchDidTap.onNext(())
        XCTAssertEqual(historicViewModel?.dataOffset, 0)
    }
    
    func testUpdateData () {
        historicViewModel?.updateData(isRefresh: true)
        XCTAssertTrue(historicViewModel?.dataOffset == 0)
    }
    
    func testGetHistoricData() {
        historicViewModel?.getHistoricData(orders: "", offset: 0, limit: 10)
        XCTAssertTrue(historicViewModel?.orders.count == 1)
    }
}
