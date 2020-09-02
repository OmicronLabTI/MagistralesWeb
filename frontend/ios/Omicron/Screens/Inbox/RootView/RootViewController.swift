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
    var dataStatusOfService: [SectionOrder] = []
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
        self.rootViewModel.goToLoginViewController.observeOn(MainScheduler.instance).subscribe(onNext: {_ in
            let storyboard = UIStoryboard(name: ViewControllerIdentifiers.storieboardName, bundle: nil)
            let loginViewController = storyboard.instantiateViewController(identifier: ViewControllerIdentifiers.loginViewController) as! LoginViewController
            UIApplication.shared.windows.first?.rootViewController = loginViewController
            UIApplication.shared.windows.first?.makeKeyAndVisible()
        }).disposed(by: self.disposeBag)
        
        rootViewModel.dataStatus.observeOn(MainScheduler.instance).subscribe(onNext: { data in
            self.dataStatusOfService = data
        }).disposed(by: disposeBag)
        
        rootViewModel.refreshSelection.subscribe(onNext: {
            if (self.dataStatusOfService.count > 0) {
                self.viewTable.selectRow(at: IndexPath(row: self.rootViewModel.selectedRow, section: 0), animated: true, scrollPosition: .none)
                self.inboxViewModel.setSelection(index: self.rootViewModel.selectedRow, section: self.dataStatusOfService[self.rootViewModel.selectedRow])
            }
        }).disposed(by: self.disposeBag)
        
        // Muestra los datos de la sección "Mis ordenes"
        rootViewModel.dataStatus.bind(to: viewTable.rx.items(cellIdentifier: ViewControllerIdentifiers.rootTableViewCell, cellType: RootTableViewCell.self)) {
            row, data, cell in
            cell.indicatorStatusImageView.image = UIImage(named: data.imageIndicatorStatus)
            cell.indicatorStatusNameLabel.text = data.statusName
            cell.indicatorStatusNumber.text = String(data.numberTask)
        }.disposed(by: disposeBag)
        
        rootViewModel.loading.observeOn(MainScheduler.instance).subscribe(onNext: { loadingResponse in
            if (loadingResponse) {
                self.lottieManager.showLoading()
            } else {
                self.lottieManager.hideLoading()
            }
        }).disposed(by: self.disposeBag)
        
        rootViewModel.error.observeOn(MainScheduler.instance).subscribe(onNext: {error in
            AlertManager.shared.showAlert(message: error, view: self)
        }).disposed(by: self.disposeBag)
        
        // Detecta el evento cuando se selecciona un status de la tabla
        viewTable.rx.itemSelected.subscribe( onNext: { indexPath in
            self.rootViewModel.selectedRow = indexPath.row
            self.inboxViewModel.setSelection(index: indexPath.row, section: self.dataStatusOfService[indexPath.row])
        }).disposed(by: disposeBag)
        
        // Muestra u oculta el refreshControl en la tabla
        rootViewModel.showRefreshControl.observeOn(MainScheduler.instance).subscribe(onNext: { _ in
            self.refreshControl.endRefreshing()
        }).disposed(by: self.disposeBag)
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
       
        self.refreshControl.tintColor = UIColor(red:0.25, green:0.72, blue:0.85, alpha:1.0)
        self.refreshControl.attributedTitle = NSAttributedString(string: "Actualizando datos")
       
        self.logoutButton.setTitle("Cerrar sesión", for: .normal)
        self.logoutButton.tintColor = .darkGray
        self.logoutButton.setImage(UIImage(named: ImageButtonNames.logout), for: .normal)
        self.logoutButton.imageEdgeInsets = UIEdgeInsets(top: 15, left: 15, bottom: 15, right: 260)
        self.logoutButton.titleEdgeInsets.left = 35
        self.logoutButton.titleLabel?.font = UIFont(name: FontsNames.SFProDisplayMedium, size: 17)
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
