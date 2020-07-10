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
        let disposeBag = DisposeBag()
        let viewModel = LoginViewModel()
        viewModel.username.onNext("admin")
        viewModel.password.onNext("12345")
        viewModel.canLogin.asObservable().subscribe(onNext: { valid in
            XCTAssertTrue(valid)
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
        let data = Login(username: "admin", password: "12345")
        let manager = NetworkManager(provider: MoyaProvider<ApiService>(stubClosure: MoyaProvider.immediatelyStub))
        manager.login(data: (data)).subscribe(onNext: { res in
            XCTAssertNotNil(res.token)
            XCTAssertEqual(res.token, "12345")
        }).disposed(by: disposeBag)
    }
}
