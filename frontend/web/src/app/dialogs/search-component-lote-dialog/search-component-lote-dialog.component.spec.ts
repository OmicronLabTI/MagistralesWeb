import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SearchComponentLoteDialogComponent } from './search-component-lote-dialog.component';
import { MessagesService } from 'src/app/services/messages.service';
import { ErrorService } from 'src/app/services/error.service';
import { PedidosService } from 'src/app/services/pedidos.service';
import { searchComponentsLotesMock } from 'src/mocks/componentsMock';
import { of, throwError } from 'rxjs';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { MAT_DIALOG_DATA, MatCheckboxModule, MatChipInputEvent, MatChipsModule,
  MatDialogModule, MatDialogRef, MatFormFieldModule, MatInputModule, MatTableModule } from '@angular/material';
import { RouterTestingModule } from '@angular/router/testing';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { DatePipe } from '@angular/common';

describe('SearchComponentLoteDialogComponent', () => {
  let component: SearchComponentLoteDialogComponent;
  let fixture: ComponentFixture<SearchComponentLoteDialogComponent>;
  let messagesServiceSpy: jasmine.SpyObj<MessagesService>;
  let ordersServiceSpy: jasmine.SpyObj<PedidosService>;
  let errorServiceSpy;
  const catalogGroup = 'MG';
  const dataSourceMock = [{
    codigoProducto: 'MP-109',
    almacen: 'PROD',
    isItemSelected: true,
    lotes: [
      {
        numeroLote: '88-22',
        cantidadDisponible: 1.405394,
        cantidadAsignada: 2.673856,
        sysNumber: 158,
        fechaExp: '29/03/2023',
        fechaExpDateTime: '2023-03-29T00:00:00',
        itemCode: 'MP-109',
        quantity: 4.07925
      }
    ]
  }];

  beforeEach(async(() => {
    messagesServiceSpy = jasmine.createSpyObj<MessagesService>('MessagesService', [
      'presentToastCustom'
    ]);

    errorServiceSpy = jasmine.createSpyObj<ErrorService>('ErrorService', [
      'httpError'
    ]);
    ordersServiceSpy = jasmine.createSpyObj<PedidosService>('PedidosService', [
      'getComponentsLotes'
    ]);
    ordersServiceSpy.getComponentsLotes.and.returnValue(of(searchComponentsLotesMock));
    TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule,
        MatTableModule,
        MatDialogModule,
        RouterTestingModule,
        MatCheckboxModule,
        MatFormFieldModule,
        MatInputModule,
        BrowserAnimationsModule,
        MatChipsModule
      ],
      declarations: [SearchComponentLoteDialogComponent],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
      providers: [DatePipe,
        {
          provide: MatDialogRef,
          useValue: { close: () => { } }
        },
        {
          provide: MAT_DIALOG_DATA, useValue: {
            modalType: 'searchComponent',
            chips: ['MP-109'],
            catalogGroupName: catalogGroup,
            data: dataSourceMock
          }
        },
        { provide: PedidosService, useValue: ordersServiceSpy },
        { provide: ErrorService, useValue: errorServiceSpy },
        { provide: MessagesService, useValue: messagesServiceSpy },
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SearchComponentLoteDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
  it('should getComponents() ok', () => {
    component.getComponentsAction();
    expect(ordersServiceSpy.getComponentsLotes).toHaveBeenCalled();
    expect(component.dataSource.data.length).toEqual(searchComponentsLotesMock.response.length);
    expect(component.lengthPaginator).toEqual(searchComponentsLotesMock.comments);
    expect(component.isDisableSearch).toBeFalsy();
  });
  it('should getComponents() failed', () => {
    ordersServiceSpy.getComponentsLotes.and.callFake(() => {
      return throwError({ error: true });
    });
    component.getComponentsAction();
    expect(ordersServiceSpy.getComponentsLotes).toHaveBeenCalled();
    expect(errorServiceSpy.httpError).toHaveBeenCalled();

  });
  it('should call addChip()', () => {
    component.keywords = [];
    component.addChip({} as MatChipInputEvent);
    expect(component.keywords.length).toEqual(0);

    component.keywords = [];
    component.addChip({ value: 'hola', input: { value: 'hola' } as HTMLInputElement } as MatChipInputEvent);
    expect(component.keywords.length).toEqual(1);
  });
  it('should call removeChip()', () => {
    component.keywords = ['crema'];
    component.removeChip('crema');
    expect(component.keywords.length).toEqual(0);
  });
  it('should selectComponent', () => {
    component.dataSource.data = [];
    component.dataSource.data.push({
      codigoProducto: 'MP-109',
      almacen: 'PROD',
      isItemSelected: true,
      lotes: [
        {
          numeroLote: '88-22',
          cantidadDisponible: 1.405394,
          cantidadAsignada: 2.673856,
          sysNumber: 158,
          fechaExp: '29/03/2023',
          fechaExpDateTime: '2023-03-29T00:00:00',
          itemCode: 'MP-109',
          quantity: 4.07925
        },
        {
          numeroLote: 'Axity-01',
          cantidadDisponible: 97.33999,
          cantidadAsignada: 1.74501,
          sysNumber: 160,
          fechaExp: null,
          fechaExpDateTime: null,
          itemCode: 'MP-109',
          quantity: 99.085
        }
      ]
    });
    component.selectComponent(dataSourceMock[0]);
    expect(component.selectComponent).toBeTruthy();
  });
  it('should call checkIsPrevious()', () => {
    component.rowPrevious = dataSourceMock[0];
    component.checkIsPrevious(dataSourceMock[0]);
    expect(component.rowPrevious).toEqual(dataSourceMock[0]);
  });
});
