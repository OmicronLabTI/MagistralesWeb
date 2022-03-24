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
import Resolver

class LoginViewModel {
    public let finishedLogin: PublishSubject<Void> = PublishSubject()
    public let loading: PublishSubject<Bool> = PublishSubject()
    public let error: PublishSubject<String> = PublishSubject()
    var username = BehaviorSubject<String>(value: String())
    var password = BehaviorSubject<String>(value: String())
    var loginDidTap = PublishSubject<Void>()
    let canLogin: Driver<Bool>
    private let disposeBag = DisposeBag()

    @Injected var networkManager: NetworkManager

    init() {
        let input = Observable.combineLatest(username, password)
        let isValid = input.map({ $0.isEmpty == false && $1.isEmpty == false })
        self.canLogin = isValid.asDriver(onErrorJustReturn: false)
        loginDidTap
            .withLatestFrom(input)
            .map({
                Login(
                    username: $0, password: self.passwordToBase64($1),
                    redirectUri: String(), clientId2: String(), origin: "app")
            }).subscribe(onNext: { [unowned self] data in
                self.loading.onNext(true)
                self.loginService(data)
            }).disposed(by: disposeBag)
    }

    func loginService(_ data: Login) {
        networkManager.login(data)
            .flatMap({ res -> Observable<UserInfoResponse> in
                Persistence.shared.saveLoginData(data: res)
                return self.networkManager.getInfoUser(data.user)
            }).subscribe(onNext: { [weak self] info  in
                guard let self = self else { return }
                self.loading.onNext(false)
                self.onSuccessLogin(info)
            }, onError: { [weak self] err in
                guard let self = self else { return }
                self.loading.onNext(false)
                self.onErrorLogin(err)
            }).disposed(by: self.disposeBag)
    }

    func onSuccessLogin(_ info: UserInfoResponse) {
        if let user = info.response {
            Persistence.shared.saveUserData(user: user)
            Persistence.shared.saveIsLogged(isLogged: true)
            self.finishedLogin.onNext(())
        } else {
            self.error.onNext(Constants.Errors.serverError.rawValue)
        }
    }

    func onErrorLogin(_ err: Error) {
        switch err {
        case RequestError.serverError(let httpError), RequestError.invalidRequest(let httpError):
            self.error.onNext(httpError?.userError ?? Constants.Errors.serverError.rawValue)
        case RequestError.unauthorized(let httpError):
            self.error.onNext(httpError?.userError ?? Constants.Errors.unauthorized.rawValue)
        default:
            self.error.onNext(Constants.Errors.serverError.rawValue)
        }
    }

    private func passwordToBase64(_ password: String) -> String {
        let utf8str = password.data(using: .utf8)
        if let base64Encoded = utf8str?.base64EncodedString(options: Data.Base64EncodingOptions(rawValue: 0)) {
            print("Encoded: \(base64Encoded)")
            return base64Encoded
        }
        return String()
    }
}
