import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {DatePipe} from '@angular/common';
import {MODAL_FIND_ORDERS} from '../../constants/const';
import {MatDialogRef} from '@angular/material/dialog';
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
  startDate = new Date();
  qfbsSelect: QfbSelect[] = [];
  constructor(private formBuilder: FormBuilder,
              private datePipe: DatePipe,
              private dialogRef: MatDialogRef<FindOrdersDialogComponent>,
              private ordersServices: PedidosService,
              private errorService: ErrorService) {
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

    this.findOrdersForm.get('ffin').setValue(new Date());
    this.startDate.setTime(this.startDate.getTime() - MODAL_FIND_ORDERS.thirtyDays);
    this.findOrdersForm.get('fini').setValue(this.startDate);

    this.findOrdersForm.valueChanges.subscribe(formData => {
      if (formData.docNum !== '' && formData.docNum) {
        this.findOrdersForm.get('dateType').disable({onlySelf: true, emitEvent: false});
        this.findOrdersForm.get('fini').disable({onlySelf: true, emitEvent: false});
        this.findOrdersForm.get('ffin').disable({onlySelf: true, emitEvent: false});
        this.findOrdersForm.get('status').disable({onlySelf: true, emitEvent: false});
        this.findOrdersForm.get('qfb').disable({onlySelf: true, emitEvent: false});
      } else {
        this.findOrdersForm.get('docNum').disable({onlySelf: true, emitEvent: false});
      }
    });

  }

  searchOrders() {
    // console.log('ini', new Date(this.findOrdersForm.get('fini').value).getTime());
    // console.log('ffin', new Date(this.findOrdersForm.get('ffin').value).getTime());
    this.dialogRef.close(this.findOrdersForm.value);
  }
}
