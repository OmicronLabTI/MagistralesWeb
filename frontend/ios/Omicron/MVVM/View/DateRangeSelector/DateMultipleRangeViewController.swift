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
        let dateRangeToHighlight = validatePaintRange() ?
            selectedStartDate...selectedEndDate :
            Date()...Date()
        return CalendarViewContent(
        calendar: calendar,
        visibleDateRange: startDate...endDate,
        monthsLayout: .vertical(options: VerticalMonthsLayoutOptions()))
        .dayRangeItemProvider(for: [dateRangeToHighlight]) { dayRangeLayoutContext in
          DayRangeIndicatorView.calendarItemModel(
            invariantViewProperties: .init(),
            viewModel: .init(framesOfDaysToHighlight: dayRangeLayoutContext.daysAndFrames.map { $0.frame }))
        }
        .dayItemProvider { day in
            return CalendarItemModel<DayLabel>(
              invariantViewProperties: .init(
                font: UIFont.systemFont(ofSize: 18),
                textColor: self.validateIsDisabledDate(day.description) ? OmicronColors.ligthGray :
                    self.validateIsSelectedItem(day.description) ? .white : .darkGray,
                backgroundColor: self.validateIsSelectedItem(day.description) ?
                        OmicronColors.primaryBlue:
                        .clear),
              viewModel: .init(day: day))
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
        let selectedDate = self.createDate(stringDate: stringDate) ?? Date()
        let selectedStartDate = Calendar.current.date(byAdding: .day, value: -7, to: self.startDate ?? Date()) ?? Date()
        let selectedEndDate = Calendar.current.date(byAdding: .day, value: 7, to: self.startDate ?? Date()) ?? Date()
        return self.startDate != nil && selectedDate < selectedStartDate ||
        self.startDate != nil && selectedDate > selectedEndDate
    }
}
