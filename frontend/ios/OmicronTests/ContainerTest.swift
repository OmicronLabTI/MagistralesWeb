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

@testable import OmicronLab

class ContainerTest: XCTestCase {

    var containerViewModel: ContainerViewModel?
    var disposeBag: DisposeBag?
    @Injected var networkManager: NetworkManager

    override func setUpWithError() throws {
        containerViewModel = ContainerViewModel()
        disposeBag = DisposeBag()
    }

    override func tearDownWithError() throws {
        containerViewModel = nil
        disposeBag = nil
    }

    func testGetContainerData() {
        containerViewModel!.getContainerData()
        guard let userData = Persistence.shared.getUserData(), let userId = userData.id else { return }
        networkManager
            .getContainer(userId: userId)
            .subscribe(onNext: { containerResponse in
                XCTAssertNotNil(containerResponse)
            })
            .disposed(by: disposeBag!)
    }

    func testGetContainerDataWhenCodeIs500() {
        containerViewModel?.getContainerData(needsErrorRes: true, statusCode: 500, testdata: Data())
        containerViewModel?.loading.subscribe(onNext: { res in
            XCTAssertFalse(res)
        }).disposed(by: disposeBag!)
    }

}
