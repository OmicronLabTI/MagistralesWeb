//
//  InboxModel.swift
//  Omicron
//
//  Created by Axity on 24/07/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import Foundation

class MessageToChangeStatus {
    var message: String
    var typeOfStatus: String
    init(message: String, typeOfStatus: String) {
        self.message = message
        self.typeOfStatus = typeOfStatus
    }
}
