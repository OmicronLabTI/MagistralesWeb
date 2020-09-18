//
//  Constants.swift
//  Omicron
//
//  Created by Diego Cárcamo on 09/07/2020.
//  Copyright © 2020 Diego Cárcamo. All rights reserved.
//

import Foundation
import UIKit

struct Constants {
    enum Errors: String {
        case errorTitle = "Error"
        case errorData = "Lo sentimos, ocurrió un error al obtener la información"
        case errorSave = "Lo sentimos, ocurrió un error al guardar la información"
        case serverError = "Lo sentimos, ocurrió un error en el servidor"
        case unauthorized = "Lo sentimos, las credenciales son inválidas"
    }
    
    enum Tags: Int {
        case loading = 101
        case moreIndicator = 201
    }
    
    enum Components: Int {
        case offset = 0
        case limit = 20
    }
}

struct ViewControllerIdentifiers {
    static let inboxViewController = "InboxViewController"
    static let storieboardName = "Main"
    static let splitViewController = "SplitViewController"
    static let cardCollectionViewCell = "CardCollectionViewCell"
    static let cardReuseIdentifier = "card"
    static let rootTableViewCell = "RootTableViewCell"
    static let orderDetailViewController = "OrderDetailViewController"
    static let detailTableViewCell = "DetailTableViewCell"
    static let orderDetailFormViewController = "OrderDetailFormViewController"
    static let loginViewController = "LoginViewController"
    static let lotsViewController = "LotsViewController"
    static let commentsViewController = "CommentsViewController"
    static let lotsTableViewCell = "LotsTableViewCell"
    static let lotsSelectedTableViewCell = "LotsSelectedTableViewCell"
    static let lotsAvailableTableViewCell = "LotsAvailableTableViewCell"
    static let signaturePadViewController = "SignaturePadViewController"
    static let componentsViewController = "ComponentsViewController"
    static let componentsTableViewCell = "ComponentsTableViewCell"
    static let headerCollectionViewCell = "HeaderCollectionViewCell"
    static let headerReuseIdentifier = "header"
}

struct OmicronColors {
    static let blue = UIColor.init(red: 84/255, green: 128/255, blue: 166/255, alpha: 1)
    static let ligthGray = UIColor.init(red: 246/255, green: 246/255, blue: 246/255, alpha: 1)
    static let assignedStatus = UIColor.init(red: 12/255, green: 204/255, blue: 86/255, alpha: 1)
    static let processStatus = UIColor.init(red: 255/255, green: 0/255, blue: 0/255, alpha: 1)
    static let pendingStatus = UIColor.init(red: 255/255, green: 184/255, blue: 0/255, alpha: 1)
    static let finishedStatus = UIColor.init(red: 28/255, green: 124/255, blue: 213/255, alpha: 1)
    static let reassignedStatus = UIColor.init(red: 186/255, green: 49/255, blue: 237/255, alpha: 1)
    static let tableStatus = UIColor.init(red: 233/255, green: 233/255, blue: 233/255, alpha: 1)
    static let tableColorRow = UIColor.init(red: 192/255, green: 219/255, blue: 243/255, alpha: 1)
    static let comments = UIColor.init(red: 231/255, green: 231/255, blue: 231/255, alpha: 1)
}

struct UsersDefaultsConstants {
    static let isLogged = "isLogged"
    static let accessToken = "accessToken"
    static let loginData = "loginData"
    static let username = "username"
    static let userData = "userData"
    static let qfbSignature = "qfbSignature"
    static let technicalSignature = "technicalSignature"
}

struct ImagesNames {
    static let openEye = "ojo.png"
    static let closeEye = "esconder.png"
}

struct CommonStrings {
    static let login = "Login"
    static let logIntoYourAccount = "Ingresa a tu cuenta"
    static let enter = "Entrar"
    static let user = "Usuario"
    static let password = "Contraseña"
    static let Emty = ""
    static let OK = "OK"
    static let cancel = "Cancelar"
    static let searchOrden = "Buscar orden / pedido"
    static let signatureViewTitleQFB = "Firma del  QFB"
    static let signatureViewTitleTechnical = "Firma del Técnico"
    static let addComponentTitle = "Agregar Componentes"
    static let confirmationMessagePendingStatus = "La orden cambiará a estatus Pendiente, ¿quieres continuar?"
    static let confirmationMessageProcessStatus = "La orden cambiará a estatus En proceso ¿quieres continuar?"
    static let errorToChangeStatus = "Ocurrió un error al cambiar de estatus la orden, por favor intente de nuevo"
    static let process = "Proceso"
    static let pending = "Pendiente"
}

struct FontsNames {
    static let SFProDisplayBold = "SFProDisplay-Bold"
    static let SFProDisplayRegular = "SFProDisplay-Regular"
    static let SFProDisplayMedium = "SFProDisplay-Medium"
}

struct StatusNameConstants {
    static let assignedStatus = "Asignadas"
    static let inProcessStatus = "En proceso"
    static let penddingStatus = "Pendiente"
    static let finishedStatus = "Terminado"
    static let reassignedStatus = "Reasignado"
    static let addComponent = "Agregar componente"
    static let save = "Guardar"
    static let seeLots = "Ver Lotes"
}

struct ImageButtonNames {
    static let assigned = "showAssignedDetailButton.png"
    static let inProcess = "showProcessDetailButton.png"
    static let pendding = "showPenddingDetailButton.png"
    static let finished = "showFinishedDetailButton.png"
    static let reasigned = "showReasignedDetailButton.png"
    static let backAssigned = "backAssigned.png"
    static let backInProcess = "backProcess.png"
    static let backFinished = "backFinished.png"
    static let backReassigned = "backReassigned.png"
    static let backPendding = "backPendding.png"
    static let logout = "logout.png"
    static let addLot = "proximo.png"
    static let removeLot = "espalda.png"
    static let similarityView = "square.grid.2x2"
    static let normalView = "rectangle.on.rectangle"
}

struct IndicatorImageStatus {
    static let assigned = "assignedStatus"
    static let inProcess = "processStatus"
    static let pendding = "pendingStatus"
    static let finished = "finishedStatus"
    static let reassined = "reassignedStatus"
}

struct FileManagerConstants {
    static let qfbSignatureName = "qfbSignature.jpg"
    static let technicalSignatureName = "tecnicalSignature.jpg"
}
