import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA} from "@angular/material/dialog";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";

@Component({
  selector: 'app-add-user-dialog',
  templateUrl: './add-user-dialog.component.html',
  styleUrls: ['./add-user-dialog.component.scss']
})
export class AddUserDialogComponent implements OnInit {
  addUserForm: FormGroup;

  constructor(@Inject(MAT_DIALOG_DATA) public data: any, private formBuilder: FormBuilder) {
    console.log('data drom: ', this.data)
    this.addUserForm = this.formBuilder.group({
      userName:['',Validators.required],
      firstName:['',Validators.required],
      lastName:['',Validators.required],
      role:['',Validators.required],
      password:['', Validators.required],
      activo:['',Validators.required]
    })
  }

  ngOnInit() {
  }

}
