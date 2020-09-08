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
    var whoRequestSignature: String
    init(signatureType: String, signature: UIImage, whoRequestSignature: String) {
        self.signatureType = signatureType
        self.signature = signature
        self.whoRequestSignature = whoRequestSignature
    }
}
