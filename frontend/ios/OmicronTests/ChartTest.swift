//
//  ChartTest.swift
//  OmicronTests
//
//  Created by Vicente Cantú on 23/09/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import XCTest
import RxSwift
import Resolver
import Moya

@testable import Magistrales

class ChartTest: XCTestCase {

    // MARK: - Variables
    var chartViewModel: ChartViewModel?
    var disposeBag: DisposeBag?
    var provider: MoyaProvider<ApiService>!
    var statusCode = 200
    var testData = Data()

    @Injected var networkManager: NetworkManager
    override func setUp() {
        chartViewModel = ChartViewModel()
        disposeBag = DisposeBag()
        provider = MoyaProvider<ApiService>(
            endpointClosure: customEndpointClosure,
            stubClosure: MoyaProvider.immediatelyStub)

    }

    func customEndpointClosure(_ target: ApiService) -> Endpoint {
        return Endpoint(url: URL(target: target).absoluteString,
                        sampleResponseClosure: { .networkResponse(self.statusCode, self.testData) },
                        method: target.method,
                        task: target.task,
                        httpHeaderFields: target.headers)
    }

    override func tearDown() {
        chartViewModel = nil
        disposeBag = nil
    }

    fileprivate let fini =
            UtilsManager.shared.formattedDateToString(date: Date().startOfMonth)
            + "-"
            + UtilsManager.shared.formattedDateToString(date: Date().endOfMonth)
    fileprivate let userId = "d125566b-6321-4854-9a42-10fb5c5e4cc1"

    // MARK: - Test Functions
    func testGetWorkloadsSuccess() {
        chartViewModel?.start.subscribe(onNext: { isStart in
            XCTAssertTrue(isStart)
        }).disposed(by: disposeBag!)

        chartViewModel?.networkManager = NetworkManager(provider: provider)
        chartViewModel?.getWorkloads()
    }

    func testGetWorkloadFailed() {
        chartViewModel?.alert.subscribe(onNext: { alert in
            XCTAssertEqual(alert.title, Constants.Errors.errorTitle.rawValue)
            XCTAssertEqual(alert.msg, Constants.Errors.errorData.rawValue)
        }).disposed(by: disposeBag!)
        statusCode = 500
        chartViewModel?.networkManager = NetworkManager(provider: provider)
        chartViewModel?.getWorkloads()
    }
}
