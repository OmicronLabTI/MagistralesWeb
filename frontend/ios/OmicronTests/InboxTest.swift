//
//  InboxTest.swift
//  OmicronTests
//
//  Created by Axity on 06/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import XCTest
import RxSwift
import Moya

@testable import Omicron

class InboxTest:  XCTestCase {
    override func setUpWithError() throws {
        // Put setup code here. This method is called before the invocation of each test method in the class.
    }

    override func tearDownWithError() throws {
        // Put teardown code here. This method is called after the invocation of each test method in the class.
    }
    
    // MARK: - VARIABLES
    let disposeBag = DisposeBag()
    let viewModel = LoginViewModel()
    let networkManager = NetworkManager(provider: MoyaProvider<ApiService>(stubClosure: MoyaProvider.immediatelyStub))
    
    // MARK: - TEST FUNCTIONS
    
//    func testChangeStatus() -> Void {
//        let changesStatusRequest: [ChangeStatusRequest] = []
//        networkManager.changeStatusOrder(changeStatusRequest: changesStatusRequest).subscribe(onNext: { res in
//            XCTAssertNotNil(res)
//            XCTAssertNotNil(res.response)
//            XCTAssertTrue(res.code == 200)
//            XCTAssertTrue(res.response?.userId == "dd4b9bab-e2e8-44a2-af87-8eda8cb510cb")
//        }).disposed(by: self.disposeBag)
//    }
}
