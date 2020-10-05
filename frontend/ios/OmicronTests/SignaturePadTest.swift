//
//  SignaturePadTest.swift
//  OmicronTests
//
//  Created by Vicente Cantú on 23/09/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import XCTest
import RxSwift
import Moya

@testable import Omicron

class SignaturePadTest: XCTestCase {

    // MARK: - Variables
    var signaturePadViewModel: SignaturePadViewModel?
    var disposeBag: DisposeBag?

    override func setUp() {
        print("XXXX setUp SignaturePadTest")
        signaturePadViewModel = SignaturePadViewModel()
        disposeBag = DisposeBag()
    }
    
    override func tearDown() {
        print("XXXX tearDown SignaturePadTest")
        signaturePadViewModel = nil
        disposeBag = nil
    }
    
    
    // MARK: - Test Functions
    func testValidSignatureType() {
        
        signaturePadViewModel!.getTypeSignature.onNext(CommonStrings.signatureViewTitleQFB)
        signaturePadViewModel!.getTypeSignature.subscribe(onNext: { typeSignature in
            XCTAssert(typeSignature == CommonStrings.signatureViewTitleQFB ||
                typeSignature == CommonStrings.signatureViewTitleTechnical)
        }).disposed(by: disposeBag!)
        
    }
    
    func testValidSignature() {
        
        signaturePadViewModel!.getSignature.onNext(UIImage(imageLiteralResourceName: "AppIcon"))
        signaturePadViewModel!.getSignature.subscribe(onNext: { signature in
            XCTAssertNotNil(signature.toBase64())
        }).disposed(by: disposeBag!)
        
    }
    
    func testValidSignatureRequest() {
        
        signaturePadViewModel!.whoRequestSignature.onNext(ViewControllerIdentifiers.orderDetailViewController)
        signaturePadViewModel!.whoRequestSignature.subscribe(onNext: { signatureRequest in
            XCTAssert(signatureRequest == ViewControllerIdentifiers.orderDetailViewController)
        }).disposed(by: disposeBag!)
    }
    
    func testAcceptDidTapSuccessOrderDetailViewController() -> Void {
        // Given
        let testIUmage = UIImage(named: ImagesNames.closeEye)
        
        self.signaturePadViewModel?.getTypeSignature.onNext(CommonStrings.signatureViewTitleQFB)
        self.signaturePadViewModel?.getSignature.onNext(testIUmage!)
        self.signaturePadViewModel?.whoRequestSignature.onNext(ViewControllerIdentifiers.orderDetailViewController)

        self.signaturePadViewModel?.orderDetailVC.showSignatureView.subscribe(onNext: { res in
            XCTAssertEqual(res, "Firma del Técnico")
            let qfbSignatureIsGet = self.signaturePadViewModel?.orderDetailVC.qfbSignatureIsGet
            XCTAssertEqual(qfbSignatureIsGet, true)
        }).disposed(by: self.disposeBag!)
        
        self.signaturePadViewModel?.acceptDidTap.onNext(())
        
    }
    
    func testAcceptDidTapSuccessLotsViewController() -> Void {
        // Given
        let testIUmage = UIImage(named: ImagesNames.closeEye)
        
        self.signaturePadViewModel?.getTypeSignature.onNext(CommonStrings.signatureViewTitleQFB)
        self.signaturePadViewModel?.getSignature.onNext(testIUmage!)
        self.signaturePadViewModel?.whoRequestSignature.onNext(ViewControllerIdentifiers.lotsViewController)

        self.signaturePadViewModel?.lotsViewModel.showSignatureView.subscribe(onNext: { res in
            XCTAssertEqual(res, CommonStrings.signatureViewTitleTechnical)
            let qfbSignature = self.signaturePadViewModel?.lotsViewModel.qfbSignatureIsGet
            XCTAssertEqual(qfbSignature, true)
        }).disposed(by: self.disposeBag!)
        
        self.signaturePadViewModel?.acceptDidTap.onNext(())
    }
    
    func testAcceptDidTapToBase64OrderLotsViewController() -> Void {
        // Given
        let testIUmage = UIImage(named: ImagesNames.closeEye)
        let expected64 = Base64.test1
        self.signaturePadViewModel?.getTypeSignature.onNext(CommonStrings.signatureViewTitleQFB)
        self.signaturePadViewModel?.getSignature.onNext(testIUmage!)
        self.signaturePadViewModel?.whoRequestSignature.onNext(ViewControllerIdentifiers.lotsViewController)

        self.signaturePadViewModel?.lotsViewModel.showSignatureView.subscribe(onNext: { res in
            let base64 = self.signaturePadViewModel?.lotsViewModel.sqfbSignature
            // When
            XCTAssertEqual(expected64, base64)
        }).disposed(by: self.disposeBag!)

        // Then
        self.signaturePadViewModel?.acceptDidTap.onNext(())
    }
    
    func testAcceptDidTapToBase64DetailViewController() -> Void {
        // Given
        let testIUmage = UIImage(named: ImagesNames.closeEye)
        let expected64 = Base64.test1
        self.signaturePadViewModel?.getTypeSignature.onNext(CommonStrings.signatureViewTitleQFB)
        self.signaturePadViewModel?.getSignature.onNext(testIUmage!)
        self.signaturePadViewModel?.whoRequestSignature.onNext(ViewControllerIdentifiers.orderDetailViewController)

        self.signaturePadViewModel?.orderDetailVC.showSignatureView.subscribe(onNext: { res in
            // When
            let base64 = self.signaturePadViewModel?.orderDetailVC.sqfbSignature
            XCTAssertEqual(expected64, base64)
        }).disposed(by: self.disposeBag!)
        
        // Then
        self.signaturePadViewModel?.acceptDidTap.onNext(())
    }
    
    func testAcceptDidTapSignatureTypeOrderDetailViewController() -> Void {
        // Given
        let testIUmage = UIImage(named: ImagesNames.closeEye)
        self.signaturePadViewModel?.getTypeSignature.onNext(CommonStrings.signatureViewTitleTechnical)
        self.signaturePadViewModel?.getSignature.onNext(testIUmage!)
        self.signaturePadViewModel?.whoRequestSignature.onNext(ViewControllerIdentifiers.orderDetailViewController)
        // Then
        self.signaturePadViewModel?.acceptDidTap.onNext(())
        
        // When
        XCTAssertEqual(self.signaturePadViewModel?.orderDetailVC.technicalSignatureIsGet, true)
    }
    
    func testAcceptDidTapSignatureTypeOrderLotsViewController() -> Void {
        // Given
        let testIUmage = UIImage(named: ImagesNames.closeEye)
        self.signaturePadViewModel?.getTypeSignature.onNext(CommonStrings.signatureViewTitleTechnical)
        self.signaturePadViewModel?.getSignature.onNext(testIUmage!)
        self.signaturePadViewModel?.whoRequestSignature.onNext(ViewControllerIdentifiers.lotsViewController)
        // Then
        self.signaturePadViewModel?.acceptDidTap.onNext(())
        
        // When
        XCTAssertEqual(self.signaturePadViewModel?.lotsViewModel.technicalSignatureIsGet, true)
    }

}
