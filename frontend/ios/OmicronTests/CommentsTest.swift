//
//  CommentsTest.swift
//  OmicronTests
//
//  Created by Vicente Cantú on 23/09/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import XCTest
import RxSwift
import Resolver

@testable import Omicron

class CommentsTest: XCTestCase {

    // MARK: - Variables
    var chartViewModel: CommentsViewModel?
    var disposeBag: DisposeBag?
    var productionOrderID: Int?
    var plannedQuantity: Decimal?
    var fechaFin: String?
    var message: String?
    var order: OrderDetailRequest?
    var expectation: XCTestExpectation?
    @Injected var networkmanager: NetworkManager
    
    override func setUp() {
        chartViewModel = CommentsViewModel()
        disposeBag = DisposeBag()
        productionOrderID = 89623
        plannedQuantity = 1.0
        fechaFin = UtilsManager.shared.formattedDateFromString(dateString: "10/09/2020", withFormat: "yyyy-MM-dd") ?? ""
        message = "Comment :D"
        order = OrderDetailRequest(
            fabOrderID: productionOrderID!,
            plannedQuantity: plannedQuantity!,
            fechaFin: fechaFin!,
            comments: message!,
            components: [])
        expectation = XCTestExpectation()
    }
    
    override func tearDown() {
        chartViewModel = nil
        disposeBag = nil
        productionOrderID = nil
        plannedQuantity = nil
        fechaFin = nil
        message = nil
        order = nil
        expectation = nil
    }
    
    // MARK: - Test Functions
        
    func testValidResponse() -> Void {
        
        self.networkmanager
            .updateDeleteItemOfTableInOrderDetail(orderDetailRequest: self.order!)
            .observeOn(MainScheduler.instance)
            .subscribe(onNext: { [weak self] res in
                XCTAssertNotNil(res.response)
                self?.expectation?.fulfill()
            }).disposed(by: self.disposeBag!)
        wait(for: [self.expectation!], timeout: 1000)
    }
        
    func testValidCode() -> Void {
        self.networkmanager
            .updateDeleteItemOfTableInOrderDetail(orderDetailRequest: self.order!)
            .observeOn(MainScheduler.instance)
            .subscribe(onNext: { [weak self] res in
                XCTAssertNotNil(res.code == 200)
                self?.expectation?.fulfill()
            }).disposed(by: self.disposeBag!)
        wait(for: [self.expectation!], timeout: 1000)
    }
    
}
