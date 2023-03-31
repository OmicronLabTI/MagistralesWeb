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
    var disposeBag: DisposeBag? = DisposeBag()
    @IBOutlet weak var deleteComponents: UIButton!
    @IBOutlet weak var addComponent: UIButton!
    @IBOutlet weak var sendToStore: UIButton!
    @IBOutlet weak var showComponents: UIButton!
    @IBOutlet weak var showObservations: UIButton!
    @IBOutlet weak var componentsView: UIView!
    @IBOutlet weak var observationsView: UIView!
    @IBOutlet weak var tableComponents: UITableView!
    @IBOutlet weak var observationsField: UITextView!
    @IBOutlet weak var segmentedControl: UISegmentedControl!
    @IBOutlet weak var newSupplie: UIStackView!
    @IBOutlet weak var historySupplie: UIStackView!
    @IBOutlet weak var dateOrderView: UIView!
    @IBOutlet weak var estatusView: UIView!
    @IBOutlet weak var tableHistory: UITableView!
    @IBOutlet weak var noHistoryResults: UIView!
    @IBOutlet weak var statusSelectedsLabel: UILabel!
    @IBOutlet weak var dateRangeSelectedLabel: UILabel!
    
    @Injected var supplieViewModel: SupplieViewModel
    @Injected var lottieManager: LottieManager
    @Injected var historyViewModel: HistoryViewModel
    var supplieList: [Supplie] = []
    var formatter = UtilsManager.shared.formatterDoublesTo6Decimals()
    var isLoading = false
    override func viewDidLoad() {
        super.viewDidLoad()
        bindShowAlert()
        repaintFilters()
        bindinChangeFilters()
        bindSegmentedControl()
        loadStyles()
        resetInfo()
        setupUI()
        bindTableData()
        bindDeleteButton()
        bindAlertDialog()
        initNavigationBar()
        disableSendToStoreBinding()
        bindLoading()
        bindReturnBack()
        segmentedControl.selectedSegmentIndex = 0
        changeSegmentedView(isSupplie: true)
        bindHistoryTable()
        bindIsLoading()
        validateHasInfo()
        historyViewModel.getHistory(offset: 0, limit: historyViewModel.limit)
    }
    override func viewDidDisappear(_ animated: Bool) {
        super.viewDidDisappear(animated)
        disposeBag = nil
    }
    func bindReturnBack() {
        supplieViewModel.returnBack.subscribe(onNext: { [weak self] _ in
            guard let self = self else { return }
            self.observationsField.text = String()
            self.showAlertSuccess(title: "Solicitud enviada exitosamente",
                                  message: String())
            self.resetValues()
            Timer.scheduledTimer(withTimeInterval: TimeInterval(2),
                                 repeats: false,
                                 block: { _ in
                self.returnBack()
            })
        }).disposed(by: disposeBag!)
    }
    func showAlertSuccess(title: String, message: String) {
        AlertManager.shared.showAlertWithoutButtons(title: title,
                                                    message: message,
                                                    view: self,
                                                    dismissTime: 2)
    }
    func initNavigationBar() {
        self.navigationItem.leftBarButtonItem = UIBarButtonItem(title: "< Mis órdenes",
                                                                style: .plain,
                                                                target: self,
                                                                action: #selector(backBtnAction(_:)))
    }
    func resetInfo() {
        observationsField.text = String()
        supplieList = []
        supplieViewModel.supplieList = []
        supplieViewModel.selectedComponentsToDelete = []
        historyViewModel.resetValues()
        repaintFilters()
        tableComponents.dataSource = [] as? any UITableViewDataSource
        tableHistory.dataSource = [] as? any UITableViewDataSource
        tableHistory.delegate = self
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
    @IBAction func openComponentsViewController(_ sender: Any) {
        let storyboard = UIStoryboard(name: ViewControllerIdentifiers.storieboardName, bundle: nil)
        let componentsVC = storyboard.instantiateViewController(
            withIdentifier: ViewControllerIdentifiers.componentsViewController) as? ComponentsViewController
        componentsVC?.typeOpen = .supplies
        componentsVC!.clearObservables()
        let navigationVC = UINavigationController(rootViewController: componentsVC ?? ComponentsViewController())
        navigationVC.modalPresentationStyle = .formSheet
        self.present(navigationVC, animated: true, completion: nil)
    }

    @IBAction func sendToStoreDidPressed(_ sender: Any) {
        let cancelAction = UIAlertAction(title: "Cancelar",
                                         style: .destructive,
                                         handler: nil)
        let okAction = UIAlertAction(title: CommonStrings.OKConst,
                                     style: .default,
                                     handler: { [weak self] _ in
            self?.sendToStoreAction()
        })
        AlertManager.shared.showAlert(title: "¡Atención! \n \(CommonStrings.confirmSendToStore)",
                                      message: String(),
                                      actions: [cancelAction, okAction],
                                      view: self)
    }

    func sendToStoreAction() {
        let observations = observationsField.text == CommonStrings.placeholderObservations ?
            "" :
            observationsField.text ?? ""
        supplieViewModel.sendToStore.onNext(observations)
    }
    @objc func backBtnAction(_ sender: UIBarButtonItem) {
        if supplieList.count == 0 {
            returnBack()
            return
        }
        self.presentConfirmDialog()
    }
    func returnBack() {
        self.navigationController?.popViewController(animated: true)
    }
    func presentConfirmDialog() {
        let cancelAction = UIAlertAction(title: "Cancelar", style: .destructive, handler: nil)
        let okAction = UIAlertAction(title: CommonStrings.OKConst,
                                     style: .default, handler: { [weak self] _ in self?.resetValues()})
        AlertManager.shared.showAlert(title: "¡Atención! \n \(CommonStrings.confirmExit)",
                                      message: String(),
                                      actions: [cancelAction, okAction],
                                      view: self)
    }
    func resetValues() {
        resetInfo()
        returnBack()
    }
    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)
    }
    func bindTableData() {
        supplieViewModel.componentsList.subscribe(onNext: { [weak self] list in
            guard let self = self else { return }
            self.supplieList = list
        }).disposed(by: disposeBag!)
        
        supplieViewModel.componentsList.subscribe(onNext: {[weak self] data in
            self?.supplieList = data
            self?.tableComponents.reloadData()
        }).disposed(by: disposeBag!)
    }

    func bindDeleteButton() {
        supplieViewModel.selectedButtonIsEnable.subscribe(onNext: {[weak self] isEnabled in
            self?.deleteComponents.isEnabled = isEnabled
        }).disposed(by: disposeBag!)
    }

    func setupUI() {
        tableComponents.delegate = self
        tableComponents.dataSource = self
        tableComponents.rowHeight = UITableView.automaticDimension
        tableHistory.delegate = self
        tableHistory.dataSource = self
        tableHistory.rowHeight = UITableView.automaticDimension
        sendToStore.isEnabled = false
        deleteComponents.isEnabled = false
        observationsField.delegate = self
        observationsField.text = CommonStrings.placeholderObservations
        observationsField.textColor = UIColor.lightGray
        UtilsManager.shared.setStyleButtonStatus(button: self.deleteComponents,
                                                 title: StatusNameConstants.deleteMultiComponents,
                                                 color: OmicronColors.processStatus,
                                                 titleColor: OmicronColors.processStatus)

        UtilsManager.shared.setStyleButtonStatus(button: self.addComponent,
                                                 title: StatusNameConstants.addComponent,
                                                 color: OmicronColors.primaryBlue,
                                                 titleColor: OmicronColors.primaryBlue)
        setBlueButtonStyle()
        changeView(false)
        observationsField.layer.borderWidth = 1
        observationsField.layer.cornerRadius = 10
        observationsField.layer.borderColor = OmicronColors.disabledButton.cgColor
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
    func changeView(_ isComponents: Bool) {
        componentsView.isHidden = isComponents
        observationsView.isHidden = !isComponents
        showComponents.tintColor = isComponents ? UIColor.black: OmicronColors.primaryBlue
        showObservations.tintColor = !isComponents ? UIColor.black: OmicronColors.primaryBlue
    }

    func disableSendToStoreBinding() {
        supplieViewModel.isSendToStoreEnabled.subscribe(onNext: {[weak self] isEnabled in
            guard let self = self else { return }
            self.sendToStore.isEnabled = isEnabled
            let color = self.sendToStore.isEnabled ? OmicronColors.primaryBlue : OmicronColors.disabledButton
                self.changeBGButton(button: self.sendToStore, backgroundColor: color)
        }).disposed(by: disposeBag!)
    }
    func bindAlertDialog() {
        supplieViewModel.showSuccessAlert.subscribe(onNext: { [weak self] alert in
            guard let self = self else { return }
            self.showAlert(alert: alert)
        }).disposed(by: disposeBag!)
    }
    func showAlert(alert: (title: String, msg: String, autoDismiss: Bool)) {
        AlertManager.shared.showAlert(title: alert.title,
                                      message: alert.msg,
                                      actions: nil,
                                      view: self,
                                      autoDismiss: alert.autoDismiss,
                                      dismissTime: 2)
    }
    func bindLoading() {
        self.supplieViewModel.loading.observeOn(MainScheduler.instance).subscribe(onNext: {loading in
            if loading {
                self.lottieManager.showLoading()
                return
            }
            self.lottieManager.hideLoading()
        }).disposed(by: disposeBag!)
    }
}
