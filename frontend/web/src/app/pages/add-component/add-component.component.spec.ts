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
import { mockIFormulaDetalleReq, lotesComponentMock, mockIFormulaReqResponse,
  ILotesFormulaReqMock, dataSourceComponentsMock } from 'src/mocks/componentsLotesMock';

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
      'updateFormula'
    ]);
    errorServiceSpy = jasmine.createSpyObj<ErrorService>('ErrorService', [
      'httpError'
    ]);
    observableServiceSpy = jasmine.createSpyObj<ObservableService>('ObservableService', [
      'getNewComponentLotes',
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
    observableServiceSpy.getNewComponentLotes.and.callFake(() => {
      return new Observable();
    });

    pedidosServiceSpy.getFormulaDetail.and.returnValue(of({ response: mockIFormulaReqResponse }));
    pedidosServiceSpy.getComponents.and.returnValue(of({response: mockIFormulaDetalleReq}));
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
    // --- Observable Service
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
        { provide: PedidosService, useValue: pedidosServiceSpy}
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
  it('should call reloadData', async () => {
    expect(component).toBeTruthy();
    spyOn(component, 'getDetalleFormula').and.returnValue(Promise.resolve());
    spyOn(component, 'getInventoryBatches').and.returnValue(Promise.resolve());
    const generateObjectsSpy = spyOn(component, 'generateObjects');
    await component.reloadData();
    expect(generateObjectsSpy).toHaveBeenCalled();
  });
  it('should call onSuccessDetailFormula', () => {
    component.onSuccessDetailFormula(mockIFormulaReqResponse);
    expect(component.componentsData).toEqual(mockIFormulaReqResponse.details);
  });
  it('should call getNewComponent', () => {
    component.getNewComponent(lotesComponentMock);
    expect(pedidosServiceSpy.getComponents).toHaveBeenCalled();
  });
  it('should call generateObjects', () => {
    component.componentsData = mockIFormulaDetalleReq;
    component.lotesData = ILotesFormulaReqMock;
    component.generateObjects();
    expect(component.dataSourceLotes.data.length).toBe(2);
  });
  it('should call setSelectedTr', () => {
    component.dataSourceComponents.data = dataSourceComponentsMock;
    component.setSelectedTr(dataSourceComponentsMock[0]);
    expect(component.dataSourceLotes.data).toEqual(dataSourceComponentsMock[0].lotes);
  });
  it('should call addLotes', () => {
    component.indexSelected = 0;
    component.componentsData = mockIFormulaDetalleReq;
    component.lotesData = ILotesFormulaReqMock;
    component.generateObjects();
    component.addLotes(component.dataSourceLotes.data[0]);
    expect(component.isReadyToSave).toBe(true);
  });
  it('should call deleteLotes', () => {
    component.dataSourceComponents.data = dataSourceComponentsMock;
    component.deleteLotes(dataSourceComponentsMock[0].lotesAsignados[0]);
    expect(component.isReadyToSave).toBe(true);
  });
  it('should call onBaseQuantityChange', () => {
    component.dataSourceComponents.data = dataSourceComponentsMock;
    component.onBaseQuantityChange(4, 0);
    expect(component.isReadyToSave).toBe(true);
  });
  it('should call onRequiredQuantityChange', () => {
    component.dataSourceComponents.data = dataSourceComponentsMock;
    component.onRequiredQuantityChange(4, 0);
    expect(component.isReadyToSave).toBe(true);
  });
  it('should call onSelectWareHouseChange', () => {
    component.dataSourceComponents.data = dataSourceComponentsMock;
    component.onSelectWareHouseChange('MAG', 0);
    expect(component.dataSourceComponents.data[0].warehouse).toBe('MAG');
  });
  it('should call getInsertElementsToSave', () => {
    component.componentsData = mockIFormulaDetalleReq;
    component.lotesData = ILotesFormulaReqMock;
    component.generateObjects();
    component.dataSourceComponents.data[0].action = 'insert';
    component.getInsertElementsToSave();
    expect(component.objectDataToSave.components[0].action).toBe(component.dataSourceComponents.data[0].action);
  });
  it('should call getUpdateElementsToSave', () => {
    component.componentsData = mockIFormulaDetalleReq;
    component.lotesData = ILotesFormulaReqMock;
    component.generateObjects();
    component.dataSourceComponents.data[0].action = 'update';
    component.getUpdateElementsToSave();
    expect(component.objectDataToSave.components[0].action).toBe(component.dataSourceComponents.data[0].action);
  });
  it('should call buildObjectToSap', () => {
    component.componentsData = mockIFormulaDetalleReq;
    component.lotesData = ILotesFormulaReqMock;
    component.generateObjects();
    component.buildObjectToSap();
  });
});
