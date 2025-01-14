//
//  ExtensionInboxViewController.swift
//  Omicron
//
//  Created by Sergio Flores Ramirez on 18/02/21.
//  Copyright © 2021 Diego Cárcamo. All rights reserved.
//

import UIKit
import RxSwift

extension InboxViewController {

    func modelBindingGrouped() {
        // Habilita o deshabilita el botón de agrupamiento por similaridad
        inboxViewModel.similarityViewButtonIsEnable.subscribe(onNext: { [weak self] isEnabled in
            guard let self = self else { return }
            self.similarityViewButton.isEnabled = isEnabled
            self.showMoreIndicators()
            self.goToTop()
        }).disposed(by: self.disposeBag)
        // Habilita o deshabilita el botón de agrupamiento por vista normal
        inboxViewModel.normalViewButtonIsEnable.subscribe(onNext: { [weak self] isEnabled in
            guard let self = self else { return }
            self.normalViewButton.isEnabled = isEnabled
            self.heigthCollectionViewConstraint.constant = 8
            self.showMoreIndicators()
            self.goToTop()
        }).disposed(by: self.disposeBag)
        // Habilita o deshabilita el botón de agrupamiento por número de orden
        inboxViewModel.groupedByOrderNumberIsEnable.subscribe(onNext: { [weak self] isEnabled in
            guard let self = self else { return }
            self.groupByOrderNumberButton.isEnabled = isEnabled
            self.showMoreIndicators()
            self.goToTop()
        }).disposed(by: self.disposeBag)
        inboxViewModel.groupedByShopTransactionIsEnable.subscribe(onNext: { [weak self] isEnabled in
            guard let self = self else { return }
            self.groupByShopTransactionButton.isEnabled = isEnabled
            self.showMoreIndicators()
            self.goToTop()
        }).disposed(by: self.disposeBag)
        self.modelBindindGrouped2()
    }

    func modelBindindGrouped2() {
        // Oculta o muestra los botones de agrupamiento cuando se se realiza una búsqueda
        inboxViewModel.hideGroupingButtons.subscribe(onNext: { [weak self] isHidden in
            guard let self = self else { return }
            self.similarityViewButton.isHidden = isHidden
            self.normalViewButton.isHidden = isHidden
            self.groupByOrderNumberButton.isHidden = isHidden
            self.groupByShopTransactionButton.isHidden = isHidden
            self.showMoreIndicators()
            self.goToTop()
        }).disposed(by: self.disposeBag)
        // retorna mensaje si no hay card para cada status
        inboxViewModel.title
            .withLatestFrom(inboxViewModel.statusDataGrouped, resultSelector: { [weak self] title, _ in
                guard let self = self else { return }
                _ = self.inboxViewModel.getStatusId(name: title)
                let message = self.inboxViewModel.ordersTemp.count == 0 ?
                    "No tienes órdenes \(title)" :
                    String()
                self.collectionView.setEmptyMessage(message)
            }).subscribe().disposed(by: disposeBag)
    }

    func registerCellsOfCollectionView() {
        collectionView.register(
            UINib(
                nibName: ViewControllerIdentifiers.cardCollectionViewCell,
                bundle: nil), forCellWithReuseIdentifier: ViewControllerIdentifiers.cardReuseIdentifier)
        collectionView.register(
            UINib(
                nibName: ViewControllerIdentifiers.cardIsolatedOrderCollectionViewCell,
                bundle: nil), forCellWithReuseIdentifier: ViewControllerIdentifiers.cardIsolatedOrderReuseIdentifier)
        collectionView.register(
            UINib(
                nibName: ViewControllerIdentifiers.headerCollectionViewCell,
                bundle: nil),
            forSupplementaryViewOfKind: UICollectionView.elementKindSectionHeader,
            withReuseIdentifier: ViewControllerIdentifiers.headerReuseIdentifier)
    }

