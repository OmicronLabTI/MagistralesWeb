import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import {MAT_DIALOG_DATA, MatDialogModule, MatDialogRef} from '@angular/material/dialog';

import { PlaceOrderDialogComponent } from './place-order-dialog.component';
import {MATERIAL_COMPONENTS} from '../../app.material';
import {CUSTOM_ELEMENTS_SCHEMA} from '@angular/core';
import {DatePipe} from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import {DataService} from '../../services/data.service';
import {MODAL_NAMES} from '../../constants/const';

describe('PlaceOrderDialogComponent', () => {
  let component: PlaceOrderDialogComponent;
  let fixture: ComponentFixture<PlaceOrderDialogComponent>;
  let dataServiceSpy;

  beforeEach(async(() => {
      dataServiceSpy = jasmine.createSpyObj<DataService>('DataService', [
          'presentToastCustom', 'getCallHttpService', 'setMessageGeneralCallHttp', 'setUrlActive', 'getFormattedNumber',
          'setQbfToPlace', 'setIsLoading'
      ]);
      TestBed.configureTestingModule({
      declarations: [ PlaceOrderDialogComponent ],
      imports: [
        HttpClientTestingModule,
        MatDialogModule,
        MATERIAL_COMPONENTS,
      ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
      providers: [
        {
          provide: MatDialogRef,
          useValue: {close: () => {}}
        },
        { provide: MAT_DIALOG_DATA, useValue: {placeOrdersData: {
              list: [],
              modalType: 'placeOrder',
              userId: '',
            }}
            },
          { provide: DataService, useValue: dataServiceSpy },
          DatePipe
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PlaceOrderDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
       expect(component).toBeTruthy();
   });
  it('should call changePlaceManual()', () => {
        component.isPlaceManual = false;
        component.changePlaceManual();
        expect(component.isPlaceManual).toBeTruthy();
   });
  it('should call placeOrder()', () => {
        component.placeOrder('anyUserId', 'anyUserName');
        expect(dataServiceSpy.setQbfToPlace).toHaveBeenCalledWith({userId: 'anyUserId', userName: 'anyUserName',
            modalType: 'placeOrder', list: [], assignType: MODAL_NAMES.assignManual,
            isFromOrderIsolated: undefined, isFromReassign: undefined});
    });
  it('should call placeOrderAutomatic()', () => {
        component.placeOrderAutomatic();
        expect(dataServiceSpy.setQbfToPlace).toHaveBeenCalledWith({
            modalType: 'placeOrder',
            list: [],
            assignType: MODAL_NAMES.assignAutomatic
        });
    });
});
