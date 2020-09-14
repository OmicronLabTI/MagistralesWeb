//
//  ComponentsViewController.swift
//  Omicron
//
//  Created by Diego Cárcamo on 09/09/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit
import TagListView
import RxCocoa
import RxSwift
import Resolver

class ComponentsViewController: UIViewController {
    @IBOutlet weak var searchBar: UISearchBar!
    @IBOutlet weak var tagsView: TagListView!
    @IBOutlet weak var tableView: UITableView!
    @IBOutlet weak var labelNoResults: UILabel!
    
    @Injected var componentsViewModel: ComponentsViewModel
    
    var disposeBag = DisposeBag()
    
    override func viewDidLoad() {
        super.viewDidLoad()
        
        initComponents()
        viewModelBinding()
    }
    
    func viewModelBinding() {
        self.searchBar.rx.text.orEmpty.bind(to: componentsViewModel.searchFilter).disposed(by: disposeBag)
        self.searchBar.rx.searchButtonClicked.bind(to: componentsViewModel.searchDidTap).disposed(by: disposeBag)
        self.componentsViewModel.dataChips.map({ data -> Bool in
            return data.count == 0
        }).asDriver(onErrorJustReturn: false).drive(self.tagsView.rx.isHidden).disposed(by: disposeBag)

        
        self.componentsViewModel.dataChips.subscribe(onNext: { [weak self] data in
            if data.count == 0 {
                return
            }
            self?.tagsView.removeAllTags()
            self?.tagsView.addTags(data)
            self?.searchBar.text = ""
        }).disposed(by: disposeBag)
        
        self.componentsViewModel.dataError.subscribe(onNext: { [weak self] err in
            AlertManager.shared.showAlert(
                title: Constants.Errors.errorTitle.rawValue,
                message: err,
                actions: nil,
                view: self)
        }).disposed(by: disposeBag)
        
        self.componentsViewModel.dataResults.bind(to: tableView.rx.items(cellIdentifier: ViewControllerIdentifiers.componentsTableViewCell)) { row, data, cell in
            cell.textLabel?.text = data.description
        }.disposed(by: disposeBag)
        
        self.tableView.rx.modelSelected(ComponentO.self).subscribe(onNext: { [weak self] data in
            self?.componentsViewModel.selectedComponent.onNext(data)
            let compFormVC = ComponentFormViewController()
            self?.navigationController?.pushViewController(compFormVC, animated: true)
        }).disposed(by: disposeBag)
        
        self.componentsViewModel.loading.subscribe(onNext: {loading in
            if loading {
                LottieManager.shared.showLoading()
                return
            }
            LottieManager.shared.hideLoading()
        }).disposed(by: disposeBag)
        
        self.componentsViewModel.dataResults.map({ data -> Bool in
            return data.count > 0
        }).asDriver(onErrorJustReturn: true).drive(labelNoResults.rx.isHidden).disposed(by: disposeBag)
    }
    
    func initComponents() {
        self.title = CommonStrings.addComponentTitle
        self.isModalInPresentation = true
        
        self.tagsView.isHidden = true
        self.tagsView.delegate = self
        self.tagsView.tagBackgroundColor = OmicronColors.blue
        self.tagsView.bounds = self.tagsView.bounds.inset(by: UIEdgeInsets(top: 0, left: -10, bottom: 0, right: 20))
        
        self.navigationItem.leftBarButtonItem = UIBarButtonItem(title: CommonStrings.cancel, style: .plain, target: self, action: #selector(ComponentsViewController.cancelButtonTap(sender:)))
    }
    
    @objc func cancelButtonTap(sender: UIButton) {
        self.dismiss(animated: true, completion: nil)
    }
}

extension ComponentsViewController: TagListViewDelegate {
    func tagRemoveButtonPressed(_ title: String, tagView: TagView, sender: TagListView) -> Void {
        self.componentsViewModel.removeChip.onNext(title)
    }
}
