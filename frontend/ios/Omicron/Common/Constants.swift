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
    static let showErrorViewController = "showError"
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
    static let OKConst = "Aceptar"
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
    static let formulaDetailCouldNotBeLoaded = "Hubo un error al cargar el detalle " +
    "de la orden de fabricación, intentar de nuevo"
    static let couldNotDeleteItem = "Hubo un error al eliminar el elemento,  intente de nuevo"
    static let qfbSignature = "Firma del  QFB"
    static let orderCouldNotBeCompleted = "La orden no puede ser Terminada, " +
    "revisa que todos los artículos tengan un lote asignado"
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
    static let errorInComments = "Ocurrió un error al guardar los comentarios, por favor intentarlo de nuevo"
    static let version = Bundle.main.infoDictionary?["CFBundleShortVersionString"] as? String ?? ""
    static let build = Bundle.main.infoDictionary?["CFBundleVersion"] as? String ?? ""
    static let errorLoadingOrders = "Ocurrió un error al cargar las órdenes de fabricación, por favor inicia sesión nuevamente"
    static let options = ["AMP", "BE", "GENERAL", "INCI", "MER", "MG", "MN",
                          "MP", "PROD", "PRONATUR", "PT", "TALLERES", "WEB"]
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
    // swiftlint:disable line_length
    static let test1 = "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\r\nNNG0Y9O1p3OmH8okZKZl7s58y4nj1HD6Z7BnbJzRxw3hruU+4QXzVvN6+AH8lfxH\r\nWQFZK7O6swOyV2X3CIIEVYJeYYhwg/BZTmTOlpyXuXG5O3Pfi9JE+/LIeZl5TWI9\r\nca74TL5lfmH+FYmLpFzSMdNv5pqZfdJoaa0MkU2VNcoN4J/SNoWT4gtFZ0FgQXXB\r\nq1mpsw4V6haKC9uKnIsWFz0qDi/eMRufzZ3dUmJdsqCkcw5rzta5yNwZc1vm2c4r\r\nm9c1P2L+rgWaC3IX/FjKKF1Z+sfCtIXNZRZl88sefhHxxZ5yWrm0/OYi/0VbvsS/\r\nFH55cfHYxesXv6vgVfxQyaisqny7hLvkh688vlr31fulWUsvLvNetnk5cbl4+Y0V\r\nQSt2rdRdWbzy4aqJqxpW01dXrP5jzfQ156vGVW1Zq7lWsbZjXcy6xvV265evf7tB\r\nsKG9ekL1vo3mGxdvfLmJt+nq5uDN9VsstlRuefO18OtbWyO2NtQ41FRtI24r2PbL\r\n9tTtrTuYO+pqzWora//cKd7ZsStx15k6n7q63ea7l+1B9yj29OydsvfyN6HfNNa7\r\n1W/dZ7Svcj/Yr9j/+EDmgRsHow+2HGIeqv/W/tuNh/UPVzQgDUUNfUcERzoa0xuv\r\nNEU1tTT7Nx/+zv27nUetj1YfMzy27Ljm8bLj708Un+g/KTnZeyr71MOW6S13T086\r\nff1MwpmLZ6PPnvs+/PvTrazWE+cCzh0973e+6QfmD0cueF9oaPNqO/yj14+HL3pf\r\nbLjkc6nxsu/l5ivjrxy/GnT11LXQa99fZ1+/0B7bfuVGyo1bN6fc7LjFu9V9W3T7\r\n2Z2COwN358OLfcV9nftVD8wf1Pxr9L/2dXh3HOsM7Wz7Kemnuw+5D5/8LPv5bVfZ\r\nL9Rfqh5ZParr9uw+2hPec/nx5MddTyRPBnrLf9X9deNTp6ff/hb8W1vfpL6uZ9Jn\r\n739f8tz0+c4/xv3R0h/f/+BF3ouBlxWvTF/tes183fom7c2jgVlvSW/X/Tn6z+Z3\r\n0e/uvc97//7fCQ/4Yk7kYoUAAAA4ZVhJZk1NACoAAAAIAAGHaQAEAAAAAQAAABoA\r\nAAAAAAKgAgAEAAAAAQAAABmgAwAEAAAAAQAAABkAAAAAezu1ZAAAABxpRE9UAAAA\r\nAgAAAAAAAAANAAAAKAAAAA0AAAAMAAABZ9fB2+sAAAEzSURBVDgRtJFLKERhGIaf\r\n4wwnidKklK2sLCgSuZSd2zBFs54I+9lYK+xl41JKFmxYz3qSFcrC0spydoqwON7v\r\nOzNmJmXnnM53e9/n///+A//4hAR/rR7Q5G/NlHJ7rW+gU0QkBpQjX7tVjhzXir+g\r\nkJYGPGkMmCMWBGG9HtDsbZpNLrnlnhsuWKVT04yABeXq7m5M7D2cUeaOfdZYJq/8\r\nQJopAaNytbuzEiLP27xxxWC9oHpWwCsFnyY+lXb+bp54ZtgFGOJUR9tQtyRghj5e\r\neKSr4nVgki8O3d6muM4HR+zQz7QDLnCs6VgVyknI/wADkia8G9e8TK9qWwhdRcyK\r\nFYt8Mq8c6QLtCk+0gz1ZGUY44Fy1KXb8DO/yS9hS06HP/jmU2FU0IKtcoOiKqXZj\r\ne8TfAAAA///MpWyMAAABK0lEQVStkb0vRFEQxX+P3X2WRKGSKESnkCDZjYaQyDZW\r\nNisKtY8VvUZFJBK9aDSEioboJTrR0ElU/gGixEZinZlrb7LZ1r2Ze2bOnfPmzjyo\r\n8c4QkCfRhlVGmKTBgnw44Vhnp3ZOWOGDqtH7fDIozMryslkJJoQwRZ0xYY9HK+KX\r\nzMvItvmhJOyWVXXxyiGbqlBnXUwQHPGtT2DVEhVF6gZbwjmJywxzzg1nFMTYKvLC\r\nE/3y7HlaiT+pwCjTEtYCGc9xLtXBrsdpZOX0ymb44pZnfX+PZRZZ44AH3jhlwFOt\r\n17isn4oqzAuL7HDNPY/cccEGfZ6V9WlGQbOXspi/t8Y7c3LebQtlf+PKx9flfaUx\r\nJUPqE21JD4GJwribcQe2Ax+4tjOMuo3+J+IXy+puKHFo3uoAAAAASUVORK5CYII="
}
