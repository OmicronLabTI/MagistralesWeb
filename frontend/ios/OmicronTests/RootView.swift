//
//  RootView.swift
//  OmicronTests
//
//  Created by Axity on 06/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import XCTest
import RxSwift
import Moya

@testable import Omicron


class RootView:  XCTestCase {
    
    override func setUpWithError() throws {
        // Put setup code here. This method is called before the invocation of each test method in the class.
    }

    override func tearDownWithError() throws {
        // Put teardown code here. This method is called after the invocation of each test method in the class.
    }
    
    // MARK: - VARIABLES
    let networkManager = NetworkManager(provider: MoyaProvider<ApiService>(stubClosure: MoyaProvider.immediatelyStub))
    let disposeBag = DisposeBag()
    let rootViewModel = RootViewModel()

    // MARK: - TEST FUNCTIONS
    func testGetStatusListServiceValid() -> Void {
        self.networkManager.getStatusList(userId: "dd4b9bab-e2e8-44a2-af87-8eda8cb510cb").subscribe(onNext: { res in
            XCTAssertNotNil(res)
            XCTAssertNotNil(res.response)
            XCTAssertTrue(res.code == 200)
            XCTAssertTrue((res.response?.status!.count)! > 0)
        }).disposed(by: self.disposeBag)
    }
}
