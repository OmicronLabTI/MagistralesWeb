//
//  CommentsTest.swift
//  OmicronTests
//
//  Created by Vicente Cantú on 23/09/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import XCTest
import RxSwift
import Moya

@testable import Omicron

class CommentsTest: XCTestCase {

    override func setUpWithError() throws {
        // Put setup code here. This method is called before the invocation of each test method in the class.
    }

    override func tearDownWithError() throws {
        // Put teardown code here. This method is called after the invocation of each test method in the class.
    }
    
    // MARK: - Variables
    let networkManager = NetworkManager(provider: MoyaProvider<ApiService>(stubClosure: MoyaProvider.immediatelyStub))
    let chartViewModel = CommentsViewModel()
    let disposeBag = DisposeBag()
    
    // MARK: - Test Functions
    
    func testInitData() {
        let productionOrderID = 89623
        let plannedQuantity: Decimal = 1.0
        let fechaFin = UtilsManager.shared.formattedDateFromString(dateString: "10/09/2020", withFormat: "yyyy-MM-dd") ?? ""
        let message = "Comment :D"
        
        let order = OrderDetailRequest(
            fabOrderID: productionOrderID,
            plannedQuantity: plannedQuantity,
            fechaFin: fechaFin,
            comments: message,
            components: [])
        XCTAssertNotNil(order)
        
        testValidResponse(order: order)
        //testValidCodeNotNull(order: order)
        testValidCode(order: order)
    }
    
    func testValidResponse(order: OrderDetailRequest) {
        NetworkManager
               .shared
               .updateDeleteItemOfTableInOrderDetail(orderDetailRequest: order)
               .observeOn(MainScheduler.instance)
               .subscribe(onNext: { res in
               XCTAssertNotNil(res.response)
           }).disposed(by: disposeBag)
    }
    
//    func testValidCodeNotNull(order: OrderDetailRequest) {
//        NetworkManager
//               .shared
//               .updateDeleteItemOfTableInOrderDetail(orderDetailRequest: order)
//               .observeOn(MainScheduler.instance)
//               .subscribe(onNext: { res in
//                XCTAssertNotNil(res.code)
//           }).disposed(by: disposeBag)
//    }
    
    func testValidCode(order: OrderDetailRequest) {
        NetworkManager
               .shared
               .updateDeleteItemOfTableInOrderDetail(orderDetailRequest: order)
               .observeOn(MainScheduler.instance)
               .subscribe(onNext: { res in
                XCTAssertNotNil(res.code == 200)
           }).disposed(by: disposeBag)
    }

}
