//
//  HistoricViewModel.swift
//  Omicron
//
//  Created by Josue Castillo on 11/09/25.
//  Copyright © 2025 Diego Cárcamo. All rights reserved.
//

import Foundation
import RxSwift
import Resolver

class HistoricViewModel {
    // MARK: Dependencias
    @Injected var networkManager: NetworkManager
    var disposeBag: DisposeBag = DisposeBag()
    
    // MARK: Observables
    var loading: PublishSubject<Bool> = PublishSubject()
    var tableData = PublishSubject<[ParentOrders]>()
    var searchDidTap = PublishSubject<Void>()
    var searchFilter = BehaviorSubject<String>(value: String())
    var dataChips = BehaviorSubject<[String]>(value: [])
    let onScroll = PublishSubject<Void>()
    
    var showAlert: PublishSubject<String> = PublishSubject()
    // MARK: Controles
    var orders: [ParentOrders] = []
    var dataOffset: Int = 0
    let limit: Int = 10
    var chips = String()
    
    init() {
        searchDidTapBinding()
        bindOnScroll()
//        dataChipsBinding()
    }
    
    func searchDidTapBinding() {
        searchDidTap.withLatestFrom(searchFilter).subscribe(onNext: { [weak self] text in
            self?.dataOffset = 0
            self?.chips = text
            self?.orders = []
            self?.getHistoryDataMock(orders: self?.chips ?? String(), offset: self?.dataOffset ?? 0, limit: 10)
            }).disposed(by: disposeBag)
        
    }
    
    func bindOnScroll() {
        self.onScroll.subscribe(onNext: {[weak self] _ in
            guard let self = self else { return }
            self.dataOffset += 10
            self.getHistoryDataMock(orders: self.chips, offset: self.dataOffset, limit: 10)
        }).disposed(by: disposeBag)
    }
    
    func getHistoricData(orders: String, offset: Int, limit: Int) {
        let request = getRequestData(orders: orders, offset: offset, limit: limit)
        
        self.loading.onNext(true)
        networkManager.getHistoric(request).subscribe(onNext: {[weak self] res in
            guard let self = self else { return }
            self.loading.onNext(false)
            if res.code == 200 {
                tableData.onNext(res.response ?? [])
                return
            }
            let errorMessage = res.userError ?? String()
            self.showAlert.onNext(errorMessage)
        }, onError: {[weak self] _ in
            guard let self = self else { return }
            self.loading.onNext(false)
            self.showAlert.onNext(CommonStrings.errorSplitOrder)
        }).disposed(by: disposeBag)
    }
    
    func getRequestData(orders: String, offset: Int, limit: Int) -> HistoricRequestModel {
        return HistoricRequestModel(
            qfb: Persistence.shared.getUserData()?.id ?? String(),
            orders: orders,
            offset: offset,
            limit: limit
        )
    }
    
    // MARK: QUITAR
    
    func getHistoryDataMock(orders: String, offset: Int, limit: Int) {
        let request = getRequestData(orders: orders, offset: offset, limit: limit)
        dump(request)
        loading.onNext(true)
        networkManager.getOrdenDetail(225305)
            .observe(on: MainScheduler.instance)
            .subscribe(onNext: {[weak self] res in
                guard let self = self else { return }
                loading.onNext(false)
                let parentOrders = getExampleMock(quantity: 3)
                self.orders.append(contentsOf: parentOrders)
                tableData.onNext(parentOrders)
                
            }, onError: { [weak self] _ in
                guard let self = self else { return }
                self.loading.onNext(false)
            }).disposed(by: self.disposeBag)
    }
    
