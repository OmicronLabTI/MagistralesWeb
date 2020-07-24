//
//  Constants.swift
//  Omicron
//
//  Created by Diego Cárcamo on 09/07/2020.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import Foundation
import UIKit
struct Constants {
    enum Errors: String {
        case serverError = "Lo sentimos, ocurrió un error en el servidor"
    }
    
    enum Tags: Int {
        case loading = 101
    }
}

struct ViewControllerIdentifiers {
    static let inboxViewController = ""
}

struct SegueIdentifiers {
    static let inboxVC = "InboxVC"
}

struct OmicronColors {
    static let blue = UIColor.init(red: 84/255, green: 128/255, blue: 166/255, alpha: 1)
    static let ligthGray = UIColor.init(red: 246/255, green: 246/255, blue: 246/255, alpha: 1)
}
