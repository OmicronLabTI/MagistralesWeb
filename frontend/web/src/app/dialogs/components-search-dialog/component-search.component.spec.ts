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

describe('ComponentSearchComponent', () => {
  let component: ComponentSearchComponent;
  let fixture: ComponentFixture<ComponentSearchComponent>;
  let ordersServiceSpy;
  let errorServiceSpy;
  beforeEach(async(() => {
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
                                                chips: ['crema']} },
        { provide: PedidosService, useValue: ordersServiceSpy },
        { provide: ErrorService, useValue: errorServiceSpy }]
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
    expect(component.queryStringComponents).toEqual(`?offset=${10}&limit=${20}&chips=$$`);

    component.keywords = ['crema'];
    component.getQueryString();
    expect(component.queryStringComponents).toEqual(`?offset=${10}&limit=${20}&chips=crema`);
  });
  it('should call checkIsPrevious()', () => {
     component.rowPrevious = {};
     component.checkIsPrevious({componente: 'crema', chips: []});
     expect(component.rowPrevious).toEqual({componente: 'crema', chips: []});
     component.keywords = ['agua'];
     component.checkIsPrevious({componente: 'crema', chips: []});

  });
});
