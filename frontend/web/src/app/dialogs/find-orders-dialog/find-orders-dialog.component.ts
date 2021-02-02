import {Component, Inject, OnDestroy, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import { CONST_STRING, CONST_USER_DIALOG, ConstOrders, MODAL_FIND_ORDERS} from '../../constants/const';
import { MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import { PedidosService} from '../../services/pedidos.service';
import { ErrorService} from '../../services/error.service';
import { QfbSelect} from '../../model/http/users';
import { Subscription} from 'rxjs';
import { UsersService} from '../../services/users.service';
import {DataService} from '../../services/data.service';


@Component({
    selector: 'app-find-orders-dialog',
    templateUrl: './find-orders-dialog.component.html',
    styleUrls: ['./find-orders-dialog.component.scss']
})
export class FindOrdersDialogComponent implements OnInit, OnDestroy {
    findOrdersForm: FormGroup;
    fullDate: string [] = [];
    qfbsSelect: QfbSelect[] = [];
    maxDate = new Date();
    dateFin = new Date();
    isBeginInitForm = true;
    isToResetData = false;
    subscriptionForm = new Subscription();
    isFromSearchOrders = false;
    defaultStartDate: Date;
    defaultEndDate: Date;
    constructor(private formBuilder: FormBuilder,
                @Inject(MAT_DIALOG_DATA) public filterData: any,
                private dialogRef: MatDialogRef<FindOrdersDialogComponent>,
                private ordersServices: PedidosService,
                private errorService: ErrorService,
                private usersService: UsersService,
                public dataService: DataService) {
        console.log('DataReceive: ', this.filterData)
        this.isFromSearchOrders = this.filterData.modalType === ConstOrders.modalOrders;
        this.fullDate = this.filterData.filterOrdersData.dateFull.split('-');
        this.findOrdersForm = this.formBuilder.group({
            docNum: ['', [Validators.maxLength(50)]],
            dateType: ['', []],
            fini: ['', []],
            ffin: ['', []],
            status: ['', []],
            qfb: ['', []],
            productCode: ['', [Validators.maxLength(40)]],
            clientName: ['', [Validators.maxLength(80)]],
            label: ['', []],
            finlabel: ['', []],
            orderIncidents: ['', []]
        });
        this.isToResetData = // add more filter to receive
            this.filterData.filterOrdersData.docNum || this.filterData.filterOrdersData.status || this.filterData.filterOrdersData.qfb
            || this.filterData.filterOrdersData.dateType !== ConstOrders.defaultDateInit || this.filterData.filterOrdersData.productCode
            || this.filterData.filterOrdersData.clientName || this.filterData.filterOrdersData.label
            || this.filterData.filterOrdersData.finlabel || this.filterData.filterOrdersData.orderIncidents;
    }

    async ngOnInit() {
        await this.usersService.getRoles().toPromise().then( resultRoles =>
            resultRoles.response.filter( resRole => resRole.description === CONST_USER_DIALOG.defaultQfb)[0].id)
            .then( resRole => this.ordersServices.getQfbs(resRole).toPromise())
            .then(resultQfb => {
                this.qfbsSelect = resultQfb.response.map(qfb => {
                    return {
                        qfbId: qfb.id,
                        qfbName: qfb.firstName + ' ' + qfb.lastName
                    };
                });
                this.qfbsSelect.sort((a, b) => {
                    return a.qfbName.localeCompare(b.qfbName);
                });
                this.findOrdersForm.get('qfb').setValue(this.filterData.filterOrdersData.qfb ? this.filterData.filterOrdersData.qfb : '' );
            }).catch(error => {
                this.dialogRef.close();
                this.errorService.httpError(error);
            });

        this.findOrdersForm.get('docNum').setValue(this.filterData.filterOrdersData.docNum ? this.filterData.filterOrdersData.docNum : '');
        const initDateTrans = this.fullDate[0].split('/');
        const finishDateTrans = this.fullDate[1].split('/');

        this.defaultStartDate = new Date(`${initDateTrans[1]}/${initDateTrans[0]}/${initDateTrans[2]}`);
        this.defaultEndDate = new Date(`${finishDateTrans[1]}/${finishDateTrans[0]}/${finishDateTrans[2]}`);

        this.findOrdersForm.get('fini').setValue(this.defaultStartDate);
        this.findOrdersForm.get('ffin').setValue(this.defaultEndDate);

        this.findOrdersForm.get('dateType').setValue(this.filterData.filterOrdersData.dateType ?
            this.filterData.filterOrdersData.dateType : ConstOrders.defaultDateInit);
        this.findOrdersForm.get('status').setValue(this.filterData.filterOrdersData.status ? this.filterData.filterOrdersData.status : '');
        this.findOrdersForm.get('productCode').setValue(this.filterData.filterOrdersData.productCode ?
            this.filterData.filterOrdersData.productCode : '');
        this.findOrdersForm.get('clientName').setValue(this.filterData.filterOrdersData.clientName ?
            this.filterData.filterOrdersData.clientName : '');
        this.findOrdersForm.get('label').setValue(this.filterData.filterOrdersData.label ?
            this.filterData.filterOrdersData.label : '');
        this.findOrdersForm.get('finlabel').setValue(this.filterData.filterOrdersData.finlabel ?
            this.filterData.filterOrdersData.finlabel : '');
        this.findOrdersForm.get('orderIncidents').setValue(this.filterData.filterOrdersData.orderIncidents ?
            this.filterData.filterOrdersData.orderIncidents : '');

        if (this.filterData.filterOrdersData.docNum) {
            this.getDisableForDocNum();
        } else if (this.filterData.filterOrdersData.qfb || this.filterData.filterOrdersData.status) {
            this.getDisableOnlyForDocNum();
        }
        this.getMaxDate();
        this.subscriptionForm = this.findOrdersForm.valueChanges.subscribe(formData => {
            if (!this.isBeginInitForm) {
                if (this.withValue(formData.docNum)) {
                    this.isToResetData = false;
                    this.getDisableForDocNum();
                } else if (!this.withValue(formData.docNum) &&
                    (
                        (this.withValue(formData.fini) && !this.isEqualDate(new Date(formData.fini), this.defaultStartDate)) ||
                        (this.withValue(formData.ffin) && !this.isEqualDate(new Date(formData.ffin), this.defaultEndDate)) ||
                        (this.withValue(formData.dateType) && formData.dateType !== ConstOrders.defaultDateInit) ||
                        this.withValue(formData.status) || this.withValue(formData.qfb) ||
                        this.withValue(formData.productCode) || this.withValue(formData.clientName) ||
                        this.withValue(formData.label) || this.withValue(formData.finlabel)
                        || this.withValue(formData.orderIncidents)
                    )) {
                    this.changeValidatorsForDocNum();
                } else {
                    this.enableAllInputs();
                }
            }
            this.isBeginInitForm = false;
            this.getMaxDate();
        });
    }

    isEqualDate(dateToCompare: Date, baseDate: Date) {
        return dateToCompare.getDate() === baseDate.getDate() &&
                dateToCompare.getUTCMonth() === baseDate.getUTCMonth() &&
                dateToCompare.getFullYear() === baseDate.getFullYear();
    }
    withValue(value) {
        return value !== null && value !== undefined && value !== '';
    }

    searchOrders() {
        this.trimFilterValues();
        this.dialogRef.close({...this.findOrdersForm.value,
                             isFromOrders: this.filterData.filterOrdersData.isFromOrders,
                             isFromIncidents: this.filterData.filterOrdersData.isFromIncidents});
    }

    getMaxDate() {
        this.dateFin.setTime(new Date(this.findOrdersForm.get('ffin').value).getTime());
        this.maxDate.setTime(new Date(this.findOrdersForm.get('fini').value).getTime() + MODAL_FIND_ORDERS.ninetyDays);
    }
    resetSearchParams() {
        this.getDisableForDocNum();
        this.getDisableOnlyForDocNum();
        this.enableAllParamsSearch();
    }
    getDisableForDocNum() {
        this.isToResetData = true;
        this.isBeginInitForm = true;
        this.findOrdersForm.get('dateType').disable({onlySelf: true, emitEvent: false});
        this.findOrdersForm.get('fini').disable({onlySelf: true, emitEvent: false});
        this.findOrdersForm.get('ffin').disable({onlySelf: true, emitEvent: false});
        this.findOrdersForm.get('status').disable({onlySelf: true, emitEvent: false});
        this.findOrdersForm.get('qfb').disable({onlySelf: true, emitEvent: false});
        this.findOrdersForm.get('productCode').disable({onlySelf: true, emitEvent: false});
        this.findOrdersForm.get('clientName').disable({onlySelf: true, emitEvent: false});
        this.findOrdersForm.get('label').disable({onlySelf: true, emitEvent: false});
        this.findOrdersForm.get('finlabel').disable({onlySelf: true, emitEvent: false});
        this.findOrdersForm.get('orderIncidents').disable({onlySelf: true, emitEvent: false});
    }
    getDisableOnlyForDocNum() {
        this.findOrdersForm.get('docNum').disable({onlySelf: true, emitEvent: false});
    }
    resetParamsValue() {
        this.findOrdersForm.get('dateType').setValue(ConstOrders.defaultDateInit);
        this.findOrdersForm.get('docNum').setValue('');
        this.findOrdersForm.get('qfb').setValue( '' );
        this.findOrdersForm.get('status').setValue('');
        this.findOrdersForm.get('productCode').setValue('');
        this.findOrdersForm.get('clientName').setValue('');
        this.findOrdersForm.get('label').setValue('');
        this.findOrdersForm.get('finlabel').setValue('');
        this.findOrdersForm.get('orderIncidents').setValue('');
    }
    enableAllParamsSearch() {
        this.getDisableForDocNum();
        this.getDisableOnlyForDocNum();
        this.resetParamsValue();
        this.enableAllInputs();
    }
    enableAllInputs() {
        this.findOrdersForm.get('dateType').enable({onlySelf: true, emitEvent: false});
        this.findOrdersForm.get('status').enable({onlySelf: true, emitEvent: false});
        this.findOrdersForm.get('qfb').enable({onlySelf: true, emitEvent: false});
        this.findOrdersForm.get('docNum').enable({onlySelf: true, emitEvent: false});
        this.findOrdersForm.get('fini').enable({onlySelf: true, emitEvent: false});
        this.findOrdersForm.get('ffin').enable({onlySelf: true, emitEvent: false});
        this.findOrdersForm.get('productCode').enable({onlySelf: true, emitEvent: false});
        this.findOrdersForm.get('clientName').enable({onlySelf: true, emitEvent: false});
        this.findOrdersForm.get('label').enable({onlySelf: true, emitEvent: false});
        this.findOrdersForm.get('finlabel').enable({onlySelf: true, emitEvent: false});
        this.findOrdersForm.get('orderIncidents').enable({onlySelf: true, emitEvent: false});
    }
    changeValidatorsForDocNum() {
        this.isToResetData = true;
        this.isBeginInitForm = true;
        this.getDisableOnlyForDocNum();
    }
    ngOnDestroy() {
        this.subscriptionForm.unsubscribe();
    }

    keyDownFunction(event: KeyboardEvent) {
        if (event.key === MODAL_FIND_ORDERS.keyEnter && ((this.findOrdersForm.get('docNum').value !== CONST_STRING.empty
            && this.findOrdersForm.get('docNum').value !== null) || (this.findOrdersForm.get('productCode').value !== CONST_STRING.empty
            && this.findOrdersForm.get('productCode').value !== null) || (this.findOrdersForm.get('clientName').value !== CONST_STRING.empty
            && this.findOrdersForm.get('clientName').value !== null) || (this.findOrdersForm.get('label').value !== CONST_STRING.empty
            && this.findOrdersForm.get('label').value !== null) || (this.findOrdersForm.get('finlabel').value !== CONST_STRING.empty
            && this.findOrdersForm.get('finlabel').value !== null) ||
            (this.findOrdersForm.get('orderIncidents').value !== CONST_STRING.empty
                && this.findOrdersForm.get('orderIncidents').value !== null))) {
            this.searchOrders();
        }
    }

    trimFilterValues() {
        this.findOrdersForm.get('clientName').setValue((this.findOrdersForm.get('clientName').value || '').trim());
        this.findOrdersForm.get('productCode').setValue((this.findOrdersForm.get('productCode').value || '').trim());
    }

    changeDocNumber(event: KeyboardEvent) {
        let invalidChars = [ "-", "+", "e", "." ];
        let currentValue = this.findOrdersForm.get('docNum').value;
        if (invalidChars.includes(event.key) || (event.key == '0' && !this.withValue(currentValue)))
        {
            event.preventDefault();
        }
        if (this.withValue(currentValue) && `${currentValue}`.length == 7 && !isNaN(event.key as any))
        {
            event.preventDefault();
        }
    }
}
