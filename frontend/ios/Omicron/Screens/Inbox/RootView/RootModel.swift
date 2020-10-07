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
class User: Codable {
    // swiftlint:disable identifier_name
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
    var productionOrderId: Int?
    var baseDocument: Int?
    var container: String?
    var tag: String?
    var plannedQuantity: Decimal?
    var startDate: String?
    var finishDate: String?
    var descriptionProduct: String?
    var statusId: Int?
    var itemCode: String?
    var productCode: String?
    var destiny: String?
    var hasMissingStock = false
    init(productionOrderId: Int?, baseDocument: Int?, container: String?, tag: String?, plannedQuantity: Decimal?,
         startDate: String?, finishDate: String?, descriptionProduct: String?, statusId: Int?, itemCode: String?,
         productCode: String?, destiny: String?, hasMissingStock: Bool) {
        self.productionOrderId = productionOrderId
        self.baseDocument = baseDocument
        self.container = container
        self.tag = tag
        self.plannedQuantity = plannedQuantity
        self.startDate = startDate
        self.finishDate = finishDate
        self.descriptionProduct = descriptionProduct
        self.statusId = statusId
        self.itemCode = itemCode
        self.productCode = productCode
        self.destiny = destiny
        self.hasMissingStock = hasMissingStock
    }
    required init?(map: Map) {}
}
extension Order: Mappable {
    func mapping(map: Map) {
        productionOrderId <- map["productionOrderId"]
        baseDocument <- map["baseDocument"]
        container <- map["container"]
        tag <- map["tag"]
        plannedQuantity <- (map["plannedQuantity"], DecimalTransform())
        startDate <- map["startDate"]
        finishDate <- map["finishDate"]
        descriptionProduct <- map["descriptionProduct"]
        itemCode <- map["itemCode"]
        destiny <- map["destiny"]
        hasMissingStock <- map["hasMissingStock"]
    }
}
struct SectionOrder {
    var statusId: Int
    var statusName: String
    var numberTask: Int
    var imageIndicatorStatus: String
    var orders: [Order]
    init(statusId: Int, statusName: String, numberTask: Int, imageIndicatorStatus: String, orders: [Order]) {
        self.statusName = statusName
        self.numberTask = numberTask
        self.imageIndicatorStatus = imageIndicatorStatus
        self.orders = orders.map({
            $0.statusId = statusId
            return $0
        })
        self.statusId = statusId
    }
}
