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

class SignaturePadViewModel  {
    
    //MARK: - Variables
    var acceptDidTap = PublishSubject<Void>()
    var getSignature = BehaviorSubject<UIImage>(value: UIImage())
    var signatureIsDone = BehaviorSubject<Bool>(value: false)
    var getTypeSignature = BehaviorSubject<String>(value: "")
    var disposeBag = DisposeBag()
    let canGetSignature: Driver<Bool>
    var dismissSignatureView = PublishSubject<Void>()
    let showAlert = PublishSubject<String>()
    @Injected var orderDetailVC: OrderDetailViewModel
    
    
    init() {
        
        let input = Observable.combineLatest(self.getTypeSignature,self.getSignature)
        let isValid = self.signatureIsDone.map({$0})
        self.canGetSignature = isValid.asDriver(onErrorJustReturn: false)
    
        self.acceptDidTap.withLatestFrom(input).map({OrderSignature(signatureType: $0, signature: $1)}).subscribe(onNext: { data in
            if (data.signatureType == CommonStrings.signatureViewTitleQFB) {
                self.orderDetailVC.qfbSignatureIsGet = true
                self.dismissSignatureView.onNext(())
                // Guarda la firma en storage del ipad
                FileManagerApp.shared.saveSignatureOnIpad(signature: data.signature, name: FileManagerConstants.qfbSignatureName)
                self.orderDetailVC.sqfbSignature = data.signature.toBase64() ?? ""
                self.orderDetailVC.showSignatureView.onNext(CommonStrings.signatureViewTitleTechnical)
            }
            
            if(data.signatureType == CommonStrings.signatureViewTitleTechnical)  {
                self.orderDetailVC.technicalSignatureIsGet = true
                self.dismissSignatureView.onNext(())
                // guarda la firma en el storage del ipad
                FileManagerApp.shared.saveSignatureOnIpad(signature: data.signature, name: FileManagerConstants.technicalSignatureName)
                self.orderDetailVC.technicalSignature = data.signature.toBase64() ?? ""
                self.orderDetailVC.validSignatures()
            }
        }).disposed(by: self.disposeBag)
    
    }
        
//    func saveSignatureOnIpad(signature: UIImage, name: String) -> Void {
//        let directoryURL = FileManager.default.urls(for: .documentDirectory, in: .userDomainMask)[0]
//        let fileURL = URL(fileURLWithPath: name, relativeTo: directoryURL).appendingPathExtension("jpg")
//        guard let data = signature.jpegData(compressionQuality: 1) else { return }
//
//        // Guarda los datos
//        do {
//            try data.write(to: fileURL)
//        } catch {
//            self.showAlert.onNext("No se pudo guardar la firma, intentar de nuevo")
//            print(error.localizedDescription)
//        }
//    }
}



extension UIImage {
    func toBase64() -> String? {
        guard let imageData = self.pngData() else { return nil }
        return imageData.base64EncodedString(options: Data.Base64EncodingOptions.lineLength64Characters)
    }
}
