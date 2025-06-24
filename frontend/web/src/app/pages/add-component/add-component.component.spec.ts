import { async, ComponentFixture, TestBed, tick } from '@angular/core/testing';

import { AddComponentComponent } from './add-component.component';
import { BatchesService } from 'src/app/services/batches.service';
import { ObservableService } from 'src/app/services/observable.service';
import { MessagesService } from 'src/app/services/messages.service';
import { DataService } from 'src/app/services/data.service';
import { ILotesAsignadosReq, ILotesFormulaReq, ILotesFormulaRes, ILotesReq, ILotesSaveRes } from 'src/app/model/http/lotesformula';
import { ErrorService } from 'src/app/services/error.service';
import { Observable, of } from 'rxjs';
import { RouterTestingModule } from '@angular/router/testing';
import { MATERIAL_COMPONENTS } from 'src/app/app.material';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { DatePipe } from '@angular/common';
import { PedidosService } from 'src/app/services/pedidos.service';
import { DetalleFormulaMock } from 'src/mocks/pedidosListMock';
import { IFormulaDetalleReq, IFormulaReq } from 'src/app/model/http/detalleformula';
import {
  mockIFormulaDetalleReq, lotesComponentMock, mockIFormulaReqResponse,
  ILotesFormulaReqMock, dataSourceComponentsMock
} from 'src/mocks/componentsLotesMock';

describe('AddComponentComponent', () => {
  let component: AddComponentComponent;
  let fixture: ComponentFixture<AddComponentComponent>;
  let pedidosServiceSpy: jasmine.SpyObj<PedidosService>;
  let errorServiceSpy;

  let batchesServiceSpy: jasmine.SpyObj<BatchesService>;
  let observableServiceSpy: jasmine.SpyObj<ObservableService>;
  let messagesServiceSpy: jasmine.SpyObj<MessagesService>;
  let dataServiceSpy: jasmine.SpyObj<DataService>;

  const iLotesFormulaRes = new ILotesFormulaRes();
  const iLotesSaveRes = new ILotesSaveRes();
  const iLotesReq = new ILotesReq();
  const iLotesFormulaReq = new ILotesFormulaReq();
  const iLotesAsignadosReq = new ILotesAsignadosReq();

  iLotesReq.fechaExp = new Date('12/12/21');
  iLotesReq.isValid = true;

  beforeEach(async(() => {
    pedidosServiceSpy = jasmine.createSpyObj<PedidosService>('PedidosService', [
      'getFormulaDetail',
      'getComponents',
      'updateFormula',
      'getComponentsLotes'
    ]);
    errorServiceSpy = jasmine.createSpyObj<ErrorService>('ErrorService', [
      'httpError'
    ]);
    observableServiceSpy = jasmine.createSpyObj<ObservableService>('ObservableService', [
      'getNewFormulaComponent',
      'setGeneralNotificationMessage',
      'setUrlActive',
      'setPathUrl',
      'setIsLoading'
    ]);
    dataServiceSpy = jasmine.createSpyObj<DataService>('DataService', [
      'calculateTernary',
      'calculateOrValueList',
      'setIsToSaveAnything'
    ]);
    batchesServiceSpy = jasmine.createSpyObj<BatchesService>('BatchesService', [
      'getInventoryBatches',
      'updateBatches',
    ]);
    messagesServiceSpy = jasmine.createSpyObj<MessagesService>('MessagesService', [
      'presentToastCustom',
      'getMessageTitle',
    ]);
    observableServiceSpy.getNewFormulaComponent.and.callFake(() => {
      return new Observable();
    });

    pedidosServiceSpy.getFormulaDetail.and.returnValue(of({ response: mockIFormulaReqResponse }));
    pedidosServiceSpy.getComponents.and.returnValue(of({ response: mockIFormulaDetalleReq }));
    pedidosServiceSpy.getComponentsLotes.and.returnValue(of({ response: lotesComponentMock }));
    pedidosServiceSpy.updateFormula.and.returnValue(of());
    dataServiceSpy.calculateTernary.and.callFake(<T, U>(validation: boolean, firstValue: T, secondaValue: U): T | U => {
      return validation ? firstValue : secondaValue;
    });
    dataServiceSpy.calculateOrValueList.and.callFake((list: boolean[]) => {
      const res = list.some((value) => value === true);
      return res;
    });
    batchesServiceSpy.getInventoryBatches.and.callFake(() => {
      return of(iLotesFormulaRes);
    });
    batchesServiceSpy.updateBatches.and.returnValue(of(iLotesSaveRes));

    messagesServiceSpy.presentToastCustom.and.returnValue(Promise.resolve());
    messagesServiceSpy.getMessageTitle.and.returnValue('');

    observableServiceSpy.setGeneralNotificationMessage.and.returnValue();
    observableServiceSpy.setUrlActive.and.returnValue();
    observableServiceSpy.setPathUrl.and.returnValue();
    TestBed.configureTestingModule({
      imports: [
        RouterTestingModule,
        MATERIAL_COMPONENTS,
        HttpClientTestingModule,
        ReactiveFormsModule,
        FormsModule,
        BrowserAnimationsModule
      ],
      declarations: [AddComponentComponent],
      providers: [
        DatePipe,
        { provide: ObservableService, useValue: observableServiceSpy },
        { provide: DataService, useValue: dataServiceSpy },
        { provide: PedidosService, useValue: pedidosServiceSpy }
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddComponentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
  it('should getLotesForComponent', () => {
    component.getLotesForComponent(mockIFormulaDetalleReq[0]);
    expect(component.dataSourceComponents.data.length).toBe(1);
  });
  it('should call onBaseQuantityChange', () => {
    component.dataSourceComponents.data = dataSourceComponentsMock;
    component.plannedQuantityControl = 2;
    component.onBaseQuantityChange(4, 0);
    expect(component.dataSourceComponents.data[0].requiredQuantity).toBe(8);
  });
  it('should call onRequiredQuantityChange', () => {
    component.dataSourceComponents.data = dataSourceComponentsMock;
    component.plannedQuantityControl = 2;
    component.onRequiredQuantityChange(4, 0);
    expect(component.dataSourceComponents.data[0].baseQuantity).toBe(2);
  });
  it('should call onSelectWareHouseChange', () => {
    component.dataSourceComponents.data = dataSourceComponentsMock;
    component.onSelectWareHouseChange('MG', 0);
    expect(component.dataSourceComponents.data[0].warehouse).toBe('MG');
  });
  it('should call addLotes', () => {
    component.generateObjectsForTables(mockIFormulaDetalleReq[0], lotesComponentMock);
    component.addLotes(component.dataSourceLotes.data[0]);
    expect(component.isReadyToSave).toBe(true);
  });
  it('should call deleteLotes', () => {
    component.dataSourceComponents.data = dataSourceComponentsMock;
    component.deleteLotes(dataSourceComponentsMock[0].lotesAsignados[0]);
    expect(component.isReadyToSave).toBe(true);
  });
  it('should call buildObjectToSap', () => {
    component.generateObjectsForTables(mockIFormulaDetalleReq[0], lotesComponentMock);
    component.generateObjectToSave();
    component.buildObjectToSap();
    expect(pedidosServiceSpy.updateFormula).toHaveBeenCalled();
  });
  it('should call deleteComponents', () => {
    component.generateObjectsForTables(mockIFormulaDetalleReq[0], lotesComponentMock);
    component.generateObjectToSave();
    component.dataSourceComponents.data[0].isChecked = true;
    component.deleteComponents();
  });
});
