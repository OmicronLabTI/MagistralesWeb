//
//  Config.swift
//  Omicron
//
//  Created by Diego Cárcamo on 09/07/2020.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import Foundation

enum Environment {
    case debug
    case release
    case stagingDebug
    case stagingRelease
    case appstore
}

struct Config {

    #if DEBUG
    static let baseUrl = URLRoot.DEBUG
    #elseif QA
    static let baseUrl = URLRoot.QA
    #elseif RELEASE
    static let baseUrl = URLRoot.RELEASE
    #elseif UAT
    static let baseUrl = URLRoot.UAT
    #endif

    static let serverOmicron = URLRoot.omicronServer

    static var isRunningTests: Bool {
        return ProcessInfo.processInfo.environment["XCTestConfigurationFilePath"] != nil
    }

}
