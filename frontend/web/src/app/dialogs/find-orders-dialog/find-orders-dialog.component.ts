import { Component, OnInit } from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {DatePipe} from '@angular/common';


@Component({
  selector: 'app-find-orders-dialog',
  templateUrl: './find-orders-dialog.component.html',
  styleUrls: ['./find-orders-dialog.component.scss']
})
export class FindOrdersDialogComponent implements OnInit {
  findOrdersForm: FormGroup;
  constructor(private formBuilder: FormBuilder, private datePipe: DatePipe) {
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
    console.log(' date: ', new Date());
    const dateOffset = (24 * 60 * 60 * 1000) * 10; // 5 days
    const myDate = new Date();
    myDate.setTime(myDate.getTime() - dateOffset);
    console.log('5 days: ', myDate);
    this.findOrdersForm.valueChanges.subscribe(formData => {
      if (formData.docNum !== '' && formData.docNum) {
        console.log('if', formData);
        this.findOrdersForm.get('dateType').disable({onlySelf: true, emitEvent: false});
        this.findOrdersForm.get('fini').disable({onlySelf: true, emitEvent: false});
        this.findOrdersForm.get('ffin').disable({onlySelf: true, emitEvent: false});
        this.findOrdersForm.get('status').disable({onlySelf: true, emitEvent: false});
        this.findOrdersForm.get('qfb').disable({onlySelf: true, emitEvent: false});
        console.log('else if 1', formData);
      } else {
        this.findOrdersForm.get('docNum').disable({onlySelf: true, emitEvent: false});
        console.log('data full: ', formData);
      }
      // console.log('init: ', this.findOrdersForm.get('docNum').disable({onlySelf: true, emitEvent: false}));
    });

  }

  searchOrders() {
    console.log('data: ', this.findOrdersForm.value);
    console.log('date init; ', this.datePipe.transform(this.findOrdersForm.get('fini').value, 'dd-MM-yyyy'));
    console.log('date finish: ', this.datePipe.transform(this.findOrdersForm.get('ffin').value, 'dd-MM-yyyy'));
  }
}
