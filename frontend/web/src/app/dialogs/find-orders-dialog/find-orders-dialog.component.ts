import { Component, OnInit } from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {DatePipe} from '@angular/common';
import {MODAL_FIND_ORDERS} from '../../constants/const';


@Component({
  selector: 'app-find-orders-dialog',
  templateUrl: './find-orders-dialog.component.html',
  styleUrls: ['./find-orders-dialog.component.scss']
})
export class FindOrdersDialogComponent implements OnInit {
  findOrdersForm: FormGroup;
  startDate = new Date();
  constructor(private formBuilder: FormBuilder,
              private datePipe: DatePipe) {
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
    console.log('data: ', this.findOrdersForm.value);
    console.log('date init; ', this.datePipe.transform(this.findOrdersForm.get('fini').value, 'dd-MM-yyyy'));
    console.log('date finish: ', this.datePipe.transform(this.findOrdersForm.get('ffin').value, 'dd-MM-yyyy'));
  }
}
