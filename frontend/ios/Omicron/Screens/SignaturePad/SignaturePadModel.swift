//
//  SignaturePadModel.swift
//  Omicron
//
//  Created by Axity on 26/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import Foundation
import UIKit

class OrderSignature  {
    var signatureType: String
    var signature: UIImage
    
    init(signatureType: String, signature: UIImage) {
        self.signatureType = signatureType
        self.signature = signature
    }
}
