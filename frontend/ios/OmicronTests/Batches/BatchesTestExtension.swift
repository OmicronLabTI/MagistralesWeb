//
//  BatchesTestExtension.swift
//  OmicronTests
//
//  Created by Sergio Flores Ramirez on 28/03/22.
//  Copyright © 2022 Diego Cárcamo. All rights reserved.
//

import XCTest
import RxSwift
import Moya
import Resolver
@testable import Magistrales
class BatchesTestExtension: XCTestCase {

    var lotsViewModel: LotsViewModel?
    var provider: MoyaProvider<ApiService>!
    var disposeBag: DisposeBag?
    var statusCode = 200
    var testData = Data()

    override func setUpWithError() throws {
        lotsViewModel = LotsViewModel()
        disposeBag = DisposeBag()
        provider = MoyaProvider<ApiService>(
            endpointClosure: customEndpointClosure,
            stubClosure: MoyaProvider.immediatelyStub)
    }

    override func tearDownWithError() throws {
        lotsViewModel = nil
        disposeBag = nil
    }

    func customEndpointClosure(_ target: ApiService) -> Endpoint {
        return Endpoint(url: URL(target: target).absoluteString,
                        sampleResponseClosure: { .networkResponse(self.statusCode, self.testData) },
                        method: target.method,
                        task: target.task,
                        httpHeaderFields: target.headers)
    }

    func testAssignLots() {
        let batchSelected = BatchSelected(
            orderId: 1234, assignedQty: Decimal(20), batchNumber: "OMK-01",
            itemCode: "OMK", action: Actions.insert.rawValue, sysNumber: 2, expiredBatch: true, areBatchesComplete: 1)
        lotsViewModel?.selectedBatches = [batchSelected]
        lotsViewModel?.showMessage.subscribe(onNext: { res in
            XCTAssertEqual(res, CommonStrings.processSuccess)
        }).disposed(by: disposeBag!)
        lotsViewModel?.assignLots()
    }
    func testSelectBatchIfNeeded() {
        let selected = LotsSelected(numeroLote: "19F02", cantidadSeleccionada: Decimal(0.81),
                                    sysNumber: 2, expiredBatch: true)
        let available = LotsAvailable(
            numeroLote: "19F02", cantidadDisponible: Decimal(0.81), cantidadAsignada: Decimal(0.81),
            cantidadSeleccionada: Decimal(0.81), sysNumber: 2, fechaExp: String())
        let lot = Lots(
            codigoProducto: "BQ 02", descripcionProducto: "SULFATO DE COBRE", almacen: "MP", totalNecesario: Decimal(5),
            totalSeleccionado: Decimal(5), lotesSelecionados: [selected], lotesDisponibles: [available])
        lotsViewModel?.documentLines = [lot]
        lotsViewModel?.dataLotsAvailable.subscribe(onNext: { res in
            if res.count != 0 {
                XCTAssertEqual(res.first?.cantidadAsignada, Decimal(0.810000))
                XCTAssertEqual(res.first?.cantidadDisponible, Decimal(0))
                XCTAssertEqual(res.first?.sysNumber, 2)
            }
        }).disposed(by: disposeBag!)
        lotsViewModel?.selectBatchIfNeeded(lot: lot, selected: [])
    }

    func testUpdateInfoSelectedBatchWhenbathSelectedIsEmpty() {
        let selected = LotsSelected(numeroLote: "19F02", cantidadSeleccionada: Decimal(0.81),
                                    sysNumber: 2, expiredBatch: true)
        let available = LotsAvailable(
            numeroLote: "19F02", cantidadDisponible: Decimal(0.81), cantidadAsignada: Decimal(0.81),
            cantidadSeleccionada: Decimal(0.81), sysNumber: 2, fechaExp: String())
        let lot = Lots(
            codigoProducto: "BQ 02", descripcionProducto: "SULFATO DE COBRE", almacen: "MP", totalNecesario: Decimal(5),
            totalSeleccionado: Decimal(5), lotesSelecionados: [selected], lotesDisponibles: [available])
        lotsViewModel?.dataLotsSelected.subscribe(onNext: { res in
            XCTAssertEqual(res.count, 0)
        }).disposed(by: disposeBag!)
        lotsViewModel?.updateInfoSelectedBatch(lot: lot)
    }

