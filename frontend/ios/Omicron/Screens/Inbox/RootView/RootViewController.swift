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
import Resolver

class RootViewController: UIViewController {
    
    // MARK: Outlets
    @IBOutlet weak var viewTable: UITableView!
    @IBOutlet weak var myOrdesLabel: UILabel!
    @IBOutlet weak var searchOrdesSearchBar: UISearchBar!
    @IBOutlet weak var logoutButton: UIButton!
    
    // Variables
    let disposeBag = DisposeBag()
    @Injected var rootViewModel: RootViewModel
    @Injected var inboxViewModel: InboxViewModel
    @Injected var lottieManager: LottieManager
    
    var refreshControl = UIRefreshControl()
    
    // MARK: Life Cycles
    override func viewDidLoad() {
        super.viewDidLoad()
        self.initComponents()
        self.viewModelBinding()
        self.viewTable.refreshControl = refreshControl
        self.setTitleCustom()
        // Configure Refresh Control
        self.refreshControl.addTarget(self, action: #selector(self.refreshOrders), for: .valueChanged)
    }
    
    override func viewWillAppear(_ animated: Bool) {
        super.viewWillAppear(true)
        self.rootViewModel.getOrders()
    }
    
    override func viewWillDisappear(_ animated: Bool) {
        super.viewWillDisappear(animated)
        self.searchOrdesSearchBar.text = ""
        self.rootViewModel.resetFilter()
    }
    
    // MARK: Functions
    @objc func refreshOrders() -> Void {
        self.rootViewModel.getOrders(isUpdate: true)
    }
    
    // Pone el título del del usuario logeado
    func setTitleCustom() -> Void {
        let navLabel = UILabel(frame: (self.navigationController?.navigationBar.frame)!)
        navLabel.numberOfLines = 0
        let navTitle = NSMutableAttributedString(string: "Hola\n", attributes:[
            NSAttributedString.Key.font: UIFont.systemFont(ofSize: 10.0)
        ])

        navTitle.append(NSMutableAttributedString(string: getUserInfo(), attributes:[
            NSAttributedString.Key.font: UIFont.boldSystemFont(ofSize: 17.0)
        ]))

        navLabel.attributedText = navTitle
        self.navigationItem.titleView = navLabel
    }
    
    func viewModelBinding() {
        
        self.logoutButton.rx.tap.bind(to: rootViewModel.logoutDidTap).disposed(by: self.disposeBag)
        
        // Cuando se presiona el botón de cerrar sesión  se redirije a Login
        self.rootViewModel.goToLoginViewController.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self]_ in
            let storyboard = UIStoryboard(name: ViewControllerIdentifiers.storieboardName, bundle: nil)
            let loginViewController = storyboard.instantiateViewController(identifier: ViewControllerIdentifiers.loginViewController) as! LoginViewController
            UIApplication.shared.windows.first?.rootViewController = loginViewController
            UIApplication.shared.windows.first?.makeKeyAndVisible()
            Resolver.cached.reset()
        }).disposed(by: self.disposeBag)
        
        // Muestra los datos de la sección "Mis ordenes"
        rootViewModel.dataStatus.bind(to: viewTable.rx.items(cellIdentifier: ViewControllerIdentifiers.rootTableViewCell, cellType: RootTableViewCell.self)) { 
            row, data, cell in
            cell.indicatorStatusImageView.image = UIImage(named: data.imageIndicatorStatus)
            cell.indicatorStatusNameLabel.text = data.statusName
            cell.indicatorStatusNumber.text = String(data.numberTask)
        }.disposed(by: disposeBag)
        
        rootViewModel.loading.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] loadingResponse in
            if (loadingResponse) {
                self?.lottieManager.showLoading()
            } else {
                self?.lottieManager.hideLoading()
            }
        }).disposed(by: self.disposeBag)
        
        rootViewModel.error.observeOn(MainScheduler.instance).subscribe(onNext: { [unowned self] error in
            AlertManager.shared.showAlert(message: error, view: self)
        }).disposed(by: self.disposeBag)
        
        // Detecta el evento cuando se selecciona un status de la tabla
        viewTable.rx.modelSelected(SectionOrder.self).subscribe(onNext: { [weak self] data in
            self?.inboxViewModel.setSelection(section: data)
        }).disposed(by: disposeBag)

        viewTable.rx.itemSelected.bind(to: self.rootViewModel.selectedRow).disposed(by: disposeBag)
        
        //Selecciona el primer elemento de estatus cuando termina la carga de datos
        self.rootViewModel.refreshSelection.withLatestFrom(self.rootViewModel.selectedRow).subscribe(onNext: { [weak self] row in
            let data = self?.rootViewModel.sections ?? []
            if data.count > 0, let selectedRow = row {
                let section = data[selectedRow.row]
                self?.viewTable.selectRow(at: selectedRow, animated: false, scrollPosition: .none)
                self?.inboxViewModel.setSelection(section: section)
            } else if data.count > 0 {
                let firstRow = IndexPath(row: 0, section: 0)
                let section = data[firstRow.row]
                self?.viewTable.selectRow(at: firstRow, animated: false, scrollPosition: .none)
                self?.inboxViewModel.setSelection(section: section)
            }
        }).disposed(by: disposeBag)
        
        // Muestra u oculta el refreshControl en la tabla
        rootViewModel.showRefreshControl.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] _ in
            self?.refreshControl.endRefreshing()
        }).disposed(by: self.disposeBag)
        
        // Búsqueda de órdenes
        self.searchOrdesSearchBar.rx.text.orEmpty.bind(to: self.rootViewModel.searchFilter).disposed(by: disposeBag)
        
        self.rootViewModel.dataFilter.withLatestFrom(self.rootViewModel.selectedRow, resultSelector: { [weak self] data, lastRow in
            let selection = lastRow ?? IndexPath(row: 0, section: 0)
            if data == nil {
                self?.viewTable.alpha = 1.0
                self?.viewTable.isUserInteractionEnabled = true
                guard let section = self?.rootViewModel.sections[selection.row] else { return }
                self?.viewTable.selectRow(at: selection, animated: false, scrollPosition: .none)
                self?.inboxViewModel.setSelection(section: section)
                self?.inboxViewModel.hideGroupingButtons.onNext(false)
                return
            }
            self?.viewTable.alpha = 0.25
            self?.viewTable.isUserInteractionEnabled = false
            self?.viewTable.deselectRow(at: selection, animated: false)
            self?.inboxViewModel.setFilter(orders: data ?? [])
            self?.inboxViewModel.hideGroupingButtons.onNext(true)
        }).subscribe().disposed(by: disposeBag)
    }
    
    func initComponents() {
        self.viewTable.tableFooterView = UIView()
        self.viewTable.backgroundColor = OmicronColors.tableStatus
        
        self.myOrdesLabel.backgroundColor = OmicronColors.tableStatus
        self.myOrdesLabel.font = UIFont(name: FontsNames.SFProDisplayBold, size: 25)
        
        self.searchOrdesSearchBar.placeholder = CommonStrings.searchOrden
        self.searchOrdesSearchBar.backgroundColor = OmicronColors.tableStatus
        self.searchOrdesSearchBar.barTintColor = OmicronColors.tableStatus
        
        self.view.backgroundColor = OmicronColors.tableStatus
       
        self.refreshControl.tintColor = OmicronColors.blue
        self.refreshControl.attributedTitle = NSAttributedString(string: "Actualizando datos")
       
        self.logoutButton.setTitle("Cerrar sesión", for: .normal)
        self.logoutButton.tintColor = .darkGray
        self.logoutButton.setImage(UIImage(named: ImageButtonNames.logout), for: .normal)
        self.logoutButton.imageView?.contentMode = .scaleAspectFit
        self.logoutButton.imageEdgeInsets = UIEdgeInsets(top: 15, left: 15, bottom: 15, right: 260)
        self.logoutButton.titleEdgeInsets.left = 35
        self.logoutButton.titleLabel?.font = UIFont(name: FontsNames.SFProDisplayMedium, size: 17)
    }
    
    private func getUserInfo() -> String {
        guard let userInfo =  Persistence.shared.getUserData() else { return "" }
        return "\(userInfo.firstName!) \(userInfo.lastName!)"
    }
}
