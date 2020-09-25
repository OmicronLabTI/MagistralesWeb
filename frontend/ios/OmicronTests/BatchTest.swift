//
//  BatchTest.swift
//  OmicronTests
//
//  Created by Axity on 24/09/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import XCTest
import RxSwift
import Moya
import Resolver

@testable import Omicron

class BatchesTest: XCTestCase {
    override func setUpWithError() throws {
        // Put setup code here. This method is called before the invocation of each test method in the class.
    }

    override func tearDownWithError() throws {
        // Put teardown code here. This method is called after the invocation of each test method in the class.
    }
    
    // MARK: -VARIABLES
    @Injected var lotsViewModel: LotsViewModel
    let disposeBag = DisposeBag()
    let networkManager = NetworkManager(provider: MoyaProvider<ApiService>(stubClosure: MoyaProvider.immediatelyStub))
    let orderId = 0
    
    // MARK: -TEST FUNCTIONS
    
    func testGetLotsSuccess() -> Void {
        
        // When
        networkManager.getLots(orderId: orderId).subscribe(onNext: { res in
            // Then
            XCTAssertNotNil(res)
            XCTAssertTrue(res.response!.count > 0)
        }).disposed(by: self.disposeBag)
    }
    
    func testAssingBatyches() -> Void {
        // Given
        var batchesToSend:[BatchSelected] = []
        let batch = BatchSelected(orderId: 89956, assignedQty: 0.257895, batchNumber: "337-19", itemCode: "MP-109", action: "insert", sysNumber: 54, expiredBatch: false)
        batchesToSend.append(batch)
        
        // When
        self.networkManager.assignLots(lotsRequest: batchesToSend).subscribe(onNext: { res in
            
            // Then
            XCTAssertNotNil(res)
        }).disposed(by: self.disposeBag)
    }
    
    func testCalculateExpiredBatchShoudBeFalse() -> Void {
        // Given
        let dateTest = "30/11/2020"
        
        // When
        let result = self.lotsViewModel.calculateExpiredBatch(date: dateTest)
        
        // Then
        XCTAssertFalse(result)
        
    }
    
    func testCalculateExpiredBatchShoudBeFalseWithNilValue() -> Void {
        // Given
        let dateTest: String? = nil
        
        // When
        let result = self.lotsViewModel.calculateExpiredBatch(date: dateTest)
        
        // Then
        XCTAssertFalse(result)
    }
    
    func testFinishOrderDidTap() -> Void {
        self.lotsViewModel.askIfUserWantToFinalizeOrder.subscribe(onNext:{ message in
            XCTAssertTrue(message == "¿Deseas terminar la orden?")
        }).disposed(by: self.disposeBag)
    }
    
    func testValidIfOrderCanBeFinalizedNotNull() {
        
        NetworkManager.shared.askIfOrderCanBeFinalized(orderId: self.orderId).subscribe(onNext: { res in
            XCTAssertNotNil(res)
        }).disposed(by: self.disposeBag)
        
    }
    
    func testValidIfOrderCanBeFinalizedValidCode() {
        
        NetworkManager.shared.askIfOrderCanBeFinalized(orderId: self.orderId).subscribe(onNext: { res in
            XCTAssertTrue(res.code == 200)
        }).disposed(by: self.disposeBag)
        
    }
    
    func testValidIfOrderCanBeFinalizedValidResponseNotNull() {
        
        NetworkManager.shared.askIfOrderCanBeFinalized(orderId: self.orderId).subscribe(onNext: { res in
            XCTAssertNotNil(res.response)
        }).disposed(by: self.disposeBag)
        
    }
    
}

