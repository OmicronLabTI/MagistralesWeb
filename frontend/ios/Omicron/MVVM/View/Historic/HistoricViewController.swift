//
//  HistoricViewController.swift
//  Omicron
//
//  Created by Josue Castillo on 11/09/25.
//  Copyright © 2025 Diego Cárcamo. All rights reserved.
//

import UIKit
import Resolver
import RxSwift


class HistoricViewController: UIViewController {
    @IBOutlet var tableView: UITableView!
    @IBOutlet var searchBar: UISearchBar!
    var disposeBag: DisposeBag? = DisposeBag()
    var isLoading = false
    
    var ordersList: [ParentOrders] = []
    
    @Injected var historicViewModel: HistoricViewModel
    @Injected var lottieManager: LottieManager
    
    override func viewDidLoad() {
        super.viewDidLoad()
        view.backgroundColor = .white
        
        // Configuración de la tabla
        tableView.dataSource = self
        tableView.delegate = self

        // Opcional: altura automática para filas expandibles
        tableView.estimatedRowHeight = 44
        tableView.rowHeight = UITableView.automaticDimension
        
        modelBinding()
        bindLoading()
        getInitialData()
    }
    
    override func viewDidDisappear(_ animated: Bool) {
        super.viewDidDisappear(animated)
        disposeBag = DisposeBag()
        historicViewModel.orders = []
        historicViewModel.tableData.onNext([])
    }
    
    func getInitialData() {
        //        historicViewModel.getHistoricData(orders: String(), offset: 0, limit: 10)
        historicViewModel.getHistoryDataMock(orders: String(), offset: 0, limit: 10)
    }
    
    func resetInfo() {
        //historicViewModel.orders = []
    }
    
    func modelBinding() {
        self.searchBar.rx.text.orEmpty.bind(to: historicViewModel.searchFilter).disposed(by: disposeBag!)
        self.searchBar.rx.searchButtonClicked.bind(to: historicViewModel.searchDidTap).disposed(by: disposeBag!)
        historicViewModel.tableData.subscribe(onNext: { [weak self] list in
            guard let self = self else { return }
            
            for order in list {
                if order.detailOrdersCount > 0 {
                    if order.orderProductionDetail[0].orderProductionDetailId != 0 {
                    let headerChildrenRow = ChildrenOrders(OrderProductionDetailId: 0, AssignedPieces: 0, AssignedQfb: String(), DateCreated:  String())
                    order.orderProductionDetail.insert(headerChildrenRow, at: 0)
                }
            }
            self.ordersList.append(contentsOf: list)
//            tableView.reloadData()
            tableView.beginUpdates()
//            tableView.insertRows(at: [IndexPath(row: newRowIndex, section: 0)], with: .automatic)
            tableView.endUpdates()
        }).disposed(by: disposeBag!)
    }
    
    func headersForChildrenRows() {
        
    }
    
    func bindLoading() {
        self.historicViewModel.loading.observe(on: MainScheduler.instance).subscribe(onNext: {loading in
            self.isLoading = loading
            if loading {
                self.lottieManager.showLoading()
                return
            }
            self.lottieManager.hideLoading()
        }).disposed(by: disposeBag!)
    }
    
    // MARK: - Navigation
    
}
extension HistoricViewController: UITableViewDataSource, UITableViewDelegate {

    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        // Contamos todas las filas visibles: 1 por cada ParentOrders + sus hijos si están expandidos
        var count = 0
        for order in ordersList {
            count += 1 // fila del padre
            if order.autoExpandOrderDetail {
                count += order.orderProductionDetail.count // filas hijas
            }
        }
        return count
    }
    
