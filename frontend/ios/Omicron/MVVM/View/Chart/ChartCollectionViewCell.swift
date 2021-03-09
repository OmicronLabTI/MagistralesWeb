//
//  ChartCollectionViewCell.swift
//  Omicron
//
//  Created by Vicente Cantu Garcia on 08/03/21.
//  Copyright © 2021 Diego Cárcamo. All rights reserved.
//

import UIKit
import Charts

class ChartCollectionViewCell: UICollectionViewCell {

    @IBOutlet weak var pieChartView: PieChartView!

    override func layoutSubviews() {
        super.layoutSubviews()
    }

    func setData(data: Workload?) {
        guard let data = data else { return }
        setDataToChart(data)
    }

    private func setDataToChart(_ workload: Workload) {
        var entries: [PieChartDataEntry] = []
        var colors: [UIColor] = []
        for index in 0...5 {
            switch index {
            case 0: guard workload.assigned ?? 0 > 0 else { break }
                entries.append(PieChartDataEntry(
                        value: Double(workload.assigned ?? 0),
                        label: StatusNameConstants.assignedStatus))
                colors.append(OmicronColors.assignedStatus)
            case 1: guard workload.processed ?? 0 > 0 else { break }
                entries.append(PieChartDataEntry(
                        value: Double(workload.processed ?? 0),
                        label: StatusNameConstants.inProcessStatus))
                colors.append(OmicronColors.processStatus)
            case 2: guard workload.pending ?? 0 > 0 else { break }
                entries.append(PieChartDataEntry(
                        value: Double(workload.pending ?? 0),
                        label: StatusNameConstants.penddingStatus))
                colors.append(OmicronColors.pendingStatus)
            case 3: guard workload.finished ?? 0 > 0 else { break }
                entries.append(PieChartDataEntry(
                        value: Double(workload.finished ?? 0),
                        label: StatusNameConstants.finishedStatus))
                colors.append(OmicronColors.finishedStatus)
            case 4: guard workload.reassigned ?? 0 > 0 else { break }
                entries.append(PieChartDataEntry(
                        value: Double(workload.reassigned ?? 0),
                        label: StatusNameConstants.reassignedStatus))
                colors.append(OmicronColors.reassignedStatus)
            case 5: guard workload.finalized ?? 0 > 0 else { break }
                entries.append(PieChartDataEntry(
                    value: Double(workload.finalized ?? 0),
                    label: StatusNameConstants.finalizedStatus))
                colors.append(UIColor.init(named: "finalized") ?? .black)
            default: break
            }
        }
        setDataToChart2(workload, colors: colors, entries: entries)

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
                           NSAttributedString.Key.font: UIFont.boldSystemFont(ofSize: 25)])

        pieChartView.centerAttributedText = myAttrString

        pieChartView.animate(xAxisDuration: 1.0, easingOption: .easeOutBack)
        pieChartView.transparentCircleRadiusPercent = 0.61
        pieChartView.drawCenterTextEnabled = true

    }

}