    func testRemoveLotAction() {
        let selected = LotsSelected(numeroLote: "OMK-01", cantidadSeleccionada: Decimal(0.81),
                                    sysNumber: 2, expiredBatch: true)
        let lot = Lots(
            codigoProducto: "BQ 02", descripcionProducto: "SULFATO DE COBRE", almacen: "MP", totalNecesario: Decimal(5),
            totalSeleccionado: Decimal(5), lotesSelecionados: [selected], lotesDisponibles: [])
        let batchSelected = BatchSelected(
            orderId: 1234, assignedQty: Decimal(20), batchNumber: "OMK-01", itemCode: "OMK-01",
            action: Actions.insert.rawValue, sysNumber: 2, expiredBatch: true, areBatchesComplete: 0)
        lotsViewModel?.dataLotsSelected.subscribe(onNext: { res in
            XCTAssertEqual(res.count, 0)
        }).disposed(by: disposeBag!)
        lotsViewModel?.selectedBatches = [batchSelected]
        lotsViewModel?.batchSelected.onNext(selected)
        lotsViewModel?.productSelected.onNext(lot)
        lotsViewModel?.removeLotDidTap.onNext(())
    }

    func testUpdateOrderDetailSuccess() {
        lotsViewModel?.updateComments.subscribe(onNext: { res in
            XCTAssertEqual(res.baseDocument, 56701)
            XCTAssertEqual(res.client, "C00474")
        }).disposed(by: disposeBag!)
        lotsViewModel?.updateOrderDetail()
    }
    func testUpdateOrderDetailWhenCodeIs500() {
        lotsViewModel?.showMessage.subscribe(onNext: { res in
            XCTAssertEqual(res, Constants.Errors.loadOrdersDetail.rawValue)
        }).disposed(by: disposeBag!)
        statusCode = 500
        lotsViewModel?.networkManager = NetworkManager(provider: provider)
        lotsViewModel?.updateOrderDetail()
    }
    func testChangeOrderToPendingStatusSuccess() {
        lotsViewModel?.backToInboxView.subscribe(onNext: { _ in
            XCTAssertTrue(true)
        }).disposed(by: disposeBag!)
        lotsViewModel?.changeOrderToPendingStatus()
    }
    func testChangeOrderToPendingStatusWhenCodeIs500() {
        lotsViewModel?.showMessage.subscribe(onNext: { res in
            XCTAssertEqual(res, CommonStrings.errorToChangeStatus)
        }).disposed(by: disposeBag!)
        statusCode = 500
        lotsViewModel?.networkManager = NetworkManager(provider: provider)
        lotsViewModel?.changeOrderToPendingStatus()
    }

    func testGetFilteredSelectedSuccess() {
        let batchSelected = BatchSelected(
            orderId: 1234, assignedQty: Decimal(20), batchNumber: "OMK-01",
            itemCode: "OMK", action: Actions.insert.rawValue, sysNumber: 2, expiredBatch: true, areBatchesComplete: 1)
        lotsViewModel?.selectedBatches = [batchSelected]
        let result = lotsViewModel?.getFilteredSelected(itemCode: "OMK", batchNumber: "OMK-01")
        XCTAssertEqual(result?.count, 1)
        XCTAssertEqual(result?.first?.cantidadSeleccionada, Decimal(20))
        XCTAssertEqual(result?.first?.expiredBatch, true)
        XCTAssertEqual(result?.first?.numeroLote, "OMK-01")

    }

}
