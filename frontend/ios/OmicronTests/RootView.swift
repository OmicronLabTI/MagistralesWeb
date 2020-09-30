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


class RootViewTest:  XCTestCase {
        
    // MARK: - VARIABLES
    var networkManager: NetworkManager?
    var disposeBag: DisposeBag?
    var rootViewModel: RootViewModel?
    
    override func setUp() {
        print("XXXX setUp RootViewTest")
        networkManager = NetworkManager(provider: MoyaProvider<ApiService>(stubClosure: MoyaProvider.immediatelyStub))
        disposeBag = DisposeBag()
        rootViewModel = RootViewModel()
    }
    
    override func tearDown() {
        print("XXXX tearDown RootViewTest")
        networkManager = nil
        disposeBag = nil
        rootViewModel = nil
    }

    // MARK: - TEST FUNCTIONS
    func testGetStatusListServiceValid() -> Void {
        self.networkManager!.getStatusList(userId: "dd4b9bab-e2e8-44a2-af87-8eda8cb510cb").subscribe(onNext: { res in
            XCTAssertNotNil(res)
            XCTAssertNotNil(res.response)
            XCTAssertTrue(res.code == 200)
            XCTAssertTrue((res.response?.status!.count)! > 0)
        }).disposed(by: self.disposeBag!)
    }
    
    func testSearchFilterShoudBeText() -> Void {
        // Given
        self.rootViewModel!.searchFilter.onNext("89")
        
        // When
        self.rootViewModel!.searchFilter.subscribe(onNext: { res in
            // Then
            XCTAssertTrue(res == "89")
        }).disposed(by: self.disposeBag!)
    }
    
    func testSearchFilterShouldBeEmpty() -> Void {
        self.rootViewModel!.searchFilter.onNext("")
        self.rootViewModel!.searchFilter.subscribe(onNext: { res in
            XCTAssertTrue(res == "")
        }).disposed(by: self.disposeBag!)
    }
    
    func testSearchFilterShouldBeEmptyWhenInputIsNotNumber() -> Void {
        self.rootViewModel!.searchFilter.onNext("sdf")
        self.rootViewModel!.searchFilter.subscribe(onNext: { res in
            XCTAssertTrue(res == "")
        }).disposed(by: self.disposeBag!)
    }
    
    
    func testResetFilterValueShouldBeNil() -> Void {
        self.rootViewModel!.dataFilter.subscribe(onNext: { res in
            XCTAssertNil(res)
        }).disposed(by: self.disposeBag!)
        self.rootViewModel!.resetFilter()
    }
    
}