    func generateHeaderOptions(_ headerText: String,
                               _ itemsSection: [Order],
                               _ header: HeaderCollectionViewCell) -> HeaderCollectionViewCell {
            let orderTemp = itemsSection.first
            header.productID.text = headerText
            if headerText.contains(CommonStrings.orderTitile)  || headerText.contains(CommonStrings.shopTransaction) {
                header.orders = Set(itemsSection.map({$0.baseDocument ?? 0}))
                header.delegate = self
                header.pdfImageView.isHidden = false
                header.patientListButton.isHidden = false
                header.doctorName.isHidden = false
                let list = self.getArrayNames(itemsSection.map({$0.patientName ?? ""}))
                let patientName = list.count > 0 ? "patientName" : "noPatientName"
                header.patientListButton.setImage(UIImage(named: patientName), for: .normal)
                header.titlePatients = headerText
                header.patientNames = Set(list)
                header.doctorName.text = orderTemp?.clientDxp
            } else {
                header.doctorName.isHidden = true
                header.delegate = nil
                header.pdfImageView.isHidden = true
                header.patientListButton.isHidden = true
            }
            return header
        }

    func modelBindingExtension3() {
        // Muestra o oculta el loading
        inboxViewModel.loading.observeOn(MainScheduler.instance).subscribe(onNext: { [weak self] showLoading in
            guard let self = self else { return }
            self.view.isUserInteractionEnabled = true
            if showLoading {
                self.lottieManager.showLoading()
                return
            }
            self.lottieManager.hideLoading()
        }).disposed(by: self.disposeBag)
        // Muestra un mensaje AlertViewController
        inboxViewModel.showAlert.observeOn(MainScheduler.instance)
            .subscribe(onNext: { [unowned self] message in
            AlertManager.shared.showAlert(message: message, view: self)
        }).disposed(by: self.disposeBag)
        collectionView.rx.itemSelected.observeOn(MainScheduler.instance)
            .subscribe(onNext: { [weak self] indexPath in
                guard let self = self else { return }
                guard self.indexPathsSelected.count > 0 else {
                    self.collectionView.deselectItem(at: indexPath, animated: false)
                    self.lastRect = self.collectionView.layoutAttributesForItem(at: indexPath)?.frame
                    self.lastIndexPath = indexPath
                    let orders = self.inboxViewModel.sectionOrders
                    let orderOptional = orders[safe: indexPath.section]?.items[safe: indexPath.row]
                    guard let order = orderOptional else { return }
                    self.detailTapped(order: order)
                    return
                }
                self.updateCellWithIndexPath(indexPath)
        }).disposed(by: self.disposeBag)
    }

