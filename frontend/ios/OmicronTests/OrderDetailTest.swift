//
//  OrderDetailTest.swift
//  OmicronTests
//
//  Created by Axity on 19/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import XCTest
import RxSwift
import Resolver

@testable import OmicronLab

class OrderDetailTest: XCTestCase {
    // MARK: - VARIABLES
    var disposeBag: DisposeBag?
    var viewModel: LoginViewModel?
    var orderId: Int?
    var orderDetailViewModel: OrderDetailViewModel?
    @Injected var networkManager: NetworkManager
    override func setUp() {
        print("XXXX setUp OrderDetailTest")
        disposeBag = DisposeBag()
        viewModel = LoginViewModel()
        orderId = 89026
        orderDetailViewModel = OrderDetailViewModel()
    }
    override func tearDown() {
        print("XXXX tearDown OrderDetailTest")
        disposeBag = nil
        viewModel = nil
        orderId = nil
        orderDetailViewModel = nil
    }
    // MARK: - TEST FUNCTIONS
    func testGetOrderDetailNoNull() {
        self.networkManager.getOrdenDetail(orderId: orderId!).subscribe(onNext: { res in
            XCTAssertNotNil(res)
        }).disposed(by: self.disposeBag!)
    }
    func testGetOrderDetailValidCode() {
        self.networkManager.getOrdenDetail(orderId: orderId!).subscribe(onNext: { res in
            XCTAssertTrue(res.code == 200)
        }).disposed(by: self.disposeBag!)
    }
    func testGetOrderDetailResponseNotNull() {
        self.networkManager.getOrdenDetail(orderId: orderId!).subscribe(onNext: { res in
            XCTAssertNotNil(res.response)
        }).disposed(by: self.disposeBag!)
    }
    func testGetOrderDetailValidProductID() {
        self.networkManager.getOrdenDetail(orderId: orderId!).subscribe(onNext: { res in
            XCTAssertTrue(res.response?.productionOrderID == 89026)
        }).disposed(by: self.disposeBag!)
    }
    func testGetOrderDetailValidContainsDetails() {
        self.networkManager.getOrdenDetail(orderId: orderId!).subscribe(onNext: { res in
            XCTAssertTrue((res.response?.details?.count)! > 0)
        }).disposed(by: self.disposeBag!)
    }
    func testFinishOrderSucess() {
        // Given
        let userId = "d125566b-6321-4854-9a42-10fb5c5e4cc1"
        let fabricationOrderId = 89830
        let qfbSignature = "lgfklgl"
        let technicalSignature = "dkdfkdfk"
        let finishOrder = FinishOrder(
            userId: userId, fabricationOrderId: fabricationOrderId,
            qfbSignature: qfbSignature, technicalSignature: technicalSignature)
        // When
        self.networkManager.finishOrder(order: finishOrder).subscribe(onNext: { res in
            // Then
            XCTAssertNotNil(res)
            XCTAssertTrue(res.response!.userId! == userId )
            XCTAssertTrue(89830 == fabricationOrderId)
        }).disposed(by: self.disposeBag!)
    }
    func testDeleteItemOfTableOrderDetailSucces() {
        // Given
        let components = Component(orderFabID: 89838, productId: "MP-024",
                                   componentDescription: "Carbopol Ultrez 21", baseQuantity: 0.012,
                                   requiredQuantity: 0.012, consumed: 0, available: 12.657999999999999,
                                   unit: "KG", warehouse: "MP", pendingQuantity: 0.012, stock: 13.994999999999999,
                                   warehouseQuantity: 12.67, action: "delete")
        let order = OrderDetailRequest(
            fabOrderID: 89838, plannedQuantity: 1, fechaFin: "2020-09-13", comments: "", warehouse: "MP", components: [components])
        // Then
        self.networkManager.updateDeleteItemOfTableInOrderDetail(orderDetailRequest: order).subscribe(onNext: { res1 in
            //When
            XCTAssertNotNil(res1.response)
            XCTAssertTrue(res1.response == "{\"89838-89838\":\"Ok\"}")
        }).disposed(by: self.disposeBag!)
    }
    func testUpdateItemOfTableOrderDetailSucces() {
        // Given
        let components = Component(
            orderFabID: 89838, productId: "MP-024", componentDescription: "Carbopol Ultrez 21", baseQuantity: 0.012,
            requiredQuantity: 0.012, consumed: 0, available: 12.657999999999999, unit: "KG", warehouse: "MP",
            pendingQuantity: 0.012, stock: 13.994999999999999, warehouseQuantity: 12.67, action: "update")
        let order = OrderDetailRequest(
            fabOrderID: 89838, plannedQuantity: 1, fechaFin: "2020-09-13", comments: "", warehouse: "MP", components: [components])
        // Then
        self.networkManager.updateDeleteItemOfTableInOrderDetail(orderDetailRequest: order).subscribe(onNext: { res1 in
            //When
            XCTAssertNotNil(res1.response)
            XCTAssertTrue(res1.response == "{\"89838-89838\":\"Ok\"}")
        }).disposed(by: self.disposeBag!)
    }
    func testSumShouldBe27dot5() {
        // Given
        let orderId = 89026
        // Then
        self.networkManager.getOrdenDetail(orderId: orderId).subscribe(onNext: { res in
        let resOfSum = self.orderDetailViewModel!.sum(tableDetails: (res.response?.details)!)
            // When
            XCTAssertTrue(resOfSum == 27.5)
        }).disposed(by: self.disposeBag!)
    }
    func testValidIfOrderCanBeFinalizedSuccess() {
        // Given
        let orderId = 89026
        // When
        self.networkManager.askIfOrderCanBeFinalized(orderId: orderId).subscribe(onNext: { res in
            // Then
            XCTAssertNotNil(res)
            XCTAssertTrue(res.success == true)
            XCTAssertEqual(res.code, 200)
        }).disposed(by: self.disposeBag!)
    }
    func testChangeStatusOrderPendingSuccess() {
        // Given
        let status = "Pendiente"
        let response = "[{\"Id\":400,\"Userid\":\"d125566b-6321-4854-9a42-10fb5c5e4cc1\",\"Salesorderid\":\"\"," +
        "\"Productionorderid\":\"89628\",\"Status\":\"Proceso\",\"Comments\":null,\"FinishDate\":null," +
        "\"CreationDate\":\"10/09/2020 09:44:31 AM\",\"CreatorUserId\":\"14409829-caa8-42f5-83e8-bc52b1f7afa5\"," +
        "\"CloseDate\":null,\"CloseUserId\":null,\"IsIsolatedProductionOrder\":true,\"IsSalesOrder\":false," +
        "\"IsProductionOrder\":true,\"StatusOrder\":5}]"
        let changeStatus = ChangeStatusRequest(userId: "", orderId: 89026, status: status)
        // When
        networkManager.changeStatusOrder(changeStatusRequest: [changeStatus]).subscribe(onNext: { res in
            // Then
            XCTAssertNotNil(res)
            XCTAssertTrue(res.response == response)
        }).disposed(by: self.disposeBag!)
    }
    func testFinishedButtonDidTapSuccess() {
        self.orderDetailViewModel?.showAlertConfirmation.subscribe(onNext: { res in
            // When
            XCTAssertEqual(res.message, CommonStrings.doYouWantToFinishTheOrder)
            XCTAssertEqual(res.typeOfStatus, StatusNameConstants.finishedStatus)
        }).disposed(by: self.disposeBag!)
        // Then
        self.orderDetailViewModel?.finishedButtonDidTap.onNext(())
    }
    func testProcessButtonDidTapSucess() {
        self.orderDetailViewModel?.showAlertConfirmation.subscribe(onNext: { res in
            // When
            XCTAssertEqual(res.message, CommonStrings.confirmationMessageProcessStatus)
            XCTAssertEqual(res.typeOfStatus, StatusNameConstants.inProcessStatus)
        }).disposed(by: self.disposeBag!)
        // Then
        self.orderDetailViewModel?.processButtonDidTap.onNext(())
    }
    func testPendingButtonDidTapSucess() {
        self.orderDetailViewModel?.showAlertConfirmation.subscribe(onNext: { res in
            // When
            XCTAssertEqual(res.message, CommonStrings.confirmationMessagePendingStatus)
            XCTAssertEqual(res.typeOfStatus, StatusNameConstants.penddingStatus)
        }).disposed(by: self.disposeBag!)
        // Then
        self.orderDetailViewModel?.pendingButtonDidTap.onNext(())
    }
    func testGetOrdendetailSucess() {
        self.orderDetailViewModel?.orderDetailData.subscribe(onNext: { res in
            // When
            if let orderDetail = res.first {
                XCTAssertEqual(orderDetail.baseDocument, 56701)
                XCTAssertEqual(orderDetail.code, "BE 271   4 CAP")
                XCTAssertEqual(orderDetail.details?.count, 5)
                XCTAssertEqual(orderDetail.isChecked, false)
            }
        }).disposed(by: self.disposeBag!)
        // Then
        self.orderDetailViewModel?.getOrdenDetail()
    }
    func testGetOrdendetailSucessIsRefresh() {
        // Given
        let isRefresh = true
        self.orderDetailViewModel?.orderDetailData.subscribe(onNext: { res in
            // When
            if let orderDetail = res.first {
                XCTAssertEqual(orderDetail.baseDocument, 56701)
                XCTAssertEqual(orderDetail.code, "BE 271   4 CAP")
                XCTAssertEqual(orderDetail.details?.count, 5)
                XCTAssertEqual(orderDetail.isChecked, false)
            }
        }).disposed(by: self.disposeBag!)
        // Then
        self.orderDetailViewModel?.getOrdenDetail(isRefresh: isRefresh)
    }
    func testShoulReturnZero() {
        // Given
        let tableDetails: [Detail] = []
        // Then
        let result = self.orderDetailViewModel?.sum(tableDetails: tableDetails)
        // When
        XCTAssertEqual(result, 0.0)
    }
    func testTerminateOrChangeStatusOfAnOrderFinishedStatus() {
        // Given
        let actionType = CommonStrings.finished
        self.orderDetailViewModel?.showSignatureView.subscribe(onNext: { res in
            // When
            XCTAssertEqual(res, CommonStrings.qfbSignature)
        }).disposed(by: self.disposeBag!)
        // Then
        self.orderDetailViewModel?.terminateOrChangeStatusOfAnOrder(actionType: actionType)
    }
    func testTerminateOrChangeStatusOfAnOrderInProcessStatus() {
        // Given
        let actionType = StatusNameConstants.inProcessStatus
        networkManager.getOrdenDetail(orderId: 89076).subscribe(onNext: { [weak self] res in
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
    func testTerminateOrChangeStatusOfAnOrderPenddingStatus() {
        // Given
        let actionType = StatusNameConstants.penddingStatus
        networkManager.getOrdenDetail(orderId: 89076).subscribe(onNext: { [weak self] res in
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
        networkManager.getOrdenDetail(orderId: 89025).subscribe(onNext: { [weak self] res in
            self?.orderDetailViewModel?.auxTabledata = res.response!.details!
            self?.orderDetailViewModel?.tempOrderDetailData = res.response
            self?.orderDetailViewModel?.tableData.subscribe(onNext: { res in
                if res.count != 0 {
                     XCTAssertEqual(res.count, 4)
                }
            }).disposed(by: (self?.disposeBag)!)
            self?.orderDetailViewModel?.deleteItemFromTable(index: index)
        }).disposed(by: self.disposeBag!)
    }
    func testGetDataTableToEditSuccess() {
        networkManager.getOrdenDetail(orderId: 89025).subscribe(onNext: { [weak self] res in
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
}
