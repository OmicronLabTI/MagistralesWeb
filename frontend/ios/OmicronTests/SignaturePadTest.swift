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

}
