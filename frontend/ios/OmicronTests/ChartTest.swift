//
//  ChartTest.swift
//  OmicronTests
//
//  Created by Vicente Cantú on 23/09/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import XCTest
import RxSwift
import Moya

@testable import Omicron

class ChartTest: XCTestCase {

    override func setUpWithError() throws {
        // Put setup code here. This method is called before the invocation of each test method in the class.
    }

    override func tearDownWithError() throws {
        // Put teardown code here. This method is called after the invocation of each test method in the class.
    }
    
    // MARK: - Variables
    let networkManager = NetworkManager(provider: MoyaProvider<ApiService>(stubClosure: MoyaProvider.immediatelyStub))
    let chartViewModel = ChartViewModel()
    let disposeBag = DisposeBag()
    
    fileprivate let fini =
            UtilsManager.shared.formattedDateToString(date: Date().startOfMonth)
            + "-"
            + UtilsManager.shared.formattedDateToString(date: Date().endOfMonth)
    fileprivate let userId = "d125566b-6321-4854-9a42-10fb5c5e4cc1"
    
    // MARK: - Test Functions
    
    func testValidResponse() {
        NetworkManager
            .shared
        .getWordLoad(data: WorkloadRequest(fini: fini, qfb: userId))
            .subscribe(onNext: { workloadResponse in
                XCTAssertNotNil(workloadResponse.response)
            }).disposed(by: disposeBag)
    }
    
    func testValidCodeNotNull() {
        NetworkManager
            .shared
        .getWordLoad(data: WorkloadRequest(fini: fini, qfb: userId))
            .subscribe(onNext: { workloadResponse in
                XCTAssertNotNil(workloadResponse.code)
            }).disposed(by: disposeBag)
    }
    
    func testValidCode() {
        NetworkManager
            .shared
        .getWordLoad(data: WorkloadRequest(fini: fini, qfb: userId))
            .subscribe(onNext: { workloadResponse in
                XCTAssert(workloadResponse.code == 200)
            }).disposed(by: disposeBag)
    }

}
