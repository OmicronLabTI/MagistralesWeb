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
    let arrayData = Observable.just([
        Section(index: 1, statusName: "Asignadas", numberTask: 5, imageIndicatorStatus: "assignedStatus"),
        Section(index: 1,statusName: "En Proceso", numberTask: 5, imageIndicatorStatus: "processStatus"),
        Section(index: 1,statusName: "Pendientes", numberTask: 1, imageIndicatorStatus: "pendingStatus"),
        Section(index: 1, statusName: "Terminado", numberTask: 1, imageIndicatorStatus: "finishedStatus"),
        Section(index: 1, statusName: "Reasignado", numberTask: 1, imageIndicatorStatus: "reassignedStatus")])
    
}