    func orderAndChildIndex(for indexPath: IndexPath) -> (parentIndex: Int, childIndex: Int?)? {
        var rowCounter = 0
        for (pIndex, order) in ordersList.enumerated() {
            if rowCounter == indexPath.row {
                return (pIndex, nil) // fila del padre
            }
            rowCounter += 1
            
            if order.autoExpandOrderDetail {
                for (cIndex, _) in order.orderProductionDetail.enumerated() {
                    if rowCounter == indexPath.row {
                        return (pIndex, cIndex) // fila hija
                    }
                    rowCounter += 1
                }
            }
        }
        return nil
    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        
        guard let indices = orderAndChildIndex(for: indexPath) else {
            return UITableViewCell()
        }
        
        if let childIndex = indices.childIndex {
            if childIndex == 0 {
                // Fila para headers de ordenes hijas
                let cell = tableView.dequeueReusableCell(withIdentifier: ViewControllerIdentifiers.childrenOrderRowViewCell, for: indexPath) as! ChildrenOrderRowViewCell
                cell.isUserInteractionEnabled = false
                cell.backgroundColor = OmicronColors.customColor
                cell.childrenOrderIdLabel.text = "No. Orden hija"
                cell.childrenOrderIdLabel.font = UIFont.boldSystemFont(ofSize: 17)
                cell.assignedPiecesLabel.text = "Piezas asignadas"
                cell.assignedPiecesLabel.font = UIFont.boldSystemFont(ofSize: 17)
                cell.assignedQfbLabel.text = "QFB asignado"
                cell.assignedQfbLabel.font = UIFont.boldSystemFont(ofSize: 17)
                cell.createdDateLabel.text = "Fecha y hora de creación"
                cell.createdDateLabel.font = UIFont.boldSystemFont(ofSize: 17)
                return cell
            } else {
                // Fila hija o informacion expandida
                let detail = ordersList[indices.parentIndex].orderProductionDetail[childIndex]
                let cell = tableView.dequeueReusableCell(withIdentifier: ViewControllerIdentifiers.childrenOrderRowViewCell, for: indexPath) as! ChildrenOrderRowViewCell
                cell.backgroundColor = OmicronColors.customColor
                cell.childrenOrderIdLabel.text = "\(detail.orderProductionDetailId)"
                cell.assignedPiecesLabel.text = "\(detail.assignedPieces)"
                cell.assignedQfbLabel.text = "\(detail.assignedQfb)"
                cell.createdDateLabel.text = "\(detail.dateCreated)"
                return cell
            }
        } else {
            // Fila padre
            
            let order = ordersList[indices.parentIndex]
//            print(indices.parentIndex)
            let cell = tableView.dequeueReusableCell(withIdentifier: ViewControllerIdentifiers.historicTableViewCell, for: indexPath) as! HistoricTableViewCell
            if indices.parentIndex%2 == 0 {
                cell.backgroundColor = OmicronColors.ligthGray
            } else {
                cell.backgroundColor = .white
            }
            cell.isSelected = order.autoExpandOrderDetail
            cell.parentOrderIdLabel.text = "\(order.orderProductionId)"
            cell.qfbLabel.text = order.qfbWhoSplit
            cell.totalPiecesLabel.text = "\(order.totalPieces)"
            cell.availablePiecesLabel.text = "\(order.availablePieces)"
            cell.childrenOrdersLabel.text = "\(order.detailOrdersCount)"
            return cell
        }
    }
    
    func tableView(_ tableView: UITableView, didSelectRowAt indexPath: IndexPath) {
        guard let indices = orderAndChildIndex(for: indexPath), indices.childIndex == nil else { return }
        
        tableView.beginUpdates()
        let parentIndex = indices.parentIndex
        ordersList[parentIndex].autoExpandOrderDetail.toggle()
        
        let startRow = indexPath.row + 1
        let count = ordersList[parentIndex].orderProductionDetail.count
        var indexPaths: [IndexPath] = []
        for i in 0..<count {
            indexPaths.append(IndexPath(row: startRow + i, section: 0))
        }
        
        if ordersList[parentIndex].autoExpandOrderDetail {
            tableView.insertRows(at: indexPaths, with: .fade)
        } else {
            tableView.deleteRows(at: indexPaths, with: .fade)
        }
        tableView.endUpdates()
    }
    
    func tableView(_ tableView: UITableView, heightForRowAt indexPath: IndexPath) -> CGFloat {
        return 60
    }
    
    func tableView(_ tableView: UITableView, willDisplay cell: UITableViewCell, forRowAt indexPath: IndexPath) {
        
        let customView = UIView()
        customView.backgroundColor = OmicronColors.tableColorRow
        cell.selectedBackgroundView = customView
        
        let lastSectionIndex = tableView.numberOfSections - 1
        let lastRowIndex = tableView.numberOfRows(inSection: lastSectionIndex) - 1
        
//        print(indexPath.row , lastRowIndex)
        if indexPath.section == lastSectionIndex &&
            !isLoading &&
            indexPath.row == lastRowIndex &&
            lastRowIndex >= 9 {
            tableView.scrollToRow(at: [0, lastRowIndex - 4],
                                  at: .middle,
                                  animated: false)
            historicViewModel.onScroll.onNext(())
        }
        
        
        
//        if indexPath.row%2 == 0 {
//            if let childCell = cell as? ChildrenOrderRowViewCell {
//                return
//            } else {
//                cell.backgroundColor = OmicronColors.ligthGray
//            }
//        } else {
//            if let childCell = cell as? ChildrenOrderRowViewCell {
//                return
//            } else {
//                cell.backgroundColor = .white
//            }
//        }
        
    }

}
