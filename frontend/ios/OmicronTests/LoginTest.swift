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

class LoginTest: XCTestCase {
    override func setUpWithError() throws {
        // Put setup code here. This method is called before the invocation of each test method in the class.
    }

    override func tearDownWithError() throws {
        // Put teardown code here. This method is called after the invocation of each test method in the class.
    }
    
    //MARK: - VARIABLES
    let disposeBag = DisposeBag()
    let viewModel = LoginViewModel()
    let networkManager = NetworkManager(provider: MoyaProvider<ApiService>(stubClosure: MoyaProvider.immediatelyStub))
    
    //MARK: -TEST FUNCTIONS
    
    func testLoginValid() {
        let testToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJwcm9maWxlIjoiYWRtaW4iLCJleHAiOjE1OTY0NzM4ODAsInVzZXIiOiJzZXJjaCJ9.v3RAx7cmoBUXq8WexeGTux-1-qy_wYM-JCLmVzpsCRY"
        self.viewModel.username.onNext("sergio")
        self.viewModel.password.onNext("Passw0rd")
        self.viewModel.canLogin.asObservable().subscribe(onNext: { valid in
            XCTAssertTrue(valid, testToken)
        }).disposed(by: self.disposeBag)
    }
    
    func testLoginNotValid() {
        self.viewModel.canLogin.asObservable().subscribe(onNext: { valid in
            XCTAssertFalse(valid)
        }).disposed(by: self.disposeBag)
    }
    
    func testLoginService() -> Void {
        let data = Login(username: "serch", password: "Password", redirectUri: "", clientId2: "", origin: "")
        let testToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJwcm9maWxlIjoiYWRtaW4iLCJleHAiOjE1OTY3MzM1NTgsInVzZXIiOiJzZXJnaW8ifQ.W9kstVRF9qm_s2diVt-Ki0xb4FwkXIA0QtEFSDAlXCM"
            self.networkManager.login(data: (data)).subscribe(onNext: { res in
            XCTAssertNotNil(res.access_token)
            XCTAssertEqual(res.access_token, testToken)
            }).disposed(by: self.disposeBag)
    }
    
    func testGetInfoUsers() -> Void {
        let username = "sflores"
        self.networkManager.getInfoUser(username: username).subscribe(onNext: { res in
            XCTAssertNotNil(res)
            XCTAssertTrue(res.code == 200)
            XCTAssertTrue(res.response?.id == "dd4b9bab-e2e8-44a2-af87-8eda8cb510cb")
            XCTAssertTrue(res.response?.userName == "sflores")
             XCTAssertTrue(res.response?.firstName == "Sergio")
             XCTAssertTrue(res.response?.lastName == "Flores")
        }).disposed(by: self.disposeBag)
    }
}


    
//    func testRenewService() {
//        let disposeBag = DisposeBag()
//        let data = Renew(refresh_token: "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJjbGllbnRJZCI6IiIsInByb2ZpbGUiOiJhZG1pbiIsImV4cCI6MTU5NjgyNjU2MiwidXNlciI6Imd1eiJ9.2O4TsKp1uGqBRJ5dobk7xZHsSe5TvXVhxRTPu0oviYY")
//        let manager = NetworkManager(provider: MoyaProvider<ApiService>(stubClosure: MoyaProvider.immediatelyStub))
//        manager.renew(data: data).subscribe(onNext: { res in
//            XCTAssertNotNil(res.access_token)
//            XCTAssertEqual(res.access_token, "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJwcm9maWxlIjoiYWRtaW4iLCJleHAiOjE1OTY1ODE3NTksInVzZXIiOiJzZXJnaW8ifQ.ArIbPJJyUSpEG3Hg9tuw00Z-eE4wtKbmsmzdS0gUuEc")
//        }).disposed(by: disposeBag)
//    }

