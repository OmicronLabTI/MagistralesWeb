import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import {MAT_DIALOG_DATA, MatDialogModule, MatDialogRef} from '@angular/material/dialog';
import { ComponentSearchComponent } from './component-search.component';
import {CUSTOM_ELEMENTS_SCHEMA} from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {MatChipInputEvent, MatChipsModule} from '@angular/material/chips';
import {
  MatTableModule,
  MatCheckboxModule,
  MatFormFieldModule,
  MatInputModule,
} from '@angular/material';
import {DatePipe} from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import {PedidosService} from '../../services/pedidos.service';
import {of, throwError} from 'rxjs';
import {ComponentSearchMock} from '../../../mocks/componentsMock';
import {ErrorService} from '../../services/error.service';
import {PageEvent} from '@angular/material/paginator';
import {RouterTestingModule} from '@angular/router/testing';
import { MessagesService } from 'src/app/services/messages.service';

describe('ComponentSearchComponent', () => {
  let component: ComponentSearchComponent;
  let fixture: ComponentFixture<ComponentSearchComponent>;
  let messagesServiceSpy: jasmine.SpyObj<MessagesService>;
  let ordersServiceSpy;
  let errorServiceSpy;
  const catalogGroup = 'MG';
  beforeEach(async(() => {
    messagesServiceSpy = jasmine.createSpyObj<MessagesService>('MessagesService', [
      'presentToastCustom'
    ]);

    errorServiceSpy = jasmine.createSpyObj<ErrorService>('ErrorService', [
      'httpError'
    ]);
    ordersServiceSpy = jasmine.createSpyObj<PedidosService>('PedidosService', [
      'getComponents'
    ]);
    ordersServiceSpy.getComponents.and.callFake(() => {
      return of(ComponentSearchMock);
    });
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, MatTableModule,
        MatDialogModule, RouterTestingModule,
        MatCheckboxModule, MatFormFieldModule, MatInputModule, BrowserAnimationsModule, MatChipsModule],
      declarations: [ ComponentSearchComponent ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
      providers: [DatePipe,
        {
          provide: MatDialogRef,
          useValue: {close: () => {}}
        },
        { provide: MAT_DIALOG_DATA, useValue: {modalType: 'searchComponent',
                                                chips: ['crema'],
                                                catalogGroupName: catalogGroup } },
        { provide: PedidosService, useValue: ordersServiceSpy },
        { provide: ErrorService, useValue: errorServiceSpy },
        { provide: MessagesService, useValue: messagesServiceSpy },]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ComponentSearchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
    expect(component.isFromSearchComponent).toBeTruthy();
  });
  it('should getComponents() ok', () => {
    component.isFromSearchComponent = true;
    component.getComponentsAction();
    expect(ordersServiceSpy.getComponents).toHaveBeenCalled();
    expect(component.dataSource.data.length).toEqual(ComponentSearchMock.response.length);
    expect(component.lengthPaginator).toEqual(ComponentSearchMock.comments);
    expect(component.isDisableSearch).toBeFalsy();
  });
  it('should getComponents() failed', () => {
    ordersServiceSpy.getComponents.and.callFake(() => {
      return throwError({ error: true });
    });
    component.isFromSearchComponent = true;
    component.getComponentsAction();
    expect(ordersServiceSpy.getComponents).toHaveBeenCalled();
    expect(errorServiceSpy.httpError).toHaveBeenCalled();

  });
  it('should call changeDataEvent()', () => {
    expect(component.changeDataEvent({pageSize: 10, pageIndex: 0} as PageEvent)).toEqual({pageSize: 10, pageIndex: 0} as PageEvent);
    expect(component.offset).toEqual(0);
    expect(component.limit).toEqual(10);
  });
  it('should call addChip()', () => {
    component.keywords = [];
    component.addChip({ } as MatChipInputEvent);
    expect(component.keywords.length).toEqual(0);

    component.keywords = [];
    component.addChip({value: 'hola', input: { value: 'hola'} as HTMLInputElement } as MatChipInputEvent);
    expect(component.keywords.length).toEqual(1);
  });
  it('should call removeChip()', () => {
    component.keywords = ['crema'];
    component.removeChip('crema');
    expect(component.keywords.length).toEqual(0);
  });
  it('should call getQueryString()', () => {
    component.offset = 10;
    component.limit = 20;
    component.keywords = [];
    component.getQueryString();
    expect(component.queryStringComponents).toEqual(`?offset=${10}&limit=${20}&chips=$$&catalogGroup=${catalogGroup}`);

    component.keywords = ['crema'];
    component.getQueryString();
    expect(component.queryStringComponents).toEqual(`?offset=${10}&limit=${20}&chips=crema&catalogGroup=${catalogGroup}`);
  });
  it('should call checkIsPrevious()', () => {
     component.rowPrevious = {};
     component.checkIsPrevious({componente: 'crema', chips: []});
     expect(component.rowPrevious).toEqual({componente: 'crema', chips: []});
     component.keywords = ['agua'];
     component.checkIsPrevious({componente: 'crema', chips: []});

  });

  it('should selectComponent', () => {
    component.dataSource.data = [];
    component.dataSource.data[0] = {
      isChecked: false,
      orderFabId: 89098,
      productId: 'EN-075',
      description: 'Pomadera 8 Oz c/ Tapa  R-89 Bonita',
      baseQuantity: 210.000000,
      requiredQuantity: 210.000000,
      consumed: 0.000000,
      available: 0.000000,
      unit: 'Pieza',
      warehouse: 'PROD',
      pendingQuantity: 210.000000,
      stock: 1606.000000,
      warehouseQuantity: 0.000000,
      hasBatches: false,
      isItemSelected: false
    };
    // component.selectComponent({componente: 'crema', chips: []});
    expect(component.selectComponent).toBeTruthy();
  });

  it('should selectComponent isFromSearchComponent = false', () => {
    component.dataSource.data = [];
    component.dataSource.data[0] = {
      isChecked: false,
      orderFabId: 89098,
      productId: 'EN-075',
      description: 'Pomadera 8 Oz c/ Tapa  R-89 Bonita',
      baseQuantity: 210.000000,
      requiredQuantity: 210.000000,
      consumed: 0.000000,
      available: 0.000000,
      unit: 'Pieza',
      warehouse: 'PROD',
      pendingQuantity: 210.000000,
      stock: 1606.000000,
      warehouseQuantity: 0.000000,
      hasBatches: false,
      isItemSelected: false
    };
    component.isFromSearchComponent = false;
    component.selectComponent({componente: 'crema', chips: []});
    expect(component.selectComponent).toBeTruthy();
  });
});
