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

@testable import Omicron

class BatchesTest: XCTestCase {
    // MARK: -VARIABLES
    var lotsViewModel: LotsViewModel?
    var disposeBag: DisposeBag?
    var orderId: Int?
    var expectation: XCTestExpectation?
    @Injected var networkManager: NetworkManager
    
    override func setUp() {
        lotsViewModel = LotsViewModel()
        disposeBag = DisposeBag()
        orderId = 0
        expectation = XCTestExpectation()
    }
    
    override func tearDown() {
        lotsViewModel = nil
        disposeBag = nil
        orderId = nil
        expectation =  nil
    }
    
    // MARK: -TEST FUNCTIONS
    func testGetLotsSuccess() -> Void {
        
        // When
        self.networkManager.getLots(orderId: self.orderId!).subscribe(onNext: { res in
            // Then
            XCTAssertNotNil(res)
            XCTAssertTrue(res.response!.count > 0)
        }).disposed(by: self.disposeBag!)
    }
    
    func testAssingBatches() -> Void {
        // Given
        var batchesToSend:[BatchSelected] = []
        let batch = BatchSelected(orderId: 89956, assignedQty: 0.257895, batchNumber: "337-19", itemCode: "MP-109", action: "insert", sysNumber: 54, expiredBatch: false)
        batchesToSend.append(batch)
        
        // When
        self.networkManager.assignLots(lotsRequest: batchesToSend).subscribe(onNext: { res in
            
            // Then
            XCTAssertNotNil(res)
        }).disposed(by: self.disposeBag!)
    }
    
    func testCalculateExpiredBatchShoudBeFalse() -> Void {
        // Given
        let dateTest = "30/11/2020"
        
        // When
        let result = self.lotsViewModel!.calculateExpiredBatch(date: dateTest)
        
        // Then
        XCTAssertFalse(result)
        
    }
    
    func testCalculateExpiredBatchShoudBeFalseWithNilValue() -> Void {
        // Given
        let dateTest: String? = nil
        
        // When
        let result = self.lotsViewModel!.calculateExpiredBatch(date: dateTest)
        
        // Then
        XCTAssertFalse(result)
    }
    
    func testCalculateExpiredBatchShoudBeFalseWithEmptyValue() -> Void {
        // Given
        let dateTest: String = ""
        // Then
        let result = self.lotsViewModel!.calculateExpiredBatch(date: dateTest)
        
        // When
        XCTAssertFalse(result)
    }
    
    func testFinishOrderDidTapSuccess() -> Void {
        self.lotsViewModel!.askIfUserWantToFinalizeOrder.subscribe(onNext:{ message in
            XCTAssertTrue(message == "¿Deseas terminar la orden?")
        }).disposed(by: self.disposeBag!)
        
        self.lotsViewModel!.finishOrderDidTap.onNext(())
    }
    
    func testFinishOrderDidTapSuccessShoulBeFalse() -> Void {
        self.lotsViewModel!.askIfUserWantToFinalizeOrder.subscribe(onNext: { res in
            XCTAssertFalse(res == "")
        }).disposed(by: self.disposeBag!)
        self.lotsViewModel!.finishOrderDidTap.onNext(())
    }
    
    func testPendingOrderDidTapSuccess() -> Void {
        self.lotsViewModel!.askIfUserWantChageOrderToPendigStatus.subscribe(onNext: { res in
            XCTAssertTrue(res == "La orden cambiará a estatus Pendiente, ¿quieres continuar?")
        }).disposed(by: self.disposeBag!)
        
        self.lotsViewModel!.pendingButtonDidTap.onNext(())
    }
    
    func testPendingOrderDidTapShoulbBeFalse() -> Void {
        self.lotsViewModel!.askIfUserWantChageOrderToPendigStatus.subscribe(onNext: { res in
            XCTAssertFalse(res == "")
        }).disposed(by: self.disposeBag!)
        
        self.lotsViewModel!.pendingButtonDidTap.onNext(())
    }
    
