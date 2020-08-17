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
            return "https://c2043d8656cd.ngrok.io/api"
        case .stagingRelease,
             .appstore,
             .release:
            return "https://c2043d8656cd.ngrok.io/api"
        }
    }()
}
