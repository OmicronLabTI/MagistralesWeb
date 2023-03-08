//
//  CreateBulkOrderViewController.swift
//  Omicron
//
//  Created by Daniel Velez on 01/03/23.
//  Copyright © 2022 Diego Cárcamo. All rights reserved.
//

import UIKit
import RxSwift
import TagListView
import RxCocoa
import Resolver

class CreateBulkOrderViewController: UIViewController {
    @IBOutlet weak var bulkSearchBar: UISearchBar!
    @IBOutlet weak var tagsView: TagListView!
    @IBOutlet weak var tableView: UITableView!
    @IBOutlet weak var noResultsLabel: UILabel!
    @Injected var bulkOrderViewModel: BulkOrderViewModel
    let disposeBag = DisposeBag()
    override func viewDidLoad() {
        okCreateOrderBinding()
        itemSelectedBinding()
        setUp()
        searchBinding()
    }

    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)
        tableView.reloadData()
    }
    
    func setUp() {
        self.tagsView.delegate = self
        bulkOrderViewModel.dataResults.map({ data -> Bool in
            return data.count > 0
        }).asDriver(onErrorJustReturn: true).drive(noResultsLabel.rx.isHidden).disposed(by: disposeBag)
    }
    
    func searchBinding() {
        self.bulkSearchBar.rx.text.orEmpty.bind(to: self.bulkOrderViewModel.searchBulk).disposed(by: disposeBag)
        self.bulkSearchBar.rx.searchButtonClicked.bind(to: bulkOrderViewModel.searchDidEnter).disposed(by: disposeBag)
        self.bulkOrderViewModel.dataChips.subscribe(onNext: { [weak self] data in
            self?.tagsView.removeAllTags()
            if data.count == 0 {
                return
            }
            self?.tagsView.addTags(data)
            self?.bulkSearchBar.text = ""
        }).disposed(by: disposeBag)
        self.bulkOrderViewModel.dataResults.bind(to: tableView.rx.items(
            cellIdentifier: ViewControllerIdentifiers.bulkTableViewCell,
            cellType: BulkOrderTableViewCell.self)) { _, data, cell in
                cell.productCodeLabel.text = data.productoId
                cell.descriptionLabel.text = data.largeDescription?.uppercased()
        }.disposed(by: disposeBag)
    }
    
    func okCreateOrderBinding(){
        self.bulkOrderViewModel.okCreateOrder.subscribe(onNext: { [weak self] message in
            guard let self = self else { return }
            let alert = UIAlertController(
                title: message,
                message: nil,
                preferredStyle: .alert)
            let okAction = UIAlertAction(title: CommonStrings.OKConst, style: .default, handler: nil)
            alert.addAction(okAction)
            self.present(alert, animated: true, completion: nil)
            if(message == CommonStrings.okCreateBulkOrder) {
                DispatchQueue.main.asyncAfter(deadline: .now() + .milliseconds(3000)) {
                    self.view.window?.rootViewController?.dismiss(animated: true)
                }
            }
        }).disposed(by: disposeBag)
    }
    private func itemSelectedBinding() {
        self.tableView.rx.modelSelected(BulkProduct.self).subscribe(onNext: { [weak self] data in
            guard let self = self else { return }
            let alert = UIAlertController(
                title: "¿Desea crear la orden de fabricación de granel \(data.productoId ?? "")?",
                message: nil,
                preferredStyle: .alert)
            let cancelAction = UIAlertAction(title: CommonStrings.cancel, style: .destructive, handler: nil)
            let okAction = UIAlertAction(title: CommonStrings.OKConst, style: .default, handler: { [weak self] _ in
                guard let self = self else { return }
                self.bulkOrderViewModel.createBulkOrder(data)
            })
            alert.addAction(cancelAction)
            alert.addAction(okAction)
            self.present(alert, animated: true, completion: nil)
        }).disposed(by: disposeBag)
    }
    

    @IBAction func cancelAction(_ sender: Any) {
        dismiss(animated: true, completion: nil)
    }
}

extension CreateBulkOrderViewController: TagListViewDelegate {
    func tagRemoveButtonPressed(_ title: String, tagView: TagView, sender: TagListView) {
        self.bulkOrderViewModel.removeChip.onNext(title)
    }
}
