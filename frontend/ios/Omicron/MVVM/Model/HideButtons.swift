//
//  HideButtons.swift
//  Omicron
//
//  Created by Sergio Flores Ramirez on 25/03/22.
//  Copyright © 2022 Diego Cárcamo. All rights reserved.
//

import Foundation

class HideButtons {
    var hideProcessBtn: Bool
    var hideFinishedBtn: Bool
    var hidePendinBtn: Bool
    var hideAddCompBtn: Bool
    var hideSaveBtn: Bool
    var hideSeeLotsBtn: Bool

    init(_ hideProcessBtn: Bool, _ hideFinishedBtn: Bool, _ hidePendinBtn: Bool,
         _ hideAddCompBtn: Bool, _ hideSaveBtn: Bool, _ hideSeeLotsBtn: Bool) {
        self.hideProcessBtn = hideProcessBtn
        self.hideFinishedBtn = hideFinishedBtn
        self.hidePendinBtn = hidePendinBtn
        self.hideAddCompBtn = hideAddCompBtn
        self.hideSaveBtn = hideSaveBtn
        self.hideSeeLotsBtn = hideSeeLotsBtn
    }
}
