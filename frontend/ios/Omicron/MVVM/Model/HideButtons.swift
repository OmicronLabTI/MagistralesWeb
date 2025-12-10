//
//  HideButtons.swift
//  Omicron
//
//  Created by Sergio Flores Ramirez on 25/03/22.
//  Copyright © 2022 Diego Cárcamo. All rights reserved.
//

import Foundation

class HideButtons {
    var process: Bool
    var finished: Bool
    var pending: Bool
    var addComp: Bool
    var save: Bool
    var seeBatches: Bool
    var saveChanges: Bool
    var splitOrder: Bool
    init(process: Bool, finished: Bool, pending: Bool,
         addComp: Bool, save: Bool, seeBatches: Bool, saveChanges: Bool, splitOrder: Bool) {
        self.process = process
        self.finished = finished
        self.pending = pending
        self.addComp = addComp
        self.save = save
        self.seeBatches = seeBatches
        self.saveChanges = saveChanges
        self.splitOrder = splitOrder
    }
}
