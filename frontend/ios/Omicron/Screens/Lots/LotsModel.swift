//
//  LotsModel.swift
//  Omicron
//
//  Created by Axity on 20/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import Foundation
import ObjectMapper

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
    var totalNecesario, totalSeleccionado: Double?
    var lotesSelecionados: [LotsSelected]?
    var lotesDisponibles: [LotsAvailable]?
    
    required init?(map: Map) {}
}

extension Lots: Mappable {
    func mapping(map: Map) {
        self.codigoProducto <- map["codigoProducto"]
        self.descripcionProducto <- map["descripcionProducto"]
        self.almacen <- map["almacen"]
        self.totalNecesario <- map["totalNecesario"]
        self.totalSeleccionado <- map["totalSeleccionado"]
        self.lotesDisponibles <- map["lotes"]
        self.lotesSelecionados <- map["lotesAsignados"]
    }
}

class LotsAux {
    var codigoProducto, descripcionProducto, almacen: String?
    var totalNecesario, totalSeleccionado: NSDecimalNumber?
    var lotesSelecionados: [LotsSelectedAux]?
    var lotesDisponibles: [LotsAvailableAux]?
    
    init (codigoProducto: String, descripcionProducto: String, almacen: String,  totalNecesario: NSDecimalNumber, totalSeleccionado: NSDecimalNumber, lotesSelecionados: [LotsSelectedAux], lotesDisponibles: [LotsAvailableAux]) {
        self.codigoProducto = codigoProducto
        self.descripcionProducto = descripcionProducto
        self.almacen = almacen
        self.totalNecesario = totalNecesario
        self.totalSeleccionado = totalSeleccionado
        self.lotesSelecionados = lotesSelecionados
        self.lotesDisponibles = lotesDisponibles
        
    }
}

class LotsAvailableAux {
    var numeroLote: String?
    var cantidadDisponible, cantidadAsignada, cantidadSeleccionada: NSDecimalNumber?
    var sysNumber: Int?
    
    init (numeroLote: String?, cantidadDisponible: NSDecimalNumber, cantidadAsignada:NSDecimalNumber, cantidadSeleccionada: NSDecimalNumber, sysNumber: Int) {
        self.numeroLote = numeroLote
        self.cantidadDisponible = cantidadDisponible
        self.cantidadAsignada = cantidadAsignada
        self.cantidadSeleccionada = cantidadSeleccionada
        self.sysNumber = sysNumber
    }
}

class LotsSelectedAux {
    var numeroLote: String?
    var cantidadSeleccionada: NSDecimalNumber?
    var sysNumber: Int?
    
    init(numeroLote: String, cantidadSeleccionada: NSDecimalNumber, sysNumber: Int) {
        self.numeroLote = numeroLote
        self.cantidadSeleccionada = cantidadSeleccionada
        self.sysNumber = sysNumber
    }
}

class LotsAvailable {
    var numeroLote: String?
    var cantidadDisponible, cantidadAsignada, cantidadSeleccionada: Double?
    var sysNumber: Int?
    
    init (numeroLote: String?, cantidadDisponible:Double, cantidadAsignada:Double, cantidadSeleccionada: Double, sysNumber: Int) {
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
        self.cantidadDisponible <- map["cantidadDisponible"]
        self.cantidadAsignada <- map["cantidadAsignada"]
        self.sysNumber <- map["sysNumber"]
    }
}

class LotsSelected: Codable {
    var numeroLote: String?
    var cantidadSeleccionada: Double?
    var sysNumber: Int?
    
    init(numeroLote: String, cantidadSeleccionada: Double, sysNumber: Int) {
        self.numeroLote = numeroLote
        self.cantidadSeleccionada = cantidadSeleccionada
        self.sysNumber = sysNumber
    }
    required init?(map: Map) { }
}

extension LotsSelected: Mappable {
    func mapping(map: Map) {
        self.numeroLote <- map["numeroLote"]
        self.cantidadSeleccionada <- map["cantidadSeleccionada"]
        self.sysNumber <- map["sysNumber"]
    }
}

class LotsAvailableInfo {
    var row: Int
    var quantitySelected: String
    
    init(row: Int,  quantitySelected: String) {
        self.row = row
        self.quantitySelected = quantitySelected
    }
}

class LotsRequest:Codable {
    var orderId: Int?
    var assignedQty: Double?
    var batchNumber: String?
    var itemCode: String?
    var action: String?
    
    init(orderId: Int, assignedQty: Double, batchNumber: String, itemCode: String, action: String) {
        self.orderId = orderId
        self.assignedQty = assignedQty
        self.batchNumber = batchNumber
        self.itemCode = itemCode
        self.action = action
    }
}
