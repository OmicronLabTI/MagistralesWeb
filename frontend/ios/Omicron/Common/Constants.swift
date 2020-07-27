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
    static let inboxViewController = "InboxViewController"
}

struct OmicronColors {
    static let blue = UIColor.init(red: 84/255, green: 128/255, blue: 166/255, alpha: 1)
    static let ligthGray = UIColor.init(red: 246/255, green: 246/255, blue: 86/255, alpha: 1)
    static let assignedStatus = UIColor.init(red: 12/255, green: 204/255, blue: 246/255, alpha: 1)
    static let processStatus = UIColor.init(red: 255/255, green: 0/255, blue: 0/255, alpha: 1)
    static let pendingStatus = UIColor.init(red: 255/255, green: 184/255, blue: 0/255, alpha: 1)
    static let finishedStatus = UIColor.init(red: 28/255, green: 124/255, blue: 213/255, alpha: 1)
    static let reassignedStatus = UIColor.init(red: 186/255, green: 49/255, blue: 237/255, alpha: 1)
    static let tableStatus = UIColor.init(red: 233/255, green: 233/255, blue: 233/255, alpha: 1)
}
