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
    var ordersList: [ParentOrders] = [ParentOrders(
        orderProductionId: 201294,
        totalPieces: 10,
        availablePieces: 6,
        qfbWhoSplit: "b49b30e1-5232-48f3-be74-axaxaxaxaxax",
        detailOrdersCount: 2,
        orderProductionDetail: [
            ChildrenOrders(
                OrderProductionDetailId: 201295,
                AssignedPieces: 2,
                AssignedQfb: "b49b30e1-5232-48f3-be74-d3f4eebe0b4f",
                DateCreated: "08/09/2025 06:39:31"
            ),
            ChildrenOrders(
                OrderProductionDetailId: 201296,
                AssignedPieces: 2,
                AssignedQfb: "b49b30e1-5232-48f3-be74-d3f4eebe0b4f",
                DateCreated: "09/09/2025 06:39:31"
            )
        ],
        autoExpandOrderDetail: false
    )]
    @Injected var historicViewModel: HistoricViewModel
    
    override func viewDidLoad() {
        super.viewDidLoad()
        getInitialData()
        modelBinding()
        self.tableView.delegate = self
        self.tableView.dataSource = self
        tableView.register(UITableViewCell.self, forCellReuseIdentifier: "cell")
    }
    
    override func viewDidDisappear(_ animated: Bool) {
        super.viewDidDisappear(animated)
        disposeBag = DisposeBag()
        tableView.reloadData()
        historicViewModel.tableData.onNext([])
    }
    
    func getInitialData() {
        //        historicViewModel.getHistoricData(orders: String(), offset: 0, limit: 10)
        historicViewModel.getHistoryDataMock(orders: String(), offset: 0, limit: 10)
    }
    
    func resetInfo() {
        historicViewModel.orders = []
    }
    
    @objc func toggleSection(_ sender: UITapGestureRecognizer) {
        guard let section = sender.view?.tag else { return }
        ordersList[section].autoExpandOrderDetail.toggle()
        tableView.reloadSections(IndexSet(integer: section), with: .automatic)
    }
    
    func modelBinding() {
        self.searchBar.rx.text.orEmpty.bind(to: historicViewModel.searchFilter).disposed(by: disposeBag!)
        self.searchBar.rx.searchButtonClicked.bind(to: historicViewModel.searchDidTap).disposed(by: disposeBag!)
        historicViewModel.tableData.subscribe(onNext: { [weak self] list in
            guard let self = self else { return }
            self.ordersList = list
            dump(self.ordersList)
            tableView.reloadData()
        }).disposed(by: disposeBag!)
        
        //        self.historicViewModel.tableData.bind(to: tableView.rx.items(
        //            cellIdentifier: ViewControllerIdentifiers.historicTableViewCell, cellType: HistoricTableViewCell.self
        //        )) { [weak self] row, data, cell in
        //            cell.accessoryType = .disclosureIndicator
        //            cell.parentOrderIdLabel.text = "\(data.orderProductionId)"
        //            cell.totalPiecesLabel.text = "\(data.totalPieces)"
        //            cell.availablePiecesLabel.text = "\(data.availablePieces)"
        //            cell.qfbLabel.text = "\(data.qfbWhoSplit)"
        //            cell.qfbLabel.sizeToFit()
        //            var newFrame = cell.qfbLabel.frame
        //            newFrame.size.width = min(newFrame.width, 250)
        //            cell.qfbLabel.frame = newFrame
        //            cell.childrenOrdersLabel.text = "\(data.detailOrdersCount)"
        //        }.disposed(by: disposeBag!)
    }
    
    // MARK: - Navigation
    
}

extension HistoricViewController: UITableViewDataSource, UITableViewDelegate {
    
    func numberOfSections(in tableView: UITableView) -> Int {
        return ordersList.count
    }
    
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return ordersList[section].autoExpandOrderDetail ? ordersList[section].orderProductionDetail.count : 0
        //        return ordersList.count
    }
    
    // DETALLES
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let detail = ordersList[indexPath.section].orderProductionDetail[indexPath.row]
        let cell = tableView.dequeueReusableCell(withIdentifier: "cell", for: indexPath)
        cell.textLabel?.text = "\(detail.assignedQfb) - \(detail.assignedPieces)"
        return cell
    }
    
    // HEADERS
    
    func tableView(_ tableView: UITableView, viewForHeaderInSection section: Int) -> UIView? {
        guard let header = tableView.dequeueReusableHeaderFooterView(withIdentifier: ViewControllerIdentifiers.HistoricHeaderView) as? HistoricTableViewCell else {
            return nil
        }
        
        let order = ordersList[section]
        header.parentOrderIdLabel.text = "Pedido #\(order.orderProductionId)"
        header.qfbLabel.text = "Pedido #\(order.orderProductionId)"
        header.totalPiecesLabel.text = "Pedido #\(order.orderProductionId)"
        header.childrenOrdersLabel.text = "Pedido #\(order.orderProductionId)"
        header.availablePiecesLabel.text = "Pedido #\(order.orderProductionId)"
        
        let tapGesture = UITapGestureRecognizer(target: self, action: #selector(toggleSection(_:)))
        header.tag = section
        header.addGestureRecognizer(tapGesture)
        
        return header.contentView
    }
    
    func tableView(_ tableView: UITableView, heightForHeaderInSection section: Int) -> CGFloat {
        return 60
    }
    
    // Opcional: altura dinámica para las filas
    func tableView(_ tableView: UITableView, heightForRowAt indexPath: IndexPath) -> CGFloat {
        return 44
    }
    
}

//extension HistoricViewController: UITableViewDelegate {
//    func tableView(_ tableView: UITableView, willDisplay cell: UITableViewCell, forRowAt indexPath: IndexPath) {
//        let customView = UIView()
//        customView.backgroundColor = OmicronColors.tableColorRow
//        cell.selectedBackgroundView = customView
//        let lastSectionIndex = tableView.numberOfSections - 1
//        let lastRowIndex = tableView.numberOfRows(inSection: lastSectionIndex) - 1
//        //        if indexPath.section == lastSectionIndex &&
//        //            !isLoading &&
//        //            indexPath.row == lastRowIndex - 3 {
//        //            tableView.scrollToRow(at: [0, lastRowIndex - 7],
//        //                                  at: .middle,
//        //                                  animated: false)
//        //            historicViewModel.onScroll.onNext(())
//        //        }
//        if indexPath.row%2 == 0 {
//            cell.backgroundColor = OmicronColors.ligthGray
//        } else {
//            cell.backgroundColor = .white
//        }
//    }
//}
