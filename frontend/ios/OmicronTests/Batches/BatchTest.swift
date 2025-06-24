//
//  BatchTest.swift
//  OmicronTests
//
//  Created by Axity on 24/09/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import XCTest
import RxSwift
import Moya
import Resolver

@testable import Magistrales

class BatchesTest: XCTestCase {
    // MARK: - VARIABLES
    var lotsViewModel: LotsViewModel?
    var disposeBag: DisposeBag?
    var orderId: Int?
    var provider: MoyaProvider<ApiService>!
    var statusCode = 200
    var testData = Data()
    @Injected var networkManager: NetworkManager
    override func setUp() {
        lotsViewModel = LotsViewModel()
        disposeBag = DisposeBag()
        orderId = 0
        provider = MoyaProvider<ApiService>(
            endpointClosure: customEndpointClosure,
            stubClosure: MoyaProvider.immediatelyStub)
    }
    override func tearDown() {
        lotsViewModel = nil
        disposeBag = nil
        orderId = nil
    }
    func customEndpointClosure(_ target: ApiService) -> Endpoint {
        return Endpoint(url: URL(target: target).absoluteString,
                        sampleResponseClosure: { .networkResponse(self.statusCode, self.testData) },
                        method: target.method,
                        task: target.task,
                        httpHeaderFields: target.headers)
    }

    // MARK: - TEST FUNCTIONS
    func testGetLotsSuccess() {
        // When
        lotsViewModel?.orderId = 0
        lotsViewModel?.getLots()
        lotsViewModel?.dataOfLots.subscribe(onNext: { res in
            if res.count != 0 {
                XCTAssertEqual(res.count, 5)
            }
        }).disposed(by: disposeBag!)
    }