    func modelBindingExtension4() {
        collectionView.rx.itemDeselected.subscribe(onNext: { [weak self] indexPath in
            guard let self = self else { return }
            guard self.indexPathsSelected.count > 0 else {
                self.lastRect = self.collectionView.layoutAttributesForItem(at: indexPath)?.frame
                self.lastIndexPath = indexPath
                let orders = self.inboxViewModel.sectionOrders
                let orderOptional = orders[safe: indexPath.section]?.items[safe: indexPath.row]
                guard let order = orderOptional else { return }
                self.detailTapped(order: order)
                return
            }
            self.updateCellWithIndexPath(indexPath)
        }).disposed(by: disposeBag)

        inboxViewModel
            .reloadData
            .observeOn(MainScheduler.instance)
            .subscribe(onNext: { [weak self] _ in
                guard let self = self, let lastRect = self.lastRect else { return }
                self.collectionView.scrollRectToVisible(lastRect, animated: true)
            })
            .disposed(by: self.disposeBag)
    }
    func updateCellWithIndexPath(_ indexPath: IndexPath) {
        indexPathsSelected.contains(indexPath) ?
            collectionView.deselectItem(at: indexPath, animated: true) :
            collectionView.selectItem(at: indexPath, animated: true, scrollPosition: .centeredVertically)

        if let index = indexPathsSelected.firstIndex(of: indexPath) {
            indexPathsSelected.remove(at: index)
        } else {
            indexPathsSelected.append(indexPath)
        }

        if indexPathsSelected.count > 0 {
            UIView.animate(withDuration: 0.2, animations: { [weak self] in
                guard let self = self else { return }
                self.removeOrdersSelectedVerticalSpace.constant = -15
                self.view.layoutIfNeeded()
            })
            processButton.isEnabled = true
            pendingButton.isEnabled = true
            finishedButton.isEnabled = true
            packageButton.isEnabled = self.inboxViewModel.ordersHasBatchesCompleted(
                indexPathOfOrdersSelected: indexPathsSelected)
        } else {
            UIView.animate(withDuration: 0.2, animations: { [weak self] in
                guard let self = self else { return }
                self.removeOrdersSelectedVerticalSpace.constant = -80
                self.view.layoutIfNeeded()
            })
            processButton.isEnabled = false
            pendingButton.isEnabled = false
            finishedButton.isEnabled = false
            packageButton.isEnabled = false
        }
    }
    func detailTapped(order: Order) {
        if self.rootViewModel.userType == .qfb {
            self.inboxViewModel.selectedOrder = order
            self.view.endEditing(true)
            self.performSegue(withIdentifier: ViewControllerIdentifiers.orderDetailViewController, sender: nil)
        }
    }
    func showSignatureVC() {
        inboxViewModel.showSignatureVc.subscribe(onNext: { [weak self] titleView in
            guard let self = self else { return }
            let storyboard = UIStoryboard(name: ViewControllerIdentifiers.storieboardName, bundle: nil)
            let signatureVC = storyboard.instantiateViewController(
                identifier: ViewControllerIdentifiers.signaturePadViewController) as? SignaturePadViewController
            signatureVC?.titleView = titleView
            signatureVC?.originView = ViewControllerIdentifiers.inboxViewController
            signatureVC?.modalPresentationStyle = .overFullScreen
            signatureVC?.modalTransitionStyle = .crossDissolve
            self.present(signatureVC!, animated: true, completion: nil)
        }).disposed(by: disposeBag)
    }

    func isUserInteractionEnabledBinding() {
        inboxViewModel.isUserInteractionEnabled
            .bind(to: view.rx.isUserInteractionEnabled).disposed(by: disposeBag)
    }

    func getNamesByOrder(productID: Int) -> Order? {
        let order = inboxViewModel.sectionOrders.first(where: { value -> Bool in
            value.items.first(where: { order in
                order.baseDocument == productID
            }) != nil
        })
        let orderSend = order?.items[0]
        orderSend?.baseDocument = productID
        return orderSend
    }

    func getArrayNames(_ names: [String]) -> [String] {
        let namesNoRepeat = Set(names)
        let stringNames = namesNoRepeat.joined(separator: ",")
        return stringNames.components(separatedBy: ",").filter({!$0.isEmpty})
    }
    func updateRemoveViewColor(title: String) -> UIColor {
        switch title {
        case StatusNameConstants.assignedStatus:
            lastColor = OmicronColors.assignedStatus
            return OmicronColors.assignedStatus
        case StatusNameConstants.inProcessStatus:
            lastColor = OmicronColors.processStatus
            return OmicronColors.processStatus
        case StatusNameConstants.penddingStatus:
            lastColor = OmicronColors.pendingStatus
            return OmicronColors.pendingStatus
        case StatusNameConstants.finishedStatus:
            lastColor = OmicronColors.finishedStatus
            return OmicronColors.finishedStatus
        case StatusNameConstants.reassignedStatus:
            lastColor = OmicronColors.reassignedStatus
            return OmicronColors.reassignedStatus
        default: return lastColor
        }
    }

    func chageStatusName(index: Int) {
        let name = self.inboxViewModel.getStatusName(index: index)
        self.inboxViewModel.title.onNext(name)
    }
}
