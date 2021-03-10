//
//  ChartViewController.swift
//  Omicron
//
//  Created by Vicente Cantú on 21/09/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import UIKit
import Charts
import Resolver
import RxSwift

class ChartViewController: UIViewController {

    @IBOutlet weak var collectionView: UICollectionView!
    @IBOutlet weak var possibleAssingLabel: UILabel!
    @IBOutlet weak var typeOfDateLabel: UILabel!
    @IBOutlet weak var daysLabel: UILabel!
    @IBOutlet weak var leftButton: UIButton!
    @IBOutlet weak var rightButton: UIButton!

    @Injected var chartViewModel: ChartViewModel

    var disposeBag: DisposeBag = DisposeBag()
    let flowLayout = SnapFlowLayout()
    var lastIndexPath: IndexPath?
    var currentIndexPath = IndexPath(item: 0, section: 0)

    override func viewDidLoad() {
        super.viewDidLoad()
        viewModelBingind()
        flowLayout.sectionInset = UIEdgeInsets(top: 0, left: 0, bottom: 0, right: 0)
        collectionView.setCollectionViewLayout(flowLayout, animated: false)
        collectionView.contentInsetAdjustmentBehavior = .always
    }

    func viewModelBingind() {

        chartViewModel
            .start
            .observeOn(MainScheduler.instance)
            .subscribe(onNext: { [weak self] firstTime in
                guard let self = self else { return }
                if firstTime {
                    self.possibleAssingLabel.text = self.chartViewModel.capacity[safe: 0] ?? "0"
                    self.typeOfDateLabel.text = self.getTitle(0)
                    self.daysLabel.text = self.chartViewModel.daysRange[0]
                } else {
                    guard let lastIndexPath = self.lastIndexPath else { return }
                    self.collectionView.scrollToItem(at: lastIndexPath, at: .centeredHorizontally, animated: true)
                }
            })
            .disposed(by: disposeBag)

        chartViewModel.getWorkloads()

        collectionView
            .rx
            .didScroll
            .subscribe { [weak self] _ in
                guard let self = self else { return }
                var visibleRect = CGRect()

                visibleRect.origin = self.collectionView.contentOffset
                visibleRect.size = self.collectionView.bounds.size

                let visiblePoint = CGPoint(x: visibleRect.midX, y: visibleRect.midY)

                guard let indexPath = self.collectionView.indexPathForItem(at: visiblePoint) else { return }
                self.possibleAssingLabel.text = self.chartViewModel.capacity[safe: indexPath.row] ?? "0"
                self.typeOfDateLabel.text = self.getTitle(indexPath.row)
                self.daysLabel.text = self.chartViewModel.daysRange[indexPath.row]
                self.lastIndexPath = indexPath
            }
            .disposed(by: disposeBag)

        chartViewModel
            .workloadData
            .bind(to: collectionView.rx.items(
                    cellIdentifier: "chart",
                    cellType: ChartCollectionViewCell.self)) { _, data, cell in
                cell.setData(data: data)
            }
            .disposed(by: disposeBag)

    }

    @IBAction func leftButtonDidPressed(_ sender: Any) {
        if currentIndexPath.item > 0 {
            currentIndexPath = IndexPath(item: currentIndexPath.item - 1, section: 0)
            collectionView.scrollToItem(at: currentIndexPath, at: .centeredHorizontally, animated: true)
            guard currentIndexPath.item == 0 else {
                leftButton.isEnabled = true
                rightButton.isEnabled = true
                return
            }
            rightButton.isEnabled = true
            leftButton.isEnabled = false
        }
    }

    @IBAction func rightButtonDidPressed(_ sender: Any) {
        if currentIndexPath.item < 2 {
            currentIndexPath = IndexPath(item: currentIndexPath.item + 1, section: 0)
            collectionView.scrollToItem(at: currentIndexPath, at: .centeredHorizontally, animated: true)
            guard currentIndexPath.item == 2 else {
                rightButton.isEnabled = true
                leftButton.isEnabled = true
                return
            }
            leftButton.isEnabled = true
            rightButton.isEnabled = false
        }
    }

    private func getTitle(_ index: Int) -> String {
        switch index {
        case 0: return "Día"
        case 1: return "Semana"
        case 2: return "Mes"
        default: return ""
        }
    }

}
