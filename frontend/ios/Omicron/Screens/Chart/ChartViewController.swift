//
//  ChartViewController.swift
//  Omicron
//
//  Created by Vicente Cantú on 21/09/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit
import Charts
import Resolver
import RxSwift

class ChartViewController: UIViewController {
    @IBOutlet weak var chartView: PieChartView!
    @IBOutlet weak var possibleAssingLabel: UILabel!
    @Injected var chartViewModel: ChartViewModel
    var disposeBag: DisposeBag = DisposeBag()
    override func viewDidLoad() {
        super.viewDidLoad()
        title = "Carga de trabajo"
        viewModelBingind()
        setupChart()
    }
    func viewModelBingind() {
        chartViewModel.getWorkload()
        chartViewModel.workloadData.bind(onNext: { [weak self] data in
            guard let self = self else { return }
            self.setDataToChart(data.first)
        }).disposed(by: disposeBag)
    }
    private func setDataToChart(_ workload: Workload?) {
        guard let workload = workload else { return }
        var entries: [PieChartDataEntry]?
        var colors: [UIColor]?
        for index in 0...5 {
            switch index {
            case 0:
                (entries, colors) =
                    setWorkloadProperties(workloadType: workload.assigned,
                                          statusName: StatusNameConstants.assignedStatus,
                                          colorName: OmicronColors.assignedStatus)
            case 1:
                (entries, colors) =
                    setWorkloadProperties(workloadType: workload.processed,
                                          statusName: StatusNameConstants.inProcessStatus,
                                          colorName: OmicronColors.processStatus)
            case 2:
                (entries, colors) =
                    setWorkloadProperties(workloadType: workload.pending,
                                          statusName: StatusNameConstants.penddingStatus,
                                          colorName: OmicronColors.pendingStatus)
            case 3:
                (entries, colors) =
                    setWorkloadProperties(workloadType: workload.finished,
                                          statusName: StatusNameConstants.finishedStatus,
                                          colorName: OmicronColors.finishedStatus)
            case 4:
                (entries, colors) =
                    setWorkloadProperties(workloadType: workload.reassigned,
                                          statusName: StatusNameConstants.reassignedStatus,
                                          colorName: OmicronColors.reassignedStatus)
            case 5:
                (entries, colors) =
                    setWorkLoadPropertiesFinalized(workloadType: workload.finalized)
            default: break
            }
        }
        setDataToChart(entries: entries ?? [], colors: colors ?? [], workload: workload)
    }
    func setWorkloadProperties(workloadType: Int?, statusName: String,
                               colorName: UIColor) -> ([PieChartDataEntry]?, [UIColor]?) {
        guard workloadType ?? 0 > 0 else { return (nil, nil) }
        var entries: [PieChartDataEntry] = []
        var colors: [UIColor] = []
        entries.append(PieChartDataEntry(
            value: Double(workloadType ?? 0), label: statusName))
        colors.append(colorName)
        return (entries, colors)
    }
    func setWorkLoadPropertiesFinalized(workloadType: Int?) -> ([PieChartDataEntry]?, [UIColor]?) {
        var entries: [PieChartDataEntry] = []
        var colors: [UIColor] = []
        guard workloadType ?? 0 > 0 else { return (nil, nil) }
        entries.append(PieChartDataEntry(
                        value: Double(workloadType ?? 0), label: StatusNameConstants.finalizedStatus))
        colors.append(UIColor.init(named: "finalized") ?? .black)
        return (entries, colors)
    }
    func setDataToChart(entries: [ChartDataEntry], colors: [UIColor], workload: Workload) {
        let set = PieChartDataSet(entries: entries, label: "")
        set.colors = colors
        set.sliceSpace = 2
        let data = PieChartData(dataSet: set)
        data.setValueFont(.systemFont(ofSize: 30, weight: .bold))
        data.setValueTextColor(.white)
        let pFormatter = NumberFormatter()
        pFormatter.numberStyle = .decimal
        data.setValueFormatter(DefaultValueFormatter(formatter: pFormatter))
        chartView.data = data
        let requests = "\(workload.totalOrders ?? 0) Pedidos"
        let orders = "\(workload.totalFabOrders ?? 0) Órdenes"
        let pieces = "\(workload.totalPieces ?? 0 ) Piezas"
        let extraData = requests + "\n" + orders + "\n" + pieces
        let myAttrString = NSAttributedString(
            string: extraData,
            attributes: [NSAttributedString.Key.foregroundColor: OmicronColors.blue,
                         NSAttributedString.Key.font: UIFont.boldSystemFont(ofSize: 25)])
        chartView.centerAttributedText = myAttrString
        let totalPossibleAssing = pFormatter.string(from: NSNumber(value: workload.totalPossibleAssign ?? 0))
        possibleAssingLabel.text = totalPossibleAssing
    }
    private func setupChart() {
        chartView.animate(xAxisDuration: 1.4, easingOption: .easeOutBack)
        chartView.transparentCircleRadiusPercent = 0.61
        chartView.drawCenterTextEnabled = true
    }
}
