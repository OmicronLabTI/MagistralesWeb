import {Component, Inject, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {MODAL_FIND_ORDERS} from '../../constants/const';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import {PedidosService} from '../../services/pedidos.service';
import {ErrorService} from '../../services/error.service';
import {QfbSelect} from '../../model/http/users';


@Component({
  selector: 'app-find-orders-dialog',
  templateUrl: './find-orders-dialog.component.html',
  styleUrls: ['./find-orders-dialog.component.scss']
})
export class FindOrdersDialogComponent implements OnInit {
  findOrdersForm: FormGroup;
  fullDate: string [] = [];
  qfbsSelect: QfbSelect[] = [];
  maxDate = new Date();
  isBeginInitForm = true;
  isToResetData = false;
  constructor(private formBuilder: FormBuilder,
              @Inject(MAT_DIALOG_DATA) public filterData: any,
              private dialogRef: MatDialogRef<FindOrdersDialogComponent>,
              private ordersServices: PedidosService,
              private errorService: ErrorService) {
      this.fullDate = this.filterData.filterOrdersData.dateFull.split('-');
      this.findOrdersForm = this.formBuilder.group({
      docNum: ['', [Validators.required, Validators.maxLength(60)]],
      dateType: ['', Validators.required],
      fini: ['', [Validators.required]],
      ffin: ['', [Validators.required]],
      status: ['', []],
      qfb: ['', []],
    });
      this.isToResetData =
          this.filterData.filterOrdersData.docNum || this.filterData.filterOrdersData.status || this.filterData.filterOrdersData.qfb;
  }

  async ngOnInit() {
      await this.ordersServices.getQfbs().toPromise().then(resQfbs => {
          this.qfbsSelect = resQfbs.response.map(qfb => {
              return {
                  qfbId: qfb.id,
                  qfbName: qfb.firstName
              };
          });
          this.findOrdersForm.get('qfb').setValue(this.filterData.filterOrdersData.qfb ? this.filterData.filterOrdersData.qfb : '' );
      }).catch(error => this.errorService.httpError(error));
      this.findOrdersForm.get('docNum').setValue(this.filterData.filterOrdersData.docNum ? this.filterData.filterOrdersData.docNum : '');
      const initDateTrans = this.fullDate[0].split('/');
      const finishDateTrans = this.fullDate[1].split('/');

      this.findOrdersForm.get('fini').setValue(new Date(`${initDateTrans[1]}/${initDateTrans[0]}/${initDateTrans[2]}`));
      this.findOrdersForm.get('ffin').setValue(new Date(`${finishDateTrans[1]}/${finishDateTrans[0]}/${finishDateTrans[2]}`));

      this.findOrdersForm.get('dateType').setValue(this.filterData.filterOrdersData.dateType ?
          this.filterData.filterOrdersData.dateType : '0');
      this.findOrdersForm.get('status').setValue(this.filterData.filterOrdersData.status ? this.filterData.filterOrdersData.status : '');
      this.getMaxDate();
      if (this.filterData.filterOrdersData.docNum) {
          this.getDisableForDocNum();
      } else if (this.filterData.filterOrdersData.qfb || this.filterData.filterOrdersData.status) {
          this.getDisableOnlyForDocNum();
      }
      this.findOrdersForm.valueChanges.subscribe(formData => {
          if (!this.isBeginInitForm) {
              if (formData.docNum !== null && formData.docNum) {
                  this.isToResetData = false;
                  this.getDisableForDocNum();
              } else if (formData.docNum !== null) {
                  this.changeValidatorsForDocNum();
              } else if (formData.docNum === '' && (formData.dateType !== '' && formData.dateType ||
                  formData.fini !== '' && formData.fini ||
                  formData.ffin !== '' && formData.ffin ||
                  formData.status !== '' && formData.status || formData.qfb !== '' && formData.qfb)) {
                  this.changeValidatorsForDocNum();
              }
          }
          this.isBeginInitForm = false;
          this.getMaxDate();
      });

  }

  searchOrders() {
      this.dialogRef.close(this.findOrdersForm.value);
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
      this.findOrdersForm.get('dateType').disable({onlySelf: true, emitEvent: false});
      this.findOrdersForm.get('fini').disable({onlySelf: true, emitEvent: false});
      this.findOrdersForm.get('ffin').disable({onlySelf: true, emitEvent: false});
      this.findOrdersForm.get('status').disable({onlySelf: true, emitEvent: false});
      this.findOrdersForm.get('qfb').disable({onlySelf: true, emitEvent: false});
  }
  getDisableOnlyForDocNum() {
      this.findOrdersForm.get('docNum').disable({onlySelf: true, emitEvent: false});
  }
  resetParamsValue() {
      this.findOrdersForm.get('docNum').setValue('');
      this.findOrdersForm.get('qfb').setValue( '' );
      this.findOrdersForm.get('status').setValue('');
  }
  enableAllParamsSearch() {
      this.getDisableForDocNum();
      this.getDisableOnlyForDocNum();
      this.resetParamsValue();
      this.findOrdersForm.get('dateType').enable({onlySelf: true, emitEvent: false});
      this.findOrdersForm.get('fini').enable({onlySelf: true, emitEvent: false});
      this.findOrdersForm.get('ffin').enable({onlySelf: true, emitEvent: false});
      this.findOrdersForm.get('status').enable({onlySelf: true, emitEvent: false});
      this.findOrdersForm.get('qfb').enable({onlySelf: true, emitEvent: false});
      this.findOrdersForm.get('docNum').enable({onlySelf: true, emitEvent: false});
  }
  changeValidatorsForDocNum() {
      this.isToResetData = false;
      this.getDisableOnlyForDocNum();
  }
}
