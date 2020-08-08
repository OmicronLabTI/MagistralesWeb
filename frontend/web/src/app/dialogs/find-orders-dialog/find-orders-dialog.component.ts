import {Component, Inject, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {DatePipe} from '@angular/common';
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
  constructor(private formBuilder: FormBuilder,
              @Inject(MAT_DIALOG_DATA) public filterData: any,
              private datePipe: DatePipe,
              private dialogRef: MatDialogRef<FindOrdersDialogComponent>,
              private ordersServices: PedidosService,
              private errorService: ErrorService) {
      this.fullDate = this.filterData.filterOrdersData.dateFull.split('-');
      console.log('dataReceive: ', this.filterData);
      this.findOrdersForm = this.formBuilder.group({
      docNum: ['', [Validators.required, Validators.maxLength(60)]],
      dateType: ['', Validators.required],
      fini: ['', [Validators.required]],
      ffin: ['', [Validators.required]],
      status: ['', []],
      qfb: ['', []],
    });

  }

  ngOnInit() {
    this.ordersServices.getQfbs().subscribe(resQfbs => {
      this.qfbsSelect = resQfbs.response.map( qfb => {
        return {
                qfbId: qfb.id,
                qfbName: qfb.firstName
              };
      });
    }, error => this.errorService.httpError(error));
    this.findOrdersForm.get('docNum').setValue(this.filterData.filterOrdersData.docNum ? this.filterData.filterOrdersData.docNum : '' );
    this.findOrdersForm.get('ffin').setValue(new Date(this.fullDate[0]));
    this.findOrdersForm.get('fini').setValue(new Date(this.fullDate[1]));
    this.findOrdersForm.get('dateType').setValue(this.filterData.filterOrdersData.dateType ? this.filterData.filterOrdersData.dateType : '' );
    this.findOrdersForm.get('status').setValue(this.filterData.filterOrdersData.status ? this.filterData.filterOrdersData.status : '' );
    this.getMaxDate();
    if (this.filterData.filterOrdersData.docNum
        || this.filterData.filterOrdersData.qfb || this.filterData.filterOrdersData.status) {
        this.getDisableForDocNum();
        this.getDisableOnlyForDocNum();
    }
    this.findOrdersForm.valueChanges.subscribe(formData => {
     if (formData.docNum !== '' && formData.docNum) {
         this.getDisableForDocNum();
      } else if (formData.docNum !== '') {
        this.getDisableOnlyForDocNum();
      } else if (formData.docNum === '' && (formData.dateType !== '' && formData.dateType  || formData.fini !== '' && formData.fini ||
         formData.ffin !== '' && formData.ffin  ||
         formData.status !== '' && formData.status || formData.qfb !== '' && formData.qfb )) {
         this.getDisableOnlyForDocNum();

     }
     this.getMaxDate();
    });

  }

  searchOrders() {
      console.log('data to send', this.findOrdersForm.value)
      this.dialogRef.close(this.findOrdersForm.value);
  }

  getMaxDate() {
      this.maxDate.setTime(new Date(this.findOrdersForm.get('fini').value).getTime() + MODAL_FIND_ORDERS.ninetyDays);
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
}
