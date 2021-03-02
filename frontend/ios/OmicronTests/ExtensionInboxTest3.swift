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
        inboxViewModel?.postOrderPDf(orders: [orderID])
    }

    func testCallFinishOrderService() {
        inboxViewModel?.qfbSignatureIsGet = true
        inboxViewModel?.technicalSignatureIsGet = true
        inboxViewModel?.indexPathOfOrdersSelected = [IndexPath(row: 0, section: 0)]
        inboxViewModel?.sectionOrders = [SectionModel(model: CommonStrings.empty, items: [order1!])]
        inboxViewModel?.isUserInteractionEnabled.subscribe(onNext: { res in
            XCTAssertTrue(res)
        }).disposed(by: disposeBag!)
        inboxViewModel?.callFinishOrderService()
    }

    func testValidOrders() {
        let ordersSelelected = IndexPath(row: 0, section: 0)
        inboxViewModel?.sectionOrders = [SectionModel(model: CommonStrings.empty, items: [order1!])]

        inboxViewModel?.validOrders(indexPathOfOrdersSelected: [ordersSelelected])
    }
}
