import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import {MAT_DIALOG_DATA, MatDialogModule, MatDialogRef} from '@angular/material/dialog';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {CUSTOM_ELEMENTS_SCHEMA} from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {
  MatTableModule,
  MatCheckboxModule,
  MatFormFieldModule,
  MatInputModule,
} from '@angular/material';
import {DatePipe} from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { MiListaComponent } from './mi-lista.component';
import {RouterTestingModule} from '@angular/router/testing';
import { DataService } from 'src/app/services/data.service';
import { OrdersService } from 'src/app/services/orders.service';
import Swal from 'sweetalert2';
import {PipesModule} from '../../pipes/pipes.module';
import { MATERIAL_COMPONENTS } from 'src/app/app.material';

describe('MiListaComponent', () => {
  let component: MiListaComponent;
  let fixture: ComponentFixture<MiListaComponent>;
  let ordersServiceSpy;
  let dataServiceSpy;
  const close = () => {};
  beforeEach(async(() => {
    ordersServiceSpy = jasmine.createSpyObj<OrdersService>('OrdersService', [
      'saveMyListComponent'
    ]);
    dataServiceSpy = jasmine.createSpyObj<DataService>('DataService', [
      'presentToastCustom',
    ]);
    TestBed.configureTestingModule({
      imports: [
        ReactiveFormsModule,
        FormsModule,
        HttpClientTestingModule,
        MatTableModule,
        MatDialogModule,
        MatCheckboxModule,
        MatFormFieldModule, MatInputModule,
        BrowserAnimationsModule, RouterTestingModule,
        MATERIAL_COMPONENTS, PipesModule],
      declarations: [ MiListaComponent ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
      providers: [
        DatePipe, {
          provide: MatDialogRef,
          useValue: {}
        },
        {
          provide: MAT_DIALOG_DATA, useValue: {}
        },
        {
          provide: MatDialogRef,
          useValue: {close}
        },
        { provide: DataService, useValue: dataServiceSpy }
        /* { provide: OrdersService, useValue: dataServiceSpy },*/
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MiListaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should key Down Function', () => {
    const keyEvent = new KeyboardEvent('keyEvent', {key: 'Enter'});
    component.keyDownFunction(keyEvent);
  });

  /*it('should save My List', (done) => {
    component.saveMyList();
    setTimeout(() => {
      expect(Swal.isVisible()).toBeTruthy();
      Swal.clickConfirm();
      done();
    });
  });*/
});
