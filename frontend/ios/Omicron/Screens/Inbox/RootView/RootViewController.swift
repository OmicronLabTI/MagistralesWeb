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
    var dataStatusOfService: [SectionOrder] = []
    lazy var inboxViewModel = self.getInboxViewModel()
    // MARK: Life Cycles
    override func viewDidLoad() {
        super.viewDidLoad()
        self.title = getUserInfo()
        self.initComponents()
        self.viewModelBinding()
            }
    
    override func viewWillAppear(_ animated: Bool) {
        super.viewWillAppear(true)
        self.rootViewModel.getOrders()

    }
    
    // MARK: Functions
    func viewModelBinding() {
        
        rootViewModel.dataStatus.observeOn(MainScheduler.instance).subscribe(onNext: { data in
            self.dataStatusOfService = data
        }).disposed(by: disposeBag)
        
        // Muestra los datos de la sección "Mis ordenes"
        rootViewModel.dataStatus.bind(to: viewTable.rx.items(cellIdentifier: ViewControllerIdentifiers.rootTableViewCell, cellType: RootTableViewCell.self)) {
            row, data, cell in
            cell.indicatorStatusImageView.image = UIImage(named: data.imageIndicatorStatus)
            cell.indicatorStatusNameLabel.text = data.statusName
            cell.indicatorStatusNumber.text = String(data.numberTask)
        }.disposed(by: disposeBag)
        
        rootViewModel.loading.observeOn(MainScheduler.instance).subscribe(onNext: { loadingResponse in
            if (loadingResponse) {
                LottieManager.shared.showLoading()
            } else {
                LottieManager.shared.hideLoading()
            }
        }).disposed(by: self.disposeBag)
        
        rootViewModel.error.observeOn(MainScheduler.instance).subscribe(onNext: {error in
            AlertManager.shared.showAlert(message: error, view: self)
        }).disposed(by: self.disposeBag)
        
        // Detecta el evento cuando se selecciona un status de la tabla
//        let index = NSIndexPath(row: 0, section: 0)
//        viewTable.selectRow(at: index as IndexPath, animated: true, scrollPosition: .middle)
        viewTable.rx.itemSelected.subscribe( onNext: { indexPath in
            self.inboxViewModel?.setSelection(index: indexPath.row, section: self.dataStatusOfService[indexPath.row])
        }).disposed(by: disposeBag)
        
        
    }
    
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
        let childrenVC = self.splitViewController?.viewControllers.map({
            return (($0 as? UINavigationController)?.viewControllers ?? [])
        }).reduce([], +)
        
        if let vc = childrenVC?.first(where: { $0.isKind(of: InboxViewController.self) }) as? InboxViewController {
            return vc.inboxViewModel
        }
        
        return nil
    }
    
    private func getUserInfo() -> String {
        guard let userInfo =  Persistence.shared.getUserData() else { return "" }
        return "\(userInfo.firstName!) \(userInfo.lastName!)"
    }
}
