//
//  ExtensionInboxTest3.swift
//  OmicronTests
//
//  Created by Sergio Flores Ramirez on 23/02/21.
//  Copyright © 2021 Diego Cárcamo. All rights reserved.
//

import XCTest
import RxSwift
import RxDataSources

@testable import OmicronLab
class ExtensionInboxTest3: XCTestCase {
    var inboxViewModel: InboxViewModel?
    var disposeBag: DisposeBag?
    var order1: Order?
    var expectedResult: String?
    override func setUpWithError() throws {
        // Put setup code here. This method is called before the invocation of each test method in the class.
        inboxViewModel = InboxViewModel()
        disposeBag = DisposeBag()
        order1 = Order(
            productionOrderId: 89284, baseDocument: 60067, container: "",
            tag: "Selecciona una...", plannedQuantity: 1, startDate: "27/08/2020",
            finishDate: "06/09/2020",
            descriptionProduct: "Aceite de Arbol de Te 0.3%, Alantoina 0.3%, Citrico 0.2%, " +
            "Extracto de Te Verde 3%, Extracto de Pepino 3%, Glicerina 3%, Hamamelis 3%, Hialuronico 3%, " +
            "Menta Piperita 0.02%, Niacinamida 2%, Pantenol 0.5%,  Salicilico 0.5%, Urea 5%, Solucion",
            statusId: 1, itemCode: "3264   120 ML", productCode: "3264", destiny: "Foráneo",
            hasMissingStock: false, finishedLabel: false)
        expectedResult = "http://172.30.5.49:5002/Pruebas_ArchivosOmicronTemp/SaleOrders/Order76260.pdf"
    }

    override func tearDownWithError() throws {
        // Put teardown code here. This method is called after the invocation of each test method in the class.
        inboxViewModel = nil
        disposeBag = nil
        order1 = nil
        expectedResult = nil
    }

    func testFinishedDidTapBinding() {
        inboxViewModel?.showAlertToChangeOrderOfStatus.subscribe(onNext: { message in
            print(message)
            XCTAssertEqual(message.typeOfStatus, StatusNameConstants.finishedStatus)
            XCTAssertEqual(message.message, CommonStrings.confirmationMessageFinishedStatus)
        }).disposed(by: disposeBag!)
        inboxViewModel?.finishedDidTap.onNext(())
    }

    func testViewKPIDidPressed() {
        inboxViewModel?.showKPIView.subscribe(onNext: { res in
            XCTAssertTrue(res)
        }).disposed(by: disposeBag!)
        inboxViewModel?.viewKPIDidPressed.onNext(())
    }

    func testSelectOrder() {
        inboxViewModel?.orderURLPDF.subscribe(onNext: { [weak self] res in
            XCTAssertEqual(res, self?.expectedResult)
        }).disposed(by: disposeBag!)
        inboxViewModel?.selectOrder.onNext(1234667)
    }

    func testPostOrderPDf() {
        let orderID = 1234
        inboxViewModel?.orderURLPDF.subscribe(onNext: { [weak self] res in
            XCTAssertEqual(res, self?.expectedResult)
        }).disposed(by: disposeBag!)
        inboxViewModel?.postOrderPDf(orders: [orderID], needsError: false)
    }

    func testPostPDFWhenCodeIs500() {
        let orderID = 1234
        inboxViewModel?.showAlert.subscribe(onNext: { res in
            XCTAssertEqual(res, CommonStrings.errorPDF)
        }).disposed(by: disposeBag!)
        inboxViewModel?.postOrderPDf(orders: [orderID], needsError: true)
    }

    func testCallFinishOrderService() {
        inboxViewModel?.qfbSignatureIsGet = true
        inboxViewModel?.technicalSignatureIsGet = true
        inboxViewModel?.indexPathOfOrdersSelected = [IndexPath(row: 0, section: 0)]
        inboxViewModel?.sectionOrders = [SectionModel(model: CommonStrings.empty, items: [order1!])]
        inboxViewModel?.isUserInteractionEnabled.subscribe(onNext: { res in
            XCTAssertTrue(res)
        }).disposed(by: disposeBag!)
        inboxViewModel?.callFinishOrderService(needsError: false, statusCode: 200, testData: Data())
    }

    func testCallFinishOrderServiceWhenCodeIs500() {
        inboxViewModel?.qfbSignatureIsGet = true
        inboxViewModel?.technicalSignatureIsGet = true
        inboxViewModel?.indexPathOfOrdersSelected = [IndexPath(row: 0, section: 0)]
        inboxViewModel?.sectionOrders = [SectionModel(model: CommonStrings.empty, items: [order1!])]
        inboxViewModel?.showAlert.subscribe(onNext: { res in
            XCTAssertEqual(res, CommonStrings.errorFinishOrders)
        }).disposed(by: disposeBag!)
        inboxViewModel?.callFinishOrderService(needsError: true, statusCode: 200, testData: Data())
    }

    func testValidOrders() {
        let ordersSelelected = IndexPath(row: 0, section: 0)
        inboxViewModel?.sectionOrders = [SectionModel(model: CommonStrings.empty, items: [order1!])]

        inboxViewModel?.validOrders(indexPathOfOrdersSelected: [ordersSelelected], needsError: false)
    }

    func testValidOrderWhenCodeIs400() {
        // swiftlint:disable line_length
        let expectedResult = "No es posible Terminar, faltan lotes para: \n122307 MP-157\n122363 MP-368\n122366 MP-157\n122368 BA-14\n122368 GR-161\n\n No es posible Terminar, falta existencia para: \n122307 EN-089\n122363 MP-368\n122366 MP-157"
        let ordersSelelected = IndexPath(row: 0, section: 0)
        inboxViewModel?.sectionOrders = [SectionModel(model: CommonStrings.empty, items: [order1!])]
        inboxViewModel?.showAlert.subscribe(onNext: { res in
            XCTAssertEqual(res, expectedResult)
        }).disposed(by: disposeBag!)
        guard let url = Bundle.main.url(forResource: "FinishOrdersErrorResponse", withExtension: "json"),
            let data = try? Data(contentsOf: url) else {
            return
        }
        inboxViewModel?.validOrders(indexPathOfOrdersSelected: [ordersSelelected], needsError: true, statusCode: 200, testData: data)
    }

    func testValidOrderWhenCodeIs500() {
        let ordersSelelected = IndexPath(row: 0, section: 0)
        inboxViewModel?.sectionOrders = [SectionModel(model: CommonStrings.empty, items: [order1!])]
        inboxViewModel?.showAlert.subscribe(onNext: { res in
            XCTAssertEqual(res, Constants.Errors.errorData.rawValue)
        }).disposed(by: disposeBag!)
        inboxViewModel?.validOrders(indexPathOfOrdersSelected: [ordersSelelected], needsError: true, statusCode: 500, testData: Data())
    }
}
