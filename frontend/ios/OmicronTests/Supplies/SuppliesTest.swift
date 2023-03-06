//
//  SupplieTest.swift
//  OmicronTests
//
//  Created by Daniel Vargas on 06/03/23.
//  Copyright © 2023 Diego Cárcamo. All rights reserved.
//

import XCTest
import RxSwift
import Moya
import Resolver

@testable import OmicronLab

class SupplieTest: XCTestCase {
    var supplieViewModel: SupplieViewModel?
    var disposeBag: DisposeBag?
    var provider: MoyaProvider<ApiService>!
    var statusCode = 200
    var testData = Data()
    @Injected var networkManager: NetworkManager
    override func setUp() {
        supplieViewModel = SupplieViewModel()
        disposeBag = DisposeBag()
        provider = MoyaProvider<ApiService>(
            endpointClosure: customEndpointClosure,
            stubClosure: MoyaProvider.immediatelyStub)
    }
    override func tearDown() {
        supplieViewModel = nil
        disposeBag = nil
    }
    func customEndpointClosure(_ target: ApiService) -> Endpoint {
        return Endpoint(url: URL(target: target).absoluteString,
                        sampleResponseClosure: { .networkResponse(self.statusCode, self.testData) },
                        method: target.method,
                        task: target.task,
                        httpHeaderFields: target.headers)
    }

    // MARK: - TEST FUNCTIONS
    func testAddValidComponent() {
        self.supplieViewModel!.supplieList = []
        self.supplieViewModel!.isSendToStoreEnabled.subscribe(onNext: { isEnabled in
            XCTAssertFalse(isEnabled == true)
            XCTAssertTrue(self.supplieViewModel?.supplieList.count == 1)
        }).disposed(by: self.disposeBag!)
        let supplie = ComponentO()
        supplie.productId = "12345 Item"
        self.supplieViewModel?.addComponent.onNext(supplie)
    }
    func testAddInvalidComponent() {
        let itemCode = "1234 item"
        let supplieAdded = Supplie()
        supplieAdded.productId = itemCode
        self.supplieViewModel!.supplieList = []
        self.supplieViewModel?.supplieList.append(supplieAdded)

        self.supplieViewModel!.showSuccessAlert.subscribe(onNext: { res in
            XCTAssertTrue(res.msg == "El componente \(itemCode) ya existe para esta solicitud")
            XCTAssertTrue(self.supplieViewModel?.supplieList.count == 1)
        }).disposed(by: self.disposeBag!)

        let supplie = ComponentO()
        supplie.productId = itemCode
        self.supplieViewModel?.addComponent.onNext(supplie)
    }
    func testValidateChangeQuantityPieces() {
        let itemCode = "item 123"
        let supplie = Supplie()
        supplie.productId = itemCode
        supplie.requestQuantity = 0
        self.supplieViewModel!.supplieList = []
        self.supplieViewModel?.supplieList.append(supplie)
        self.supplieViewModel!.isSendToStoreEnabled.subscribe(onNext: { isEnabled in
            XCTAssertTrue(isEnabled == true)
        }).disposed(by: self.disposeBag!)
        self.supplieViewModel?.changeQuantityPieces(itemCode: itemCode,
                                                    quantity: 1000)
    }
    func testDeleteComponentsAdd() {
        self.supplieViewModel!.selectedButtonIsEnable.subscribe(onNext: { isEnabled in
            XCTAssertTrue(isEnabled == true)
            XCTAssertTrue(self.supplieViewModel!.selectedComponentsToDelete.count == 1)
        }).disposed(by: disposeBag!)

        let itemCode = "12345"
        self.supplieViewModel!.selectedComponentsToDelete = []
        self.supplieViewModel!.validateItemsToDelete(itemCode: itemCode)
    }
    func testDeleteComponentsDelete() {
        self.supplieViewModel!.selectedButtonIsEnable.subscribe(onNext: { isEnabled in
            XCTAssertTrue(isEnabled == false)
            XCTAssertTrue(self.supplieViewModel!.selectedComponentsToDelete.count == 0)
        }).disposed(by: disposeBag!)

        let itemCode = "12345"
        self.supplieViewModel!.selectedComponentsToDelete = [itemCode]
        self.supplieViewModel!.validateItemsToDelete(itemCode: itemCode)
    }
    func testValidateExistsInList() {
        let itemCode = "12345"
        self.supplieViewModel!.selectedComponentsToDelete = [itemCode]
        XCTAssertTrue(self.supplieViewModel!.validateExistsInList(itemCode: itemCode))
    }
    func testDeleteComponents() {
        let supplie1 = Supplie()
        supplie1.productId = "123456"
        let supplie2 = Supplie()
        supplie2.productId = "6523213"

        self.supplieViewModel!.supplieList = [supplie1, supplie2]
        self.supplieViewModel!.selectedComponentsToDelete = ["123456"]
        self.supplieViewModel!.deleteComponents.subscribe(onNext: { _ in
            XCTAssertTrue(self.supplieViewModel!.supplieList.count == 1)
        }).disposed(by: disposeBag!)
        self.supplieViewModel!.deleteComponents.onNext(())
    }
}
