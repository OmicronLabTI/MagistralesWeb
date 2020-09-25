//
//  PersistenceTest.swift
//  OmicronTests
//
//  Created by Vicente Cantú on 25/09/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import XCTest

@testable import Omicron

class PersistenceTest: XCTestCase {

    override func setUpWithError() throws {
        // Put setup code here. This method is called before the invocation of each test method in the class.
    }

    override func tearDownWithError() throws {
        // Put teardown code here. This method is called after the invocation of each test method in the class.
    }
    
    let persistence = Persistence.shared
    
    func testLoginTestNotNull() {
        let loginData = persistence.getLoginData()
        XCTAssertNotNil(loginData)
    }
    
    func testGetUserDataNotNull() {
        let userData = persistence.getUserData()
        XCTAssertNotNil(userData)
    }

}
