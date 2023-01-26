//
//  PatientListModel.swift
//  Omicron
//
//  Created by Daniel Velez on 25/01/23.
//  Copyright © 2023 Diego Cárcamo. All rights reserved.
//

import Foundation

class PatientListData {
    var title: String = ""
    var list: [String] = []
    init(title:String, list: [String]){
        self.title = title
        self.list = list
    }
}
