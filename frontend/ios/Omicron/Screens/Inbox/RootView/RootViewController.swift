//
//  RootViewController.swift
//  Omicron
//
//  Created by Axity on 29/07/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit
import RxSwift
import RxCocoa

class RootViewController: UIViewController {

    // MARK: Outlets
    @IBOutlet weak var viewTable: UITableView!
    @IBOutlet weak var myOrdesLabel: UILabel!
    @IBOutlet weak var searchOrdesSearchBar: UISearchBar!
    
    // Variables
    let disposeBag = DisposeBag()
    let rootViewModel = RootViewModel()
    lazy var inboxViewModel = self.getInboxViewModel()
    var dataStatus:[Section] = []
    // MARK: Life Cycles
    override func viewDidLoad() {
        super.viewDidLoad()
        
        NetworkManager.shared.getInfoUser(userId: "sergio").subscribe(onNext: { [weak self] res in
            print("--------------------> \(res)")
            
            }, onError: { errorService in
                print("Error: \(errorService)")
        }).disposed(by: self.disposeBag)
        
        // Desde aqui consumo el servicio
        var assignedOrders =
        [
            Order(no: 1, baseDocument: "Documento base 1", container: "Contenedor 1", tag: "Tag 1", plannedQuantity: "cantidad planeada 1", startDate: "27/03/2020", finishDate: "24/04/2020", descriptionProduct: "Descripción del producto 1"),
            Order(no: 1, baseDocument: "Documento base 2", container: "Contenedor 1", tag: "Tag 1", plannedQuantity: "cantidad planeada 1", startDate: "27/03/2020", finishDate: "24/04/2020", descriptionProduct: "Descripción del producto 1"),
            Order(no: 1, baseDocument: "Documento base 3", container: "Contenedor 1", tag: "Tag 1", plannedQuantity: "cantidad planeada 1", startDate: "27/03/2020", finishDate: "24/04/2020", descriptionProduct: "Descripción del producto 1")
        ]
        
        let inProcessOrdes =
        [
            Order(no: 1, baseDocument: "Documento base 1", container: "Contenedor 1", tag: "Tag 1", plannedQuantity: "cantidad planeada 1", startDate: "27/03/2020", finishDate: "24/04/2020", descriptionProduct: "Descripción del producto 1"),
            Order(no: 1, baseDocument: "Documento base 2", container: "Contenedor 1", tag: "Tag 1", plannedQuantity: "cantidad planeada 1", startDate: "27/03/2020", finishDate: "24/04/2020", descriptionProduct: "Descripción del producto 1"),
            Order(no: 1, baseDocument: "Documento base 3", container: "Contenedor 1", tag: "Tag 1", plannedQuantity: "cantidad planeada 1", startDate: "27/03/2020", finishDate: "24/04/2020", descriptionProduct: "Descripción del producto 1"),
            Order(no: 1, baseDocument: "Documento base 4", container: "Contenedor 1", tag: "Tag 1", plannedQuantity: "cantidad planeada 1", startDate: "27/03/2020", finishDate: "24/04/2020", descriptionProduct: "Descripción del producto 1")
        ]
        
        let penddingOrders =
        [
            Order(no: 1, baseDocument: "Documento base 1", container: "Contenedor 1", tag: "Tag 1", plannedQuantity: "cantidad planeada 1", startDate: "27/03/2020", finishDate: "24/04/2020", descriptionProduct: "Descripción del producto 1"),
            Order(no: 1, baseDocument: "Documento base 2", container: "Contenedor 1", tag: "Tag 1", plannedQuantity: "cantidad planeada 1", startDate: "27/03/2020", finishDate: "24/04/2020", descriptionProduct: "Descripción del producto 1"),
            Order(no: 1, baseDocument: "Documento base 3", container: "Contenedor 1", tag: "Tag 1", plannedQuantity: "cantidad planeada 1", startDate: "27/03/2020", finishDate: "24/04/2020", descriptionProduct: "Descripción del producto 1"),
            Order(no: 1, baseDocument: "Documento base 4", container: "Contenedor 1", tag: "Tag 1", plannedQuantity: "cantidad planeada 1", startDate: "27/03/2020", finishDate: "24/04/2020", descriptionProduct: "Descripción del producto 1"),
            Order(no: 1, baseDocument: "Documento base 5", container: "Contenedor 1", tag: "Tag 1", plannedQuantity: "cantidad planeada 1", startDate: "27/03/2020", finishDate: "24/04/2020", descriptionProduct: "Descripción del producto 1"),
            Order(no: 1, baseDocument: "Documento base 6", container: "Contenedor 1", tag: "Tag 1", plannedQuantity: "cantidad planeada 1", startDate: "27/03/2020", finishDate: "24/04/2020", descriptionProduct: "Descripción del producto 1")
        ]
        
        let finishedOrders =
        [
            Order(no: 1, baseDocument: "Documento base 1", container: "Contenedor 1", tag: "Tag 1", plannedQuantity: "cantidad planeada 1", startDate: "27/03/2020", finishDate: "24/04/2020", descriptionProduct: "Descripción del producto 1")
        ]
        
        let reasignedOrders =
        [
            Order(no: 1, baseDocument: "Documento base 1", container: "Contenedor 1", tag: "Tag 1", plannedQuantity: "cantidad planeada 1", startDate: "27/03/2020", finishDate: "24/04/2020", descriptionProduct: "Descripción del producto 1")
        ]
        
        self.dataStatus = [
            Section( statusName: "Asignadas", numberTask: assignedOrders.count, imageIndicatorStatus: "assignedStatus"),
            Section(statusName: "En Proceso", numberTask: inProcessOrdes.count, imageIndicatorStatus: "processStatus"),
            Section(statusName: "Pendientes", numberTask: penddingOrders.count, imageIndicatorStatus: "pendingStatus"),
            Section(statusName: "Terminado", numberTask: finishedOrders.count, imageIndicatorStatus: "finishedStatus"),
            Section(statusName: "Reasignado", numberTask: reasignedOrders.count, imageIndicatorStatus: "reassignedStatus")
        ]

        
//        self.rootViewModel.dataStatus
//            .observeOn(MainScheduler.instance)
//            .subscribe(onNext: { res in
//                self.dataStatus = res
//                print("data: \(self.dataStatus)")
//            }).disposed(by: disposeBag)
        
        Observable.just(dataStatus).bind(to: viewTable.rx.items(cellIdentifier: ViewControllerIdentifiers.rootTableViewCell, cellType:  RootTableViewCell.self)) { _, data, cell in
                    cell.indicatorStatusImageView.image = UIImage(named: data.imageIndicatorStatus)
                    cell.indicatorStatusNameLabel.text = data.statusName
                    cell.indicatorStatusNumber.text = String(data.numberTask)
                }.disposed(by: disposeBag)
        
        
        
//        self.rootViewModel.dataStatus.observeOn(MainScheduler.instance).bind(to: viewTable.rx.items(cellIdentifier: ViewControllerIdentifiers.rootTableViewCell, cellType:  RootTableViewCell.self)) { _, data, cell in
//            cell.indicatorStatusImageView.image = UIImage(named: data.imageIndicatorStatus)
//            cell.indicatorStatusNameLabel.text = data.statusName
//            cell.indicatorStatusNumber.text = String(data.numberTask)
//        }.disposed(by: disposeBag)
//        initComponents()
        
        self.title = "Sergio Flores"
        let index = NSIndexPath(row: 0, section: 0)
        viewTable.selectRow(at: index as IndexPath, animated: true, scrollPosition: .middle)
        viewTable.rx.itemSelected.subscribe( onNext: { [weak self] indexPath in
        let orden = Order(no: 1, baseDocument: "Documento base 1", container: "Contenedor 1", tag: "Tag 1", plannedQuantity: "cantidad planeada 1", startDate: "27/03/2020", finishDate: "24/04/2020", descriptionProduct: "Descripción del producto 1")
            self?.inboxViewModel?.setSelection(index: indexPath.row, orden: orden)
            }).disposed(by: disposeBag)
    }
    
    // MARK: Functions
    func initComponents() {
        self.viewTable.tableFooterView = UIView()
        self.viewTable.backgroundColor = OmicronColors.tableStatus
        self.myOrdesLabel.backgroundColor = OmicronColors.tableStatus
        self.myOrdesLabel.font = UIFont(name: FontsNames.SFProDisplayBold, size: 25)
        self.searchOrdesSearchBar.text = CommonStrings.searchOrden
        self.searchOrdesSearchBar.backgroundColor = OmicronColors.tableStatus
        self.searchOrdesSearchBar.barTintColor = OmicronColors.tableStatus
        self.view.backgroundColor = OmicronColors.tableStatus
    }
    
    private func getInboxViewModel() -> InboxViewModel? {
        if let vc = self.splitViewController?.viewControllers.first(where: { $0.isKind(of: InboxViewController.self) }) as? InboxViewController {
            return vc.inboxModel
        }
        return nil
    }
}
