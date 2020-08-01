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
    case unauthorized
    case notFound
    case invalidResponse
    case serverError(error: HttpError?)
    case serverUnavailable
}

class NetworkManager {
    static let shared: NetworkManager = NetworkManager()
    private var provider: MoyaProvider<ApiService> = MoyaProvider<ApiService>(plugins: [AccessTokenPlugin{_ in "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJwcm9maWxlIjoiYWRtaW4iLCJleHAiOjE1OTYyNzE5NzYsInVzZXIiOiJzZXJnaW8ifQ.0UTtUBZAeJ_Ehn8pv5oECQER33hAatP5qPYBNX7dQhc"}])
    
    init(provider: MoyaProvider<ApiService> = MoyaProvider<ApiService>()) {
        self.provider = provider
    }
    
    func login(data: Login) -> Observable<LoginResponse> {
        let req: ApiService = ApiService.login(data: data)
        let res: Observable<LoginResponse> = makeRequest(request: req)
        return res
    }
    
//    func getInfoUser() -> Observable<UserInfoResponse> {
//        let req: ApiService = ApiService.getInfoUser()
//        let res: Observable<UserInfoResponse> = makeRequest(request: req)
//        return res
//    }
    
    private func makeRequest<T: BaseMappable>(request: ApiService) -> Observable<T> {
        return Observable<T>.create({ [weak self] observer in
            self?.provider.request(request) { result in
                switch result {
                case let .success(response):
                    let statusCode = response.statusCode
                    let json = try? response.mapJSON()
                    
                    switch statusCode {
                    case 200...299:
                        let res = Mapper<T>().map(JSONObject: json)
                        if (res != nil){
                            observer.onNext(res!)
                        } else {
                            observer.onError(RequestError.invalidResponse)
                        }
                        break
                    case 400:
                        let err = Mapper<HttpError>().map(JSONObject: json)
                        observer.onError(RequestError.invalidRequest(error: err))
                        break
                    case 401:
                        observer.onError(RequestError.unauthorized)
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
                case .failure(_):
                    observer.onError(RequestError.serverUnavailable)
                    break
                }
                
                observer.onCompleted()
            }
            return Disposables.create()
        })
    }
}
