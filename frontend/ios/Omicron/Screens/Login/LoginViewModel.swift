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
    public let finishedLogin : PublishSubject<Void> = PublishSubject()
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
                Login(username: $0, password: $1, redirectUri: "", clientId2: "")
            })
            .subscribe(onNext: { data in
                self.loading.onNext(true)
                NetworkManager.shared.login(data: data)
                    .flatMap({ res -> Observable<UserInfoResponse> in
                        Persistence.shared.saveLoginData(data: res)
                        return NetworkManager.shared.getInfoUser(username: data.user)
                    }).subscribe(onNext: { [weak self] info  in
                        self?.loading.onNext(false)
                        if let user = info.response {
                            Persistence.shared.saveUserData(user: user)
                            self?.finishedLogin.onNext(())
                        } else {
                            self?.error.onNext(Constants.Errors.serverError.rawValue)
                        }
                        }, onError: { [weak self] err in
                            self?.loading.onNext(false)
                            switch (err) {
                            case RequestError.serverError(let httpError), RequestError.invalidRequest(let httpError):
                                self?.error.onNext(httpError?.userError ?? Constants.Errors.serverError.rawValue)
                            case RequestError.unauthorized(let httpError):
                                self?.error.onNext(httpError?.userError ?? Constants.Errors.unauthorized.rawValue)
                            default:
                                self?.error.onNext(Constants.Errors.serverError.rawValue)
                            }
                    }).disposed(by: self.disposeBag)
            }).disposed(by: disposeBag)
    }
}
