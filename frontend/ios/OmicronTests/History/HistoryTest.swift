//
//  HistoryTest.swift
//  OmicronTests
//
//  Created by Daniel Vargas on 28/03/23.
//  Copyright © 2023 Diego Cárcamo. All rights reserved.
//

import XCTest
import RxSwift
import Moya
import Resolver

@testable import Magistrales

class HistoryTest: XCTestCase {
    var historyViewModel: HistoryViewModel?
    var disposeBag: DisposeBag?
    var provider: MoyaProvider<ApiService>!
    var statusCode = 200
    var testData = Data()
    @Injected var networkManager: NetworkManager
    override func setUp() {
        historyViewModel = HistoryViewModel()
        disposeBag = DisposeBag()
        provider = MoyaProvider<ApiService>(
            endpointClosure: customEndpointClosure,
            stubClosure: MoyaProvider.immediatelyStub)
    }
    override func tearDown() {
        historyViewModel = nil
        disposeBag = nil
    }
    func customEndpointClosure(_ target: ApiService) -> Endpoint {
        return Endpoint(url: URL(target: target).absoluteString,
                        sampleResponseClosure: { .networkResponse(self.statusCode, self.testData) },
                        method: target.method,
                        task: target.task,
                        httpHeaderFields: target.headers)
    }

    func testResetVales() {
        historyViewModel?.resetValues()
        XCTAssertTrue(historyViewModel?.offset == 0)
        XCTAssertTrue(historyViewModel?.historyList.count == 0)
    }
    func testSelectedRangeDateObs() {
        historyViewModel?.offset = 0
        historyViewModel?.historyList = []
        historyViewModel?.selectedHistoryList.subscribe(onNext: { res in
            XCTAssertTrue(res.count == 1)
        }).disposed(by: disposeBag!)
        historyViewModel?.selectedRangeDateObs.onNext((startDate: Date(), endDate: Date()))
    }
    func testSelectedStatus() {
        historyViewModel?.offset = 0
        historyViewModel?.historyList = []
        historyViewModel?.selectedStatus = ["ABIERTO"]
        historyViewModel?.selectedHistoryList.subscribe(onNext: { [weak self] res in
            XCTAssertTrue(self?.historyViewModel?.selectedStatus.count == 2)
        }).disposed(by: disposeBag!)
        historyViewModel?.selectedStatusObs.onNext(["ABIERTO","CANCELADO"])
    }
    func testOnScroll() {
        historyViewModel?.offset = 0
        historyViewModel?.historyList = [
            RawMaterialItem(docEntry: 1,
                            itemCode: String(),
                            description: String(),
                            applicationName: String(),
                            quantity: 1.0,
                            unit: String(),
                            targetWarehosue: String(),
                            docDate: String(),
                            status: String())]
        historyViewModel?.selectedStatus = ["ABIERTO"]
        historyViewModel?.totalData = 40
        historyViewModel?.selectedHistoryList.subscribe(onNext: { res in
            XCTAssertTrue(res.count == 2)
        }).disposed(by: disposeBag!)
        historyViewModel?.onScroll.onNext(())
    }
}
