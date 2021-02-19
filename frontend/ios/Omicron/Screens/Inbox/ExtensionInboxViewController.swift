//
//  ExtensionInboxViewController.swift
//  Omicron
//
//  Created by Sergio Flores Ramirez on 18/02/21.
//  Copyright © 2021 Diego Cárcamo. All rights reserved.
//

import UIKit

extension InboxViewController {
    func showSignatureVC() {
        inboxViewModel.showSignatureVc.subscribe(onNext: { [weak self] titleView in
            guard let self = self else { return }
            let storyboard = UIStoryboard(name: ViewControllerIdentifiers.storieboardName, bundle: nil)
            let signatureVC = storyboard.instantiateViewController(
                identifier: ViewControllerIdentifiers.signaturePadViewController) as? SignaturePadViewController
            signatureVC?.titleView = titleView
            signatureVC?.originView = ViewControllerIdentifiers.inboxViewController
            signatureVC?.modalPresentationStyle = .overFullScreen
            self.present(signatureVC!, animated: true, completion: nil)
        }).disposed(by: disposeBag)
    }

    func finishOrders() {
        inboxViewModel.finishOrders.subscribe(onNext: { [weak self] _ in
            self?.inboxViewModel.callFinishOrderService(
                indexPathOfOrdersSelected: self?.indexPathsSelected)
        }).disposed(by: disposeBag)
    }

    func isUserInteractionEnabledBinding() {
        inboxViewModel.isUserInteractionEnabled
            .bind(to: view.rx.isUserInteractionEnabled).disposed(by: disposeBag)
    }
}
