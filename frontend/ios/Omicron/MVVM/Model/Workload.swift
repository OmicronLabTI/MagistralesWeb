//
//  Workload.swift
//  Omicron
//
//  Created by Vicente Cantú on 21/09/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import Foundation
import ObjectMapper

class WorkloadResponse: HttpResponse {
    var response: [Workload]?
    required init?(map: Map) {
        super.init(map: map)
    }
    override func mapping(map: Map) {
        super.mapping(map: map)
        response <- map["response"]
    }
}
class Workload {
    var user: String?
    var totalPossibleAssign: Int?
    var assigned: Int?
    var processed: Int?
    var pending: Int?
    var finished: Int?
    var finalized: Int?
    var reassigned: Int?
    var totalOrders: Int?
    var totalFabOrders: Int?
    var totalPieces: Int?
    required init?(map: Map) {}
}
extension Workload: Mappable {
    func mapping(map: Map) {
        self.user <- map["user"]
        self.totalPossibleAssign <- map["totalPossibleAssign"]
        self.assigned <- map["assigned"]
        self.processed <- map["processed"]
        self.pending <- map["pending"]
        self.finished <- map["finished"]
        self.finalized <- map["finalized"]
        self.reassigned <- map["reassigned"]
        self.totalOrders <- map["totalOrders"]
        self.totalFabOrders <- map["totalFabOrders"]
        self.totalPieces <- map["totalPieces"]
    }
}
struct WorkloadRequest: Encodable {
    var fini: String
    var qfb: String
}
