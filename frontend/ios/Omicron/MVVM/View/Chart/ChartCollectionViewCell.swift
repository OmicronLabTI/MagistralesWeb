//
//  ChartCollectionViewCell.swift
//  Omicron
//
//  Created by Vicente Cantu Garcia on 08/03/21.
//  Copyright © 2021 Diego Cárcamo. All rights reserved.
//

import UIKit
import Charts

class ChartCollectionViewCell: UICollectionViewCell, ChartViewDelegate {

    @IBOutlet weak var pieChartView: PieChartView!

    override func layoutSubviews() {
        super.layoutSubviews()
    }

    func setData(data: Workload?) {
        pieChartView.delegate = self
        guard let data = data else { return }
        setDataToChart(data)
    }

    private func setDataToChart(_ workload: Workload) {
        var entries: [PieChartDataEntry] = []
        var colors: [UIColor] = []

        var config: (entry: [PieChartDataEntry], color: [UIColor]) = ( entry: [], color: [])
        for index in 0...5 {
            switch index {
            case 0:
                config = getConfigPieChartDataEntry(
                    workload.assigned ?? 0, StatusNameConstants.assignedStatus, OmicronColors.assignedStatus)
            case 1:
                config = getConfigPieChartDataEntry(
                    workload.processed ?? 0, StatusNameConstants.inProcessStatus, OmicronColors.processStatus)
            case 2:
                config = getConfigPieChartDataEntry(
                    workload.pending ?? 0, StatusNameConstants.penddingStatus, OmicronColors.pendingStatus)
            case 3:
                config = getConfigPieChartDataEntry(
                    workload.finished ?? 0, StatusNameConstants.finishedStatus, OmicronColors.finishedStatus)
            case 4:
                config = getConfigPieChartDataEntry(
                    workload.reassigned ?? 0, StatusNameConstants.reassignedStatus, OmicronColors.reassignedStatus)
            case 5:
                config = getConfigPieChartDataEntry(
                    workload.finalized ?? 0, StatusNameConstants.finalizedStatus,
                    UIColor.init(named: "finalized") ?? .black)
            default: break
            }
            entries += config.entry
            colors += config.color
        }
        setDataToChart2(workload, colors: colors, entries: entries)

    }

    func getConfigPieChartDataEntry(
        _ workloadQty: Int, _ labelValue: String,
        _ color: UIColor) -> (entry: [PieChartDataEntry], color: [UIColor]) {
            if workloadQty > 0 {
                return (entry: [PieChartDataEntry(value: Double(workloadQty), label: labelValue)], color: [color])
            }
            return (entry: [], color: [])
        }

    private func setDataToChart2(_ workload: Workload, colors: [UIColor], entries: [PieChartDataEntry]) {

        let set = PieChartDataSet(entries: entries, label: "")
        set.colors = colors
        set.sliceSpace = 2

        let data = PieChartData(dataSet: set)
        data.setValueFont(.systemFont(ofSize: 30, weight: .bold))
        data.setValueTextColor(.white)

        let pFormatter = NumberFormatter()
        pFormatter.numberStyle = .decimal
        data.setValueFormatter(DefaultValueFormatter(formatter: pFormatter))

        pieChartView.data = data

        let requests = "\(workload.totalOrders ?? 0) Pedidos"
        let orders = "\(workload.totalFabOrders ?? 0) Órdenes"
        let pieces = "\(workload.totalPieces ?? 0 ) Piezas"

        let extraData = requests + "\n" + orders + "\n" + pieces
        let myAttrString = NSAttributedString(
              string: extraData,
              attributes: [NSAttributedString.Key.foregroundColor: OmicronColors.blue,
                           NSAttributedString.Key.font: UIFont.fontDefaultBold(25)])

        pieChartView.centerAttributedText = myAttrString

        pieChartView.animate(xAxisDuration: 1.0, easingOption: .easeOutBack)
        pieChartView.transparentCircleRadiusPercent = 0.61
        pieChartView.drawCenterTextEnabled = true

    }

}
