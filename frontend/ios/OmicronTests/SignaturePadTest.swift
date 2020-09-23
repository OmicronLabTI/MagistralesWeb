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

    override func setUpWithError() throws {
        // Put setup code here. This method is called before the invocation of each test method in the class.
    }

    override func tearDownWithError() throws {
        // Put teardown code here. This method is called after the invocation of each test method in the class.
    }
    
    // MARK: - Variables
    let networkManager = NetworkManager(provider: MoyaProvider<ApiService>(stubClosure: MoyaProvider.immediatelyStub))
    let signaturePadViewModel = SignaturePadViewModel()
    let disposeBag = DisposeBag()
    
    // MARK: - Test Functions
    
    func testValidSignatureType() {
        
        signaturePadViewModel.getTypeSignature.onNext(CommonStrings.signatureViewTitleQFB)
        signaturePadViewModel.getTypeSignature.subscribe(onNext: { typeSignature in
            XCTAssert(typeSignature == CommonStrings.signatureViewTitleQFB ||
                typeSignature == CommonStrings.signatureViewTitleTechnical)
        }).disposed(by: disposeBag)
        
    }
    
    func testValidSignature() {
        
        signaturePadViewModel.getSignature.onNext(UIImage(imageLiteralResourceName: "AppIcon"))
        signaturePadViewModel.getSignature.subscribe(onNext: { signature in
            XCTAssertNotNil(signature.toBase64())
        }).disposed(by: disposeBag)
        
    }
    
    func testValidSignatureRequest() {
        
        signaturePadViewModel.whoRequestSignature.onNext(ViewControllerIdentifiers.orderDetailViewController)
        signaturePadViewModel.whoRequestSignature.subscribe(onNext: { signatureRequest in
            XCTAssert(signatureRequest == ViewControllerIdentifiers.orderDetailViewController)
        }).disposed(by: disposeBag)
        
    }

}
