//
//  RootModel.swift
//  Omicron
//
//  Created by Axity on 30/07/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//
import Foundation
import ObjectMapper

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

class UserInfoResponse {
    var id: String?
    var userName: String?
    var firstName: String?
    var lastName: String?
    var role: Int?
    var password: String?
    var activo: Int?
    required init?(map: Map) {}
}

extension UserInfoResponse: Mappable {
    func mapping(map: Map) {
        self.id <- map["id"]
        self.userName <- map["userName"]
        self.firstName <- map["firstName"]
        self.lastName <- map["lastName"]
        self.role <- map["role"]
        self.password <- map ["password"]
        self.activo <- map["activo"]
    }
}


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
