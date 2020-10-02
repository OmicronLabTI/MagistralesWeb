//
//  OmicronTests.swift
//  OmicronTests
//
//  Created by Diego Cárcamo on 09/07/2020.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import XCTest
import RxSwift
import Resolver
import Moya

@testable import Omicron

class LoginTest: XCTestCase {
    
    //MARK: - VARIABLES
    var disposeBag: DisposeBag?
    var loginViewModel: LoginViewModel?
    @Injected var networkManager: NetworkManager
    
    override func setUp() {
        print("XXXX setUp LoginTest")
        self.disposeBag = DisposeBag()
        self.loginViewModel = LoginViewModel()
    }
    
    override func tearDown() {
        print("XXXX tearDown LoginTest")
        self.disposeBag = nil
        self.loginViewModel = nil
    }
    
    //MARK: -TEST FUNCTIONS
    
    func testLoginValid() {
        // Given
        let testToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJwcm9maWxlIjoiYWRtaW4iLCJleHAiOjE1OTY0NzM4ODAsInVzZXIiOiJzZXJjaCJ9.v3RAx7cmoBUXq8WexeGTux-1-qy_wYM-JCLmVzpsCRY"
        self.loginViewModel!.username.onNext("sergio")
        self.loginViewModel!.password.onNext("Passw0rd")
        // Then
        self.loginViewModel!.canLogin.asObservable().subscribe(onNext: { valid in
            // When
            XCTAssertTrue(valid, testToken)
        }).disposed(by: self.disposeBag!)
    }
    
    func testLoginNotValid() {
        // Then
        self.loginViewModel!.canLogin.asObservable().subscribe(onNext: { valid in
            // When
            XCTAssertFalse(valid)
        }).disposed(by: self.disposeBag!)
    }
    
    func testLoginService() -> Void {
        // Given
        let data = Login(username: "serch", password: "Password", redirectUri: "", clientId2: "", origin: "")
        let testToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJwcm9maWxlIjoiYWRtaW4iLCJleHAiOjE1OTY3MzM1NTgsInVzZXIiOiJzZXJnaW8ifQ.W9kstVRF9qm_s2diVt-Ki0xb4FwkXIA0QtEFSDAlXCM"
        // Then
        self.networkManager.login(data: (data)).subscribe(onNext: { res in
            // When
            XCTAssertNotNil(res.accessToken)
            XCTAssertEqual(res.accessToken, testToken)
        }).disposed(by: self.disposeBag!)
    }
    
    func testGetInfoUsers() -> Void {
        // Given
        let username = "sflores"
        // Then
        self.networkManager.getInfoUser(username: username).subscribe(onNext: { res in
            // When
            XCTAssertNotNil(res)
            XCTAssertTrue(res.code == 200)
            XCTAssertTrue(res.response?.id == "dd4b9bab-e2e8-44a2-af87-8eda8cb510cb")
            XCTAssertTrue(res.response?.userName == "sflores")
            XCTAssertTrue(res.response?.firstName == "Sergio")
            XCTAssertTrue(res.response?.lastName == "Flores")
        }).disposed(by: self.disposeBag!)
    }
    
    func testDoLoginSucess() -> Void {
        // Given
        self.loginViewModel?.username.onNext("sflores")
        self.loginViewModel?.password.onNext("Sergio123")
        
        self.loginViewModel?.finishedLogin.subscribe(onNext: { _ in
            // When
            XCTAssert(true)
        }).disposed(by: self.disposeBag!)
        
        // Then
        self.loginViewModel?.loginDidTap.onNext(())
    }
    
//    func dataFail() -> Data {
//        guard let url = Bundle.main.url(forResource: "getComponents", withExtension: "json"),
//            let data = try? Data(contentsOf: url) else {
//                return Data()
//        }
//        return data
//    }
//
//    func testDoLoginUserNotExist() -> Void {
//        // Given
//        self.loginViewModel?.username.onNext("sflor")
//        self.loginViewModel?.password.onNext("Sergio123")
//
////        let customEndpointClosure = { (target: ApiService) -> Endpoint in
////            return Endpoint(url: URL(target: target).absoluteString,
////                            sampleResponseClosure: { .networkResponse(401 , self.dataFail()) },
////                            method: target.method,
////                            task: target.task,
////                            httpHeaderFields: target.headers)
////        }
////
////        let stubbingProvider = MoyaProvider<ApiService>(endpointClosure: customEndpointClosure, stubClosure: MoyaProvider.immediatelyStub)
////
////
////        let ss = NetworkManager(provider: stubbingProvider)
////        ss.getLots(orderId: 123).subscribe(onNext: { res in
////
////        }, onError: {erro in
////
////        }).disposed(by: self.disposeBag!)
//
//        self.loginViewModel?.error.subscribe(onNext: { error in
//            // When
//            XCTAssertEqual(true, true)
//        }).disposed(by: self.disposeBag!)
//
//        // Then
//        self.loginViewModel?.loginDidTap.onNext(())
//    }
//
//    func testDoLoginUserInvalidCredentials() -> Void {
//        // Given
//        self.loginViewModel?.username.onNext("sflores")
//        self.loginViewModel?.password.onNext("Sergio")
//
//        self.loginViewModel?.error.subscribe(onNext: { error in
//            // When
//            XCTAssertEqual(true, true)
//        }).disposed(by: self.disposeBag!)
//
//        // Then
//        self.loginViewModel?.loginDidTap.onNext(())
//    }
//
//    func testLoginFailedServerResponseStatusCode500() -> Void {
//        // Given
//        self.loginViewModel?.username.onNext("sflores")
//        self.loginViewModel?.password.onNext("Sergio213")
//
//        self.loginViewModel?.error.subscribe(onNext: { error in
//            // When
//            XCTAssertEqual(true, true)
//        }).disposed(by: self.disposeBag!)
//
//        // Then
//        self.loginViewModel?.loginDidTap.onNext(())
//    }
//
//
}

