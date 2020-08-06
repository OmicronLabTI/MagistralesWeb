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
    
    func selectSectionNotNil() {
        let inboxViewModel = InboxViewModel()
        
        
        let testToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJwcm9maWxlIjoiYWRtaW4iLCJleHAiOjE1OTY0NzM4ODAsInVzZXIiOiJzZXJjaCJ9.v3RAx7cmoBUXq8WexeGTux-1-qy_wYM-JCLmVzpsCRY"
        let disposeBag = DisposeBag()
        let viewModel = LoginViewModel()
        viewModel.username.onNext("sergio")
        viewModel.password.onNext("Passw0rd")
        viewModel.canLogin.asObservable().subscribe(onNext: { valid in
            XCTAssertTrue(valid, testToken)
            }).disposed(by: disposeBag)
    }
}
