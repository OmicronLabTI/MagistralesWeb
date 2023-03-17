//
//  DayRangeIndicatorViewExtension.swift
//  Omicron
//
//  Created by Daniel Vargas on 14/03/23.
//  Copyright © 2023 Diego Cárcamo. All rights reserved.
//

import HorizonCalendar

extension DayRangeIndicatorView: CalendarItemViewRepresentable {
  struct InvariantViewProperties: Hashable {
      let indicatorColor = OmicronColors.primaryBlue.withAlphaComponent(0.15)
  }

  struct ViewModel: Equatable {
    let framesOfDaysToHighlight: [CGRect]
  }

  static func makeView(
    withInvariantViewProperties invariantViewProperties: InvariantViewProperties)
    -> DayRangeIndicatorView {
    DayRangeIndicatorView(indicatorColor: invariantViewProperties.indicatorColor)
  }

  static func setViewModel(_ viewModel: ViewModel, on view: DayRangeIndicatorView) {
    view.framesOfDaysToHighlight = viewModel.framesOfDaysToHighlight
  }

}
