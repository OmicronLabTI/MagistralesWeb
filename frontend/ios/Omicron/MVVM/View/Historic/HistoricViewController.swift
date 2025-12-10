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
    @IBOutlet weak var noResultsLabel: UILabel!
    var disposeBag: DisposeBag? = DisposeBag()
    var isLoading = false
    var refreshControl = UIRefreshControl()
    
    var ordersList: [ParentOrders] = []
    
    @Injected var historicViewModel: HistoricViewModel
    @Injected var lottieManager: LottieManager
    
    override func viewDidLoad() {
        super.viewDidLoad()
        view.backgroundColor = .white
        self.title = "Órdenes padre abiertas"
        
        // Configuración de la tabla
        tableView.dataSource = self
        tableView.delegate = self
        searchBar.delegate = self

        // Opcional: altura automática para filas expandibles
        tableView.estimatedRowHeight = 44
        tableView.rowHeight = UITableView.automaticDimension
        
        refreshControl.addTarget(self, action: #selector(refreshData), for: .valueChanged)
        tableView.refreshControl = refreshControl
        
        modelBinding()
        bindLoading()
        getInitialData()
    }
    
    override func viewDidDisappear(_ animated: Bool) {
        super.viewDidDisappear(animated)
        disposeBag = DisposeBag()
        historicViewModel.orders = []
        historicViewModel.chips = String()
        historicViewModel.dataOffset = 0
        historicViewModel.tableData.onNext([])
    }
    
    func getInitialData() {
        historicViewModel.getHistoricData(orders: String(), offset: 0, limit: 10)
    }
    
    func resetInfo() {
        self.ordersList = []
        self.tableView.reloadData()
    }
    
    func modelBinding() {
        self.searchBar.backgroundImage = UIImage()
        if let textField = searchBar.value(forKey: "searchField") as? UITextField {
            textField.backgroundColor = .white  // fondo blanco
            
            textField.layer.borderColor = UIColor.lightGray.cgColor // color del borde
            textField.layer.borderWidth = 1.0 / UIScreen.main.scale // 1 px real (retina safe)
            
            textField.layer.cornerRadius = 8   // opcional, para bordes redondeados
            textField.layer.masksToBounds = true
        }
        self.searchBar.rx.text.orEmpty.bind(to: historicViewModel.searchFilter).disposed(by: disposeBag!)
        self.searchBar.rx.searchButtonClicked.bind(to: historicViewModel.searchDidTap).disposed(by: disposeBag!)
        self.historicViewModel.restartTable.subscribe(onNext: {[weak self] restartTable in
            if restartTable {
                self?.resetInfo()
            }
        }).disposed(by: disposeBag!)
        self.historicViewModel.endRefreshing.observe(on: MainScheduler.instance).subscribe(onNext: { [weak self]  _ in
            self?.refreshControl.endRefreshing()
        }).disposed(by: self.disposeBag!)
        historicViewModel.tableData.subscribe(onNext: { [weak self] list in
            guard let self = self else { return }
            
            if let searchBarText = self.searchBar.text, !searchBarText.isEmpty && list.count == 0 {
                tableView.isHidden = true
                noResultsLabel.isHidden = false
            } else {
                tableView.isHidden = false
                noResultsLabel.isHidden = true
                for order in list {
                    if order.orderProductionDetail.count > 0 {
                        if order.orderProductionDetail[0].orderProductionDetailId != "0" {
                            let headerChildrenRow = ChildrenOrders(
                                orderProductionDetailId: "0",
                                assignedPieces: 0,
                                assignedQfb: "",
                                dateCreated: ""
                            )
                            order.orderProductionDetail.insert(headerChildrenRow, at: 0)
                        }
                    }
                }
                
                // Contar cuántas filas había antes
                let oldCount = tableView.numberOfRows(inSection: 0)
                
                // Agregar las nuevas órdenes al modelo
                self.ordersList.append(contentsOf: list)
                
                // Calcular los nuevos IndexPath (padres + hijos si están expandidos)
                var newIndexPaths: [IndexPath] = []
                var row = oldCount
                for order in list {
                    // Padre
                    newIndexPaths.append(IndexPath(row: row, section: 0))
                    row += 1
                    
                    // Hijos visibles si está expandido
                    if order.autoExpandOrderDetail {
                        for _ in order.orderProductionDetail {
                            newIndexPaths.append(IndexPath(row: row, section: 0))
                            row += 1
                        }
                    }
                }
                
                // Insertar las nuevas filas
                tableView.beginUpdates()
                tableView.insertRows(at: newIndexPaths, with: .automatic)
                tableView.endUpdates()
            }
        }).disposed(by: disposeBag!)
    }
    
    func getMoreData() {
        if historicViewModel.orders.count < historicViewModel.totalOrders {
            historicViewModel.onScroll.onNext(())
        }
    }
    
    @objc func refreshData() {
        self.historicViewModel.updateData(isRefresh: true)
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
extension HistoricViewController: UITableViewDataSource, UITableViewDelegate, UISearchBarDelegate {
    
    func searchBar(_ searchBar: UISearchBar, textDidChange searchText: String) {
        if searchText.isEmpty {
            historicViewModel.searchFilter.onNext(String())
            historicViewModel.searchDidTap.onNext(())
        }
    }

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
                guard let cell = tableView.dequeueReusableCell(
                    withIdentifier: ViewControllerIdentifiers.childrenOrderRowViewCell,
                    for: indexPath) as? ChildrenOrderRowViewCell else {
                    fatalError("No se pudo cargar la celda de childrenOrderRowViewCell")
                }
                cell.isUserInteractionEnabled = false
                cell.backgroundColor = OmicronColors.customColor
                UtilsManager.shared.labelsStyle(label: cell.childrenOrderIdLabel, text: "N° Orden hija",
                                                fontSize: 17, typeFont: "bold")
                UtilsManager.shared.labelsStyle(label: cell.assignedPiecesLabel, text: "Piezas asignadas",
                                                fontSize: 17, typeFont: "bold")
                UtilsManager.shared.labelsStyle(label: cell.assignedQfbLabel, text: "QFB asignado",
                                                fontSize: 17, typeFont: "bold")
                UtilsManager.shared.labelsStyle(label: cell.createdDateLabel, text: "Fecha y hora de creación",
                                                fontSize: 17, typeFont: "bold")
                return cell
            } else {
                // Fila hija o informacion expandida
                let detail = ordersList[indices.parentIndex].orderProductionDetail[childIndex]
                guard let cell = tableView.dequeueReusableCell(
                    withIdentifier: ViewControllerIdentifiers.childrenOrderRowViewCell,
                    for: indexPath) as? ChildrenOrderRowViewCell else {
                    fatalError("No se pudo cargar la celda de childrenOrderRowViewCell")
                }
                cell.backgroundColor = OmicronColors.customColor
                UtilsManager.shared.labelsStyle(label: cell.childrenOrderIdLabel, text: "\(detail.orderProductionDetailId)",
                                                fontSize: 17, typeFont: "regular")
                UtilsManager.shared.labelsStyle(label: cell.assignedPiecesLabel, text: "\(detail.assignedPieces)",
                                                fontSize: 17, typeFont: "regular")
                UtilsManager.shared.labelsStyle(label: cell.assignedQfbLabel, text: "\(detail.assignedQfb)",
                                                fontSize: 17, typeFont: "regular")
                UtilsManager.shared.labelsStyle(label: cell.createdDateLabel, text: "\(detail.dateCreated)",
                                                fontSize: 17, typeFont: "regular")
                return cell
            }
        } else {
            // Fila padre
            let order = ordersList[indices.parentIndex]
            guard let cell = tableView.dequeueReusableCell(
                withIdentifier: ViewControllerIdentifiers.historicTableViewCell,
                for: indexPath) as? HistoricTableViewCell else {
                fatalError("No se pudo cargar la celda de childrenOrderRowViewCell")
            }
            if indices.parentIndex%2 == 0 {
                if order.autoExpandOrderDetail {
                    cell.backgroundColor = OmicronColors.tableColorRow
                } else {
                    cell.backgroundColor = OmicronColors.ligthGray
                }
            } else {
                if order.autoExpandOrderDetail {
                    cell.backgroundColor = OmicronColors.tableColorRow
                } else {
                    cell.backgroundColor = .white
                }
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
        
        if let cell = tableView.cellForRow(at: indexPath) as? HistoricTableViewCell {
            let isExpanded = ordersList[parentIndex].autoExpandOrderDetail
            let imageName = isExpanded ? "chevron.up" : "chevron.down"
            cell.actionImage.image = UIImage(systemName: imageName)
        }

        let startRow = indexPath.row + 1
        let count = ordersList[parentIndex].orderProductionDetail.count
        var indexPaths: [IndexPath] = []
        for index in 0..<count {
            indexPaths.append(IndexPath(row: startRow + index, section: 0))
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
    
    func scrollViewDidScroll(_ scrollView: UIScrollView) {
        let offsetY = scrollView.contentOffset.y
        let contentHeight = scrollView.contentSize.height
        let height = scrollView.frame.size.height
        
        if offsetY > contentHeight - height + 100 && !isLoading {
            isLoading = true
            getMoreData()
        }
    }
    
    func tableView(_ tableView: UITableView, willDisplay cell: UITableViewCell, forRowAt indexPath: IndexPath) {
        
        let customView = UIView()
        customView.backgroundColor = OmicronColors.tableColorRow
        cell.selectedBackgroundView = customView
        
    }

}
