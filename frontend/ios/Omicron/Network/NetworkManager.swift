//
//  NetworkManager.swift
//  Omicron
//
//  Created by Diego Cárcamo on 09/07/2020.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import Foundation
import Moya
import RxSwift
import ObjectMapper

enum RequestError: Error {
    case unknownError
    case invalidRequest(error: HttpError?)
    case unauthorized(error: HttpError?)
    case notFound
    case invalidResponse
    case serverError(error: HttpError?)
    case serverUnavailable
}

protocol AuthorizedTargetType: TargetType {
    var needsAuth: Bool { get }
}

class NetworkManager: SessionProtocol {
    static let shared: NetworkManager = NetworkManager()
    private lazy var provider: MoyaProvider<ApiService> = MoyaProvider<ApiService>()
    
    init(provider: MoyaProvider<ApiService> = MoyaProvider<ApiService>(plugins: [
        AuthPlugin(tokenClosure: { return Persistence.shared.getLoginData()?.access_token }),
        NetworkLoggerPlugin(configuration: .init(formatter: .init(responseData: JSONResponseDataFormatter), logOptions: .verbose))
        ])) {
        self.provider = provider
    }
    
    func getTokenRefreshService() -> Single<Response> {
        let data = Renew(refreshToken: Persistence.shared.getLoginData()?.refresh_token ?? "")
        return self.provider.rx.request(.renew(data: data))
    }

    // Parse and save your token locally or do any thing with the new token here
    func tokenDidRefresh(response: LoginResponse) {
        Persistence.shared.saveLoginData(data: response)
    }

    // Log the user out or do anything related here
    public func didFailedToRefreshToken() {
        // TODO LOGOUT USER
    }
    
    func login(data: Login) -> Observable<LoginResponse> {
        let req: ApiService = ApiService.login(data: data)
        let res: Observable<LoginResponse> = makeRequest(request: req)
        return res
    }
    
    func getInfoUser(userId: String) -> Observable<UserInfoResponse> {
        let req: ApiService = ApiService.getInfoUser(userId: userId)
        let res: Observable<UserInfoResponse> = makeRequest(request: req)
        return res
    }
    
    func getStatusList(qfbId: String) -> Observable<StatusResponse> {
        let req: ApiService = ApiService.getStatusList(qfbId: qfbId)
        let res: Observable<StatusResponse> = makeRequest(request: req)
        return res
    }
    
    func renew(data: Renew) -> Observable<LoginResponse> {
        let req: ApiService = ApiService.renew(data: data)
        let res: Observable<LoginResponse> = makeRequest(request: req)
        return res
    }
    
    func getOrdenDetail(orderId: Int) -> Observable<OrderDetailResponse> {
        let req: ApiService = ApiService.getOrdenDetail(orderId: orderId)
        let res: Observable<OrderDetailResponse> = makeRequest(request: req)
        return res
    }
    
    
    func deleteItemOfOrdenDetail(orderDetailRequest:  OrderDetailRequest) -> Observable<OrderDetailResponse> {
        let req: ApiService = ApiService.deleteItemOfOrdenDetail(orderDetailRequest: orderDetailRequest)
        let res: Observable<OrderDetailResponse> = makeRequest(request: req)
        return res
    }

