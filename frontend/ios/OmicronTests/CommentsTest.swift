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
import Moya

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
    var provider: MoyaProvider<ApiService>!
    var statusCode = 200
    var testData = Data()
    @Injected var networkmanager: NetworkManager
    override func setUp() {
        statusCode = 200
        testData = Data()
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
            comments: message!, warehouse: "MP",
            components: [])
        provider = MoyaProvider<ApiService>(
            endpointClosure: customEndpointClosure,
            stubClosure: MoyaProvider.immediatelyStub)
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

    func customEndpointClosure(_ target: ApiService) -> Endpoint {
        return Endpoint(url: URL(target: target).absoluteString,
                        sampleResponseClosure: { .networkResponse(self.statusCode, self.testData) },
                        method: target.method,
                        task: target.task,
                        httpHeaderFields: target.headers)
    }
    // MARK: - Test Functions
    func testValidResponse() {
        self.networkmanager
            .updateDeleteItemOfTableInOrderDetail(self.order!)
            .observeOn(MainScheduler.instance)
            .subscribe(onNext: { res in
                XCTAssertNotNil(res.response)
            }).disposed(by: self.disposeBag!)
    }
    func testAceptDidTapSuccessFromOrderDetailViewController() {
        // Given
        sut?.textView.onNext("Texto de Prueba")
        sut?.originView = ViewControllerIdentifiers.orderDetailViewController
        networkmanager.getOrdenDetail(90876).subscribe(onNext: { [weak self] res in
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
        networkmanager.getOrdenDetail(90876).subscribe(onNext: { [weak self] res in
            self?.sut?.orderDetail = [res.response!]
            self?.sut?.backToLots.subscribe(onNext: { _ in
                // When
                XCTAssertTrue(true)
            }).disposed(by: (self?.disposeBag)!)
            // Then
            self?.sut?.aceptDidTap.onNext(())
        }).disposed(by: self.disposeBag!)
    }

    func testAceptDidTapSuccessFromLotsViewControllerWhenCodeIs500() {
        // Given
        sut?.textView.onNext("Texto de Prueba")
        statusCode = 500
        sut?.networkmanager = NetworkManager(provider: provider)
        sut?.originView = ViewControllerIdentifiers.lotsViewController
        networkmanager.getOrdenDetail(90876).subscribe(onNext: { [weak self] res in
            self?.sut?.orderDetail = [res.response!]
            self?.sut?.showAlert.subscribe(onNext: { res in
                XCTAssertEqual(res, CommonStrings.errorInComments)
            }).disposed(by: (self?.disposeBag!)!)
            self?.sut?.aceptDidTap.onNext(())
        }).disposed(by: self.disposeBag!)
    }
}
