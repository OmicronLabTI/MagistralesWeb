//
//  LotsAvailableTableViewCell.swift
//  Omicron
//
//  Created by Axity on 21/08/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit
import RxSwift
import RxCocoa
import Resolver

class LotsAvailableTableViewCell: UITableViewCell {
    // MARK: - OUTLETS
    @IBOutlet weak var lotsLabel: UILabel!
    @IBOutlet weak var quantityAvailableLabel: UILabel!
    @IBOutlet weak var quantitySelected: UITextField!
    @IBOutlet weak var quantityAssignedLabel: UILabel!
    // MARK: - VARIABLES
    @Injected var lotsViewModel: LotsViewModel
    var disposeBag = DisposeBag()
    var row: Int?
    weak var itemModel: LotsAvailable?
    override func awakeFromNib() {
        super.awakeFromNib()
        // Initialization code
        UtilsManager.shared.labelsStyle(label: self.lotsLabel, text: "", fontSize: 14)
        UtilsManager.shared.labelsStyle(label: self.quantityAvailableLabel, text: "", fontSize: 14)
        UtilsManager.shared.labelsStyle(label: self.quantityAssignedLabel, text: "", fontSize: 14)
        self.quantitySelected.font = UIFont(name: FontsNames.SFProDisplayMedium, size: 14)
        self.quantitySelected.delegate = self
    }
    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)
        // Configure the view for the selected state
    }
    func setExpiredBatches(_ expired: Bool) {
        lotsLabel.textColor = expired ? UIColor.red : UIColor.black
        quantityAvailableLabel.textColor = expired ? UIColor.red : UIColor.black
        quantitySelected.textColor = expired ? UIColor.red : UIColor.black
        quantityAssignedLabel.textColor = expired ? UIColor.red : UIColor.black
    }
}
extension LotsAvailableTableViewCell: UITextFieldDelegate {
    func textFieldDidBeginEditing(_ textField: UITextField) {
        var tableView: UITableView?
        var superview: UIView? = self.superview
        while superview != nil && tableView == nil {
            tableView = superview as? UITableView
            superview = superview?.superview
        }
        if tableView != nil && self.row != nil {
            let indexPath = IndexPath(row: self.row!, section: 0)
            tableView!.selectRow(at: indexPath, animated: false, scrollPosition: .none)
            tableView!.delegate?.tableView?(tableView!, didSelectRowAt: indexPath)
        }
        self.lotsViewModel.lastResponder.onNext(textField)
    }
    func textFieldShouldEndEditing(_ textField: UITextField) -> Bool {
        return true
    }
    func textField(_ textField: UITextField,
                   shouldChangeCharactersIn range: NSRange, replacementString string: String) -> Bool {
        guard textField == self.quantitySelected, let textFieldString = textField.text as NSString? else {
            return true
        }
        let newString = textFieldString.replacingCharacters(in: range, with: string)
        let expression = "^([0-9]+)?(\\.([0-9]{1,8})?)?$"
        let regex = try? NSRegularExpression(pattern: expression, options: .caseInsensitive)
        // swiftlint:disable legacy_constructor
        let numberOfMathces = regex?.numberOfMatches(
            in: newString, options: NSRegularExpression.MatchingOptions(rawValue: 0),
            range: NSMakeRange(0, newString.count))
        if numberOfMathces == 0 {
            return false
        }
        self.itemModel?.cantidadSeleccionada = newString.isEmpty ? 0 : Decimal(string: newString)
        return true
    }
}
