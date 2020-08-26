//
//  SignaturePadModel.swift
//  Omicron
//
//  Created by Axity on 26/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import Foundation

class OrderSignature  {
    var order: Int
    var signature: String
    
    init(order: Int, signature: String) {
        self.order = order
        self.signature = signature
    }
}
