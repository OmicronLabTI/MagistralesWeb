//
//  SupplieViewController.swift
//  Omicron
//
//  Created by Daniel Vargas on 28/02/23.
//  Copyright © 2023 Diego Cárcamo. All rights reserved.
//

import UIKit
import RxSwift
import RxCocoa
import Resolver

class SupplieViewController: UIViewController {
    let disposeBag = DisposeBag()
    @IBOutlet weak var deleteComponents: UIButton!
    @IBOutlet weak var addComponent: UIButton!
    @IBOutlet weak var sendToStore: UIButton!
    @IBOutlet weak var showComponents: UIButton!
    @IBOutlet weak var showObservations: UIButton!
    @IBOutlet weak var componentsView: UIView!
    @IBOutlet weak var observationsView: UIView!
    @IBOutlet weak var tableComponents: UITableView!
    
    @Injected var supplieViewModel: SupplieViewModel

    var supplieList: [ComponentO] = []

    override func viewDidLoad() {
        super.viewDidLoad()
        setupUI()
        bindTableData()
        bindDeleteButton()
        bindAlertDialog()
        initNavigationBar()
    }
    func initNavigationBar() {
        self.navigationItem.leftBarButtonItem = UIBarButtonItem(title: "< Atrás", style: .plain, target: self, action: #selector(backBtnAction(_:)))
    }

    @objc func backBtnAction(_ sender: UIBarButtonItem) {
        
    }
    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)
    }
    func bindTableData() {
        supplieViewModel.componentsList.subscribe(onNext: { [weak self] list in
            guard let self = self else { return }
            self.supplieList = list
            self.disableSendToStore()
        }).disposed(by: disposeBag)

        supplieViewModel
            .componentsList
            .bind(to: tableComponents.rx.items(
                cellIdentifier: ViewControllerIdentifiers.supplieTableViewCell,
                cellType: SupplieTableViewCell.self
            )) { index, supplie, cell in
                cell.idLabel.text = String(self.supplieList.count - index)
                cell.codeLabel.text = supplie.productId
                cell.descriptionLabel.text = supplie.description
                cell.quantityTextField.text = String(0)
                cell.storeDestinationLabel.text = supplie.warehouse
                cell.unityLabel.text = supplie.unit
                cell.index = index
                cell.supplie = supplie
            }
            .disposed(by: disposeBag)
    }

    func bindDeleteButton() {
        supplieViewModel.selectedButtonIsEnable.subscribe(onNext: {[weak self] isEnabled in
            self?.deleteComponents.isEnabled = isEnabled
        }).disposed(by: disposeBag)
    }

    func setupUI() {
        tableComponents.delegate = self
        tableComponents.dataSource = self
        tableComponents.rowHeight = UITableView.automaticDimension
        sendToStore.isEnabled = false
        deleteComponents.isEnabled = false
        UtilsManager.shared.setStyleButtonStatus(button: self.deleteComponents,
                                                 title: StatusNameConstants.deleteMultiComponents,
                                                 color: OmicronColors.primaryBlue,
                                                 titleColor: OmicronColors.primaryBlue)

        UtilsManager.shared.setStyleButtonStatus(button: self.addComponent,
                                                 title: StatusNameConstants.addComponent,
                                                 color: OmicronColors.primaryBlue,
                                                 titleColor: OmicronColors.primaryBlue)
        setBlueButtonStyle()
        changeView(false)
    }

    func setBlueButtonStyle() {
        sendToStore.setTitle(StatusNameConstants.sendToStore, for: .normal)
        sendToStore.setTitleColor(UIColor.white, for: .normal)
        sendToStore.setTitleColor(UIColor.black, for: .disabled)
        sendToStore.layer.borderWidth = 1
        sendToStore.layer.cornerRadius = 10
        sendToStore.titleLabel?.font = UIFont(name: FontsNames.SFProDisplayBold, size: 16)
        changeBGButton(button: sendToStore,
                       backgroundColor: OmicronColors.disabledButton)
    }
    func changeBGButton(button: UIButton, backgroundColor: UIColor) {
        button.layer.borderColor = backgroundColor.cgColor
        button.backgroundColor = backgroundColor
    }
    @IBAction func showComponents(_ sender: Any) {
        changeView(false)
    }
    @IBAction func showObservations(_ sender: Any) {
        changeView(true)
    }
    @IBAction func deleteComponents(_ sender: Any) {
        supplieViewModel.deleteSelectedComponents()
    }
    func changeView(_ isComponents: Bool) {
        componentsView.isHidden = isComponents
        observationsView.isHidden = !isComponents
        showComponents.tintColor = isComponents ? UIColor.black: OmicronColors.primaryBlue
        showObservations.tintColor = !isComponents ? UIColor.black: OmicronColors.primaryBlue
    }

    @IBAction func openComponentsViewController(_ sender: Any) {
        let storyboard = UIStoryboard(name: ViewControllerIdentifiers.storieboardName, bundle: nil)
        let componentsVC = storyboard.instantiateViewController(
            withIdentifier: ViewControllerIdentifiers.componentsViewController) as? ComponentsViewController
        componentsVC?.typeOpen = .supplies
        let navigationVC = UINavigationController(rootViewController: componentsVC ?? ComponentsViewController())
        navigationVC.modalPresentationStyle = .formSheet
        self.present(navigationVC, animated: true, completion: nil)
    }

    func disableSendToStore() {
        sendToStore.isEnabled = supplieList.allSatisfy { $0.baseQuantity != 0 } && supplieList.count > 0
        let color = sendToStore.isEnabled ? OmicronColors.primaryBlue : OmicronColors.disabledButton
        changeBGButton(button: sendToStore, backgroundColor: color)
    }
    func bindAlertDialog() {
        supplieViewModel.showSuccessAlert.subscribe(onNext: { [weak self] msg in
            AlertManager.shared.showAlert(title: "", message: msg, actions: nil, view: self)
        }).disposed(by: disposeBag)
    }
}
