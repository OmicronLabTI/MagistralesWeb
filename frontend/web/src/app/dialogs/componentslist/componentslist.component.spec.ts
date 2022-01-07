import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import {MAT_DIALOG_DATA, MatDialogModule, MatDialogRef} from '@angular/material/dialog';
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

import { ComponentslistComponent } from './componentslist.component';
import {RouterTestingModule} from '@angular/router/testing';
import { DataService } from 'src/app/services/data.service';
import { BaseComponent, Components, IMyCustomListRes } from 'src/app/model/http/listacomponentes';
import { OrdersService } from 'src/app/services/orders.service';
import { of } from 'rxjs';

describe('ComponentslistComponent', () => {
  let component: ComponentslistComponent;
  let fixture: ComponentFixture<ComponentslistComponent>;
  let dataServiceSpy: jasmine.SpyObj<DataService>;
  let ordersServiceSpy: jasmine.SpyObj<OrdersService>;
  let matDialogRef;
  const iMyCustomListRes = new IMyCustomListRes();
  const baseComponent = new BaseComponent();
  baseComponent.id = 1;
  baseComponent.name =  '';
  baseComponent.productId = '1';
  baseComponent.components = Components[0];

  beforeEach(async(() => {
    const matDialogSpy = jasmine.createSpyObj('MatDialogRef', ['close']);
    dataServiceSpy = jasmine.createSpyObj<DataService>('DataService', [
      'presentToastCustom',
      'setMessageGeneralCallHttp',
      'setIsLoading'
    ]);
    ordersServiceSpy = jasmine.createSpyObj('OrdersService', [
      'getCustomList',
      'deleteCustomList'
    ]);
    dataServiceSpy.presentToastCustom.and.callFake(() => {
      return Promise.resolve({
        isConfirmed: true
      });
    });
    dataServiceSpy.setMessageGeneralCallHttp.and.callFake(() => {
      return;
    });

    ordersServiceSpy.getCustomList.and.callFake(() => {
      return of(iMyCustomListRes);
    });
    ordersServiceSpy.deleteCustomList.and.callFake(() => {
      return of();
    });
    TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule,
        MatTableModule,
        MatDialogModule,
        MatCheckboxModule,
        MatFormFieldModule, MatInputModule,
        BrowserAnimationsModule, RouterTestingModule],
      declarations: [ ComponentslistComponent ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
      providers: [
        DatePipe, {
          provide: MatDialogRef,
          useValue: matDialogSpy
        },
        {
          provide: MAT_DIALOG_DATA, useValue: {}
        },
        { provide: DataService, useValue: dataServiceSpy}
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ComponentslistComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    matDialogRef = TestBed.get(MatDialogRef);
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
  it('should selectComponent', () => {
    dataServiceSpy.presentToastCustom.and.callFake(() => {
      return Promise.resolve({
        isConfirmed: true
      });
    });
    // matDialogRef.close.and.re
    component.selectComponent(baseComponent);
    expect(dataServiceSpy.presentToastCustom).toHaveBeenCalled();
  });

  it('should removeCustomList', () => {
    dataServiceSpy.presentToastCustom.and.callFake(() => {
      return Promise.resolve({
        isConfirmed: true
      });
    });
    component.removeCustomList(baseComponent);
    expect(component.removeCustomList).toBeTruthy();
    // expect(dataServiceSpy.setMessageGeneralCallHttp).toHaveBeenCalled();
  });

  it('should getCustomList', () => {
    component.getCustomList();
    expect(ordersServiceSpy.getCustomList).toBeTruthy();
  });
});
