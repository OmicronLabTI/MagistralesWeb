//
//  OrderDetailTestExtension.swift
//  OmicronTests
//
//  Created by Sergio Flores Ramirez on 28/03/22.
//  Copyright © 2022 Diego Cárcamo. All rights reserved.
//

import XCTest
import RxSwift
import Resolver
import Moya
@testable import Magistrales
class OrderDetailTestExtension: XCTestCase {
    var disposeBag: DisposeBag?
    var viewModel: LoginViewModel?
    var orderDetailViewModel: OrderDetailViewModel?
    var provider: MoyaProvider<ApiService>!
    var statusCode = 200
    var testData = Data()

    @Injected var networkManager: NetworkManager

    override func setUpWithError() throws {
        disposeBag = DisposeBag()
        viewModel = LoginViewModel()
        orderDetailViewModel = OrderDetailViewModel()
        provider = MoyaProvider<ApiService>(
            endpointClosure: customEndpointClosure,
            stubClosure: MoyaProvider.immediatelyStub)
    }

    override func tearDownWithError() throws {
        disposeBag = nil
        viewModel = nil
        orderDetailViewModel = nil
    }

    func customEndpointClosure(_ target: ApiService) -> Endpoint {
        return Endpoint(url: URL(target: target).absoluteString,
                        sampleResponseClosure: { .networkResponse(self.statusCode, self.testData) },
                        method: target.method,
                        task: target.task,
                        httpHeaderFields: target.headers)
    }

    // MARK: - TEST FUNCTIONS
    func testGetDataTableToEditSuccess() {
        networkManager.getOrdenDetail(89025).subscribe(onNext: { [weak self] res in
            self?.orderDetailViewModel?.tempOrderDetailData = res.response
            let data = self?.orderDetailViewModel?.getDataTableToEdit()
            XCTAssertEqual(data?.baseDocument, 56701)
            XCTAssertEqual(data?.unit, "Pieza")
        }).disposed(by: self.disposeBag!)
    }
    func testValidSignaturesSuccess() {
        // Given
        self.orderDetailViewModel?.technicalSignatureIsGet = true
        self.orderDetailViewModel?.qfbSignatureIsGet = true
        self.orderDetailViewModel?.backToInboxView.subscribe(onNext: { _ in
            XCTAssert(true)
        }).disposed(by: self.disposeBag!)
        self.orderDetailViewModel?.validSignatures()
    }

    func testValidSignaturesWhenCodeIs500() {
        // Given
        self.orderDetailViewModel?.technicalSignatureIsGet = true
        self.orderDetailViewModel?.qfbSignatureIsGet = true
        self.orderDetailViewModel?.backToInboxView.subscribe(onNext: { _ in
            XCTAssert(true)
        }).disposed(by: self.disposeBag!)
        statusCode = 500
        orderDetailViewModel?.networkManager = NetworkManager(provider: provider)
        self.orderDetailViewModel?.validSignatures()
        orderDetailViewModel?.loading.subscribe(onNext: { _ in
            XCTAssertFalse(false)
        }).disposed(by: disposeBag!)
    }

    func testChangeStatusWhenCodeIs500() {
        orderDetailViewModel?.showAlert.subscribe(onNext: { res in
            XCTAssertEqual(res, CommonStrings.errorToChangeStatus)
        }).disposed(by: disposeBag!)
        statusCode = 500
        orderDetailViewModel?.networkManager = NetworkManager(provider: provider)
        orderDetailViewModel?.changeStatus(actionType: StatusNameConstants.inProcessStatus)
    }

    func testChangeStatusSuccess() {
        networkManager.getOrdenDetail(353435).subscribe(onNext: { [weak self] res in
            let orderDetail = res.response
            self?.orderDetailViewModel?.tempOrderDetailData = orderDetail
            self?.orderDetailViewModel?.changeStatus(actionType: StatusNameConstants.inProcessStatus)
            self?.orderDetailViewModel?.loading.subscribe(onNext: { _ in
                XCTAssertFalse(false)
            }).disposed(by: (self?.disposeBag!)!)
        }).disposed(by: disposeBag!)
    }

    func testTerminateOrChangeStatusOfAnOrderPenddingStatus() {
        // Given
        let actionType = StatusNameConstants.penddingStatus
        networkManager.getOrdenDetail(89076).subscribe(onNext: { [weak self] res in
            let orderDetail = res.response
            self?.orderDetailViewModel?.tempOrderDetailData = orderDetail
            self?.orderDetailViewModel?.backToInboxView.subscribe(onNext: { _ in
                // When
                XCTAssertTrue(true)
            }).disposed(by: (self?.disposeBag)!)
            // Then
            self?.orderDetailViewModel?.terminateOrChangeStatusOfAnOrder(actionType: actionType)
        }).disposed(by: self.disposeBag!)
    }
    func testDeleteItemFromTable() {
        let index = 0
        networkManager.getOrdenDetail(89025).subscribe(onNext: { [weak self] res in
            self?.orderDetailViewModel?.auxTabledata = res.response!.details!
            self?.orderDetailViewModel?.tempOrderDetailData = res.response
            self?.orderDetailViewModel?.tableData.subscribe(onNext: { res in
                if res.count != 0 {
                     XCTAssertEqual(res.count, 4)
                }
            }).disposed(by: (self?.disposeBag)!)
            self?.orderDetailViewModel?.deleteItemFromTable(indexs: [index])
        }).disposed(by: self.disposeBag!)
    }
    func testDeleteItemFromTableWhenCodeIs500() {
        let index = 0
        networkManager.getOrdenDetail(89025).subscribe(onNext: { [weak self] res in
            self?.orderDetailViewModel?.auxTabledata = res.response!.details!
            self?.orderDetailViewModel?.tempOrderDetailData = res.response
            self?.orderDetailViewModel?.tableData.subscribe(onNext: { res in
                if res.count != 0 {
                     XCTAssertEqual(res.count, 4)
                }
            }).disposed(by: (self?.disposeBag)!)
            self?.statusCode = 500
            self?.orderDetailViewModel?.networkManager = NetworkManager(provider: (self?.provider)!)
            self?.orderDetailViewModel?.deleteItemFromTable(indexs: [index])
        }).disposed(by: self.disposeBag!)

        orderDetailViewModel?.showAlert.subscribe(onNext: { res in
            XCTAssertEqual(res, CommonStrings.couldNotDeleteItem)
        }).disposed(by: disposeBag!)
        statusCode = 500
        orderDetailViewModel?.networkManager = NetworkManager(provider: provider)
        orderDetailViewModel?.deleteItemFromTable(indexs: [index])
    }
}
