import { Component, OnInit } from '@angular/core';
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {CONST_USER_DIALOG} from "../../constants/const";

@Component({
  selector: 'app-search-users-dialog',
  templateUrl: './search-users-dialog.component.html',
  styleUrls: ['./search-users-dialog.component.scss']
})
export class SearchUsersDialogComponent implements OnInit {
  searchUserForm: FormGroup;
  constructor(private formBuilder: FormBuilder, ) {
    this.searchUserForm = this.formBuilder.group({
      userNameSe: ['', [Validators.required, Validators.maxLength(50)]],
      firstNameSe: ['', [Validators.required, Validators.maxLength(50)]],
      lastNameSe: ['', [Validators.required, Validators.maxLength(50)]],
      userTypeRSe: ['', Validators.required],
      activoSe: ['', [Validators.required]],
      asignableSe: ['', [Validators.required]]
    });
  }

  ngOnInit() {
  }

}
