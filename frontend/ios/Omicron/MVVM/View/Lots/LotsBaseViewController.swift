//
//  LotsBaseViewController.swift
//  Omicron
//
//  Created by Daniel Vargas on 19/05/25.
//  Copyright © 2025 Diego Cárcamo. All rights reserved.
//
import UIKit
import RxSwift
import RxCocoa
import Resolver

class LotsBaseViewController: UIViewController {
    // MARK: - OUTLEST
    @IBOutlet weak var titleLabel: UILabel!
    @IBOutlet weak var hashtagLabel: UILabel!
    @IBOutlet weak var codeLabel: UILabel!
    @IBOutlet weak var descriptionLabel: UILabel!
    @IBOutlet weak var warehouseCodeLabel: UILabel!
    @IBOutlet weak var totalNeededLabel: UILabel!
    @IBOutlet weak var totalSelectedLabel: UILabel!
    @IBOutlet weak var lotsAvailableLabel: UILabel!
    @IBOutlet weak var laLotsLabel: UILabel!
    @IBOutlet weak var laQuantityAvailableLabel: UILabel!
    @IBOutlet weak var laQuantitySelectedLabel: UILabel!
    @IBOutlet weak var laQuantityAssignedLabel: UILabel!
    @IBOutlet weak var lotsSelectedLabel: UILabel!
    @IBOutlet weak var lsLotsLabel: UILabel!
    @IBOutlet weak var lsQuantityAvailableLabel: UILabel!
    
    @IBOutlet weak var lineDocTable: UITableView!
    @IBOutlet weak var lotsAvailablesTable: UITableView!
    @IBOutlet weak var lotsSelectedTable: UITableView!
    
    @IBOutlet weak var codeDescriptionLabel: UILabel!
    @IBOutlet weak var orderNumberLabel: UILabel!
    @IBOutlet weak var manufacturingOrderLabel: UILabel!
    @IBOutlet weak var addLotButton: UIButton!
    @IBOutlet weak var removeLotButton: UIButton!
    
    @IBOutlet weak var lineOfDocumentsView: UIView!
    @IBOutlet weak var lotsAvailable: UIView!
    @IBOutlet weak var lotsSelected: UIView!
    @IBOutlet weak var backButtonLabel: UILabel!
    @IBOutlet weak var backButtonStackView: UIStackView!
    
    @Injected var inboxViewModel: InboxViewModel

    var orderId = -1
    var formatter = UtilsManager.shared.formatterDoublesTo6Decimals()
    var statusType = CommonStrings.empty
    var codeDescription = CommonStrings.empty
    var orderNumber = CommonStrings.empty
    var manufacturingOrder = CommonStrings.empty
    var comments = CommonStrings.empty
    var orderDetail: [OrderDetail] = []
    var emptyStockProductId: [String] = []
    
    func changeTextColorOfLabels(color: UIColor) {
        self.titleLabel.textColor = color
        self.hashtagLabel.textColor = color
        self.codeLabel.textColor = color
        self.descriptionLabel.textColor = color
        self.warehouseCodeLabel.textColor = color
        self.totalNeededLabel.textColor = color
        self.totalSelectedLabel.textColor = color
        self.lotsAvailableLabel.textColor = color
        self.laLotsLabel.textColor = color
        self.laQuantityAvailableLabel.textColor = color
        self.laQuantitySelectedLabel.textColor = color
        self.laQuantityAssignedLabel.textColor = color
        self.lotsSelectedLabel.textColor = color
        self.lsLotsLabel.textColor = color
        self.lsQuantityAvailableLabel.textColor = color
        self.codeDescriptionLabel.textColor = color
        self.orderNumberLabel.textColor = color
        self.manufacturingOrderLabel.textColor = color
    }
    
    func setStyleView(view: UIView) {
        view.layer.shadowColor = UIColor.black.cgColor
        view.layer.shadowOpacity = 0.2
        view.layer.shadowOffset  = CGSize(width: 0.1, height: 0.1)
        view.layer.shadowRadius = 5
        view.layer.cornerRadius = 10
    }
    
    func setBackButtonLabelText() {
        backButtonLabel.text = inboxViewModel.currentSection.statusName
    }
    
    func loadInfo() {
        changeTextColorOfLabels(color: .black)
        let orderNumber = orderNumber == "0" ? CommonStrings.empty : orderNumber
        orderNumberLabel.attributedText = UtilsManager.shared.boldSubstring(
            text: "\(CommonStrings.orderNumber) \(orderNumber)", textToBold: CommonStrings.orderNumber)
        manufacturingOrderLabel.attributedText = UtilsManager.shared.boldSubstring(
            text: "\(CommonStrings.manufacturingOrder) \(manufacturingOrder)",
            textToBold: CommonStrings.manufacturingOrder)
        let titleFontSize = codeDescription.count  > 170 ? CGFloat(11) : CGFloat(15)
        var codeDescriptionArray = codeDescription.components(separatedBy: "  ")
        if codeDescriptionArray.count > 0 {
            let code = codeDescriptionArray[0]
            codeDescriptionArray.remove(at: 0)
            let description = codeDescriptionArray.joined(separator: " ")
            let codeAtr = UtilsManager.shared.boldSubstring(text: code,
                                                            textToBold: code, fontSize: titleFontSize,
                                                            textColor: OmicronColors.blue)
            let descriptionAtr = UtilsManager.shared.boldSubstring(text: description,
                                                                   textToBold: description, fontSize: titleFontSize,
                                                                   textColor: .gray)
            let pipeAtr = UtilsManager.shared.boldSubstring(text: " | ", textToBold: " | ",
                                                            fontSize: titleFontSize, textColor: .black)
            let richText = NSMutableAttributedString()
            richText.append(codeAtr)
            richText.append(pipeAtr)
            richText.append(descriptionAtr)
            codeDescriptionLabel.attributedText = richText
        }
    }
}


extension LotsBaseViewController: UITableViewDelegate {
    // Pinta una fila o otra no en la tabla
    func tableView(_ tableView: UITableView, willDisplay cell: UITableViewCell, forRowAt indexPath: IndexPath) {
        let customView = UIView()
        customView.backgroundColor = OmicronColors.blue
        cell.selectedBackgroundView = customView
        if indexPath.row%2 == 0 {
            cell.backgroundColor = OmicronColors.tableColorRow
        } else {
            cell.backgroundColor = .white
        }
    }
    func scrollViewDidScroll(_ scrollView: UIScrollView) {
        guard let tableView = scrollView as? UITableView else { return }
        tableView.removeMoreIndicator()
    }
    // swiftlint:disable file_length
}
