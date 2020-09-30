////
////  ComponetsTest.swift
////  OmicronTests
////
////  Created by Vicente Cantú on 23/09/20.
////  Copyright © 2020 Diego Cárcamo. All rights reserved.
////
//
//import XCTest
//import RxSwift
//import Moya
//
//@testable import Omicron
//
//class ComponetsTest: XCTestCase {
//    
//    //MARK: - VARIABLES
//    var disposeBag: DisposeBag?
//    var componentsViewModel: ComponentsViewModel?
//    var networkManager: NetworkManager?
//    
//    override func setUp() {
//        disposeBag = DisposeBag()
//        componentsViewModel = ComponentsViewModel()
//    }
//    
//    override func tearDown() {
//        disposeBag = nil
//        componentsViewModel = nil
//    }
//    
//    //MARK: - TEST FUNCTIONS
//    
//    func testValidResponse() {
//        
//        componentsViewModel!.dataChips.onNext(["Base"])
//        componentsViewModel!.dataChips.subscribe(onNext: { chips in
//            
//            let request = ComponentRequest(
//                offset: Constants.Components.offset.rawValue,
//                limit: Constants.Components.limit.rawValue,
//                chips: chips)
//            
//            NetworkManager.shared.getComponents(data: request).subscribe(onNext: { res in
//                XCTAssertNotNil(res.response)
//                self.testSaveComponent(componentO: res.response)
//            }).disposed(by: self.disposeBag!)
//            
//        }).disposed(by: disposeBag!)
//        
//    }
//    
//    func testValidCodeNotNull() {
//        
//        componentsViewModel!.dataChips.onNext(["Base"])
//        componentsViewModel!.dataChips.subscribe(onNext: { chips in
//            
//            let request = ComponentRequest(
//                offset: Constants.Components.offset.rawValue,
//                limit: Constants.Components.limit.rawValue,
//                chips: chips)
//            
//            NetworkManager.shared.getComponents(data: request).subscribe(onNext: { res in
//                XCTAssertNotNil(res.code)
//            }).disposed(by: self.disposeBag!)
//            
//        }).disposed(by: disposeBag!)
//        
//    }
//    
//    func testValidCode() {
//        
//        componentsViewModel!.dataChips.onNext(["Base"])
//        componentsViewModel!.dataChips.subscribe(onNext: { chips in
//            
//            let request = ComponentRequest(
//                offset: Constants.Components.offset.rawValue,
//                limit: Constants.Components.limit.rawValue,
//                chips: chips)
//            
//            NetworkManager.shared.getComponents(data: request).subscribe(onNext: { res in
//                XCTAssert(res.code == 200)
//            }).disposed(by: self.disposeBag!)
//            
//        }).disposed(by: disposeBag!)
//        
//    }
//    
//    func testSaveComponent(componentO: [ComponentO]?) {
//        
//        XCTAssertNotNil(componentO)
//        XCTAssertNotNil(componentO?.first)
//        
//        guard let componentO = componentO else { return }
//        guard let comp = componentO.first else { return }
//        let values = ComponentFormValues(baseQuantity: 2.0, requiredQuantity: 2.0, warehouse: "MN")
//        
//        let productOrderId = 89466
//        let plannedQuantity: Decimal = 1.0
//        let fechaFin = UtilsManager.shared.formattedDateFromString(dateString: "13/09/2020", withFormat: "yyyy-MM-dd") ?? ""
//        
//        let component = Component(
//            orderFabID: productOrderId,
//            productId: comp.productId ?? "",
//            componentDescription: componentO.description,
//            baseQuantity: values.baseQuantity,
//            requiredQuantity: values.requiredQuantity,
//            consumed: NSDecimalNumber(decimal: comp.consumed ?? 0).doubleValue,
//            available: NSDecimalNumber(decimal: comp.available ?? 0).doubleValue,
//            unit: comp.unit ?? "",
//            warehouse: values.warehouse,
//            pendingQuantity: NSDecimalNumber(decimal: comp.pendingQuantity ?? 0).doubleValue,
//            stock: NSDecimalNumber(decimal: comp.stock ?? 0).doubleValue,
//            warehouseQuantity: NSDecimalNumber(decimal: comp.warehouseQuantity ?? 0).doubleValue,
//            action: "insert")
//        
//        let orderDetailReq = OrderDetailRequest(
//            fabOrderID: component.orderFabId,
//            plannedQuantity: plannedQuantity,
//            fechaFin: fechaFin,
//            comments: "",
//            components: [component])
//        
//        testValidResponse(req: orderDetailReq)
//        testValidCodeNotNull(req: orderDetailReq)
//        testValidCode(req: orderDetailReq)
//    }
//    
//    func testValidResponse(req: OrderDetailRequest) {
//        NetworkManager.shared.updateDeleteItemOfTableInOrderDetail(orderDetailRequest: req).subscribe(onNext: { res in
//            XCTAssertNotNil(res.response)
//        }).disposed(by: disposeBag!)
//    }
//    
//    func testValidCodeNotNull(req: OrderDetailRequest) {
//        NetworkManager.shared.updateDeleteItemOfTableInOrderDetail(orderDetailRequest: req).subscribe(onNext: { res in
//            XCTAssertNotNil(res.code)
//        }).disposed(by: disposeBag!)
//    }
//    
//    func testValidCode(req: OrderDetailRequest) {
//        NetworkManager.shared.updateDeleteItemOfTableInOrderDetail(orderDetailRequest: req).subscribe(onNext: { res in
//            XCTAssert(res.code == 200)
//        }).disposed(by: disposeBag!)
//    }
//
//}
