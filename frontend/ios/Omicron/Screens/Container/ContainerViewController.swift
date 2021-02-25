//
//  ContainerViewController.swift
//  Omicron
//
//  Created by Vicente Cantu Garcia on 19/02/21.
//  Copyright © 2021 Diego Cárcamo. All rights reserved.
//

import UIKit
import RxSwift
import RxCocoa
import Resolver

class ContainerViewController: UIViewController {

    @IBOutlet weak var tableView: UITableView!

    @Injected var containerViewModel: ContainerViewModel
    @Injected var lottieManager: LottieManager

    let disposeBag = DisposeBag()
    var firstTime = true
    let formatter = UtilsManager.shared.formatterDoublesTo6Decimals()

    override func viewDidLoad() {
        super.viewDidLoad()
        viewModeBinding()
        isModalInPresentation = true
    }

    override func viewDidAppear(_ animated: Bool) {
        if firstTime {
              firstTime.toggle()
              tableViewBinding()
        }
        containerViewModel.getContainerData()
    }

    func viewModeBinding() {

        containerViewModel
            .loading
            .observeOn(MainScheduler.instance)
            .subscribe(onNext: { [weak self] loadingResponse in
                if loadingResponse {
                    self?.lottieManager.showLoading()
                } else {
                    self?.lottieManager.hideLoading()
                }
            })
            .disposed(by: disposeBag)

    }

    func tableViewBinding() {

        containerViewModel
            .containerData
            .bind(to: tableView.rx.items(
                cellIdentifier: "container_cell",
                cellType: ContainerTableViewCell.self
            )) { _, data, cell in
                cell.containerLabel.text = ("Código: \(data.codeItem ?? "")")
                cell.quantityLabel.text = data.unit == CommonStrings.piece ?
                                                String(format: "%.0f", data.quantity ?? 0.0) :
                                                self.formatter.string(from: NSNumber(value: data.quantity ?? 0.0))
                cell.unitLabel.text = data.unit
                cell.descriptionLabel.text = data.description
            }
            .disposed(by: disposeBag)

    }

    @IBAction func acceptDidPressed(_ sender: Any) {
        dismiss(animated: true, completion: nil)
    }

}
