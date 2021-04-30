//
//  ChartViewModel.swift
//  Omicron
//
//  Created by Vicente Cantú on 21/09/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import Foundation
import RxSwift
import RxCocoa
import Resolver
import Charts

enum TypeOfCalendarChart: String {
    case day = "Día"
    case week = "Semana"
    case month = "Mes"
}

class ChartViewModel {

    var workloadData = ReplaySubject<[Workload?]>.create(bufferSize: 1)
    var start = PublishSubject<Bool>()
    var disposeBag = DisposeBag()
    var capacity: [String] = ["", "", ""]
    var daysRange: [String] = []
    var firstTime = true

    @Injected var networkManager: NetworkManager

    init() { }

    func getWorkloads() {

        guard let userData = Persistence.shared.getUserData(), let userId = userData.id else { return }
        var workloads: [Workload?] = []
        let initToday = UtilsManager.shared.formattedDateToString(date: Date().todayInZero)
            + "-"
            + UtilsManager.shared.formattedDateToString(date: Date().todayInZero)
        let today = Date.getDayOfWeek(today: "\(Date.today())")
        let week = Date.getRangeOfDateByWeek(dayOfWeek: today ?? 0)
        let finiMonth =
            UtilsManager.shared.formattedDateToString(date: Date().startOfMonth)
                + "-"
                + UtilsManager.shared.formattedDateToString(date: Date().endOfMonth)
        let daysRange = [UtilsManager.shared.formattedDateToString(date: Date().todayInZero), week ?? "", finiMonth]
        self.daysRange = daysRange.map({ $0.replacingOccurrences(of: "-", with: " al ") })

        let numberFormatter = NumberFormatter()
        numberFormatter.numberStyle = .decimal

        self.networkManager.getWordLoad(data: WorkloadRequest(fini: initToday, qfb: userId))
            .subscribe(onNext: { [weak self] workloadResponse in
                guard let self = self else { return }
                workloads.append(workloadResponse.response?.first)
                self.capacity[0] = numberFormatter
                    .string(
                        from: NSNumber(value: workloadResponse.response?.first?.totalPossibleAssign ?? 0)) ?? ""
                self.networkManager.getWordLoad(data: WorkloadRequest(fini: week ?? "", qfb: userId))
                    .subscribe(onNext: { [weak self] workloadResponse in
                        guard let self = self else { return }
                        workloads.append(workloadResponse.response?.first)
                        self.capacity[1] = numberFormatter
                            .string(
                                from: NSNumber(value: workloadResponse.response?.first?.totalPossibleAssign ?? 0)) ?? ""
                        self.networkManager.getWordLoad(data: WorkloadRequest(fini: finiMonth, qfb: userId))
                            .subscribe(onNext: { [weak self] workloadResponse in
                                guard let self = self else { return }
                                workloads.append(workloadResponse.response?.first)
                                self.capacity[2] = numberFormatter
                                    .string(
                                        from: NSNumber(value: workloadResponse.response?.first?.totalPossibleAssign ?? 0)) ?? ""
                                self.workloadData.onNext(workloads)
                                if self.firstTime {
                                    self.firstTime = false
                                    self.start.onNext(true)
                                } else {
                                    self.start.onNext(false)
                                }
                                }, onError: { error in
                                    print(error)
                                }).disposed(by: self.disposeBag)

                        }, onError: { error in
                            print(error)
                        }).disposed(by: self.disposeBag)

                }, onError: { error in
                    print(error)
            }).disposed(by: disposeBag)

    }

}
