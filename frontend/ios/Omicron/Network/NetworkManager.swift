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
import Alamofire

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
    // MARK: Variables
    let requestTimeoutClosure = { (endpoint: Endpoint, done: @escaping MoyaProvider<ApiService>.RequestResultClosure) in
        do {
            var request = try endpoint.urlRequest()
            request.timeoutInterval = 10
            done(.success(request))
        } catch {
            return
        }
    }

    func serverErrorEndpoint(statusCode: Int, data: Data) -> (ApiService) -> Endpoint {
        let serverErrorEndpointClosure = { (target: ApiService) -> Endpoint in
            return Endpoint(
                url: URL(target: target).absoluteString,
                sampleResponseClosure: { .networkResponse(statusCode, data) },
                method: target.method, task: target.task, httpHeaderFields: target.headers)
        }
        return serverErrorEndpointClosure
    }
    private lazy var providerChoseed = MoyaProvider<ApiService>()
    private lazy var provider: MoyaProvider<ApiService> = MoyaProvider<ApiService>()
    init(provider: MoyaProvider<ApiService> = MoyaProvider<ApiService>(plugins: [
        AuthPlugin(tokenClosure: { return Persistence.shared.getLoginData()?.accessToken }),
        NetworkLoggerPlugin(
            configuration: .init(formatter: .init(responseData: JSONResponseDataFormatter), logOptions: .verbose))
    ])) {
        self.provider = provider
    }

