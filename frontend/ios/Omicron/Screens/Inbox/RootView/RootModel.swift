//
//  RootModel.swift
//  Omicron
//
//  Created by Axity on 30/07/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

struct Section: Codable {
    var index: Int
    var statusName: String
    var numberTask: Int
    var imageIndicatorStatus: String
    
    init( index: Int, statusName: String, numberTask: Int, imageIndicatorStatus: String) {
        self.index = index
        self.statusName = statusName
        self.numberTask = numberTask
        self.imageIndicatorStatus = imageIndicatorStatus
    }
}
