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
    
    init (numeroLote: String?, cantidadDisponible: Decimal, cantidadAsignada: Decimal, cantidadSeleccionada: Decimal, sysNumber: Int) {
        self.numeroLote = numeroLote
        self.cantidadDisponible = cantidadDisponible
        self.cantidadAsignada = cantidadAsignada
        self.cantidadSeleccionada = cantidadSeleccionada
        self.sysNumber = sysNumber
    }
    required init?(map: Map) {}
}

extension LotsAvailable: Mappable {
    func mapping(map: Map) {
        self.numeroLote <- map["numeroLote"]
        self.cantidadDisponible <- (map["cantidadDisponible"], DecimalTransform())
        self.cantidadAsignada <- (map["cantidadAsignada"], DecimalTransform())
        self.sysNumber <- map["sysNumber"]
    }
}

class LotsSelected: Codable {
    var numeroLote: String?
    var cantidadSeleccionada: Decimal?
    var sysNumber: Int?
    
    init(numeroLote: String, cantidadSeleccionada: Decimal, sysNumber: Int) {
        self.numeroLote = numeroLote
        self.cantidadSeleccionada = cantidadSeleccionada
        self.sysNumber = sysNumber
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

class LotsAvailableInfo {
    var documentSelected: Lots?
    var availableSelected: LotsAvailable?
    var quantitySelected: Decimal
    
    init(documentSelected: Lots?, availableSelected: LotsAvailable?,  quantitySelected: Decimal) {
        self.documentSelected = documentSelected
        self.availableSelected = availableSelected
        self.quantitySelected = quantitySelected
    }
}

class BatchSelected: Codable {
    var orderId: Int?
    var assignedQty: Decimal?
    var batchNumber: String?
    var itemCode: String?
    var action: String?
    var sysNumber: Int?
    
    init(orderId: Int?, assignedQty: Decimal?, batchNumber: String?, itemCode: String?, action: String?, sysNumber: Int?) {
        self.orderId = orderId
        self.assignedQty = assignedQty
        self.batchNumber = batchNumber
        self.itemCode = itemCode
        self.action = action
        self.sysNumber = sysNumber
    }
    
    func toLotsSelected() -> LotsSelected {
        return LotsSelected(numeroLote: self.batchNumber!, cantidadSeleccionada: self.assignedQty!, sysNumber: self.sysNumber!)
    }
}

class LotsRequest: Codable {
    var orderId: Int?
    var assignedQty: Decimal?
    var batchNumber: String?
    var itemCode: String?
    var action: String?
    
    init(orderId: Int?, assignedQty: Decimal?, batchNumber: String?, itemCode: String?, action: String?) {
        self.orderId = orderId
        self.assignedQty = assignedQty
        self.batchNumber = batchNumber
        self.itemCode = itemCode
        self.action = action
    }
}


class cacheLotsSelected {
    var indexOdLineDocumentsTableSelected: Int
    var lotsSelected: [LotsSelected]
    
    init (indexOdLineDocumentsTableSelected: Int, lotsSelected: [LotsSelected]) {
        self.indexOdLineDocumentsTableSelected = indexOdLineDocumentsTableSelected
        self.lotsSelected = lotsSelected
    }
}
