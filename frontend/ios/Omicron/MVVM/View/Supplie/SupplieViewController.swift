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
    }

    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)
    }
    func bindTableData() {
        supplieViewModel
            .componentsList
            .bind(to: tableComponents.rx.items(
                cellIdentifier: ViewControllerIdentifiers.supplieTableViewCell,
                cellType: SupplieTableViewCell.self
            )) { index, supplie, cell in
                cell.idLabel.text = String(index)
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

    func setupUI() {
        tableComponents.delegate = self
        tableComponents.dataSource = self
        UtilsManager.shared.setStyleButtonStatus(button: self.deleteComponents,
                                                 title: StatusNameConstants.deleteMultiComponents,
                                                 color: OmicronColors.primaryBlue,
                                                 titleColor: OmicronColors.primaryBlue)

        UtilsManager.shared.setStyleButtonStatus(button: self.addComponent,
                                                 title: StatusNameConstants.addComponent,
                                                 color: OmicronColors.primaryBlue,
                                                 titleColor: OmicronColors.primaryBlue)

        UtilsManager.shared.setStyleButtonStatus(button: self.sendToStore,
                                                 title: StatusNameConstants.sendToStore,
                                                 color: OmicronColors.primaryBlue,
                                                 backgroudColor: OmicronColors.primaryBlue,
                                                 titleColor: UIColor.white)
    }

    @IBAction func showComponents(_ sender: Any) {
        changeView(false)
    }
    @IBAction func showObservations(_ sender: Any) {
        changeView(true)
    }
    func changeView(_ isComponents: Bool) {
        componentsView.isHidden = isComponents
        observationsView.isHidden = !isComponents
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

}
