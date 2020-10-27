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

@testable import OmicronLab

class CommentsTest: XCTestCase {
    // MARK: - Variables
    var sut: CommentsViewModel?
    var disposeBag: DisposeBag?
    var productionOrderID: Int?
    var plannedQuantity: Decimal?
    var fechaFin: String?
    var message: String?
    var order: OrderDetailRequest?
    @Injected var networkmanager: NetworkManager
    override func setUp() {
        sut = CommentsViewModel()
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
    }
    override func tearDown() {
        sut = nil
        disposeBag = nil
        productionOrderID = nil
        plannedQuantity = nil
        fechaFin = nil
        message = nil
        order = nil
    }
    // MARK: - Test Functions
    func testValidResponse() {
        self.networkmanager
            .updateDeleteItemOfTableInOrderDetail(orderDetailRequest: self.order!)
            .observeOn(MainScheduler.instance)
            .subscribe(onNext: { res in
                XCTAssertNotNil(res.response)
            }).disposed(by: self.disposeBag!)
    }
    func testAceptDidTapSuccessFromOrderDetailViewController() {
        // Given
        sut?.textView.onNext("Texto de Prueba")
        sut?.originView = ViewControllerIdentifiers.orderDetailViewController
        networkmanager.getOrdenDetail(orderId: 90876).subscribe(onNext: { [weak self] res in
            self?.sut?.orderDetail = [res.response!]
            self?.sut?.backToOrderDetail.subscribe(onNext: { _ in
                // When
                XCTAssertTrue(true)
            }).disposed(by: (self?.disposeBag)!)
            // Then
            self?.sut?.aceptDidTap.onNext(())
        }).disposed(by: self.disposeBag!)
    }
    func testAceptDidTapSuccessFromLotsViewController() {
        // Given
        sut?.textView.onNext("Texto de Prueba")
        sut?.originView = ViewControllerIdentifiers.lotsViewController
        networkmanager.getOrdenDetail(orderId: 90876).subscribe(onNext: { [weak self] res in
            self?.sut?.orderDetail = [res.response!]
            self?.sut?.backToLots.subscribe(onNext: { _ in
                // When
                XCTAssertTrue(true)
            }).disposed(by: (self?.disposeBag)!)
            // Then
            self?.sut?.aceptDidTap.onNext(())
        }).disposed(by: self.disposeBag!)
    }
}