    func testAssingBatchesWhenResponseIsEmpty() {
        // Given
        var batchesToSend: [BatchSelected] = []
        let batch = BatchSelected(
            orderId: 89956, assignedQty: 0.257895, batchNumber: "337-19",
            itemCode: "MP-109", action: "insert", sysNumber: 54,
            expiredBatch: false, areBatchesComplete: 1)
        batchesToSend.append(batch)
        lotsViewModel?.sendToServerAssignedLots(lotsToSend: batchesToSend)
    }
    func testAssingBatchesWhenResponseNoEmpty() {
        // Given
        var batchesToSend: [BatchSelected] = []
        let batch = BatchSelected(
            orderId: 89956, assignedQty: 0.257895, batchNumber: "337-19",
            itemCode: "MP-109", action: "insert", sysNumber: 54,
            expiredBatch: false, areBatchesComplete: 0)
        batchesToSend.append(batch)
        testData = UtilsManager.shared.getDataFor(resourse: "SendBatchToServerNoEmptyResponse", withExtension: "json")
        let batchStr = "\n7J3WM91-2"
        lotsViewModel?.showMessage.subscribe(onNext: { res in
            XCTAssertEqual(res, "\(Constants.Errors.assignedBatches.rawValue) \(batchStr)")
        }).disposed(by: disposeBag!)
        lotsViewModel?.networkManager = NetworkManager(provider: provider)
        lotsViewModel?.sendToServerAssignedLots(lotsToSend: batchesToSend)
    }
    func testCalculateExpiredBatchShoudBeFalse() {
        // Given
        let dateTest = "30/11/2020"
        // When
        let result = UtilsManager.shared.calculateExpiredBatch(date: dateTest)
        // Then
        XCTAssertTrue(result)
        lotsViewModel?.validIfOrderCanBeFinalized()
    }
    func testCalculateExpiredBatchShoudBeFalseWithNilValue() {
        // Given
        let dateTest: String? = nil
        // When
        let result = UtilsManager.shared.calculateExpiredBatch(date: dateTest)
        // Then
        XCTAssertFalse(result)
    }
    func testCalculateExpiredBatchShoudBeFalseWithEmptyValue() {
        // Given
        let dateTest: String = ""
        // Then
        let result = UtilsManager.shared.calculateExpiredBatch(date: dateTest)
        // When
        XCTAssertFalse(result)
    }
    func testFinishOrderDidTapSuccess() {
        self.lotsViewModel!
            .askIfUserWantToFinalizeOrder.subscribe(onNext: { message in
            XCTAssertTrue(message == "¿Deseas terminar la orden?")
        }).disposed(by: self.disposeBag!)
        self.lotsViewModel!.finishOrderDidTap.onNext(())
    }
    func testFinishOrderDidTapSuccessShoulBeFalse() {
        self.lotsViewModel!.askIfUserWantToFinalizeOrder.subscribe(onNext: { res in
            XCTAssertFalse(res == "")
        }).disposed(by: self.disposeBag!)
        self.lotsViewModel!.finishOrderDidTap.onNext(())
    }
    func testPendingOrderDidTapSuccess() {
        self.lotsViewModel!.askIfUserWantChageOrderToPendigStatus.subscribe(onNext: { res in
            XCTAssertTrue(res == "La orden cambiará a estatus Pendiente, ¿quieres continuar?")
        }).disposed(by: self.disposeBag!)
        self.lotsViewModel!.pendingButtonDidTap.onNext(())
        lotsViewModel?.assignLots()
    }
    func testPendingOrderDidTapShoulbBeFalse() {
        self.lotsViewModel!.askIfUserWantChageOrderToPendigStatus.subscribe(onNext: { res in
            XCTAssertFalse(res == "")
        }).disposed(by: self.disposeBag!)
        self.lotsViewModel!.pendingButtonDidTap.onNext(())
    }
    func testAddLotsDidTapShoulBeNil() {
        self.lotsViewModel!.batchSelected.subscribe(onNext: { res in
            XCTAssertNil(res)
        }).disposed(by: self.disposeBag!)
        self.lotsViewModel!.addLotDidTap.onNext(())
    }
    func testAddDidTapDataLotsSelectedEmpty() {
        let lotSelected: [LotsSelected] = []
        let lotsAvailable = LotsAvailable(
            numeroLote: "19A51", cantidadDisponible: 0, cantidadAsignada: 0.3,
            cantidadSeleccionada: 0, sysNumber: 3, fechaExp: "23/01/2024")
        let batch = Lots(
            codigoProducto: "MP-014", descripcionProducto: "Sulfato de Cobre pentahidratado USP",
            almacen: "MG", totalNecesario: 0.5, totalSeleccionado: 0.0,
            lotesSelecionados: lotSelected, lotesDisponibles: [lotsAvailable])
        let lotsAvailableSleceted = LotsAvailable(
            numeroLote: "19F02", cantidadDisponible: 0.108800, cantidadAsignada: 0.7,
            cantidadSeleccionada: 0, sysNumber: 4, fechaExp: "07/06/2024")
        let documentLines = Lots(
            codigoProducto: "MP-014", descripcionProducto: "Sulfato de Cobre pentahidratado USP",
            almacen: "MN", totalNecesario: 0.1800000, totalSeleccionado: 0.100000,
            lotesSelecionados: [], lotesDisponibles: [])
        self.lotsViewModel!.documentLines.append(documentLines)
        self.lotsViewModel!.productSelected.onNext(batch)
        self.lotsViewModel!.availableSelected.onNext(lotsAvailableSleceted)
        self.lotsViewModel!.addLotDidTap.onNext(())
        self.lotsViewModel!.dataLotsSelected.subscribe(onNext: { res in
            XCTAssertEqual(res.count, 0)
        }).disposed(by: self.disposeBag!)
    }
    func testAddLotsDidTapSucess() {
        let lotSelected: [LotsSelected] = []
        let lotsAvailable = LotsAvailable(
            numeroLote: "19A51", cantidadDisponible: 0, cantidadAsignada: 0.3,
            cantidadSeleccionada: 0, sysNumber: 3, fechaExp: "23/01/2024")
        let batch = Lots(
            codigoProducto: "MP-014", descripcionProducto: "Sulfato de Cobre pentahidratado USP",
            almacen: "MG", totalNecesario: 0.5, totalSeleccionado: 0.0,
            lotesSelecionados: lotSelected, lotesDisponibles: [lotsAvailable])
        let lotsAvailableSleceted = LotsAvailable(
            numeroLote: "19F02", cantidadDisponible: 0.108800, cantidadAsignada: 0.7,
            cantidadSeleccionada: 0.108800, sysNumber: 4, fechaExp: "07/06/2024")
        let documentLines = Lots(
            codigoProducto: "MP-014", descripcionProducto: "Sulfato de Cobre pentahidratado USP",
            almacen: "MN", totalNecesario: 0.1800000, totalSeleccionado: 0.108800,
            lotesSelecionados: [], lotesDisponibles: [])
        self.lotsViewModel!.documentLines.append(documentLines)
        self.lotsViewModel!.productSelected.onNext(batch)
        self.lotsViewModel!.availableSelected.onNext(lotsAvailableSleceted)
        self.lotsViewModel!.addLotDidTap.onNext(())
        self.lotsViewModel!.dataLotsSelected.subscribe(onNext: { res in
            XCTAssertTrue(res.count > 0)
            XCTAssertEqual(res[0].numeroLote, "19F02")
            XCTAssertEqual(res[0].cantidadSeleccionada, 0.108800)
            XCTAssertFalse(res[0].expiredBatch)
        }).disposed(by: self.disposeBag!)
    }
    func testSaveLotsDidTapNotChanges() {
        self.lotsViewModel!.saveLotsDidTap.onNext(())
        self.lotsViewModel!.showMessage.subscribe(onNext: { res in
            XCTAssertEqual(res, "No se han realizado modificaciones de lotes")
        }).disposed(by: self.disposeBag!)
        self.lotsViewModel!.saveLotsDidTap.onNext(())
    }
    func testValidIfOrderCanBeFinalizedNotNull() {
        self.networkManager.askIfOrderCanBeFinalized(self.orderId!).subscribe(onNext: { res in
            XCTAssertNotNil(res)
        }).disposed(by: self.disposeBag!)    }
    func testValidIfOrderCanBeFinalizedValidCode() {
        self.networkManager.askIfOrderCanBeFinalized(self.orderId!).subscribe(onNext: { res in
            XCTAssertTrue(res.code == 200)
        }).disposed(by: self.disposeBag!)
    }
    func testGetLostWhenCodeIs500() {
        lotsViewModel?.showMessage.subscribe(onNext: { res in
            XCTAssertEqual(res, Constants.Errors.loadBatches.rawValue)
        }).disposed(by: disposeBag!)
        statusCode = 500
        lotsViewModel?.networkManager = NetworkManager(provider: provider)
        lotsViewModel?.getLots()
    }
    func testSendToServerAssignedLotsWhenCodeIs500() {
        lotsViewModel?.showMessage.subscribe(onNext: { res in
            XCTAssertEqual(res, Constants.Errors.assignedBatchesTryAgain.rawValue)
        }).disposed(by: disposeBag!)
        statusCode = 500
        lotsViewModel?.networkManager = NetworkManager(provider: provider)
        lotsViewModel?.sendToServerAssignedLots(lotsToSend: [])
    }
    func testValidIfOrderCanBeFinalizedWhenCodeIs500() {
        lotsViewModel?.showMessage.subscribe(onNext: { res in
            XCTAssertEqual(res, Constants.Errors.errorData.rawValue)
        }).disposed(by: disposeBag!)
        statusCode = 500
        lotsViewModel?.networkManager = NetworkManager(provider: provider)
        lotsViewModel?.validIfOrderCanBeFinalized()
    }
    func testValidIfOrderCanBeFinalizedWhithCode400() {
        // swiftlint:disable line_length
        let expectedResult = "No es posible Terminar, faltan lotes para: \n122307 MP-157\n122363 MP-368\n122366 MP-157\n122368 BA-14\n122368 GR-161\n\n No es posible Terminar, falta existencia para: \n122307 EN-089\n122363 MP-368\n122366 MP-157"
        lotsViewModel?.showMessage.subscribe(onNext: { res in
            XCTAssertEqual(res, expectedResult)
        }).disposed(by: disposeBag!)
        testData = UtilsManager.shared.getDataFor(resourse: "FinishOrdersErrorResponse", withExtension: "json")
        lotsViewModel?.validIfOrderCanBeFinalized()
    }
    func testCallFinishOrderServiceWhenCodeIs500() {
        lotsViewModel?.technicalSignatureIsGet = true
        lotsViewModel?.qfbSignatureIsGet = true
        lotsViewModel?.showMessage.subscribe(onNext: { res in
            XCTAssertEqual(res, CommonStrings.errorFinishOrder)
        }).disposed(by: disposeBag!)
        statusCode = 500
        lotsViewModel?.networkManager = NetworkManager(provider: provider)
        lotsViewModel?.callFinishOrderService()
    }
    func testCallFinishOrderServiceSuccess() {
        lotsViewModel?.technicalSignatureIsGet = true
        lotsViewModel?.qfbSignatureIsGet = true
        lotsViewModel?.backToInboxView.subscribe(onNext: { _ in
            XCTAssertTrue(true)
        }).disposed(by: disposeBag!)
        lotsViewModel?.callFinishOrderService()
    }
}
