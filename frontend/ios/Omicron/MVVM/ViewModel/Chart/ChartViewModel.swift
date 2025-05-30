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
import DGCharts

enum TypeOfCalendarChart: String {
    case day = "Día"
    case week = "Semana"
    case month = "Mes"
}

class ChartViewModel {

    var alert: PublishSubject<(title: String, msg: String)> = PublishSubject<(title: String, msg: String)>()
    var workloadData = ReplaySubject<[Workload?]>.create(bufferSize: 1)
    var start = PublishSubject<Bool>()
    var disposeBag = DisposeBag()
    var capacity: [String] = [String(), String(), String()]
    var daysRange: [String] = []
    var firstTime = true

    @Injected var networkManager: NetworkManager

    init() { }

    func getWorkloads() {

        guard let userData = Persistence.shared.getUserData(), let userId = userData.id else { return }
        let initialDay = UtilsManager.shared.formattedDateToString(date: Date().todayInZero)
        let byDay = initialDay
            + "-"
        + UtilsManager.shared.formattedDateToString(date: Date().nextDay(dateString: initialDay))

        let numberDay = Date.getDayOfWeek(today: "\(Date.today())")

        let byWeek = Date.getRangeOfDateByWeek(dayOfWeek: numberDay ?? 0) ?? String()

        let byMonth =
            UtilsManager.shared.formattedDateToString(date: Date().startOfMonth)
                + "-"
                + UtilsManager.shared.formattedDateToString(date: Date().endOfMonth)
        let daysRange = [UtilsManager.shared.formattedDateToString(date: Date().todayInZero),
                         byWeek, byMonth]
        self.daysRange = daysRange.map({ $0.replacingOccurrences(of: "-", with: " al ") })

        workloadService(initToday: byDay, userId: userId, week: byWeek, finiMonth: byMonth)
    }

    func workloadService(initToday: String, userId: String, week: String, finiMonth: String) {
        var workloads: [Workload?] = []
        let reqByDay = WorkloadRequest(fini: initToday, qfb: userId)
        networkManager.getWordLoad(reqByDay)
            .flatMap({ res -> Observable<WorkloadResponse> in
                // Get workload by day
                self.doProcessInWorloadRes(res, 0, &workloads)
                let reqByWeek = WorkloadRequest(fini: week, qfb: userId)

                return self.networkManager.getWordLoad(reqByWeek)
            }).flatMap({ res -> Observable<WorkloadResponse> in
                // Get workload by week
                self.doProcessInWorloadRes(res, 1, &workloads)
                let reqWorloadByMonth = WorkloadRequest(fini: finiMonth, qfb: userId)
                return self.networkManager.getWordLoad(reqWorloadByMonth)
            }).subscribe(onNext: { [weak self] res in
                guard let self = self else { return }
                // Get workload by month
                self.doProcessInWorloadRes(res, 2, &workloads)
                self.workloadData.onNext(workloads)

                self.setFirsTime(isFirstTime: self.firstTime)
                self.start.onNext(self.firstTime)
            }, onError: { [weak self] _ in
                guard let self = self else { return }
                self.alert.onNext((
                    title: Constants.Errors.errorTitle.rawValue,
                    msg: Constants.Errors.errorData.rawValue))
            }).disposed(by: disposeBag)
    }

    func doProcessInWorloadRes(_ res: WorkloadResponse, _ index: Int, _ workloads: inout [Workload?]) {
        let numberFormatter = NumberFormatter()
        numberFormatter.numberStyle = .decimal

        if let workload = res.response?.first {
            workloads.append(workload)
            let totalAssined = workload.totalPossibleAssign ?? 0
            let totalAssinedStr = NSNumber(value: totalAssined)
            capacity[index] = numberFormatter.string(from: totalAssinedStr) ?? String()
        }
    }

    func setFirsTime(isFirstTime: Bool) {
        if isFirstTime {
            self.firstTime = false
        }
    }
}
