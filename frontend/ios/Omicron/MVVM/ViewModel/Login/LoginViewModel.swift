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
    var statusCode = 500
    var testData = Data()
    var isTest = false
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
            })
            .subscribe(onNext: { [unowned self] data in
                self.loading.onNext(true)
                self.networkManager.login(data: data, needsError: isTest, statusCode: statusCode, testData: testData)
                    .flatMap({ res -> Observable<UserInfoResponse> in
                        Persistence.shared.saveLoginData(data: res)
                        return self.networkManager.getInfoUser(username: data.user, isTest: isTest, statusCode: statusCode, testData: testData)
                    }).subscribe(onNext: { [weak self] info  in
                        self?.loading.onNext(false)
                        if let user = info.response {
                            Persistence.shared.saveUserData(user: user)
                            Persistence.shared.saveIsLogged(isLogged: true)
                            self?.finishedLogin.onNext(())
                        } else {
                            self?.error.onNext(Constants.Errors.serverError.rawValue)
                        }
                        }, onError: { [weak self] err in
                            self?.loading.onNext(false)
                            switch err {
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

    private func passwordToBase64(_ password: String) -> String {
        let utf8str = password.data(using: .utf8)

        if let base64Encoded = utf8str?.base64EncodedString(options: Data.Base64EncodingOptions(rawValue: 0)) {
            print("Encoded: \(base64Encoded)")
            return base64Encoded
        }
        return String()
    }
}
