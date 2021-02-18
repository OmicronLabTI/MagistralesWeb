//
//  Extensions.swift
//  Omicron
//
//  Created by Vicente Cantú on 22/09/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import Foundation

// MARK: - Encodable

extension Encodable {
  var dictionary: [String: Any]? {
    guard let data = try? JSONEncoder().encode(self) else { return nil }
    return (try? JSONSerialization.jsonObject(with: data, options: .allowFragments)).flatMap { $0 as? [String: Any] }
  }
}

// MARK: - Date

extension Date {

    var startOfMonth: Date {

        let calendar = Calendar(identifier: .gregorian)
        let components = calendar.dateComponents([.year, .month], from: self)

        return  calendar.date(from: components)!
    }

    var endOfMonth: Date {
        var components = DateComponents()
        components.day = -1
        components.month = 1
        return Calendar(identifier: .gregorian).date(byAdding: components, to: startOfMonth)!
    }

}

// MARK: - Array
extension Array {
    subscript (safe index: Int) -> Element? {
        return indices ~= index ? self[index] : nil
    }
}
