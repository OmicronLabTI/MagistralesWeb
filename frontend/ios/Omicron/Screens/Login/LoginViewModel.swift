//
//  LoginViewModel.swift
//  Omicron
//
//  Created by Diego Cárcamo on 09/07/2020.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import Foundation
import RxSwift
import RxCocoa
import CryptoKit

class LoginViewModel {
    public let loginResponse : PublishSubject<LoginResponse> = PublishSubject()
    public let loading: PublishSubject<Bool> = PublishSubject()
    public let error : PublishSubject<String> = PublishSubject()
    
    var username = BehaviorSubject<String>(value: "")
    var password = BehaviorSubject<String>(value: "")
    var loginDidTap = PublishSubject<Void>()
    
    let canLogin: Driver<Bool>
    
    private let disposeBag = DisposeBag()
    
    init() {
        let input = Observable.combineLatest(username, password)
        let isValid = input.map({ $0.isEmpty == false && $1.isEmpty == false })
        
        self.canLogin = isValid.asDriver(onErrorJustReturn: false)
        
        loginDidTap
            .withLatestFrom(input)
            .map({
                Login(username: $0, password: $1)
            })
            .subscribe(onNext: { data in
                self.loading.onNext(true)
                NetworkManager.shared.login(data: data).subscribe(onNext: { [weak self] res in
                    self?.loading.onNext(false)
                    UserDefaults.standard.set(true, forKey: "SessionActive")
                    self?.loginResponse.onNext(res)
                    }, onError: { [weak self] err in
                        self?.loading.onNext(false)
                        switch (err) {
                        case RequestError.serverError(let httpError), RequestError.invalidRequest(let httpError):
                            self?.error.onNext(httpError?.error ?? Constants.Errors.serverError.rawValue)
                        default:
                            self?.error.onNext(Constants.Errors.serverError.rawValue)
                        }
                }).disposed(by: self.disposeBag)
            }).disposed(by: disposeBag)
    }
    
    func convertPasswordToSHA256(password: String) -> String {
        guard let data = password.data(using: .utf8) else { return  "" }
        let digest = SHA256.hash(data: data)
        return digest.hexStr
    }
}

extension Digest {
    var bytes: [UInt8] { Array(makeIterator()) }
    var data: Data { Data(bytes) }
    
    var hexStr: String {
        bytes.map {
            String(format: "%02X", $0)
        }.joined()
    }
}
