//
//  SignaturePadViewModel.swift
//  Omicron
//
//  Created by Axity on 26/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import Foundation
import RxCocoa
import RxSwift
import Resolver

class SignaturePadViewModel {
    // MARK: - Variables
    var acceptDidTap = PublishSubject<Void>()
    var getSignature = BehaviorSubject<UIImage>(value: UIImage())
    var signatureIsDone = BehaviorSubject<Bool>(value: false)
    var getTypeSignature = BehaviorSubject<String>(value: "")
    var disposeBag = DisposeBag()
    let canGetSignature: Driver<Bool>
    var dismissSignatureView = PublishSubject<Void>()
    let showAlert = PublishSubject<String>()
    let whoRequestSignature = PublishSubject<String>()
    @Injected var orderDetailVC: OrderDetailViewModel
    @Injected var lotsViewModel: LotsViewModel
    init() {
        let input = Observable.combineLatest(self.getTypeSignature,
                                             self.getSignature, self.whoRequestSignature)
        let isValid = self.signatureIsDone.map({$0})
        self.canGetSignature = isValid.asDriver(onErrorJustReturn: false)
        self.acceptDidTap.withLatestFrom(input)
            .map({OrderSignature(signatureType: $0, signature: $1, whoRequestSignature: $2)})
            .subscribe(onNext: { [weak self] data in
                if data.signatureType == CommonStrings.signatureViewTitleQFB {
                    self?.dismissSignatureView.onNext(())
                    switch data.whoRequestSignature {
                    case ViewControllerIdentifiers.orderDetailViewController:
                        self?.orderDetailVC.qfbSignatureIsGet = true
                        self?.orderDetailVC.sqfbSignature = data.signature.toBase64() ?? CommonStrings.empty
                        self?.orderDetailVC.showSignatureView.onNext(CommonStrings.signatureViewTitleTechnical)
                    case ViewControllerIdentifiers.lotsViewController:
                        self?.lotsViewModel.qfbSignatureIsGet = true
                        self?.lotsViewModel.sqfbSignature = data.signature.toBase64() ?? CommonStrings.empty
                        self?.lotsViewModel.showSignatureView.onNext(CommonStrings.signatureViewTitleTechnical)
                    default:
                        print("")
                    }
                }
                if data.signatureType == CommonStrings.signatureViewTitleTechnical {
                    switch data.whoRequestSignature {
                    case ViewControllerIdentifiers.orderDetailViewController:
                        self?.orderDetailVC.technicalSignatureIsGet = true
                        self?.orderDetailVC.technicalSignature = data.signature.toBase64() ?? CommonStrings.empty
                        self?.orderDetailVC.validSignatures()
                    case ViewControllerIdentifiers.lotsViewController:
                        self?.lotsViewModel.technicalSignatureIsGet = true
                        self?.lotsViewModel.technicalSignature = data.signature.toBase64() ?? CommonStrings.empty
                        self?.lotsViewModel.callFinishOrderService()
                    default:
                        print("")
                    }
                    self?.dismissSignatureView.onNext(())
                }
            }).disposed(by: self.disposeBag)
    }
}
extension UIImage {
    func toBase64() -> String? {
        guard let imageData = self.pngData() else { return nil }
        return imageData.base64EncodedString(options: Data.Base64EncodingOptions.lineLength64Characters)
    }
}
