//
//  ContainerTest.swift
//  OmicronTests
//
//  Created by Vicente Cantu Garcia on 02/03/21.
//  Copyright © 2021 Diego Cárcamo. All rights reserved.
//

import XCTest
import RxSwift
import Resolver
import Moya

@testable import Magistrales

class ContainerTest: XCTestCase {

    var containerViewModel: ContainerViewModel?
    var disposeBag: DisposeBag?
    var provider: MoyaProvider<ApiService>!
    var statusCode = 200
    var testData = Data()
    @Injected var networkManager: NetworkManager

    override func setUpWithError() throws {
        statusCode = 200
        testData = Data()
        containerViewModel = ContainerViewModel()
        disposeBag = DisposeBag()
        provider = MoyaProvider<ApiService>(
            endpointClosure: customEndpointClosure,
            stubClosure: MoyaProvider.immediatelyStub)
    }

    override func tearDownWithError() throws {
        containerViewModel = nil
        disposeBag = nil
    }

    func customEndpointClosure(_ target: ApiService) -> Endpoint {
        return Endpoint(url: URL(target: target).absoluteString,
                        sampleResponseClosure: { .networkResponse(self.statusCode, self.testData) },
                        method: target.method,
                        task: target.task,
                        httpHeaderFields: target.headers)
    }

    func testGetContainerData() {
        containerViewModel!.getContainerData()
        guard let userData = Persistence.shared.getUserData(), let userId = userData.id else { return }
        networkManager
            .getContainer(userId)
            .subscribe(onNext: { containerResponse in
                XCTAssertNotNil(containerResponse)
            })
            .disposed(by: disposeBag!)
    }

    func testGetContainerDataWhenCodeIs500() {
        statusCode = 500
        containerViewModel?.networkManager = NetworkManager(provider: provider)
        containerViewModel?.getContainerData()
        containerViewModel?.loading.subscribe(onNext: { res in
            XCTAssertFalse(res)
        }).disposed(by: disposeBag!)
    }

}
