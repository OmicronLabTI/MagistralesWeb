//
//  CalentarMonthItemView.swift
//  Omicron
//
//  Created by Daniel Vargas on 17/03/23.
//  Copyright © 2023 Diego Cárcamo. All rights reserved.
//

import HorizonCalendar
struct MonthLabel: CalendarItemViewRepresentable {

    /// Properties that are set once when we initialize the view.
    struct InvariantViewProperties: Hashable {
        let font: UIFont
        let textColor: UIColor
        let backgroundColor: UIColor
    }

    /// Properties that will vary depending on the particular date being displayed.
    struct ViewModel: Equatable {
        let month: Month
    }

    static func makeView(
        withInvariantViewProperties invariantViewProperties: InvariantViewProperties)
        -> UILabel {
        let label = UILabel()

        label.backgroundColor = invariantViewProperties.backgroundColor
        label.font = invariantViewProperties.font
        label.textColor = invariantViewProperties.textColor
        label.textAlignment = .left
        return label
    }

    static func setViewModel(_ viewModel: ViewModel, on view: UILabel) {
        let dateFormatter = DateFormatter()
        dateFormatter.dateFormat = "MMMM"
        dateFormatter.locale = NSLocale(localeIdentifier: "es_MX") as Locale
        let monthString = self.getMonthString(viewModel.month.components.month ?? 1)
        let year = viewModel.month.year
        view.text = "\(monthString) de \(year)"
    }
    static func getMonthString(_ monthNumber: Int) -> String {
        let monsth = [1: "Enero",
                      2: "Febrero",
                      3: "Marzo",
                      4: "Abril",
                      5: "Mayo",
                      6: "Junio",
                      7: "Julio",
                      8: "Agosto",
                      9: "Septiembre",
                      10: "Octubre",
                      11: "Noviembre",
                      12: "Diciembre"]
        return monsth[monthNumber] ?? String()
    }
}
