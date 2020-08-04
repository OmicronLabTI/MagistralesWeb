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

class UserInfoResponse: HttpResponse {
    var response: User?

    required init?(map: Map) {
        super.init(map: map)
    }
    
    override func mapping(map: Map) {
        super.mapping(map: map)
        response <- map["response"]
    }
}

class User {
    var id: String?
    var userName: String?
    var firstName: String?
    var lastName: String?
    var role: Int?
    var password: String?
    var activo: Int?
    required init?(map: Map) {}
}

extension User: Mappable {
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


struct Order: Codable {
    var no: Int
    var baseDocument: String
    var container: String
    var tag: String
    var plannedQuantity: String
    var startDate: String
    var finishDate: String
    var descriptionProduct: String
        
    init(no: Int, baseDocument: String, container: String, tag: String, plannedQuantity: String, startDate: String, finishDate: String, descriptionProduct: String) {
        self.no = no
        self.baseDocument = baseDocument
        self.container = container
        self.tag = tag
        self.plannedQuantity = plannedQuantity
        self.startDate = startDate
        self.finishDate = finishDate
        self.descriptionProduct = descriptionProduct
    }
}
