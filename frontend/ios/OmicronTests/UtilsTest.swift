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

class UtilsTest: XCTestCase {
    // MARK: VARIABLES
    var utils: UtilsManager?
    override func setUp() {
        print("XXXX setUp UtilsTest")
        utils = UtilsManager.shared
    }
    override func tearDown() {
        print("XXXX tearDown UtilsTest")
        utils = nil
    }
    func testTransformDataToShowSuccess() {
        // given
        let testDate = "14/08/2020"
        // when
        let result = utils!.formattedDateFromString(dateString: testDate, withFormat: "yyyy-MM-dd")
        //then
        XCTAssertEqual(result, "2020-08-14")
    }
    func testTransformDataNotNil() {
        let testDate = "14/08/2020"
        let result = utils!.formattedDateFromString(dateString: testDate, withFormat: "yyyy-MM-dd")
        XCTAssertNotNil(result)
    }
    func testFormattedDateToString() {
        let result = utils!.formattedDateToString(date: Date(timeIntervalSinceReferenceDate: -123456789.0))
        XCTAssertEqual(result, "01/02/1997")
    }
}
