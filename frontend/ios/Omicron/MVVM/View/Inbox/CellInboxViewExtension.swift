//
//  CellInboxViewExtension.swift
//  Omicron
//
//  Created by Daniel Velez on 25/01/23.
//  Copyright © 2023 Diego Cárcamo. All rights reserved.
//

import Foundation
import Differentiator

extension InboxViewController: CardCellDelegate {

    func returnCardIsolateOrderCollectionViewCell(
        indexPath: IndexPath, element: SectionModel<String, Order>.Item,
        decimalPart: Double?) -> CardIsolatedOrderCollectionViewCell {
            guard let cell = collectionView.dequeueReusableCell(
                withReuseIdentifier: ViewControllerIdentifiers.cardIsolatedOrderReuseIdentifier,
                for: indexPath) as? CardIsolatedOrderCollectionViewCell
            else { return CardIsolatedOrderCollectionViewCell() }
            cell.row = indexPath.row
            cell.order = element
            cell.numberDescriptionLabel.text = "\(element.productionOrderId ?? 0)"
            cell.plannedQuantityDescriptionLabel.text = decimalPart  ?? 0.0 > 0.0 ?
            String(format: DecimalFormat.six.rawValue,
                   NSDecimalNumber(decimal: element.plannedQuantity ?? 0.0).doubleValue) :
            "\(element.plannedQuantity ?? 0.0)"
            cell.startDateDescriptionLabel.text = element.startDate ?? CommonStrings.empty
            cell.finishDateDescriptionLabel.text = element.finishDate ?? CommonStrings.empty
            cell.productDescriptionLabel.text = element.descriptionProduct?.uppercased() ?? CommonStrings.empty
            cell.missingStockImage.isHidden = !element.hasMissingStock
            cell.isSelected = indexPathsSelected.contains(indexPath)
            cell.itemCode.text = element.itemCode
            cell.qfbNameContainer.isHidden = rootViewModel.userType != .technical
            cell.qfbName.text = "Quím. \(element.qfbName ?? "")"
            cell.itemCodeConstrains.constant = rootViewModel.userType != .technical ? 16 : 52
            cell.descriptionConstraint.constant = rootViewModel.userType != .technical ? 16 : 52
            return cell
        }

    func returnCardCollectionViewCell(indexPath: IndexPath, element: SectionModel<String, Order>.Item,
                                      decimalPart: Double?) -> CardCollectionViewCell {
        guard let cell = collectionView.dequeueReusableCell(
            withReuseIdentifier: ViewControllerIdentifiers.cardReuseIdentifier,
            for: indexPath) as? CardCollectionViewCell else { return CardCollectionViewCell() }
        cell.row = indexPath.row
        cell.order = element
        cell.numberDescriptionLabel.text = "\(element.productionOrderId ?? 0)"
        cell.baseDocumentDescriptionLabel.text = element.baseDocument == 0 ?
            CommonStrings.empty : "\(element.baseDocument ?? 0)"
        cell.tagDescriptionLabel.text = element.tag
        cell.containerLabel.text = element.container
        cell.tagDescriptionLabel.textColor = !element.finishedLabel ? .red : .systemGreen
        cell.plannedQuantityDescriptionLabel.text = decimalPart  ?? 0.0 > 0.0 ?
            String(format: DecimalFormat.six.rawValue,
                   NSDecimalNumber(decimal: element.plannedQuantity ?? 0.0).doubleValue) :
            "\(element.plannedQuantity ?? 0.0)"
        cell.startDateDescriptionLabel.text = element.startDate ?? CommonStrings.empty
        cell.finishDateDescriptionLabel.text = element.finishDate ?? CommonStrings.empty
        cell.productDescriptionLabel.text = element.descriptionProduct?.uppercased() ?? CommonStrings.empty
        cell.missingStockImage.isHidden = !element.hasMissingStock
        cell.isSelected = indexPathsSelected.contains(indexPath)
        cell.itemCode.text = element.itemCode
        cell.destiny.text = element.destiny
        let patientName = (element.patientName != "") ? "patientName" : "noPatientName"
        cell.patientListButton.setImage(UIImage(named: patientName), for: .normal)
        cell.pdfDownloadButton.isHidden = !rootViewModel.needSearch
        cell.patientListButton.isHidden = !groupByOrderNumberButton.isEnabled && !rootViewModel.needSearch
        cell.qfbNameContainer.isHidden = rootViewModel.userType != .technical
        cell.qfbName.text = "Quím. \(element.qfbName ?? "")"
        cell.itemCodeConstrains.constant = rootViewModel.userType != .technical ? 16 : 52
        cell.descriptionConstraint.constant = rootViewModel.userType != .technical ? 16 : 52
        return cell
    }

    func downloadPdf(id: Int) {
        inboxViewModel.downloadPDF([id])
    }

    func patientList(order: Order) {
        let names: String = order.patientName ?? ""
        let orderId = order.baseDocument ?? 0
        let title = "Pedido: \(String(orderId))"
        let dataSend = PatientListData(
            title: title,
            list: self.getArrayNames([names])
        )
        self.performSegue(withIdentifier: ViewControllerIdentifiers.patientListViewController, sender: dataSend)
    }

}
