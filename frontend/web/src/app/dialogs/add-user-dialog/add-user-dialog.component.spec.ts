import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AddUserDialogComponent } from './add-user-dialog.component';
import {CUSTOM_ELEMENTS_SCHEMA} from "@angular/core";
import { HttpClientModule } from '@angular/common/http';
import {
  MatCardModule,
  MatFormFieldModule, MatInputModule,
  MatDialogModule,
  MatSelectModule
} from '@angular/material';
import {MAT_DIALOG_DATA} from "@angular/material/dialog";

describe('AddUserDialogComponent', () => {
  let component: AddUserDialogComponent;
  let fixture: ComponentFixture<AddUserDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports:[FormsModule,HttpClientModule,MatCardModule,ReactiveFormsModule, MatDialogModule,BrowserAnimationsModule,
        MatFormFieldModule,
          MatSelectModule,
        MatInputModule],
      declarations: [ AddUserDialogComponent ],
      schemas:[CUSTOM_ELEMENTS_SCHEMA],
      providers:[
          { provide: MAT_DIALOG_DATA, useValue: {} }
      ]
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
