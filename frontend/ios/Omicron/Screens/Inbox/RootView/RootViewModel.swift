//
//  RootViewModel.swift
//  Omicron
//
//  Created by Axity on 29/07/20.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//
import RxSwift
import RxCocoa

class RootViewModel {
    public let dataStatus: PublishSubject<[Section]> = PublishSubject()
    init() {
//        var assignedOrders =
//        [
//            Orden(No: 1, BaseDocument: "Documento base 1", Container: "Contenedor 1", Tag: "Tag 1", PlannedQuantity: "cantidad planeada 1", startDate: "27/03/2020", finishDate: "24/04/2020", descriptionProduct: "Descripción del producto 1"),
//            Orden(No: 1, BaseDocument: "Documento base 2", Container: "Contenedor 1", Tag: "Tag 1", PlannedQuantity: "cantidad planeada 1", startDate: "27/03/2020", finishDate: "24/04/2020", descriptionProduct: "Descripción del producto 1"),
//            Orden(No: 1, BaseDocument: "Documento base 3", Container: "Contenedor 1", Tag: "Tag 1", PlannedQuantity: "cantidad planeada 1", startDate: "27/03/2020", finishDate: "24/04/2020", descriptionProduct: "Descripción del producto 1")
//        ]
//
//        let inProcessOrdes =
//        [
//            Orden(No: 1, BaseDocument: "Documento base 1", Container: "Contenedor 1", Tag: "Tag 1", PlannedQuantity: "cantidad planeada 1", startDate: "27/03/2020", finishDate: "24/04/2020", descriptionProduct: "Descripción del producto 1"),
//            Orden(No: 1, BaseDocument: "Documento base 2", Container: "Contenedor 1", Tag: "Tag 1", PlannedQuantity: "cantidad planeada 1", startDate: "27/03/2020", finishDate: "24/04/2020", descriptionProduct: "Descripción del producto 1"),
//            Orden(No: 1, BaseDocument: "Documento base 3", Container: "Contenedor 1", Tag: "Tag 1", PlannedQuantity: "cantidad planeada 1", startDate: "27/03/2020", finishDate: "24/04/2020", descriptionProduct: "Descripción del producto 1"),
//            Orden(No: 1, BaseDocument: "Documento base 4", Container: "Contenedor 1", Tag: "Tag 1", PlannedQuantity: "cantidad planeada 1", startDate: "27/03/2020", finishDate: "24/04/2020", descriptionProduct: "Descripción del producto 1")
//        ]
//
//        let penddingOrders =
//        [
//            Orden(No: 1, BaseDocument: "Documento base 1", Container: "Contenedor 1", Tag: "Tag 1", PlannedQuantity: "cantidad planeada 1", startDate: "27/03/2020", finishDate: "24/04/2020", descriptionProduct: "Descripción del producto 1"),
//            Orden(No: 1, BaseDocument: "Documento base 2", Container: "Contenedor 1", Tag: "Tag 1", PlannedQuantity: "cantidad planeada 1", startDate: "27/03/2020", finishDate: "24/04/2020", descriptionProduct: "Descripción del producto 1"),
//            Orden(No: 1, BaseDocument: "Documento base 3", Container: "Contenedor 1", Tag: "Tag 1", PlannedQuantity: "cantidad planeada 1", startDate: "27/03/2020", finishDate: "24/04/2020", descriptionProduct: "Descripción del producto 1"),
//            Orden(No: 1, BaseDocument: "Documento base 4", Container: "Contenedor 1", Tag: "Tag 1", PlannedQuantity: "cantidad planeada 1", startDate: "27/03/2020", finishDate: "24/04/2020", descriptionProduct: "Descripción del producto 1"),
//            Orden(No: 1, BaseDocument: "Documento base 5", Container: "Contenedor 1", Tag: "Tag 1", PlannedQuantity: "cantidad planeada 1", startDate: "27/03/2020", finishDate: "24/04/2020", descriptionProduct: "Descripción del producto 1"),
//            Orden(No: 1, BaseDocument: "Documento base 6", Container: "Contenedor 1", Tag: "Tag 1", PlannedQuantity: "cantidad planeada 1", startDate: "27/03/2020", finishDate: "24/04/2020", descriptionProduct: "Descripción del producto 1")
//        ]
//
//        let finishedOrders =
//        [
//            Orden(No: 1, BaseDocument: "Documento base 1", Container: "Contenedor 1", Tag: "Tag 1", PlannedQuantity: "cantidad planeada 1", startDate: "27/03/2020", finishDate: "24/04/2020", descriptionProduct: "Descripción del producto 1")
//        ]
//
//        let reasignedOrders =
//        [
//            Orden(No: 1, BaseDocument: "Documento base 1", Container: "Contenedor 1", Tag: "Tag 1", PlannedQuantity: "cantidad planeada 1", startDate: "27/03/2020", finishDate: "24/04/2020", descriptionProduct: "Descripción del producto 1")
//        ]
//
//        self.dataStatus.onNext([
//            Section( statusName: "Asignadas", numberTask: assignedOrders.count, imageIndicatorStatus: "assignedStatus"),
//            Section(statusName: "En Proceso", numberTask: inProcessOrdes.count, imageIndicatorStatus: "processStatus"),
//            Section(statusName: "Pendientes", numberTask: penddingOrders.count, imageIndicatorStatus: "pendingStatus"),
//            Section(statusName: "Terminado", numberTask: finishedOrders.count, imageIndicatorStatus: "finishedStatus"),
//            Section(statusName: "Reasignado", numberTask: reasignedOrders.count, imageIndicatorStatus: "reassignedStatus")
//        ])
        
//        self.dataStatus = ([
//                Section( statusName: "Asignadas", numberTask: assignedOrders.count, imageIndicatorStatus: "assignedStatus"),
//                Section(statusName: "En Proceso", numberTask: inProcessOrdes.count, imageIndicatorStatus: "processStatus"),
//                Section(statusName: "Pendientes", numberTask: penddingOrders.count, imageIndicatorStatus: "pendingStatus"),
//                Section(statusName: "Terminado", numberTask: finishedOrders.count, imageIndicatorStatus: "finishedStatus"),
//                Section(statusName: "Reasignado", numberTask: reasignedOrders.count, imageIndicatorStatus: "reassignedStatus")
//            ])
    }
    
}
