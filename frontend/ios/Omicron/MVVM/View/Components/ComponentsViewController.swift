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
    @IBOutlet weak var noResultsLabel: UILabel!
    @IBOutlet weak var mostCommontTableView: UITableView!
    @IBOutlet weak var heightMostCommonTableConstraint: NSLayoutConstraint!
    @Injected var componentsViewModel: ComponentsViewModel
    @Injected var supplieViewModel: SupplieViewModel
    @Injected var bulkOrderViewModel: BulkOrderViewModel
    var isLoading = false
    var typeOpen = TypeComponentsOpenDialog.detailOrder
    var disposeBag = DisposeBag()
    var delegate: ComponentsDelegate?
    var warehouses: [String] = []

    override func viewDidLoad() {
        super.viewDidLoad()
        initComponents()
        if typeOpen == .bulkOrder {
            viewModelBindingBulkOrders()
        } else {
            viewModelBinding()
        }
        viewModelBindingCommon()
        hideMostCommonComponents()
        bindingDataToMostComoonTable()
        let typeReq = getMostCommonTypeRequest()
        componentsViewModel.getMostCommonComponentsService(type: typeReq)
    }
    func clearObservables() {
        componentsViewModel.clearObservables()
    }
    func getMostCommonTypeRequest() -> String {
        let dictTypes = [
            TypeComponentsOpenDialog.supplies: TypeMostCommonRequest.inputRequest.rawValue,
            TypeComponentsOpenDialog.detailOrder: TypeMostCommonRequest.detailOrder.rawValue,
            TypeComponentsOpenDialog.bulkOrder: TypeMostCommonRequest.bulkOrder.rawValue,
        ]
        
        return dictTypes[self.typeOpen] ?? String()
    }
    func viewModelBindingCommon() {
        componentsViewModel.bindingData.map({ data -> Bool in
            return data.count > 0
        }).asDriver(onErrorJustReturn: true).drive(noResultsLabel.rx.isHidden).disposed(by: disposeBag)
        self.componentsViewModel.loading.subscribe(onNext: { loading in
            self.isLoading = loading
            if loading {
                LottieManager.shared.showLoading()
                return
            }
            LottieManager.shared.hideLoading()
        }).disposed(by: disposeBag)
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
        // se refresca la tabla despues del filtrado con chips y la respuesta del servicio
        self.componentsViewModel.dataResults.bind(to: tableView.rx.items(
            cellIdentifier: ViewControllerIdentifiers.componentsTableViewCell,
            cellType: ComponentsTableViewCell.self)) { _, data, cell in
                cell.productCodeLabel.text = data.productId
                cell.descriptionLabel.text = data.description?.uppercased()
        }.disposed(by: disposeBag)
        // se selecciona un elemento de la tabla
        self.tableView.rx.modelSelected(ComponentO.self).subscribe(onNext: { [weak self] data in
            self!.continueItemSelected(data)
        }).disposed(by: disposeBag)
        self.componentsViewModel.dataResults.map({ data -> Bool in
            return data.count > 0
        }).asDriver(onErrorJustReturn: true).drive(labelNoResults.rx.isHidden).disposed(by: disposeBag)
        itemSelectedOfMostCommonComponentsTable()
    }

    func continueItemSelected(_ data: ComponentO) {
        switch typeOpen {
        case .detailOrder: self.createFormView(data: data)
        case .supplies: closeSelection(data: data)
        default: break
        }
    }
    func createFormView(data: ComponentO) {
        self.componentsViewModel.selectedComponent.onNext(data)
        let compFormVC = ComponentFormViewController()
        compFormVC.selectedComponent = data
        compFormVC.delegate = self.delegate
        compFormVC.warehouses = self.warehouses
        self.navigationController?.pushViewController(compFormVC, animated: true)
    }
    func closeSelection(data: ComponentO) {
        self.dismiss(animated: false, completion: nil)
        supplieViewModel.addComponent.onNext(data)
    }

    func initComponents() {
        self.componentsViewModel.typeOpen = self.typeOpen
        self.title = self.typeOpen == .bulkOrder ? CommonStrings.createBuildOrder : CommonStrings.addComponentTitle
        self.isModalInPresentation = true
        self.tableView.delegate = self
        self.mostCommontTableView.delegate = self
        self.tagsView.isHidden = true
        self.tagsView.delegate = self
        self.tagsView.tagBackgroundColor = OmicronColors.blue
        self.tagsView.bounds = self.tagsView.bounds.inset(by: UIEdgeInsets(top: 0, left: -10, bottom: 0, right: 20))
        self.heightMostCommonTableConstraint.constant = 200
        self.navigationItem.leftBarButtonItem = UIBarButtonItem(
            title: CommonStrings.cancel, style: .plain, target: self,
            action: #selector(ComponentsViewController.cancelButtonTap(sender:)))
    }
    @objc func cancelButtonTap(sender: UIButton) {
        self.dismiss(animated: true, completion: nil)
    }

    private func hideMostCommonComponents() {
        self.searchBar.rx.textDidBeginEditing.subscribe(onNext: { [weak self] _ in
            self?.heightMostCommonTableConstraint.constant = 0
        }).disposed(by: disposeBag)
    }

    private func bindingDataToMostComoonTable() {
        componentsViewModel.bindingData.bind(
            to: mostCommontTableView.rx.items(
                cellIdentifier: ViewControllerIdentifiers.mostCommonComponentsTableViewCell,
                cellType: MostCommonComponentsTableViewCell.self)) {_, data, cell in
            cell.productCodeLabel.text = data.productId
            cell.descriptionLabel.text = data.description?.uppercased()
        }.disposed(by: disposeBag)
    }

    private func itemSelectedOfMostCommonComponentsTable() {
        self.mostCommontTableView.rx.modelSelected(ComponentO.self).subscribe(onNext: { [weak self] data in
            self!.continueItemSelected(data)
        }).disposed(by: disposeBag)
    }
}
extension ComponentsViewController: TagListViewDelegate {
    func tagRemoveButtonPressed(_ title: String, tagView: TagView, sender: TagListView) {
            self.bulkOrderViewModel.removeChip.onNext(title)
            self.componentsViewModel.removeChip.onNext(title)
    }
}
extension ComponentsViewController: UITableViewDelegate {
    func tableView(_ tableView: UITableView, willDisplay cell: UITableViewCell, forRowAt indexPath: IndexPath) {
        let customView = UIView()
        customView.backgroundColor = OmicronColors.blue
        cell.selectedBackgroundView = customView
        let lastSectionIndex = tableView.numberOfSections - 1
        let lastRowIndex = tableView.numberOfRows(inSection: lastSectionIndex) - 1
        if indexPath.section == lastSectionIndex &&
            indexPath.row == lastRowIndex - 3 &&
            !isLoading &&
            lastRowIndex > 10 {
            tableView.scrollToRow(at: [0, lastRowIndex - 4],
                                  at: .middle,
                                  animated: false)
            componentsViewModel.onScroll.onNext(())
            bulkOrderViewModel.onScroll.onNext(())
        }
        if indexPath.row%2 == 0 {
            cell.backgroundColor = OmicronColors.tableColorRow
        } else {
            cell.backgroundColor = .white
        }
    }
}
