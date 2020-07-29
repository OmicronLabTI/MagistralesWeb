import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import { AddUserDialogComponent } from './add-user-dialog.component';
import {CUSTOM_ELEMENTS_SCHEMA} from "@angular/core";
import { HttpClientModule } from '@angular/common/http';
import {
  MatCardModule,

  MatDialogModule
} from '@angular/material';
import {MAT_DIALOG_DATA} from "@angular/material/dialog";

describe('AddUserDialogComponent', () => {
  let component: AddUserDialogComponent;
  let fixture: ComponentFixture<AddUserDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports:[FormsModule,HttpClientModule,MatCardModule,ReactiveFormsModule, MatDialogModule],
      declarations: [ AddUserDialogComponent ],
      schemas:[CUSTOM_ELEMENTS_SCHEMA],
      providers:[  { provide: MAT_DIALOG_DATA, useValue: {} }]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddUserDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
