import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import {CUSTOM_ELEMENTS_SCHEMA} from '@angular/core';
import {DatePipe} from '@angular/common';
import { MATERIAL_COMPONENTS } from 'src/app/app.material';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';

import { FabordersListComponent } from './faborders-list.component';
import {DataService} from "../../services/data.service";
import {ConstOrders, HttpServiceTOCall} from "../../constants/const";
import {Observable, of} from "rxjs";
import {RolesMock} from "../../../mocks/rolesMock";

describe('FabordersListComponent', () => {
  let component: FabordersListComponent;
  let fixture: ComponentFixture<FabordersListComponent>;
  let dataServiceSpy;
  beforeEach(async(() => {
    dataServiceSpy = jasmine.createSpyObj<DataService>('DataService', [
      'presentToastCustom', 'getCallHttpService', 'setMessageGeneralCallHttp', 'setUrlActive', 'setIsLoading',
      'setCallHttpService', 'setMessageGeneralCallHttp', 'getOrderIsolated', 'removeOrderIsolated', 'getNewSearchOrdersModal',
        'getCallHttpService', 'transformDate', 'setSearchComponentModal', 'getNewDataToFilter', 'setCancelOrders', 'setQbfToPlace',
        'getItemOnDataOnlyIds', 'getIsThereOnData', 'getItemOnDateWithFilter'
    ]);
    dataServiceSpy.getNewSearchOrdersModal.and.callFake(() => {
      return new Observable();
    });
    dataServiceSpy.getCallHttpService.and.callFake(() => {
      return new Observable();
    });
    dataServiceSpy.getOrderIsolated.and.callFake(() => {
      return '12345Id';
    });
    TestBed.configureTestingModule({
      declarations: [ FabordersListComponent ],
      imports: [ RouterTestingModule, MATERIAL_COMPONENTS, HttpClientTestingModule, BrowserAnimationsModule ],
      providers: [
        DatePipe,
        { provide: DataService, useValue: dataServiceSpy },
      ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FabordersListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
    expect(dataServiceSpy.setUrlActive).toHaveBeenCalledWith(HttpServiceTOCall.ORDERS_ISOLATED);
    expect(component.filterDataOrders.isFromOrders).toBeFalsy();
    expect(component.filterDataOrders.dateType).toEqual(ConstOrders.defaultDateInit);
    // expect(component.filterDataOrders.dateFull.includes('-')).toBeTruthy();
    // expect(component.queryString.includes('-')).toBeTruthy();
    expect(dataServiceSpy.getOrderIsolated).toHaveBeenCalled();
    expect(dataServiceSpy.removeOrderIsolated).toHaveBeenCalled();

  });
});
