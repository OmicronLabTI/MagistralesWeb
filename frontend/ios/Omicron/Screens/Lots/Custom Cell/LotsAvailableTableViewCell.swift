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
    
    // MARK: -OUTLETS
    @IBOutlet weak var lotsLabel: UILabel!
    @IBOutlet weak var quantityAvailableLabel: UILabel!
    @IBOutlet weak var quantitySelected: UITextField!
    @IBOutlet weak var quantityAssignedLabel: UILabel!
    
    // MARK: -VARIABLES
    @Injected var lotsViewModel: LotsViewModel
    var disposeBag = DisposeBag()
    var quantitySelectedInput = BehaviorRelay<String>(value: "")
    var row: Int?
    override func awakeFromNib() {
        super.awakeFromNib()
        
        // Initialization code
        UtilsManager.shared.labelsStyle(label: self.lotsLabel, text: "", fontSize: 14)
        UtilsManager.shared.labelsStyle(label: self.quantityAvailableLabel, text: "", fontSize: 14)
        UtilsManager.shared.labelsStyle(label: self.quantityAssignedLabel, text: "", fontSize: 14)
        self.quantitySelected.font = UIFont(name: FontsNames.SFProDisplayMedium, size: 14)
        self.quantitySelected.delegate = self
        self.quantitySelected.rx.text.orEmpty.bind(to: lotsViewModel.quantitySelectedInput).disposed(by: self.disposeBag)
    }

    override func setSelected(_ selected: Bool, animated: Bool) {
        super.setSelected(selected, animated: animated)
        // Configure the view for the selected state
        self.quantitySelectedInput.accept(self.quantitySelected.text!)
    }
}

extension LotsAvailableTableViewCell: UITextFieldDelegate {
    func textFieldShouldBeginEditing(_ textField: UITextField) -> Bool {
        var cell: LotsAvailableTableViewCell?
        var superview: UIView? = textField.superview
        
        while superview != nil && cell == nil {
            cell = superview as? LotsAvailableTableViewCell
            superview = superview?.superview
        }
        
        if cell != nil {
            cell?.setSelected(true, animated: true)
        }
        return true
    }
    
    func textFieldShouldEndEditing(_ textField: UITextField) -> Bool {
        var cell: LotsAvailableTableViewCell?
        var superview: UIView? = textField.superview
        
        while superview != nil && cell == nil {
            cell = superview as? LotsAvailableTableViewCell
            superview = superview?.superview
        }
        
        if cell != nil {
            cell?.setSelected(false, animated: true)
        }
        return true
    }
    
    func textField(_ textField: UITextField, shouldChangeCharactersIn range: NSRange, replacementString string: String) -> Bool {
        
        guard textField == self.quantitySelected, let textFieldString = textField.text as NSString? else {
            return true
        }
        let newString = textFieldString.replacingCharacters(in: range, with: string)
        let expression = "^([0-9]+)?(\\.([0-9]{1,8})?)?$"
        
        let regex = try? NSRegularExpression(pattern: expression, options: .caseInsensitive)
        let numberOfMathces = regex?.numberOfMatches(in: newString, options: NSRegularExpression.MatchingOptions(rawValue: 0), range: NSMakeRange(0, newString.count))
        
        if numberOfMathces == 0 {
            return false
        }
        
        return true
    }
}
