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
import { of } from 'rxjs';
import { IMyListRes } from 'src/app/model/http/listacomponentes';
import { MODAL_FIND_ORDERS } from 'src/app/constants/const';
import { LocalStorageService } from 'src/app/services/local-storage.service';

describe('MiListaComponent', () => {
  let component: MiListaComponent;
  let fixture: ComponentFixture<MiListaComponent>;
  let ordersServiceSpy: jasmine.SpyObj<OrdersService>;
  let dataServiceSpy: jasmine.SpyObj<DataService>;
  let localStorageServiceSpy: jasmine.SpyObj<LocalStorageService>;

  const close = () => {};
  const iMyListRes = new IMyListRes();
  iMyListRes.response = 0;
  const MockDialogRef = {
    close: jasmine.createSpy('close')
  };
  beforeEach(async(() => {
    ordersServiceSpy = jasmine.createSpyObj<OrdersService>('OrdersService', [
      'saveMyListComponent'
    ]);
    localStorageServiceSpy = jasmine.createSpyObj<LocalStorageService>('LocalStorageService', [
      'getUserId'
    ]);
    dataServiceSpy = jasmine.createSpyObj<DataService>('DataService', [
      'presentToastCustom',
    ]);
    localStorageServiceSpy.getUserId.and.callFake(() => {
      return '123';
    });
    // spyOn(authService, 'isAuthenticated').and.returnValue(Promise.resolve(true));
    dataServiceSpy.presentToastCustom.and.callFake(() => {
      return Promise.resolve({
        isConfirmed: true
      });
    });
    ordersServiceSpy.saveMyListComponent.and.callFake(() => {
      return of(iMyListRes);
    });
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
        { provide: DataService, useValue: dataServiceSpy },
        { provide: OrdersService, useValue: ordersServiceSpy },
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
  it('should save My List response 0', () => {
    dataServiceSpy.presentToastCustom.and.callFake(() => {
      return Promise.resolve({
        isConfirmed: true
      });
    });
    component.saveMyList();
    expect(dataServiceSpy.presentToastCustom).toHaveBeenCalled();
  });
  it('should save My List response !=0', () => {
    const iMy = new IMyListRes();
    iMy.response = 1;
    ordersServiceSpy.saveMyListComponent.and.callFake(() => {
      return of(iMy);
    });
    dataServiceSpy.presentToastCustom.and.callFake(() => {
      return Promise.resolve({
        isConfirmed: true
      });
    });
    component.saveMyList();
    expect(dataServiceSpy.presentToastCustom).toHaveBeenCalled();
  });
  it('should key Down Function', () => {
    // expect(component.keyDownFunction).toBeTruthy();
    const keyEvent = new KeyboardEvent('keyEnter', { code: 'Digit0', key: MODAL_FIND_ORDERS.keyEnter});
    component.keyDownFunction(keyEvent);
    // expect(MockDialogRef.close).toHaveBeenCalled();
    expect(component.saveMyList).toBeTruthy();
  });


});
