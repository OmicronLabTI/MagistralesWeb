//
//  LotsModel.swift
//  Omicron
//
//  Created by Axity on 20/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import Foundation
import ObjectMapper
class AssingbBatchResponse: HttpResponse {
    var response: [String]?
    required init?(map: Map) {
        super.init(map: map)
    }
    override func mapping(map: Map) {
        response <- map["response"]
    }
}

class LotsByProductResponse: HttpResponse {
    var response: AddComponentLots?
    required init?(map: Map) {
        super.init(map: map)
    }
    override func mapping(map: Map) {
        super.mapping(map: map)
        response <- map["response"]
    }
}

class AddComponentLots: Mappable {
    var codigoProducto, almacen: String
    var lotes: [LotsAvailable]
    
    init(codigoProducto: String, almacen: String, lotes: [LotsAvailable]) {
        self.codigoProducto = codigoProducto
        self.almacen = almacen
        self.lotes = lotes
    }
    required init?(map: Map) {
        self.codigoProducto = String()
        self.almacen = String()
        self.lotes = []
    }
    func mapping(map: Map) {
        self.codigoProducto <- map["codigoProducto"]
        self.almacen <- map["almacen"]
        self.lotes <- map["lotes"]
    }
}

class LotsResponse: HttpResponse {
    var response: [Lots]?
    required init?(map: Map) {
        super.init(map: map)
    }
    override func mapping(map: Map) {
        super.mapping(map: map)
        response <- map["response"]
    }
}

class Lots {
    var codigoProducto, descripcionProducto, almacen: String?
    var totalNecesario, totalSeleccionado: Decimal?
    var lotesSelecionados: [LotsSelected]?
    var lotesDisponibles: [LotsAvailable]?
    init(codigoProducto: String, descripcionProducto: String, almacen: String,
         totalNecesario: Decimal, totalSeleccionado: Decimal,
         lotesSelecionados: [LotsSelected], lotesDisponibles: [LotsAvailable]) {
        self.codigoProducto = codigoProducto
        self.descripcionProducto = descripcionProducto
        self.almacen = almacen
        self.totalNecesario = totalNecesario
        self.totalSeleccionado = totalSeleccionado
        self.lotesSelecionados = lotesSelecionados
        self.lotesDisponibles = lotesDisponibles
    }
    required init?(map: Map) {}
}
extension Lots: Mappable {
    func mapping(map: Map) {
        self.codigoProducto <- map["codigoProducto"]
        self.descripcionProducto <- map["descripcionProducto"]
        self.almacen <- map["almacen"]
        self.totalNecesario <- (map["totalNecesario"], DecimalTransform())
        self.totalSeleccionado <- (map["totalSeleccionado"], DecimalTransform())
        self.lotesDisponibles <- map["lotes"]
        self.lotesSelecionados <- map["lotesAsignados"]
    }
}
class LotsAvailable {
    var numeroLote: String?
    var cantidadDisponible, cantidadAsignada, cantidadSeleccionada: Decimal?
    var sysNumber: Int?
    var fechaExp: String?
    var expiredBatch: Bool = false
    init (numeroLote: String?, cantidadDisponible: Decimal, cantidadAsignada: Decimal,
          cantidadSeleccionada: Decimal, sysNumber: Int, fechaExp: String?) {
        self.numeroLote = numeroLote
        self.cantidadDisponible = cantidadDisponible
        self.cantidadAsignada = cantidadAsignada
        self.cantidadSeleccionada = cantidadSeleccionada
        self.sysNumber = sysNumber
        self.fechaExp = fechaExp
    }
    required init?(map: Map) {}
}
extension LotsAvailable: Mappable {
    func mapping(map: Map) {
        self.numeroLote <- map["numeroLote"]
        self.cantidadDisponible <- (map["cantidadDisponible"], DecimalTransform())
        self.cantidadAsignada <- (map["cantidadAsignada"], DecimalTransform())
        self.sysNumber <- map["sysNumber"]
        self.fechaExp <- map["fechaExp"]
    }
}
class LotsSelected: Codable {
    var numeroLote: String?
    var cantidadSeleccionada: Decimal?
    var sysNumber: Int?
    var expiredBatch: Bool = false
    init(numeroLote: String, cantidadSeleccionada: Decimal, sysNumber: Int, expiredBatch: Bool) {
        self.numeroLote = numeroLote
        self.cantidadSeleccionada = cantidadSeleccionada
        self.sysNumber = sysNumber
        self.expiredBatch = expiredBatch
    }
    required init?(map: Map) { }
}
extension LotsSelected: Mappable {
    func mapping(map: Map) {
        self.numeroLote <- map["numeroLote"]
        self.cantidadSeleccionada <- (map["cantidadSeleccionada"], DecimalTransform())
        self.sysNumber <- map["sysNumber"]
    }
}

class BatchSelected: Codable {
    var orderId: Int?
    var assignedQty: Decimal?
    var batchNumber: String?
    var itemCode: String?
    var action: String?
    var sysNumber: Int?
    var expiredBatch: Bool = false
    var areBatchesComplete: Int
    init(orderId: Int?, assignedQty: Decimal?, batchNumber: String?,
         itemCode: String?, action: String?, sysNumber: Int?, expiredBatch: Bool, areBatchesComplete: Int) {
        self.orderId = orderId
        self.assignedQty = assignedQty
        self.batchNumber = batchNumber
        self.itemCode = itemCode
        self.action = action
        self.sysNumber = sysNumber
        self.expiredBatch = expiredBatch
        self.areBatchesComplete = areBatchesComplete
    }
    func toLotsSelected() -> LotsSelected {
        return LotsSelected(numeroLote: self.batchNumber!, cantidadSeleccionada: self.assignedQty!,
                            sysNumber: self.sysNumber!, expiredBatch: self.expiredBatch)
    }
}