//    init(provider: MoyaProvider<ApiService> = MoyaProvider<ApiService>(stubClosure: MoyaProvider.immediatelyStub,plugins: [
//        AuthPlugin(tokenClosure: { return Persistence.shared.getLoginData()?.accessToken })
//    ])) {
//        self.provider = provider
//    }
    // MARK: Functions
    func getTokenRefreshService() -> Single<Response> {
        let data = Renew(refresh_token: Persistence.shared.getLoginData()?.refreshToken ?? "")
        return self.provider.rx.request(.renew(data: data))
    }
    // Parse and save your token locally or do any thing with the new token here
    func tokenDidRefresh(response: LoginResponse) {
        Persistence.shared.saveLoginData(data: response)
    }
    // Log the user out or do anything related here
    public func didFailedToRefreshToken() { }
    // Realiza el login
    func login(data: Login, needsError: Bool = false, statusCode: Int = 500,
               testData: Data = Data()) -> Observable<LoginResponse> {
        let req: ApiService = ApiService.login(data: data)
        let res: Observable<LoginResponse> = makeRequest(
            request: req, needsErrorResponse: needsError, statusCode: statusCode, testData: testData)
        return res
    }
    // Obtiene la información del usuario logeado
    func getInfoUser(username: String, isTest: Bool = false, statusCode: Int = 500,
                     testData: Data = Data()) -> Observable<UserInfoResponse> {
        let req: ApiService = ApiService.getInfoUser(username: username)
        let res: Observable<UserInfoResponse> = makeRequest(
            request: req, needsErrorResponse: isTest, statusCode: statusCode, testData: testData)
        return res
    }
    // Obtiene las órdenes de fabricación en una lista de por status
    func getStatusList(userId: String, needsError: Bool = false, statusCode: Int = 500,
                       testData: Data = Data()) -> Observable<StatusResponse> {
        let req: ApiService = ApiService.getStatusList(userId: userId)
        let res: Observable<StatusResponse> = makeRequest(
            request: req, needsErrorResponse: needsError,
            statusCode: statusCode, testData: testData)
        return res
    }
    func renew(data: Renew) -> Observable<LoginResponse> {
        let req: ApiService = ApiService.renew(data: data)
        let res: Observable<LoginResponse> = makeRequest(request: req)
        return res
    }
    // Obtiene el detalle de la fórmula
    func getOrdenDetail(orderId: Int, needsError: Bool = false, statusCode: Int = 500,
                        testData: Data = Data()) -> Observable<OrderDetailResponse> {
        let req: ApiService = ApiService.getOrdenDetail(orderId: orderId)
        let res: Observable<OrderDetailResponse> = makeRequest(
            request: req, needsErrorResponse: needsError,
            statusCode: statusCode, testData: testData)
        return res
    }
    // Chambia de status una orden de fabricación
    func changeStatusOrder(changeStatusRequest: [ChangeStatusRequest], needsError: Bool = false, statusCode: Int = 500,
                           testData: Data = Data()) -> Observable<ChangeStatusRespose> {
        let req: ApiService = ApiService.changeStatusOrder(changeStatusRequest: changeStatusRequest)
        let res: Observable<ChangeStatusRespose> = makeRequest(
            request: req, needsErrorResponse: needsError,
            statusCode: statusCode, testData: testData)
        return res
    }
    // Actualiza, elimina un elemento de la tabla en detalle de la formula
    func updateDeleteItemOfTableInOrderDetail(
        orderDetailRequest: OrderDetailRequest, needsError: Bool = false, statusCode: Int = 500,
        testData: Data = Data()) -> Observable<DeleteOrUpdateItemOfTableResponse> {
        let req: ApiService = ApiService.deleteItemOfOrdenDetail(orderDetailRequest: orderDetailRequest)
        let res: Observable<DeleteOrUpdateItemOfTableResponse> = makeRequest(
            request: req, needsErrorResponse: needsError,
            statusCode: statusCode, testData: testData)
        return res
    }
    // Obtiene los lotes para un orderId
    func getLots(orderId: Int, needsError: Bool = false, statusCode: Int = 500,
                 testData: Data = Data()) -> Observable<LotsResponse> {
        let req: ApiService = ApiService.getLots(orderId: orderId)
        let res: Observable<LotsResponse> = makeRequest(
            request: req, needsErrorResponse: needsError,
            statusCode: statusCode, testData: testData)
        return res
    }
    // Finaliza la order de fabricación
    func finishOrder(order: FinishOrder, needsError: Bool = false, statusCode: Int = 500,
                     testData: Data = Data()) -> Observable<FinishOrderResponse> {
        let req: ApiService = ApiService.finishOrder(finishOrder: order)
        let res: Observable<FinishOrderResponse> = makeRequest(
            request: req, needsErrorResponse: needsError,
            statusCode: statusCode, testData: testData)
        return res
    }
    // Asigna lotes a una orden de fabricación
    func assignLots(lotsRequest: [BatchSelected], needsError: Bool = false, statusCode: Int = 500,
                    testData: Data = Data()) -> Observable<AssingbBatchResponse> {
        let req: ApiService = ApiService.assingLots(lotsRequest: lotsRequest)
        let res: Observable<AssingbBatchResponse> = makeRequest(
            request: req, needsErrorResponse: needsError,
            statusCode: statusCode, testData: testData)
        return res
    }
    // Se pregunta si una orden  se puede finalizar o no
    func askIfOrderCanBeFinalized(orderId: Int, needsError: Bool = false, statusCode: Int = 500,
                                  testData: Data = Data()) -> Observable<OrderDetailResponse> {
        let req: ApiService = ApiService.askIfOrderCanBeFinalized(orderId: orderId)
        let res: Observable<OrderDetailResponse> = makeRequest(
            request: req, needsErrorResponse: needsError,
            statusCode: statusCode, testData: testData)
        return res
    }
    // Obtiene listado de componentes
    func getComponents(data: ComponentRequest, needsError: Bool = false, statusCode: Int = 500,
                       testData: Data = Data()) -> Observable<ComponentResponse> {
        let req: ApiService = ApiService.getComponents(data: data)
        let res: Observable<ComponentResponse> = makeRequest(
            request: req, needsErrorResponse: needsError,
                                                             statusCode: statusCode, testData: testData)
        return res
    }
    // Obtiene el listado de componentes más comunes
    func getMostCommonComponents(needsError: Bool = false, statusCode: Int = 500,
                                 testData: Data = Data()) -> Observable<ComponentResponse> {
        let req: ApiService = ApiService.getMostCommonComponents
        let res: Observable<ComponentResponse> = makeRequest(
            request: req, needsErrorResponse: needsError,
            statusCode: statusCode, testData: testData)
        return res
    }
    // Obtiene la carga de trabajo
    func getWordLoad(data: WorkloadRequest, needsError: Bool = false, statusCode: Int = 500,
                     testData: Data = Data()) -> Observable<WorkloadResponse> {
        let req: ApiService = ApiService.getWorkload(data: data)
        let res: Observable<WorkloadResponse> = makeRequest(
            request: req, needsErrorResponse: needsError,
            statusCode: statusCode, testData: testData)
        return res
    }

    // Obtiene la carga de trabajo
    func validateOrders(orderIDs: [Int], needsError: Bool = false, statusCode: Int = 500,
                        testData: Data = Data()) -> Observable<ValidateOrderModel> {
        let req: ApiService = ApiService.validateOrders(orderId: orderIDs)
        let res: Observable<ValidateOrderModel> = makeRequest(
            request: req, needsErrorResponse: needsError,
            statusCode: statusCode, testData: testData)
        return res
    }

    // Obtiene el pdf de el pedido
    func postOrdersPDF(orders: [Int], needsError: Bool = false, statusCode: Int = 500,
                       testData: Data = Data()) -> Observable<OrderPDF> {
        let req: ApiService = ApiService.postOrdersPDF(orders: orders)
        let res: Observable<OrderPDF> = makeRequest(
            request: req, needsErrorResponse: needsError,
            statusCode: statusCode, testData: testData)
        return res
    }

    // Comprueba la coneccion con la VPN
    func getConnect(needsError: Bool = false, statusCode: Int = 500,
                    testData: Data = Data()) -> Observable<ConnectModel> {
        let req: ApiService = ApiService.getConnect
        let res: Observable<ConnectModel> = makeRequest(
            request: req, needsVPN: true, needsErrorResponse: needsError,
            statusCode: statusCode, testData: testData)
        return res
    }

    // Obtiene los envases requeridos para los pedidos asignados
    func getContainer(userId: String, needsError: Bool = false, statusCode: Int = 500,
                      testData: Data = Data()) -> Observable<ContainerResponse> {
        let req: ApiService = ApiService.getContainer(userId: userId)
        let res: Observable<ContainerResponse> = makeRequest(
            request: req, needsErrorResponse: needsError,
            statusCode: statusCode, testData: testData)
        return res
    }

    private func makeRequest<T: BaseMappable>(
        request: ApiService, needsVPN: Bool = false, needsErrorResponse: Bool = false,
        statusCode: Int = 500, testData: Data = Data()) -> Observable<T> {
        providerChoseed = provider
        if needsVPN { providerChoseed = MoyaProvider<ApiService>(requestClosure: requestTimeoutClosure) }
        if needsErrorResponse { providerChoseed = MoyaProvider<ApiService>(endpointClosure: serverErrorEndpoint(
                statusCode: statusCode, data: testData),
            stubClosure: MoyaProvider.immediatelyStub) }
        return Observable<T>.create({ [weak self] observer in
            let res = !request.needsAuth ?
                self?.providerChoseed.rx.request(request).filterSuccessfulStatusAndRedirectCodes() :
                self?.providerChoseed.rx
                    .request(request)
                    .filterSuccessfulStatusAndRedirectCodes()
                    .refreshAuthenticationTokenIfNeeded(sessionServiceDelegate: self!)
            _ = res?.asObservable().subscribe(onNext: { response in
                let json = try? response.mapJSON()
                let res = Mapper<T>().map(JSONObject: json)
                if res != nil {
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
                    case 401:
                        let err = Mapper<HttpError>().map(JSONObject: json)
                        observer.onError(RequestError.unauthorized(error: err))
                    case 404:
                        observer.onError(RequestError.notFound)
                    case 500...:
                        let err = Mapper<HttpError>().map(JSONObject: json)
                        observer.onError(RequestError.serverError(error: err))
                    default:
                        observer.onError(RequestError.unknownError)
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
    func refreshAuthenticationTokenIfNeeded(sessionServiceDelegate: SessionProtocol) -> Single<Response> {
        return
            // Retry and process the request if any error occurred
            self.retryWhen { responseFromFirstRequest in
                responseFromFirstRequest.asObservable()
                    .flatMap { originalRequestResponseError -> PrimitiveSequence<SingleTrait, Element> in
                        if let moyaError: MoyaError = originalRequestResponseError as? MoyaError,
                            let res = moyaError.response {
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
                                    if let json = try? tokenRefreshResponse.mapJSON(),
                                        let tokenRes = Mapper<LoginResponse>().map(JSONObject: json) {
                                        sessionServiceDelegate.tokenDidRefresh(response: tokenRes)
                                    }
                                    // Retry the original request one more time
                                    return self.retry(1)
                                }
                            } else {
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
