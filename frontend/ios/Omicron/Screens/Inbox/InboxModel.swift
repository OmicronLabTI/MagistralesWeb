//
//  InboxModel.swift
//  Omicron
//
//  Created by Axity on 24/07/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import Foundation

struct Orden: Codable {
    var No: Int
    var BaseDocument: String
    var Container: String
    var Tag: String
    var PlannedQuantity: String
    var startDate: String
    var finishDate: String
    var descriptionProduct: String
        
    init(No: Int, BaseDocument: String, Container: String, Tag: String, PlannedQuantity: String, startDate: String, finishDate: String, descriptionProduct: String) {
        self.No = No
        self.BaseDocument = BaseDocument
        self.Container = Container
        self.Tag = Tag
        self.PlannedQuantity = PlannedQuantity
        self.startDate = startDate
        self.finishDate = finishDate
        self.descriptionProduct = descriptionProduct
    }
}
    