    func testAddLotsDidTapShoulBeNil() -> Void {

        self.lotsViewModel!.batchSelected.subscribe(onNext: { res in
            XCTAssertNil(res)
        }).disposed(by: self.disposeBag!)
        
        self.lotsViewModel!.addLotDidTap.onNext(())
    }
    
    func testAddDidTapDataLotsSelectedEmpty() -> Void {
        let lotSelected: [LotsSelected] = []
        let lotsAvailable = LotsAvailable(numeroLote: "19A51", cantidadDisponible: 0, cantidadAsignada: 0.3, cantidadSeleccionada: 0, sysNumber: 3, fechaExp: "23/01/2024")
        
        let batch = Lots(codigoProducto: "MP-014", descripcionProducto: "Sulfato de Cobre pentahidratado USP", almacen: "MG", totalNecesario: 0.5, totalSeleccionado: 0.0, lotesSelecionados: lotSelected, lotesDisponibles: [lotsAvailable])
        
        let lotsAvailableSleceted = LotsAvailable(numeroLote: "19F02", cantidadDisponible: 0.108800, cantidadAsignada: 0.7, cantidadSeleccionada:  0, sysNumber: 4, fechaExp: "07/06/2024")
        
        let documentLines = Lots(codigoProducto: "MP-014", descripcionProducto: "Sulfato de Cobre pentahidratado USP", almacen: "MN", totalNecesario: 0.1800000, totalSeleccionado: 0.100000, lotesSelecionados: [], lotesDisponibles: [])
        
        self.lotsViewModel!.documentLines.append(documentLines)
        
        self.lotsViewModel!.productSelected.onNext(batch)
        self.lotsViewModel!.availableSelected.onNext(lotsAvailableSleceted)
        
        self.lotsViewModel!.addLotDidTap.onNext(())
        
        self.lotsViewModel!.dataLotsSelected.subscribe(onNext: { res in
            XCTAssertEqual(res.count, 0)
        }).disposed(by: self.disposeBag!)
    }
    
    func testAddLotsDidTapSucess() -> Void {
        let lotSelected: [LotsSelected] = []
        let lotsAvailable = LotsAvailable(numeroLote: "19A51", cantidadDisponible: 0, cantidadAsignada: 0.3, cantidadSeleccionada: 0, sysNumber: 3, fechaExp: "23/01/2024")
        
        let batch = Lots(codigoProducto: "MP-014", descripcionProducto: "Sulfato de Cobre pentahidratado USP", almacen: "MG", totalNecesario: 0.5, totalSeleccionado: 0.0, lotesSelecionados: lotSelected, lotesDisponibles: [lotsAvailable])
    
        let lotsAvailableSleceted = LotsAvailable(numeroLote: "19F02", cantidadDisponible: 0.108800, cantidadAsignada: 0.7, cantidadSeleccionada:  0.108800, sysNumber: 4, fechaExp: "07/06/2024")
        
        
        let documentLines = Lots(codigoProducto: "MP-014", descripcionProducto: "Sulfato de Cobre pentahidratado USP", almacen: "MN", totalNecesario: 0.1800000, totalSeleccionado: 0.108800, lotesSelecionados: [], lotesDisponibles: [])
        
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
        self.networkManager.askIfOrderCanBeFinalized(orderId: self.orderId!).subscribe(onNext: { [weak self] res in
            XCTAssertNotNil(res)
            self?.expectation?.fulfill()
        }).disposed(by: self.disposeBag!)
        wait(for: [self.expectation!], timeout: 1000)
        
    }
    
    func testValidIfOrderCanBeFinalizedValidCode() {
        
        self.networkManager.askIfOrderCanBeFinalized(orderId: self.orderId!).subscribe(onNext: { [weak self] res in
            XCTAssertTrue(res.code == 200)
            self?.expectation?.fulfill()
        }).disposed(by: self.disposeBag!)
        wait(for: [self.expectation!], timeout: 1000)
    }
}

