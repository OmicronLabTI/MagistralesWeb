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

    var todayInZero: Date {
        let date = Calendar.current.date(bySettingHour: 0, minute: 0, second: 0, of: Date())!
        return date
    }

}

// MARK: - Array
extension Array {
    subscript (safe index: Int) -> Element? {
        return indices ~= index ? self[index] : nil
    }
}

extension Date {
    static func getRangeOfDateByWeek(dayOfWeek: Int) -> String? {
            var startDate = String()
            var endDate = String()
            switch dayOfWeek {
            case 1: // Domingo
                startDate = UtilsManager.shared.formattedDateFromString(
                    dateString: "\(Date.today().previous(.monday))",
                    inputFormat: "yyyy-MM-dd HH:mm:ss Z",
                    outputFormat: "dd/MM/yyyy") ?? String()
                endDate = UtilsManager.shared.formattedDateFromString(
                    dateString: "\(Date.today())",
                    inputFormat: "yyyy-MM-dd HH:mm:ss Z",
                    outputFormat: "dd/MM/yyyy") ?? String()
                return "\(startDate)-\(endDate)"
            case 2...7: // Lunes a Sábado
                startDate = UtilsManager.shared.formattedDateFromString(
                    dateString: "\(Date.today().previous(.monday))",
                    inputFormat: "yyyy-MM-dd HH:mm:ss Z",
                    outputFormat: "dd/MM/yyyy") ?? String()
                endDate = UtilsManager.shared.formattedDateFromString(
                    dateString: "\(Date.today().next(.sunday))",
                    inputFormat: "yyyy-MM-dd HH:mm:ss Z",
                    outputFormat: "dd/MM/yyyy") ?? String()
                return "\(startDate)-\(endDate)"
            default:
                return nil
            }
        }

    static func getDayOfWeek(today: String) -> Int? {
        let dateFormatterGet = DateFormatter()
        dateFormatterGet.dateFormat = "yyyy-MM-dd HH:mm:ss Z"
        let dateGet = dateFormatterGet.date(from: today)!
        let formatter  = DateFormatter()
        formatter.dateFormat = "yyyy/MM/dd"
        let todayDate = formatter.date(from: formatter.string(from: dateGet))!
        let myCalendar = NSCalendar(calendarIdentifier: NSCalendar.Identifier.gregorian)!
        let myComponents = myCalendar.components(.weekday, from: todayDate)
        let weekDay = myComponents.weekday
        return weekDay
    }

  static func today() -> Date {
      return Date()
  }

  func next(_ weekday: Weekday, considerToday: Bool = false) -> Date {
    return get(.next,
               weekday,
               considerToday: considerToday)
  }

  func previous(_ weekday: Weekday, considerToday: Bool = false) -> Date {
    return get(.previous,
               weekday,
               considerToday: considerToday)
  }

  func get(_ direction: SearchDirection,
           _ weekDay: Weekday,
           considerToday consider: Bool = false) -> Date {

    let dayName = weekDay.rawValue

    let weekdaysName = getWeekDaysInEnglish().map { $0.lowercased() }

    assert(weekdaysName.contains(dayName), "weekday symbol should be in form \(weekdaysName)")

    let searchWeekdayIndex = weekdaysName.firstIndex(of: dayName)! + 1

    let calendar = Calendar(identifier: .gregorian)

    if consider && calendar.component(.weekday, from: self) == searchWeekdayIndex {
      return self
    }

    var nextDateComponent = calendar.dateComponents([.hour, .minute, .second], from: self)
    nextDateComponent.weekday = searchWeekdayIndex

    let date = calendar.nextDate(after: self,
                                 matching: nextDateComponent,
                                 matchingPolicy: .nextTime,
                                 direction: direction.calendarSearchDirection)

    return date!
  }

}

// MARK: Helper methods
extension Date {
  func getWeekDaysInEnglish() -> [String] {
    var calendar = Calendar(identifier: .gregorian)
    calendar.locale = Locale(identifier: "en_US_POSIX")
    return calendar.weekdaySymbols
  }

  enum Weekday: String {
    case monday, tuesday, wednesday, thursday, friday, saturday, sunday
  }

  enum SearchDirection {
    case next
    case previous

    var calendarSearchDirection: Calendar.SearchDirection {
      switch self {
      case .next:
        return .forward
      case .previous:
        return .backward
      }
    }
  }
}
