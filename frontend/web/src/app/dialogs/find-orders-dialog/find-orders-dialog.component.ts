import { Component, OnInit } from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {DatePipe} from "@angular/common";


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
      status: ['', Validators.required],
      qfb: ['', [Validators.required]],
    });
  }

  ngOnInit() {
  }

  searchOrders() {
    console.log('data: ', this.findOrdersForm.value);
    console.log('date init; ', this.datePipe.transform(this.findOrdersForm.get('fini').value, 'dd-MM-yyyy'));
    console.log('date finish: ', this.datePipe.transform(this.findOrdersForm.get('ffin').value, 'dd-MM-yyyy'));
  }
}
