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
    @Injected var inboxViewModel: InboxViewModel
    var order1: Order?
    override func setUp() {
        disposeBag = DisposeBag()
        componentsViewModel = ComponentsViewModel()
        order1 = Order(
            areBatchesComplete: true, productionOrderId: 89284, baseDocument: 60067, container: "",
            tag: "Selecciona una...", plannedQuantity: 1, startDate: "27/08/2020",
            finishDate: "06/09/2020",
            descriptionProduct: "Aceite de Arbol de Te 0.3%, Alantoina 0.3%, Citrico 0.2%, " +
            "Extracto de Te Verde 3%, Extracto de Pepino 3%, Glicerina 3%, Hamamelis 3%, Hialuronico 3%, " +
            "Menta Piperita 0.02%, Niacinamida 2%, Pantenol 0.5%,  Salicilico 0.5%, Urea 5%, Solucion",
            statusId: 1, itemCode: "3264   120 ML", productCode: "3264", destiny: "Foráneo",
            hasMissingStock: false, finishedLabel: false)
    }
    override func tearDown() {
        disposeBag = nil
        componentsViewModel = nil
        order1 = nil
    }
    // MARK: - TEST FUNCTIONS
    func testValidResponse() {
        componentsViewModel!.dataChips.onNext(["Base"])
        componentsViewModel!.dataChips.subscribe(onNext: { [weak self] chips in
            let request = ComponentRequest(
                offset: Constants.Components.offset.rawValue,
                limit: Constants.Components.limit.rawValue,
                chips: chips)
            self?.networkManager.getComponents(data: request).subscribe(onNext: { res in
                XCTAssertNotNil(res.response)
            }).disposed(by: (self?.disposeBag)!)
        }).disposed(by: disposeBag!)
    }
    func testValidResponseWhenCodeIs500() {
        componentsViewModel?.dataError.subscribe(onNext: { res in
            XCTAssertEqual(res, Constants.Errors.errorData.rawValue)
        }).disposed(by: disposeBag!)
        componentsViewModel?.getComponents(chips: ["Base"], needsError: true)
    }
    func testValidCodeNotNull() {
        componentsViewModel!.dataChips.onNext(["Base"])
        componentsViewModel!.dataChips.subscribe(onNext: { [weak self] chips in
            let request = ComponentRequest(
                offset: Constants.Components.offset.rawValue,
                limit: Constants.Components.limit.rawValue,
                chips: chips)
            self?.networkManager.getComponents(data: request).subscribe(onNext: { res in
                XCTAssertNotNil(res.code)
            }).disposed(by: (self?.disposeBag)!)
        }).disposed(by: disposeBag!)
    }
    func testValidCode() {
        componentsViewModel!.dataChips.onNext(["Base"])
        componentsViewModel!.dataChips.subscribe(onNext: { [weak self] chips in
            let request = ComponentRequest(
                offset: Constants.Components.offset.rawValue,
                limit: Constants.Components.limit.rawValue,
                chips: chips)
            self?.networkManager.getComponents(data: request).subscribe(onNext: { res in
                XCTAssert(res.code == 200)
            }).disposed(by: (self?.disposeBag)!)
        }).disposed(by: disposeBag!)
    }
    func testSaveComponentSuccess() {
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
                }).disposed(by: (self?.disposeBag)!)
            }).disposed(by: (self?.disposeBag)!)
        }).disposed(by: disposeBag!)
    }
    func testSaveComponentWhenCodeIs500() {
        let req = OrderDetailRequest(
            fabOrderID: 213, plannedQuantity: Decimal(0),
            fechaFin: String(), comments: String(), warehouse: String(), components: [])
        componentsViewModel?.dataError.subscribe(onNext: { res in
            XCTAssertEqual(res, Constants.Errors.errorSave.rawValue)
        }).disposed(by: disposeBag!)
        componentsViewModel?.saveComponent(req: req, needsError: true)
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
            comments: "", warehouse: "MP",
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
    func testRemoveChip() {
        componentsViewModel?.dataChips.subscribe(onNext: { res in
            if res.count == 2 {
                XCTAssertEqual(res[0], "Ivermectina")
                XCTAssertEqual(res[1], "Exicpiente")
            }
        }).disposed(by: disposeBag!)
        componentsViewModel?.dataChips.onNext(["Crema", "Ivermectina", "Exicpiente"])
        componentsViewModel?.removeChip.onNext("Crema")
    }

    func testSaveDidTapSuccess() {
        let values = ComponentFormValues(baseQuantity: 2,
                                         requiredQuantity: 1,
                                         warehouse: "MG")
        let componentSelected = ComponentO()
        componentSelected.available = 2
        componentSelected.baseQuantity = 3
        componentSelected.consumed = Decimal(2)
        componentSelected.description = String()
        componentSelected.orderFabId = 1235
        componentSelected.pendingQuantity = 3
        componentSelected.productId = "1245"
        componentSelected.requiredQuantity = 2
        componentSelected.stock = 3
        componentSelected.unit = CommonStrings.piece
        componentSelected.warehouse = String()
        componentsViewModel?.saveSuccess.subscribe(onNext: { _ in
            XCTAssertTrue(true)
        }).disposed(by: disposeBag!)
        inboxViewModel.selectedOrder = order1
        componentsViewModel?.selectedComponent.onNext(componentSelected)
        componentsViewModel?.saveDidTap.onNext(values)
    }

    func testSaveDidTapDataEmpty() {
        let values = ComponentFormValues(baseQuantity: 2,
                                         requiredQuantity: 1,
                                         warehouse: "MG")
        let componentSelected = ComponentO()
        componentsViewModel?.saveSuccess.subscribe(onNext: { _ in
            XCTAssertTrue(true)
        }).disposed(by: disposeBag!)
        inboxViewModel.selectedOrder = order1
        componentsViewModel?.selectedComponent.onNext(componentSelected)
        componentsViewModel?.saveDidTap.onNext(values)
    }

    func testSaveDidTapOrderSelectedIsNil() {
        let values = ComponentFormValues(baseQuantity: 2,
                                         requiredQuantity: 1,
                                         warehouse: "MG")
        let componentSelected = ComponentO()
        componentsViewModel?.saveSuccess.subscribe(onNext: { _ in
            XCTAssertTrue(true)
        }).disposed(by: disposeBag!)
        inboxViewModel.selectedOrder = nil
        componentsViewModel?.selectedComponent.onNext(componentSelected)
        componentsViewModel?.saveDidTap.onNext(values)
    }

    func testSaveDidTapOrderIsNil() {
        let values = ComponentFormValues(baseQuantity: 2,
                                         requiredQuantity: 1,
                                         warehouse: "MG")
        let componentSelected = ComponentO()
        componentsViewModel?.saveSuccess.subscribe(onNext: { _ in
            XCTAssertTrue(true)
        }).disposed(by: disposeBag!)
        order1?.productionOrderId = nil
        inboxViewModel.selectedOrder = order1
        componentsViewModel?.selectedComponent.onNext(componentSelected)
        componentsViewModel?.saveDidTap.onNext(values)
    }

    func testGetMostComponetService() {
        componentsViewModel?.bindingData.subscribe(onNext: { res in
            if res.count == 2 {
                XCTAssertEqual(res.count, 2)
                XCTAssertEqual(res[0].productId, "EN-002")
                XCTAssertEqual(res[0].description, "Airless AcrÝlico con Tapa y Base Plateada 30 ml")
                XCTAssertEqual(res[1].productId, "EN-006")
                XCTAssertEqual(res[1].description, "Airless Pump Star 150 ml Blanco")
            }
        }).disposed(by: disposeBag!)
        componentsViewModel?.getMostCommonComponentsService()
    }

    func testGetMostComponetServiceWhenCodeIs500() {
        componentsViewModel?.dataError.subscribe(onNext: { res in
            XCTAssertEqual(res, Constants.Errors.errorData.rawValue)
        }).disposed(by: disposeBag!)
        componentsViewModel?.getMostCommonComponentsService(needsError: true)
    }
}
