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

@testable import OmicronLab

class ChartTest: XCTestCase {

    // MARK: - Variables
    var chartViewModel: ChartViewModel?
    var disposeBag: DisposeBag?
    @Injected var networkManager: NetworkManager
    override func setUp() {
        print("XXXX setUp ChartTest")
        chartViewModel = ChartViewModel()
        disposeBag = DisposeBag()
    }
    override func tearDown() {
        print("XXXX tearDown ChartTest")
        chartViewModel = nil
        disposeBag = nil
    }
    fileprivate let fini =
            UtilsManager.shared.formattedDateToString(date: Date().startOfMonth)
            + "-"
            + UtilsManager.shared.formattedDateToString(date: Date().endOfMonth)
    fileprivate let userId = "d125566b-6321-4854-9a42-10fb5c5e4cc1"
    // MARK: - Test Functions
    func testValidResponse() {
        networkManager
        .getWordLoad(data: WorkloadRequest(fini: fini, qfb: userId))
            .subscribe(onNext: { workloadResponse in
                XCTAssertNotNil(workloadResponse.response)
            }).disposed(by: disposeBag!)
    }
    func testValidCodeNotNull() {
        networkManager
        .getWordLoad(data: WorkloadRequest(fini: fini, qfb: userId))
            .subscribe(onNext: { workloadResponse in
                XCTAssertNotNil(workloadResponse.code)
            }).disposed(by: disposeBag!)
    }
    func testValidCode() {
        networkManager
        .getWordLoad(data: WorkloadRequest(fini: fini, qfb: userId))
            .subscribe(onNext: { workloadResponse in
                XCTAssert(workloadResponse.code == 200)
            }).disposed(by: disposeBag!)
    }

}
