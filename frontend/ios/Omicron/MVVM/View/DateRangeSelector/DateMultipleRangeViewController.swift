//
//  DateMultipleRangeViewController.swift
//  Omicron
//
//  Created by Daniel Vargas on 14/03/23.
//  Copyright © 2023 Diego Cárcamo. All rights reserved.
//

import Foundation
import HorizonCalendar

extension DateRangeSelectorViewController {

    func makeContent() -> CalendarViewContent {
        let startDateString = "2020-01-01"
        let formatter = DateFormatter()
        formatter.dateFormat = "yyyy-MM-dd"
        let startDate = formatter.date(from: startDateString) ?? Date()
        let endDate = Date()

        let selectedStartDate = self.startDate ?? Date()
        let selectedEndDate = self.endDate ?? Date()

        calendar.locale = NSLocale(localeIdentifier: "es_MX") as Locale
        let dateRanges = validatePaintRange() ?
            selectedStartDate...selectedEndDate :
            Date()...Date()
        
        return CalendarViewContent(
            calendar: calendar,
            visibleDateRange: startDate...endDate,
            monthsLayout: .vertical(options: VerticalMonthsLayoutOptions()))
        .dayItemProvider { [calendar] day in
            var invariantViewProperties = DayView.InvariantViewProperties.baseInteractive

            // Determinar colores según si el día está deshabilitado, seleccionado o dentro del rango pero no seleccionado
            let textColor: UIColor
            let backgroundColor: UIColor
            
            // Si el día está deshabilitado
            if self.validateIsDisabledDate(day.description) {
                textColor = OmicronColors.ligthGray
                backgroundColor = .clear
            }
            // Si el día está seleccionado
            else if self.validateIsSelectedItem(day.description) {
                textColor = .white
                backgroundColor = OmicronColors.primaryBlue
            }
            // Si no está seleccionado ni deshabilitado
            else {
                textColor = .darkGray
                backgroundColor = .clear
            }

            invariantViewProperties.backgroundShapeDrawingConfig.fillColor = backgroundColor
            invariantViewProperties.textColor = textColor

            return DayView.calendarItemModel(
                invariantViewProperties: invariantViewProperties,
                content: .init(
                  dayText: "\(day.day)",
                  accessibilityLabel: "\(day.day)",
                  accessibilityHint: nil))
        }
        .dayRangeItemProvider(for: [dateRanges]) { dayRangeLayoutContext in
          DayRangeIndicatorView.calendarItemModel(
            invariantViewProperties: .init(),
            content: .init(framesOfDaysToHighlight: dayRangeLayoutContext.daysAndFrames.map { $0.frame }))
        }
        .interMonthSpacing(10)
        .verticalDayMargin(4)
        .horizontalDayMargin(4)
    }

    func validatePaintRange() -> Bool {
        return self.startDate != nil && self.endDate != nil
    }
    func validateIsSelectedItem(_ stringDate: String) -> Bool {
        let selectedDate = self.createDate(stringDate: stringDate)
        return self.startDate != nil && self.startDate == selectedDate ||
        self.endDate != nil && self.endDate == selectedDate
    }

    func validateIsDisabledDate(_ stringDate: String) -> Bool {
        guard let maxRangeDays = self.delegate?.maxRangeDays else { return false }
        let selectedDate = self.createDate(stringDate: stringDate) ?? Date()
        let selectedStartDate = Calendar.current.date(byAdding: .day,
                                                      value: -maxRangeDays,
                                                      to: self.startDate ?? Date()) ?? Date()
        let selectedEndDate = Calendar.current.date(byAdding: .day,
                                                    value: maxRangeDays,
                                                    to: self.startDate ?? Date()) ?? Date()
        return self.startDate != nil && selectedDate < selectedStartDate ||
        self.startDate != nil && selectedDate > selectedEndDate || selectedDate > Date()
    }
    
    func isDateWithinRangeButNotSelected(_ stringDate: String) -> Bool {
        guard let startDate = self.startDate else { return false }
        guard let endDate = self.endDate else { return false }
        guard let selectedDate = self.createDate(stringDate: stringDate) else { return false }

        // Verifica si la fecha está dentro del rango pero no es la seleccionada
        return selectedDate > startDate && selectedDate < endDate && !(selectedDate == startDate || selectedDate == endDate)
    }
}
