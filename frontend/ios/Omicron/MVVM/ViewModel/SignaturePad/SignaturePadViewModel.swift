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
    var getTypeSignature = BehaviorSubject<String>(value: CommonStrings.empty)
    var disposeBag = DisposeBag()
    let canGetSignature: Driver<Bool>
    var dismissSignatureView = PublishSubject<Void>()
    let showAlert = PublishSubject<String>()
    let whoRequestSignature = PublishSubject<String>()

    @Injected var orderDetailVC: OrderDetailViewModel
    @Injected var lotsViewModel: LotsViewModel
    @Injected var inboxVM: InboxViewModel

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
                    self?.caseWhenQFBsigns(data: data)
                }
                if data.signatureType == CommonStrings.signatureViewTitleTechnical {
                    self?.caseWhenTheTechnicianSigns(data: data)
                    self?.dismissSignatureView.onNext(())
                }
            }).disposed(by: self.disposeBag)
    }

    func caseWhenQFBsigns(data: OrderSignature) {
        switch data.whoRequestSignature {
        case ViewControllerIdentifiers.orderDetailViewController:
            orderDetailVC.qfbSignatureIsGet = true
            orderDetailVC.sqfbSignature = data.signature.toBase64() ?? CommonStrings.empty
            orderDetailVC.showSignatureView.onNext(CommonStrings.signatureViewTitleTechnical)
        case ViewControllerIdentifiers.lotsViewController:
            lotsViewModel.qfbSignatureIsGet = true
            lotsViewModel.sqfbSignature = data.signature.toBase64() ?? CommonStrings.empty
            lotsViewModel.showSignatureView.onNext(CommonStrings.signatureViewTitleTechnical)
        case ViewControllerIdentifiers.inboxViewController:
            inboxVM.qfbSignatureIsGet = true
            inboxVM.sqfbSignature = data.signature.toBase64() ?? CommonStrings.empty
            inboxVM.showSignatureVc.onNext(CommonStrings.signatureViewTitleTechnical)
        default:
            break
        }
    }

    func caseWhenTheTechnicianSigns(data: OrderSignature) {
        switch data.whoRequestSignature {
        case ViewControllerIdentifiers.orderDetailViewController:
            orderDetailVC.technicalSignatureIsGet = true
            orderDetailVC.technicalSignature = data.signature.toBase64() ?? CommonStrings.empty
            orderDetailVC.validSignatures()
        case ViewControllerIdentifiers.lotsViewController:
            lotsViewModel.technicalSignatureIsGet = true
            lotsViewModel.technicalSignature = data.signature.toBase64() ?? CommonStrings.empty
            lotsViewModel.callFinishOrderService()
        case ViewControllerIdentifiers.inboxViewController:
            inboxVM.technicalSignatureIsGet = true
            inboxVM.technicalSignature = data.signature.toBase64() ?? CommonStrings.empty
            inboxVM.callFinishOrderService()
        default:
            break
        }
    }
}
extension UIImage {
    func toBase64() -> String? {
        guard let imageData = self.pngData() else { return nil }
        return imageData.base64EncodedString(options: Data.Base64EncodingOptions.lineLength64Characters)
    }
}
