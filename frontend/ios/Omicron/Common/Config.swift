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

    #if DEVELOPMENT
    static let baseUrl = URLRoot.qaServer
    #else
    static let baseUrl = URLRoot.prodServer
    #endif

    static let serverOmicron = URLRoot.omicronServer

    static var isRunningTests: Bool {
        return ProcessInfo.processInfo.environment["XCTestConfigurationFilePath"] != nil
    }

}
