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
    @Injected var lottieManager: LottieManager
    
    var disposeBag: DisposeBag = DisposeBag()
    
    override func viewDidLoad() {
        super.viewDidLoad()
        title = "Carga de trabajo"
        viewModelBingind()
        setupChart()
    }
    
    func viewModelBingind() {
        
        chartViewModel.getWorkload()
        
        chartViewModel.loading.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] showLoading in
            
            guard let self = self else { return }
            if showLoading {
                self.lottieManager.showLoading()
            } else {
                self.lottieManager.hideLoading()
            }
            
        }).disposed(by: disposeBag)
        
        chartViewModel.workloadData.bind(onNext: { [weak self] data in
            guard let self = self else { return }
            self.setDataToChart(data.first)
        }).disposed(by: disposeBag)
        
    }
    
    private func setDataToChart(_ workload: Workload?) {
        
        guard let workload = workload else {
            self.lottieManager.hideLoading()
            return
        }
        
        var entries: [PieChartDataEntry] = []
        
        for i in 0...5 {
            
            switch i {
            case 0:
                guard workload.assigned ?? 0 > 0 else { break }
                entries.append(PieChartDataEntry(value: Double(workload.assigned ?? 0), label: StatusNameConstants.assignedStatus))
            case 1:
                guard workload.processed ?? 0 > 0 else { break }
                entries.append(PieChartDataEntry(value: Double(workload.processed ?? 0), label: StatusNameConstants.inProcessStatus))
            case 2:
                guard workload.pending ?? 0 > 0 else { break }
                entries.append(PieChartDataEntry(value: Double(workload.pending ?? 0), label: StatusNameConstants.penddingStatus))
            case 3:
                guard workload.finalized ?? 0 > 0 else { break }
                entries.append(PieChartDataEntry(value: Double(workload.finalized ?? 0), label: StatusNameConstants.finishedStatus))
            case 4:
                guard workload.reassigned ?? 0 > 0 else { break }
                entries.append(PieChartDataEntry(value: Double(workload.reassigned ?? 0), label: StatusNameConstants.reassignedStatus))
            default: break
            }
            
        }
        
        let set = PieChartDataSet(entries: entries, label: "Estatus")
        set.colors = [OmicronColors.assignedStatus, OmicronColors.processStatus, OmicronColors.pendingStatus, OmicronColors.finishedStatus, OmicronColors.reassignedStatus]
        set.sliceSpace = 2
        
        let data = PieChartData(dataSet: set)
        data.setValueFont(.systemFont(ofSize: 30, weight: .bold))
        data.setValueTextColor(.white)
        
        let pFormatter = NumberFormatter()
        pFormatter.numberStyle = .none
        data.setValueFormatter(DefaultValueFormatter(formatter: pFormatter))
        
        chartView.data = data
        
        let requests = "\(workload.totalFabOrders ?? 0) Pédidos"
        let orders = "\(workload.totalOrders ?? 0) Órdenes"
        let pieces = "\(workload.totalPieces ?? 0 ) Piezas"
        
        let extraData = requests + "\n" + orders + "\n" + pieces
        let myAttrString = NSAttributedString(string: extraData, attributes: [NSAttributedString.Key.foregroundColor: OmicronColors.blue, NSAttributedString.Key.font: UIFont.boldSystemFont(ofSize: 25)])
        
        chartView.centerAttributedText = myAttrString
        
        possibleAssingLabel.text = "\(workload.totalPossibleAssign ?? 0)"
        
    }
    
    private func setupChart() {
        
        chartView.animate(xAxisDuration: 1.4, easingOption: .easeOutBack)
        chartView.transparentCircleRadiusPercent = 0.61
        chartView.drawCenterTextEnabled = true
        
    }

}
