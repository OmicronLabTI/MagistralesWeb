//
//  DateRangeSelectorViewController.swift
//  Omicron
//
//  Created by Daniel Vargas on 14/03/23.
//  Copyright © 2023 Diego Cárcamo. All rights reserved.
//
import HorizonCalendar
import UIKit
import Resolver
class DateRangeSelectorViewController: UIViewController {
    @IBOutlet weak var calendarContainer: UIView!
    @IBOutlet weak var acceptButton: UIButton!
    @Injected var historyViewModel: HistoryViewModel

    var startDate: Date?
    var endDate: Date?
    lazy var calendarView: CalendarView = CalendarView(initialContent: makeContent())
    lazy var calendar = Calendar.current
    lazy var dayDateFormatter: DateFormatter = {
      let dateFormatter = DateFormatter()
      dateFormatter.calendar = calendar
      dateFormatter.locale = calendar.locale
      dateFormatter.dateFormat = DateFormatter.dateFormat(
        fromTemplate: "EEEE, MMM d, yyyy",
        options: 0,
        locale: calendar.locale ?? Locale.current)
      return dateFormatter
    }()

    override func viewDidLoad() {
        super.viewDidLoad()
        calendarContainer.addSubview(calendarView)
        calendarView.translatesAutoresizingMaskIntoConstraints = false

        NSLayoutConstraint.activate([
          calendarView.leadingAnchor.constraint(equalTo: calendarContainer.leadingAnchor),
          calendarView.trailingAnchor.constraint(equalTo: calendarContainer.trailingAnchor),
          calendarView.topAnchor.constraint(equalTo: calendarContainer.topAnchor),
          calendarView.bottomAnchor.constraint(equalTo: calendarContainer.bottomAnchor)
        ])
        calendarView.scroll(toDayContaining: self.startDate ?? Date(), scrollPosition: .centered, animated: false)
        calendarView.daySelectionHandler = { [weak self] day in
          guard let self else { return }
            if self.validateIsDisabledDate(day.description) {
                return
            }
            if self.endDate != nil {
                self.startDate = nil
                self.endDate = nil
                self.calendarView.setContent(self.makeContent())
                self.validateAcceptButton()
                return
            }
            if self.startDate != nil {
                let dates = self.reorderDates(startDate: self.startDate ?? Date(),
                                              endDate: self.createDate(stringDate: day.description)
                                              ?? Date())
                self.startDate = dates.startDate
                self.endDate = dates.endDate
            } else {
                self.startDate = self.createDate(stringDate: day.description)
            }
            self.calendarView.setContent(self.makeContent())
            self.validateAcceptButton()
        }
    }

    @IBAction func accepButtonDidPresset(_ sender: Any) {
        historyViewModel.selectedRangeDateObs.onNext((startDate: self.startDate ?? Date(),
                                                      endDate: self.endDate ?? Date()))
        self.dismiss(animated: true)
    }
    @IBAction func cancelButtonDidPresset(_ sender: Any) {
        self.dismiss(animated: true)
    }
    func reorderDates(startDate: Date, endDate: Date) -> (startDate: Date, endDate: Date) {
        return startDate > endDate ?
        (startDate: endDate, endDate: startDate) :
        (startDate: startDate, endDate: endDate)
    }
    func validateAcceptButton() {
        self.acceptButton.isEnabled = self.startDate != nil && self.endDate != nil
    }
    func createDate(stringDate: String) -> Date? {
        let dateFormatter = DateFormatter()
        dateFormatter.dateFormat = "yyyy/MM/dd"
        return dateFormatter.date(from: stringDate)
    }
}
