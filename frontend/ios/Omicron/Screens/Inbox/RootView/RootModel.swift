//
//  RootModel.swift
//  Omicron
//
//  Created by Axity on 30/07/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//
import Foundation
import ObjectMapper

// MARK: UserModel
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


// MARK: StatusModel
class StatusResponse: HttpResponse {
    var response: Status?
    
    required init?(map: Map) {
        super.init(map: map)
    }
    
    override func mapping(map: Map) {
        super.mapping(map: map)
        response <- map["response"]
    }
}

class Status {
    var status: [StatusDetail]?
    required init?(map: Map) {}
}

extension Status: Mappable {
    func mapping(map: Map) {
        self.status <- map["status"]
    }
}



class StatusDetail {
    var statusId: Int?
    var statusName: String?
    var orders: [Order]?
    
    required init?(map: Map) {}
}

extension StatusDetail: Mappable {
    func mapping(map: Map) {
        self.statusId <- map["statusId"]
        self.statusName <- map["statusName"]
        self.orders <- map["orders"]
    }
}

class Order {
    var no: Int?
    var baseDocument: String?
    var container: String?
    var tag: String?
    var plannedQuantity: String?
    var startDate: String?
    var finishDate: String?
    var descriptionProduct: String?
    required init?(map: Map) {}
}

extension Order: Mappable {
    func mapping(map: Map) {
        no <- map["no"]
        baseDocument <- map["baseDocument"]
        container <- map["container"]
        tag <- map["tag"]
        plannedQuantity <- map["plannedQuantity"]
        startDate <- map["startDate"]
        finishDate <- map["finishDate"]
        descriptionProduct <- map["descriptionProduct"]
    }
}

class StatusRequest: Codable {
    var qfbId: Int
    
    init(qfbId: Int) {
        self.qfbId = qfbId
    }
}

struct Section {
    var statusName: String
    var numberTask: Int
    var imageIndicatorStatus: String
    var orders: [Order]
    
    init(statusName: String, numberTask: Int, imageIndicatorStatus: String, orders: [Order]) {
        self.statusName = statusName
        self.numberTask = numberTask
        self.imageIndicatorStatus = imageIndicatorStatus
        self.orders = orders
    }
}
