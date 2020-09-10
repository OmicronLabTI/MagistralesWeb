import {Component, Inject, OnDestroy, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {CONST_STRING, CONST_USER_DIALOG, ConstOrders, MODAL_FIND_ORDERS} from '../../constants/const';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import {PedidosService} from '../../services/pedidos.service';
import {ErrorService} from '../../services/error.service';
import {QfbSelect} from '../../model/http/users';
import {Subscription} from 'rxjs';
import {UsersService} from '../../services/users.service';


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
  isBeginInitForm = true;
  isToResetData = false;
  subscriptionForm = new Subscription();
  isFromSearchOrders = false;
  constructor(private formBuilder: FormBuilder,
              @Inject(MAT_DIALOG_DATA) public filterData: any,
              private dialogRef: MatDialogRef<FindOrdersDialogComponent>,
              private ordersServices: PedidosService,
              private errorService: ErrorService,
              private usersService: UsersService) {
      console.log('data Receive: ', this.filterData.filterOrdersData)
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
    });
      this.isToResetData = // add more filter to receive
          this.filterData.filterOrdersData.docNum || this.filterData.filterOrdersData.status || this.filterData.filterOrdersData.qfb
          || this.filterData.filterOrdersData.dateType !== ConstOrders.defaultDateInit || this.filterData.filterOrdersData.productCode
          || this.filterData.filterOrdersData.clientName;
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

      this.findOrdersForm.get('fini').setValue(new Date(`${initDateTrans[1]}/${initDateTrans[0]}/${initDateTrans[2]}`));
      this.findOrdersForm.get('ffin').setValue(new Date(`${finishDateTrans[1]}/${finishDateTrans[0]}/${finishDateTrans[2]}`));

      this.findOrdersForm.get('dateType').setValue(this.filterData.filterOrdersData.dateType ?
          this.filterData.filterOrdersData.dateType : ConstOrders.defaultDateInit);
      this.findOrdersForm.get('status').setValue(this.filterData.filterOrdersData.status ? this.filterData.filterOrdersData.status : '');
      this.findOrdersForm.get('productCode').setValue(this.filterData.filterOrdersData.productCode ?
          this.filterData.filterOrdersData.productCode : '');
      this.findOrdersForm.get('clientName').setValue(this.filterData.filterOrdersData.clientName ?
          this.filterData.filterOrdersData.clientName : '');
      this.getMaxDate();
      if (this.filterData.filterOrdersData.docNum) {
          this.getDisableForDocNum();
      } else if (this.filterData.filterOrdersData.qfb || this.filterData.filterOrdersData.status) {
          this.getDisableOnlyForDocNum();
      }
      this.subscriptionForm = this.findOrdersForm.valueChanges.subscribe(formData => {
          console.log('dataChanging: ', formData)
          if (!this.isBeginInitForm) {
              if (formData.docNum !== null && formData.docNum) {
                  this.isToResetData = false;
                  this.getDisableForDocNum();
              } else if (formData.docNum !== null) {
                  this.changeValidatorsForDocNum();
              } else if (formData.docNum === '' && (formData.dateType !== '' && formData.dateType ||
                  formData.fini !== '' && formData.fini ||
                  formData.ffin !== '' && formData.ffin ||
                  formData.status !== '' && formData.status ||
                  formData.qfb !== '' && formData.qfb ||
                  formData.productCode !== '' && formData.productCode ||
                  formData.clientName !== '' && formData.clientName)) {
                  this.changeValidatorsForDocNum();
              }
          }
          this.isBeginInitForm = false;
          this.getMaxDate();
      });

  }

  searchOrders() {
      this.dialogRef.close({...this.findOrdersForm.value, isFromOrders: this.filterData.filterOrdersData.isFromOrders});
  }

  getMaxDate() {
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
  }
  enableAllParamsSearch() {
      this.getDisableForDocNum();
      this.getDisableOnlyForDocNum();
      this.resetParamsValue();
      this.findOrdersForm.get('dateType').enable({onlySelf: true, emitEvent: false});
      this.findOrdersForm.get('status').enable({onlySelf: true, emitEvent: false});
      this.findOrdersForm.get('qfb').enable({onlySelf: true, emitEvent: false});
      this.findOrdersForm.get('docNum').enable({onlySelf: true, emitEvent: false});
      this.findOrdersForm.get('fini').enable({onlySelf: true, emitEvent: false});
      this.findOrdersForm.get('ffin').enable({onlySelf: true, emitEvent: false});
      this.findOrdersForm.get('productCode').enable({onlySelf: true, emitEvent: false});
      this.findOrdersForm.get('clientName').enable({onlySelf: true, emitEvent: false});
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
            && this.findOrdersForm.get('productCode').value !== null))) {
            this.searchOrders();
      }
    }
}
