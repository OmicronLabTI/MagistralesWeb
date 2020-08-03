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
    
    // MARK: Life Cycles
    override func viewDidLoad() {
        super.viewDidLoad()
        initComponents()
        self.title = "Sergio Flores"
        
        rootViewModel.arrayData.bind(to: viewTable.rx.items(cellIdentifier: ViewControllerIdentifiers.rootTableViewCell, cellType:  RootTableViewCell.self)) { _, data, cell in
            cell.indicatorStatusImageView.image = UIImage(named: data.imageIndicatorStatus)
            cell.indicatorStatusNameLabel.text = data.statusName
            cell.indicatorStatusNumber.text = String(data.numberTask)
        }.disposed(by: disposeBag)
        
        let index = NSIndexPath(row: 0, section: 0)
        viewTable.selectRow(at: index as IndexPath, animated: true, scrollPosition: .middle)
        viewTable.rx.itemSelected.subscribe( onNext: { [weak self] indexPath in
            self?.inboxViewModel?.setSelection(index: indexPath.row)
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
