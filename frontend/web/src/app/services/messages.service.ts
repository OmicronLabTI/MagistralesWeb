import { Injectable } from '@angular/core';
import Swal, { SweetAlertIcon } from 'sweetalert2';
import { CONST_NUMBER, CONST_STRING, MessageType } from '../constants/const';
import { CancelOrderReq } from '../model/http/pedidos';

@Injectable({
  providedIn: 'root'
})
export class MessagesService {

  constructor() { }
  presentToastCustom(
    title: string,
    icon: SweetAlertIcon,
    html: string = CONST_STRING.empty,
    showConfirmButton: boolean = false,
    showCancelButton: boolean = false,
    popupCustom = CONST_STRING.empty
  ) {
    return new Promise((resolve) => {
      Swal.fire({
        title,
        html,
        icon,
        timer: showConfirmButton ? CONST_NUMBER.zero : CONST_NUMBER.timeToast,
        showConfirmButton,
        showCancelButton,
        heightAuto: false,
        confirmButtonText: 'Aceptar',
        cancelButtonText: 'Cancelar',
        buttonsStyling: false,

        customClass: {
          container: 'swal2-actions',
          popup: popupCustom,
          confirmButton: 'confirm-button-class',
          cancelButton: 'cancel-button-class',
          title:
            popupCustom !== CONST_STRING.empty
              ? 'swal2-title2'
              : CONST_STRING.empty,
          content:
            popupCustom !== CONST_STRING.empty
              ? 'swal2-title2'
              : CONST_STRING.empty,
        },
      }).then((result) => resolve(result));
    });
  }

  getMessageTitle(
    itemsWithError: any[],
    messageType: MessageType,
    isFromCancel = false
  ): string {
    let errorOrders = '';
    let firstMessage = '';
    let finishMessaje = '';
    switch (messageType) {
      case MessageType.processOrder:
      case MessageType.processDetailOrder:
        firstMessage = '';
        finishMessaje = '\n';
        break;
      case MessageType.placeOrder:
        firstMessage = 'La orden de fabricación ';
        finishMessaje = 'no pudo ser Asignada \n';
        break;
      case MessageType.saveBatches:
        firstMessage = 'Error al asignar lotes a ';
        finishMessaje = ', por favor verificar \n';
        break;
      case MessageType.materialRequest:
        firstMessage = 'Ya se ha generado una solicitud para la orden ';
        finishMessaje = '\n';
        break;
      case MessageType.ordersWithoutQr:
        firstMessage = 'La orden de fabricación ';
        finishMessaje = 'no cuenta con código qr \n';
        break;
    }
    if (!isFromCancel) {
      itemsWithError.forEach((order: string) => {
        errorOrders += `${firstMessage} ${order} ${finishMessaje}`;
      });
    } else {
      itemsWithError.forEach((order: CancelOrderReq) => {
        errorOrders += `${order.reason} \n`;
      });
    }
    return errorOrders;
  }
}
