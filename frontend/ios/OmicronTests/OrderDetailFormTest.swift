//
//  OrderDetailFormTest.swift
//  OmicronTests
//
//  Created by Axity on 24/09/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import XCTest
import RxSwift
import RxCocoa
import Moya
import Resolver

@testable import Omicron
class OrderDetailFormTest: XCTestCase {
    
    // MARK: -VARIABLES
    let disposeBag = DisposeBag()
    @Injected var orderDetailFormViewModel: OrderDetailFormViewModel
    let networkManager = NetworkManager(provider: MoyaProvider<ApiService>(stubClosure: MoyaProvider.immediatelyStub))
    
    // MARCK: - FUNCTION TEST
    
    func testEditTableSuccessValueNotNill() -> Void  {
        // Given
        self.networkManager.getOrdenDetail(orderId: 89900).subscribe(onNext: { res in
            XCTAssertNotNil(res.response?.details)
        }).disposed(by: self.disposeBag)
    }
    
//    func testEditTableSucess() -> Void {
//        self.networkManager.getOrdenDetail(orderId: 89900).subscribe(onNext: { [weak self] res in
//            // Given
//            let index = 0
//            let baseQuantity = 0.0
//            let requiredQuantity = 0.0
//            let werehouse = "AMP"
//
//            self?.orderDetailFormViewModel.success.subscribe(onNext: { res in
//                print(res)
//
//            }).disposed(by: self!.disposeBag)
//
//            self?.orderDetailFormViewModel.editItemTable(index: index, data: res.response!, baseQuantity: baseQuantity, requiredQuantity: requiredQuantity, werehouse: werehouse)
//        
//
//
//        }).disposed(by: self.disposeBag)
//    }
    
}
