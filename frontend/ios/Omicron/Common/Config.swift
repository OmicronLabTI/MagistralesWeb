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
    static let env: Environment = {
        #if STAGING_DEBUG
            return .stagingDebug
        #elseif STAGING_RELEASE
            return .stagingRelease
        #elseif APPSTORE
            return .appstore
        #elseif DEBUG
            return .debug
        #elseif RELEASE
            return .release
        #endif
    }()
    static let baseUrl: String = {
        switch env {
        case .debug,
             .stagingDebug:
            return "http://201.163.179.29:8000/api"
//            return "https://magistralesprod.omicronlab.com.mx/api"
        case .stagingRelease,
             .appstore,
             .release:
            return "http://201.163.179.29:8000/api"
//            return "https://magistralesprod.omicronlab.com.mx/api"
        }
    }()
    static let serverOmicron: String = {
            switch env {
            case .debug,
                 .stagingDebug:
                return "http://172.30.5.49:5100/"
            case .stagingRelease,
                 .appstore,
                 .release:
                return "http://172.30.5.49:5100/"
            }
        }()
    static var isRunningTests: Bool {
        return ProcessInfo.processInfo.environment["XCTestConfigurationFilePath"] != nil
    }
}