    private func makeRequest<T: BaseMappable>(request: ApiService) -> Observable<T> {
        return Observable<T>.create({ [weak self] observer in
            let r = !request.needsAuth ?
                self?.provider.rx.request(request) :
                self?.provider.rx
                                .request(request)
                                .filterSuccessfulStatusAndRedirectCodes()
                                .refreshAuthenticationTokenIfNeeded(sessionServiceDelegate: self!)
            
            let _ = r?.asObservable().subscribe(onNext: { response in
                let json = try? response.mapJSON()
                
                let res = Mapper<T>().map(JSONObject: json)
                if (res != nil) {
                    observer.onNext(res!)
                } else {
                    observer.onError(RequestError.invalidResponse)
                }
            }, onError: { error in
                if let moyaError: MoyaError = error as? MoyaError, let res = moyaError.response {
                    let statusCode = res.statusCode
                    let json = try? res.mapJSON()

                    switch statusCode {
                    case 400:
                        let err = Mapper<HttpError>().map(JSONObject: json)
                        observer.onError(RequestError.invalidRequest(error: err))
                        break
                    case 401:
                        let err = Mapper<HttpError>().map(JSONObject: json)
                        observer.onError(RequestError.unauthorized(error: err))
                        break
                    case 404:
                        observer.onError(RequestError.notFound)
                        break
                    case 500...:
                        let err = Mapper<HttpError>().map(JSONObject: json)
                        observer.onError(RequestError.serverError(error: err))
                        break
                    default:
                        observer.onError(RequestError.unknownError)
                        break
                    }
                    return
                }
                observer.onError(RequestError.serverUnavailable)
            })
            
            return Disposables.create()
        })
    }
}

private func JSONResponseDataFormatter(_ data: Data) -> String {
    do {
        let dataAsJSON = try JSONSerialization.jsonObject(with: data)
        let prettyData = try JSONSerialization.data(withJSONObject: dataAsJSON, options: .prettyPrinted)
        return String(data: prettyData, encoding: .utf8) ?? String(data: data, encoding: .utf8) ?? ""
    } catch {
        return String(data: data, encoding: .utf8) ?? ""
    }
}

struct AuthPlugin: PluginType {
    let tokenClosure: () -> String?
    
    func prepare(_ request: URLRequest, target: TargetType) -> URLRequest {
        guard
            let token = tokenClosure(),
            let target = target as? AuthorizedTargetType,
            target.needsAuth
            else {
                return request
        }
        
        var request = request
        request.addValue("Bearer " + token, forHTTPHeaderField: "Authorization")
        return request
    }
}

private protocol SessionProtocol {
    func getTokenRefreshService() -> Single<Response>
    func didFailedToRefreshToken()
    func tokenDidRefresh (response: LoginResponse)
}

private extension PrimitiveSequence where Trait == SingleTrait, Element == Response {
    // Tries to refresh auth token on 401 error and retry the request.
    // If the refresh fails it returns an error .
    func refreshAuthenticationTokenIfNeeded(sessionServiceDelegate : SessionProtocol) -> Single<Response> {
        return
            // Retry and process the request if any error occurred
            self.retryWhen { responseFromFirstRequest in
                responseFromFirstRequest.asObservable().flatMap { originalRequestResponseError -> PrimitiveSequence<SingleTrait, Element> in
                    if let moyaError: MoyaError = originalRequestResponseError as? MoyaError, let res = moyaError.response {
                        let statusCode = res.statusCode
                        if statusCode == 401 {
                            // Token expired >> Call refresh token request
                            return sessionServiceDelegate
                                .getTokenRefreshService()
                                .filterSuccessfulStatusAndRedirectCodes()
                                .catchError { tokenRefreshRequestError -> Single<Response> in
                                    // Failed to refresh token
                                    //
                                    // Logout or do any thing related
                                    sessionServiceDelegate.didFailedToRefreshToken()
                                    return Single.error(tokenRefreshRequestError)
                            }
                            .flatMap { tokenRefreshResponse -> Single<Response> in
                                // Refresh token response string
                                // Save new token locally to use with any request from now on
                                if let json = try? tokenRefreshResponse.mapJSON(), let tokenRes = Mapper<LoginResponse>().map(JSONObject: json) {
                                    sessionServiceDelegate.tokenDidRefresh(response: tokenRes)
                                }

                                // Retry the original request one more time
                                return self.retry(1)
                            }
                        }
                        else {
                            // Retuen errors other than 401 & 403 of the original request
                            return Single.error(originalRequestResponseError)
                        }
                    }
                    // Return any other error
                    return Single.error(originalRequestResponseError)
                }
        }
    }
}
