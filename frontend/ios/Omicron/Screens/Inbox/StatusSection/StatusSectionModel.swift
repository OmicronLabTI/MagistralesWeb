//
//  StatusSectionModel.swift
//  Omicron
//
//  Created by Axity on 27/07/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import Foundation

struct Section: Codable {
    var statusName: String
    var numberTask: Int
    var imageIndicatorStatus: String
    
    init(statusName: String, numberTask: Int, imageIndicatorStatus: String) {
        self.statusName = statusName
        self.numberTask = numberTask
        self.imageIndicatorStatus = imageIndicatorStatus
    }
}
