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
    static let cardIsolatedOrderCollectionViewCell = "CardIsolatedOrderCollectionViewCell"
    static let cardReuseIdentifier = "card"
    static let cardIsolatedOrderReuseIdentifier = "card_isolated_order"
    static let rootTableViewCell = "RootTableViewCell"
    static let kpiCell = "KPICell"
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
    static let darkGray = UIColor.init(red: 102, green: 106, blue: 109, alpha: 1)
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
    static let empty = ""
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
    static let product = "Producto:"
    static let noSimilarity = "Sin similitud"
    static let search = "Búsqueda"
    static let separationSpaces = "   "
    static let doYouWantToFinishTheOrder = "¿Deseas terminar la orden?"
    static let formulaDetail = "Detalle de la fórmula"
    static let updatingData = "Actualizando datos"
    static let sumOfFormula = "Sumatoria de la fórmula: "
    static let orderNumber = "Número de pedido:"
    static let container = "Envase:"
    static let tag = "Etiqueta:"
    static let manufacturingOrder = "Orden de fabricación:"
    static let plannedQuantity = "Cantidad planificada:"
    static let manufacturingDate = "Fecha de fabricación:"
    static let finishdate = "Fecha de finalización:"
    static let destiny = "Destino:"
    static let piece = "Pieza"
    static let components = "Componentes: "
    static let code = "Código"
    static let baseQuantity = "Cant. Base"
    static let pQuantity = "Cant. requerida"
    static let unit = "Unidad"
    static let warehouse = "Almacén"
    static let description = "Descripción"
    static let bold = "bold"
    static let baseDocument = "Documento base:"
    static let edit = "Editar"
    static let delete = "Eliminar"
    static let deleteComponentMessage = "El componente será eliminado, ¿quieres continuar?"
    static let formulaDetailCouldNotBeLoaded = "Hubo un error al cargar el detalle de la orden de fabricación, intentar de nuevo"
    static let couldNotDeleteItem = "Hubo un error al eliminar el elemento,  intente de nuevo"
    static let qfbSignature = "Firma del  QFB"
    static let orderCouldNotBeCompleted = "La orden no puede ser Terminada, revisa que todos los artículos tengan un lote asignado"
    static let clear = "Limpiar"
    static let accept = "Aceptar"
    static let finished = "Terminado"
    static let batchesTitle = "Lotes"
    static let documentsLines = "Líneas de documentos"
    static let hashtag = "#"
    static let articleDescription = "Descripción del artículo"
    static let warehouseCode = "Código de almacén"
    static let totalNedded = "Total necesario"
    static let totalSelect = "Total Seleccionado"
    static let batchAvailable = "Lotes Disponibles"
    static let quantityAvailable = "Cantidad disponible"
    static let quantitySelected = "Cantidad seleccionada"
    static let quantityAssigned = "Cantidad asignada"
    static let batchSelected = "Lotes seleccionados"
    static let order = "Pedido:"
    static let ordersWithoutOrder = "Órdenes sin pedido: "
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
    static let finalizedStatus = "Finalizado"
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
    static let message = "message"
    static let messsageFill = "message.fill"
    static let rectangule3offgrid = "rectangle.3.offgrid"
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

struct Base64 {
    static let test1 = "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"
}
