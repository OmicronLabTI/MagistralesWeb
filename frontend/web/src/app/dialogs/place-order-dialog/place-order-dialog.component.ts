import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { PedidosService } from '../../services/pedidos.service';
import { ErrorService } from '../../services/error.service';
import { Clasification, QfbClassification, QfbWithNumber } from '../../model/http/users';
import { CONST_NUMBER, CONST_STRING, defaultClasificationColor, initialClasificationModal, MODAL_NAMES } from '../../constants/const';
import { ObservableService } from 'src/app/services/observable.service';
import { UsersService } from 'src/app/services/users.service';
import { DataService } from '../../services/data.service';
import { LocalStorageService } from 'src/app/services/local-storage.service';

@Component({
    selector: 'app-place-order-dialog',
    templateUrl: './place-order-dialog.component.html',
    styleUrls: ['./place-order-dialog.component.scss']
})
export class PlaceOrderDialogComponent implements OnInit {
    isPlaceManual = false;
    idQfbSelected = '';
    qfbs: QfbWithNumber[] = [];
    currentQfbs: QfbWithNumber[] = [];
    currentQfbType = CONST_STRING.empty;
    clasificationList: Clasification[] = [];
    constructor(
        private dialogRef: MatDialogRef<PlaceOrderDialogComponent>,
        private ordersServices: PedidosService,
        private errorService: ErrorService,
        @Inject(MAT_DIALOG_DATA) public placeData: any,
        private observableService: ObservableService,
        private localstorageService: LocalStorageService) {
        if (this.placeData.placeOrdersData) {
            this.idQfbSelected = this.placeData.placeOrdersData.userId ? this.placeData.placeOrdersData.userId : '';
        }
        if (this.placeData.placeOrdersData.qfbClassification) {
            this.changeTypeQfb(this.placeData.placeOrdersData.qfbClassification, true);
        } else {
            this.changeTypeQfb(initialClasificationModal, true);
        }
        this.isPlaceManual = this.idQfbSelected !== CONST_STRING.empty;
    }

    async ngOnInit() {
        await this.ordersServices.getQfbsWithOrders().toPromise().then(resultQfbs => {
            const newResponse = resultQfbs.response.filter(qfb =>
                !this.placeData.placeOrdersData.isFromReassign ? qfb.asignable === CONST_NUMBER.one :
                    qfb.asignable === CONST_NUMBER.one || qfb.asignable === CONST_NUMBER.zero);
            newResponse.forEach(qfbNew => {
                qfbNew.countTotalOrders = new Intl.NumberFormat().format(Number(qfbNew.countTotalOrders));
                qfbNew.countTotalFabOrders = new Intl.NumberFormat().format(Number(qfbNew.countTotalFabOrders));
                qfbNew.countTotalPieces = new Intl.NumberFormat().format(Number(qfbNew.countTotalPieces));
                qfbNew.clasification = qfbNew.clasification || CONST_STRING.empty;
            });
            this.qfbs = newResponse;
            this.changeCurrentQfbs();
        }).catch(error => {
            this.errorService.httpError(error);
            this.dialogRef.close();
        });
        this.clasificationList = this.localstorageService.getClasificationList().sort((a, b) =>
            a.value.localeCompare(b.value));
    }

    changePlaceManual() {
        this.isPlaceManual = true;
    }

    placeOrder(userId: string, userName: string) {
        this.observableService.setQbfToPlace({
            userId, userName,
            modalType: this.placeData.placeOrdersData.modalType, list: this.placeData.placeOrdersData.list,
            assignType: MODAL_NAMES.assignManual, isFromOrderIsolated: this.placeData.placeOrdersData.isFromOrderIsolated,
            isFromReassign: this.placeData.placeOrdersData.isFromReassign, clasification: this.currentQfbType
        });
        this.dialogRef.close();
    }

    placeOrderAutomatic() {
        this.observableService.setQbfToPlace({
            modalType: this.placeData.placeOrdersData.modalType,
            list: this.placeData.placeOrdersData.list,
            assignType: MODAL_NAMES.assignAutomatic
        });
        this.dialogRef.close();
    }
    changeTypeQfb(qfbClassification: string, isFromInit: boolean = false) {
        this.currentQfbType = qfbClassification;
        if (!isFromInit) {
            this.changeCurrentQfbs();
        }
    }
    changeCurrentQfbs() {
        this.currentQfbs = this.qfbs.filter(qfb => qfb.clasification.toLowerCase() === this.currentQfbType.toLowerCase());
    }
}
