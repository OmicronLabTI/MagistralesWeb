//
//  UtilsTest.swift
//  OmicronTests
//
//  Created by Axity on 19/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import  XCTest
import  RxSwift
import Moya

@testable import Omicron

class Utils: XCTestCase {
    override func setUpWithError() throws {
        // Put setup code here. This method is called before the invocation of each test method in the class.
    }

    override func tearDownWithError() throws {
        // Put teardown code here. This method is called after the invocation of each test method in the class.
    }
    
    func testTransformDataToShowSuccess() -> Void {
        // given
        let testDate = "14/08/2020"
        
        // when
        let result = UtilsManager.shared.formattedDateFromString(dateString: testDate, withFormat: "yyyy-MM-dd")
        
        //then
        XCTAssertEqual(result, "2020-08-14")
        XCTAssertNotNil(result)
    }
}
