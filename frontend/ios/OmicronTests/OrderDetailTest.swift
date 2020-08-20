//
//  OrderDetailTest.swift
//  OmicronTests
//
//  Created by Axity on 19/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import  XCTest
import  RxSwift
import Moya

@testable import Omicron

class OrderDetailTest: XCTestCase {
    override func setUpWithError() throws {
        // Put setup code here. This method is called before the invocation of each test method in the class.
    }

    override func tearDownWithError() throws {
        // Put teardown code here. This method is called after the invocation of each test method in the class.
    }
    
    // MARK: -VARIABLES
    let disposeBag = DisposeBag()
    let viewModel = LoginViewModel()
    let networkManager = NetworkManager(provider: MoyaProvider<ApiService>(stubClosure: MoyaProvider.immediatelyStub))
    
    // TEST FUNCTIONS
    func testGetOrderDetail() -> Void {
        let orderId = 89026
        self.networkManager.getOrdenDetail(orderId: orderId).subscribe(onNext: { res in
            XCTAssertNotNil(res)
            XCTAssertTrue(res.code == 200)
            XCTAssertNotNil(res.response)
            XCTAssertTrue(res.response?.productionOrderID == 89026)
            XCTAssertTrue((res.response?.details!.count)! > 0)
        }).disposed(by: self.disposeBag)
    }
}
