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
    var totalNecesario, totalSeleccionado: Int?
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

class LotsAvailable {
    var numeroLote: String?
    var cantidadDisponible, cantidadAsignada: Double?
    var sysNumber: Int?
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

class LotsSelected {
    var numeroLote: String?
    var cantidadSeleccionada: Double?
    var sysNumber: Int?
    required init?(map: Map) { }
}

extension LotsSelected: Mappable {
    func mapping(map: Map) {
        self.numeroLote <- map["numeroLote"]
        self.cantidadSeleccionada <- map["cantidadSeleccionada"]
        self.sysNumber <- map["sysNumber"]
    }
}
