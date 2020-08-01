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
