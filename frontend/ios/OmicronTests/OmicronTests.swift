//
//  OmicronTests.swift
//  OmicronTests
//
//  Created by Diego Cárcamo on 09/07/2020.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import XCTest
import RxSwift
import Moya

@testable import Omicron

class OmicronTests: XCTestCase {
    override func setUpWithError() throws {
        // Put setup code here. This method is called before the invocation of each test method in the class.
    }

    override func tearDownWithError() throws {
        // Put teardown code here. This method is called after the invocation of each test method in the class.
    }
    
    func testLoginValid() {
        let testToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJwcm9maWxlIjoiYWRtaW4iLCJleHAiOjE1OTY0NzM4ODAsInVzZXIiOiJzZXJjaCJ9.v3RAx7cmoBUXq8WexeGTux-1-qy_wYM-JCLmVzpsCRY"
        let disposeBag = DisposeBag()
        let viewModel = LoginViewModel()
        viewModel.username.onNext("sergio")
        viewModel.password.onNext("Passw0rd")
        viewModel.canLogin.asObservable().subscribe(onNext: { valid in
            XCTAssertTrue(valid, testToken)
            }).disposed(by: disposeBag)
    }
    
    func testLoginNotValid() {
        let disposeBag = DisposeBag()
        let viewModel = LoginViewModel()
        viewModel.canLogin.asObservable().subscribe(onNext: { valid in
            XCTAssertFalse(valid)
            }).disposed(by: disposeBag)
    }
    
    func testLoginService() {
        let disposeBag = DisposeBag()
        let data = Login(username: "serch", password: "Password", redirectUri: "", clientId2: "")
        let manager = NetworkManager(provider: MoyaProvider<ApiService>(stubClosure: MoyaProvider.immediatelyStub))
        manager.login(data: (data)).subscribe(onNext: { res in
            XCTAssertNotNil(res.access_token)
            XCTAssertEqual(res.access_token, "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJwcm9maWxlIjoiYWRtaW4iLCJleHAiOjE1OTY0NzM4ODAsInVzZXIiOiJzZXJjaCJ9.v3RAx7cmoBUXq8WexeGTux-1-qy_wYM-JCLmVzpsCRY")
        }).disposed(by: disposeBag)
    }
}
