//
//  ComponetsTest.swift
//  OmicronTests
//
//  Created by Vicente Cantú on 23/09/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import XCTest
import RxSwift
import Resolver
@testable import OmicronLab
class ComponetsTest: XCTestCase {
    // MARK: - VARIABLES
    var disposeBag: DisposeBag?
    var componentsViewModel: ComponentsViewModel?
    @Injected var networkManager: NetworkManager
    override func setUp() {
        disposeBag = DisposeBag()
        componentsViewModel = ComponentsViewModel()
    }
    override func tearDown() {
        disposeBag = nil
        componentsViewModel = nil
    }
    // MARK: - TEST FUNCTIONS
    func testValidResponse() {
        let expectation = XCTestExpectation(description: "ComponetsTest")
        componentsViewModel!.dataChips.onNext(["Base"])
        componentsViewModel!.dataChips.subscribe(onNext: { [weak self] chips in
            let request = ComponentRequest(
                offset: Constants.Components.offset.rawValue,
                limit: Constants.Components.limit.rawValue,
                chips: chips)
            self?.networkManager.getComponents(data: request).subscribe(onNext: { res in
                XCTAssertNotNil(res.response)
                expectation.fulfill()
            }).disposed(by: (self?.disposeBag)!)
        }).disposed(by: disposeBag!)
        wait(for: [expectation], timeout: 1000)
    }
    func testValidCodeNotNull() {
        let expectation = XCTestExpectation(description: "ComponetsTest")
        componentsViewModel!.dataChips.onNext(["Base"])
        componentsViewModel!.dataChips.subscribe(onNext: { [weak self] chips in
            let request = ComponentRequest(
                offset: Constants.Components.offset.rawValue,
                limit: Constants.Components.limit.rawValue,
                chips: chips)
            self?.networkManager.getComponents(data: request).subscribe(onNext: { res in
                XCTAssertNotNil(res.code)
                expectation.fulfill()
            }).disposed(by: (self?.disposeBag)!)
        }).disposed(by: disposeBag!)
        wait(for: [expectation], timeout: 1000)
    }
    func testValidCode() {
        let expectation = XCTestExpectation(description: "ComponetsTest")
        componentsViewModel!.dataChips.onNext(["Base"])
        componentsViewModel!.dataChips.subscribe(onNext: { [weak self] chips in
            let request = ComponentRequest(
                offset: Constants.Components.offset.rawValue,
                limit: Constants.Components.limit.rawValue,
                chips: chips)
            self?.networkManager.getComponents(data: request).subscribe(onNext: { res in
                XCTAssert(res.code == 200)
                expectation.fulfill()
            }).disposed(by: (self?.disposeBag)!)
        }).disposed(by: disposeBag!)
        wait(for: [expectation], timeout: 1000)
    }
    func testSaveComponentSuccess() {
        let expectationGetComponents = XCTestExpectation(description: "ComponetsTest")
        let expecUpdateDeleteItemOfTable = XCTestExpectation()
        var orderDetailRequest: OrderDetailRequest?
        componentsViewModel!.dataChips.onNext(["Base"])
        componentsViewModel!.dataChips.subscribe(onNext: { [weak self] chips in
            let request = ComponentRequest(
                offset: Constants.Components.offset.rawValue,
                limit: Constants.Components.limit.rawValue,
                chips: chips)
            self?.networkManager.getComponents(data: request).subscribe(onNext: { [weak self] res in
                orderDetailRequest = self?.returnOrderDetailRequest(componentO: res.response)
                self?.networkManager.updateDeleteItemOfTableInOrderDetail(orderDetailRequest: orderDetailRequest!)
                    .subscribe(onNext: { res in
                    XCTAssertNotNil(res.response)
                    expecUpdateDeleteItemOfTable.fulfill()
                }).disposed(by: (self?.disposeBag)!)
                expectationGetComponents.fulfill()
            }).disposed(by: (self?.disposeBag)!)
        }).disposed(by: disposeBag!)
        wait(for: [expectationGetComponents, expecUpdateDeleteItemOfTable], timeout: 1000)
    }
    func returnOrderDetailRequest(componentO: [ComponentO]?) -> OrderDetailRequest? {
        guard let componentO = componentO else { return nil }
        guard let comp = componentO.first else { return  nil }
        let values = ComponentFormValues(baseQuantity: 2.0, requiredQuantity: 2.0, warehouse: "MN")
        let productOrderId = 89466
        let plannedQuantity: Decimal = 1.0
        let fechaFin =
            UtilsManager.shared.formattedDateFromString(dateString: "13/09/2020", withFormat: "yyyy-MM-dd") ?? ""
        let component = Component(
            orderFabID: productOrderId,
            productId: comp.productId ?? "",
            componentDescription: componentO.description,
            baseQuantity: values.baseQuantity,
            requiredQuantity: values.requiredQuantity,
            consumed: NSDecimalNumber(decimal: comp.consumed ?? 0).doubleValue,
            available: NSDecimalNumber(decimal: comp.available ?? 0).doubleValue,
            unit: comp.unit ?? "",
            warehouse: values.warehouse,
            pendingQuantity: NSDecimalNumber(decimal: comp.pendingQuantity ?? 0).doubleValue,
            stock: NSDecimalNumber(decimal: comp.stock ?? 0).doubleValue,
            warehouseQuantity: NSDecimalNumber(decimal: comp.warehouseQuantity ?? 0).doubleValue,
            action: "insert")
        let orderDetailReq = OrderDetailRequest(
            fabOrderID: component.orderFabId,
            plannedQuantity: plannedQuantity,
            fechaFin: fechaFin,
            comments: "",
            components: [component])
        return orderDetailReq
    }
    func testSearchDidTapSuccess() {
        componentsViewModel?.searchFilter.onNext("Crema")
        componentsViewModel?.dataChips.subscribe(onNext: { res in
            print(res)
            if res.count > 0 {
                XCTAssertEqual(res[0], "Crema")
            }
        }).disposed(by: self.disposeBag!)
        componentsViewModel?.searchDidTap.onNext(())
    }
}
