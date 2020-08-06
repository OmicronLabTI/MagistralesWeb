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
    private let disposeBag = DisposeBag()
    override func setUpWithError() throws {
        // Put setup code here. This method is called before the invocation of each test method in the class.
    }

    override func tearDownWithError() throws {
        // Put teardown code here. This method is called after the invocation of each test method in the class.
    }
    
    
    func testGetStatusListValid() {
        let qfb = StatusRequest(qfbId: 1)
        NetworkManager.shared.getStatusList(qfbId: qfb).subscribe(onNext: { res in
            XCTAssert(res.status!.count > 0)
        }).disposed(by: self.disposeBag)
    }
    
    func testGetStatusListArgumentsnil() {
        let qfb = StatusRequest(qfbId: 1)
        NetworkManager.shared.getStatusList(qfbId: qfb).subscribe(onNext: { res in
                XCTAssertNotNil(res)
        }).disposed(by: disposeBag)
    }
}