    func getExampleMock(quantity: Int) -> [ParentOrders] {
        var parentExample: [ParentOrders] = []
        if quantity == 0 {
            parentExample = []
        }
        if quantity == 1 {
            parentExample = [
                ParentOrders(
                    orderProductionId: 201294,
                    totalPieces: 10,
                    availablePieces: 6,
                    qfbWhoSplit: "b49b30e1-5232-48f3-be74-axaxaxaxaxax",
                    detailOrdersCount: 2,
                    orderProductionDetail: [
                        ChildrenOrders(
                            OrderProductionDetailId: 201295,
                            AssignedPieces: 2,
                            AssignedQfb: "b49b30e1-5232-48f3-be74-d3f4eebe0b4f",
                            DateCreated: "08/09/2025 06:39:31"
                        ),
                        ChildrenOrders(
                            OrderProductionDetailId: 201296,
                            AssignedPieces: 2,
                            AssignedQfb: "b49b30e1-5232-48f3-be74-d3f4eebe0b4f",
                            DateCreated: "09/09/2025 06:39:31"
                        )
                    ],
                    autoExpandOrderDetail: false
                )
            ]
        }
        if quantity == 2 {
            parentExample = [
                ParentOrders(
                    orderProductionId: 201294,
                    totalPieces: 10,
                    availablePieces: 6,
                    qfbWhoSplit: "b49b30e1-5232-48f3-be74-axaxaxaxaxax",
                    detailOrdersCount: 2,
                    orderProductionDetail: [
                        ChildrenOrders(
                            OrderProductionDetailId: 201295,
                            AssignedPieces: 2,
                            AssignedQfb: "b49b30e1-5232-48f3-be74-d3f4eebe0b4f",
                            DateCreated: "08/09/2025 06:39:31"
                        ),
                        ChildrenOrders(
                            OrderProductionDetailId: 201296,
                            AssignedPieces: 2,
                            AssignedQfb: "b49b30e1-5232-48f3-be74-d3f4eebe0b4f",
                            DateCreated: "09/09/2025 06:39:31"
                        )
                    ],
                    autoExpandOrderDetail: false
                ),
                ParentOrders(
                    orderProductionId: 201295,
                    totalPieces: 5,
                    availablePieces: 5,
                    qfbWhoSplit: "c59c40e2-1234-56f7-be74-bxbbxbbxbbxb",
                    detailOrdersCount: 0,
                    orderProductionDetail: [],
                    autoExpandOrderDetail: false
                )
            ]
        }
        if quantity > 2 {
            parentExample = [
                ParentOrders(
                    orderProductionId: 201294,
                    totalPieces: 10,
                    availablePieces: 6,
                    qfbWhoSplit: "b49b30e1-5232-48f3-be74-axaxaxaxaxax",
                    detailOrdersCount: 2,
                    orderProductionDetail: [
                        ChildrenOrders(
                            OrderProductionDetailId: 201295,
                            AssignedPieces: 2,
                            AssignedQfb: "b49b30e1-5232-48f3-be74-d3f4eebe0b4f",
                            DateCreated: "08/09/2025 06:39:31"
                        ),
                        ChildrenOrders(
                            OrderProductionDetailId: 201296,
                            AssignedPieces: 2,
                            AssignedQfb: "b49b30e1-5232-48f3-be74-d3f4eebe0b4f",
                            DateCreated: "09/09/2025 06:39:31"
                        )
                    ],
                    autoExpandOrderDetail: false
                ),
                ParentOrders(
                    orderProductionId: 201295,
                    totalPieces: 5,
                    availablePieces: 5,
                    qfbWhoSplit: "c59c40e2-1234-56f7-be74-bxbbxbbxbbxb",
                    detailOrdersCount: 0,
                    orderProductionDetail: [],
                    autoExpandOrderDetail: false
                ),
                ParentOrders(
                    orderProductionId: 201296,
                    totalPieces: 12,
                    availablePieces: 10,
                    qfbWhoSplit: "d69d50f3-2345-67g8-ce85-cycycycycycy",
                    detailOrdersCount: 1,
                    orderProductionDetail: [
                        ChildrenOrders(
                            OrderProductionDetailId: 201297,
                            AssignedPieces: 5,
                            AssignedQfb: "d69d50f3-2345-67g8-ce85-cycycycycycy",
                            DateCreated: "10/09/2025 08:00:00"
                        )
                    ],
                    autoExpandOrderDetail: false
                ),
                ParentOrders(orderProductionId: 201297, totalPieces: 8, availablePieces: 8, qfbWhoSplit: "e70e61f4-3456-78h9-de96-dzdzdzdzdzdz", detailOrdersCount: 0, orderProductionDetail: [], autoExpandOrderDetail: false),
                ParentOrders(orderProductionId: 201298, totalPieces: 15, availablePieces: 10, qfbWhoSplit: "f81f72g5-4567-89i0-ef07-eeeeeeeeeeee", detailOrdersCount: 2, orderProductionDetail: [
                    ChildrenOrders(OrderProductionDetailId: 201299, AssignedPieces: 4, AssignedQfb: "f81f72g5-4567-89i0-ef07-eeeeeeeeeeee", DateCreated: "11/09/2025 09:15:00"),
                    ChildrenOrders(OrderProductionDetailId: 201300, AssignedPieces: 6, AssignedQfb: "f81f72g5-4567-89i0-ef07-eeeeeeeeeeee", DateCreated: "11/09/2025 09:20:00")
                ], autoExpandOrderDetail: false),
                ParentOrders(orderProductionId: 201301, totalPieces: 20, availablePieces: 20, qfbWhoSplit: "g92g83h6-5678-90j1-fg18-ffffffffffff", detailOrdersCount: 0, orderProductionDetail: [], autoExpandOrderDetail: false),
                ParentOrders(orderProductionId: 201302, totalPieces: 7, availablePieces: 7, qfbWhoSplit: "h03h94i7-6789-01k2-gh29-gggggggggggg", detailOrdersCount: 1, orderProductionDetail: [
                    ChildrenOrders(OrderProductionDetailId: 201303, AssignedPieces: 7, AssignedQfb: "h03h94i7-6789-01k2-gh29-gggggggggggg", DateCreated: "12/09/2025 10:00:00")
                ], autoExpandOrderDetail: false),
                ParentOrders(orderProductionId: 201304, totalPieces: 9, availablePieces: 5, qfbWhoSplit: "i14i05j8-7890-12l3-hi30-hhhhhhhhhhhh", detailOrdersCount: 0, orderProductionDetail: [], autoExpandOrderDetail: false),
                ParentOrders(orderProductionId: 201305, totalPieces: 6, availablePieces: 6, qfbWhoSplit: "j25j16k9-8901-23m4-ij41-iiiiiiiiiiii", detailOrdersCount: 2, orderProductionDetail: [
                    ChildrenOrders(OrderProductionDetailId: 201306, AssignedPieces: 3, AssignedQfb: "j25j16k9-8901-23m4-ij41-iiiiiiiiiiii", DateCreated: "13/09/2025 11:10:00"),
                    ChildrenOrders(OrderProductionDetailId: 201307, AssignedPieces: 3, AssignedQfb: "j25j16k9-8901-23m4-ij41-iiiiiiiiiiii", DateCreated: "13/09/2025 11:15:00")
                ], autoExpandOrderDetail: true),
                ParentOrders(orderProductionId: 201308, totalPieces: 11, availablePieces: 11, qfbWhoSplit: "k36k27l0-9012-34n5-jk52-jjjjjjjjjjjj", detailOrdersCount: 0, orderProductionDetail: [], autoExpandOrderDetail: false),
                ParentOrders(orderProductionId: 201309, totalPieces: 13, availablePieces: 13, qfbWhoSplit: "l47l38m1-0123-45o6-kl63-kkkkkkkkkkkk", detailOrdersCount: 1, orderProductionDetail: [
                    ChildrenOrders(OrderProductionDetailId: 201310, AssignedPieces: 13, AssignedQfb: "l47l38m1-0123-45o6-kl63-kkkkkkkkkkkk", DateCreated: "14/09/2025 12:30:00")
                ], autoExpandOrderDetail: true),
//                ParentOrders(orderProductionId: 201311, totalPieces: 14, availablePieces: 14, qfbWhoSplit: "m58m49n2-1234-56p7-lm74-llllllllllll", detailOrdersCount: 0, orderProductionDetail: [], autoExpandOrderDetail: false),
//                ParentOrders(orderProductionId: 201312, totalPieces: 18, availablePieces: 15, qfbWhoSplit: "n69n50o3-2345-67q8-mn85-mmmmmmmmmmmm", detailOrdersCount: 2, orderProductionDetail: [
//                    ChildrenOrders(OrderProductionDetailId: 201313, AssignedPieces: 8, AssignedQfb: "n69n50o3-2345-67q8-mn85-mmmmmmmmmmmm", DateCreated: "15/09/2025 13:45:00"),
//                    ChildrenOrders(OrderProductionDetailId: 201314, AssignedPieces: 7, AssignedQfb: "n69n50o3-2345-67q8-mn85-mmmmmmmmmmmm", DateCreated: "15/09/2025 13:50:00")
//                ], autoExpandOrderDetail: true),
//                ParentOrders(orderProductionId: 201315, totalPieces: 5, availablePieces: 5, qfbWhoSplit: "o70o61p4-3456-78r9-no96-nnnnnnnnnnnn", detailOrdersCount: 0, orderProductionDetail: [], autoExpandOrderDetail: false),
//                ParentOrders(orderProductionId: 201316, totalPieces: 9, availablePieces: 9, qfbWhoSplit: "p81p72q5-4567-89s0-op07-pppppppppppp", detailOrdersCount: 1, orderProductionDetail: [
//                    ChildrenOrders(OrderProductionDetailId: 201317, AssignedPieces: 9, AssignedQfb: "p81p72q5-4567-89s0-op07-pppppppppppp", DateCreated: "16/09/2025 14:00:00")
//                ], autoExpandOrderDetail: true),
//                ParentOrders(orderProductionId: 201318, totalPieces: 16, availablePieces: 12, qfbWhoSplit: "q92q83r6-5678-90t1-qr18-qqqqqqqqqqqq", detailOrdersCount: 0, orderProductionDetail: [], autoExpandOrderDetail: false),
//                ParentOrders(orderProductionId: 201319, totalPieces: 20, availablePieces: 20, qfbWhoSplit: "r03r94s7-6789-01u2-rs29-rrrrrrrrrrrr", detailOrdersCount: 2, orderProductionDetail: [
//                    ChildrenOrders(OrderProductionDetailId: 201320, AssignedPieces: 10, AssignedQfb: "r03r94s7-6789-01u2-rs29-rrrrrrrrrrrr", DateCreated: "17/09/2025 15:15:00"),
//                    ChildrenOrders(OrderProductionDetailId: 201321, AssignedPieces: 10, AssignedQfb: "r03r94s7-6789-01u2-rs29-rrrrrrrrrrrr", DateCreated: "17/09/2025 15:20:00")
//                ], autoExpandOrderDetail: true)
                
            ]
        }
        return parentExample
    }
}
