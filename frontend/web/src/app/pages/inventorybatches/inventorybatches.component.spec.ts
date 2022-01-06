import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InventorybatchesComponent } from './inventorybatches.component';
import { RouterTestingModule } from '@angular/router/testing';
import {DatePipe} from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { MATERIAL_COMPONENTS } from 'src/app/app.material';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { DataService } from 'src/app/services/data.service';
import { BatchesService } from 'src/app/services/batches.service';
import { ErrorService } from 'src/app/services/error.service';
import { ILotesAsignadosReq, ILotesFormulaReq, ILotesFormulaRes, ILotesReq,
  ILotesSaveRes, ILotesSelectedReq } from 'src/app/model/http/lotesformula';
import { of } from 'rxjs';
import { CONST_NUMBER } from 'src/app/constants/const';

describe('InventorybatchesComponent', () => {
  let component: InventorybatchesComponent;
  let fixture: ComponentFixture<InventorybatchesComponent>;
  let dataServiceSpy: jasmine.SpyObj<DataService>;
  let batchesServiceSpy: jasmine.SpyObj<BatchesService>;
  let errorServiceSpy;
  const iLotesFormulaRes = new ILotesFormulaRes();
  const iLotesSaveRes = new ILotesSaveRes();
  const iLotesReq = new ILotesReq();
  const iLotesFormulaReq = new ILotesFormulaReq();
  const iLotesAsignadosReq = new ILotesAsignadosReq();

  // const iLotesSelectedReq = new ILotesSelectedReq();

  iLotesReq.fechaExp = new Date('12/12/21');
  iLotesReq.isValid = true;
  beforeEach(async(() => {
    errorServiceSpy = jasmine.createSpyObj<ErrorService>('ErrorService', [
      'httpError'
    ]);
    batchesServiceSpy = jasmine.createSpyObj<BatchesService>('BatchesService', [
      'getInventoryBatches',
      'updateBatches',
    ]);
    batchesServiceSpy.getInventoryBatches.and.callFake(() => {
      return of(iLotesFormulaRes);
    });
    batchesServiceSpy.updateBatches.and.returnValue(of(iLotesSaveRes));

    dataServiceSpy = jasmine.createSpyObj<DataService>('DataService', [
      'setGeneralNotificationMessage',
      'setUrlActive',
      'presentToastCustom',
      'getMessageTitle',
      'setPathUrl'
    ]);
    dataServiceSpy.setGeneralNotificationMessage.and.returnValue();
    dataServiceSpy.setUrlActive.and.returnValue();
    dataServiceSpy.presentToastCustom.and.returnValue(Promise.resolve());
    dataServiceSpy.getMessageTitle.and.returnValue('');
    dataServiceSpy.setPathUrl.and.returnValue();

    TestBed.configureTestingModule({
      imports: [
        RouterTestingModule,
        MATERIAL_COMPONENTS,
        HttpClientTestingModule,
        ReactiveFormsModule,
        FormsModule,
        BrowserAnimationsModule
      ],
      declarations: [ InventorybatchesComponent ],
      providers: [DatePipe]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InventorybatchesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
    expect(component.lotesSelectedColumns).toEqual([
      'lote',
      'seleccionada',
      'opciones'
    ]);
    expect(component.detailsColumns).toEqual([
      'cons',
      'codigoProducto',
      'descripcionProducto',
      'almacen',
      'totalNecesario',
      'totalSeleccionado'
    ]);
    expect(component.lotesColumns).toEqual([
      'cons',
      'disponible',
      'seleccionada',
      'asignada',
      'opciones'
    ]);
  });

  it('should call getInventoryBatches() ok', () => {
    component.ordenFabricacionId = '1234';
    batchesServiceSpy.getInventoryBatches.and.callFake(() => {
      return of(iLotesFormulaRes);
    });
    // expect(component.getInventoryBatches()).toBeTruthy();
    component.getInventoryBatches();
    // expect(batchesServiceSpy.getInventoryBatches).toHaveBeenCalled();
  });

  it('should return true', () => {
    // component.indexSelected = 0;
    const iLotesSelectReq = [
      {
        numeroLote: '1',
        cantidadSeleccionada: 1,
        sysNumber: 1,
        noidb: false,
        isValid: false
      }
    ];
    const iLotesReqq = component.dataSourceLotes.data = [
      {
        numeroLote: '1',
        cantidadDisponible: 1,
        cantidadAsignada: 1,
        cantidadSeleccionada: 1,
        sysNumber: 1,
        fechaExp: new Date('5/09/1993'),
        isValid: true
      }
    ];

    iLotesAsignadosReq.numeroLote = '1';
    iLotesAsignadosReq.sysNumber = 1;
    iLotesAsignadosReq.cantidadSeleccionada = 1;
    iLotesAsignadosReq.action = '';
    iLotesAsignadosReq.isValid = false;

    component.dataSourceDetails.data =
    [{
      codigoProducto: '1',
      descripcionProducto: '',
      almacen: '',
      totalNecesario: 1,
      totalSeleccionado: 1,
      selected: true,
      lotesSeleccionados: iLotesSelectReq,
      lotes: iLotesReqq,
      lotesAsignados: [iLotesAsignadosReq]
    }];
    iLotesFormulaReq.codigoProducto = '1';
    iLotesFormulaReq.descripcionProducto = '';
    iLotesFormulaReq.almacen = '';
    iLotesFormulaReq.totalNecesario = 1;
    iLotesFormulaReq.totalSeleccionado = 1;
    iLotesFormulaReq.lotesSeleccionados = iLotesSelectReq,
    iLotesFormulaReq.selected = true,
    iLotesFormulaReq.lotes = iLotesReqq,
    iLotesFormulaReq.lotesAsignados = [iLotesAsignadosReq];

    // component.setSelectedTr(iLotesFormulaReq);
    // expect(component.setSelectedTr()).toBeTruthy();
  });

  it('should return getBatchesFromSelected', () => {
    const iLotesSelectReq = [
      {
        numeroLote: '1',
        cantidadSeleccionada: 1,
        sysNumber: 1,
        isValid: false
      }
    ];
    const iLotesReqq = component.dataSourceLotes.data = [
      {
        numeroLote: '1',
        cantidadDisponible: 1,
        cantidadAsignada: 1,
        cantidadSeleccionada: 1,
        sysNumber: 1,
        fechaExp: new Date('5/09/1993'),
        isValid: true
      }
    ];

    const iLotesAsignadosReqq = component.dataSourceLotesAsignados.data = [
      {
        numeroLote: '1',
        cantidadSeleccionada: 1,
        sysNumber: 1,
        action: '',
        isValid: false
      }
    ];
    component.dataSourceDetails.data =
      [{
        codigoProducto: '87987',
        descripcionProducto: '',
        almacen: '',
        totalNecesario: 1,
        totalSeleccionado: 1,
        selected: true,
        lotesSeleccionados: iLotesSelectReq,
        lotes: iLotesReqq,
        lotesAsignados: iLotesAsignadosReqq
      }];
    component.dataSourceLotes.data = [
      {
        numeroLote: '1',
        cantidadDisponible: 1,
        cantidadAsignada: 1,
        cantidadSeleccionada: 1,
        sysNumber: 1,
        fechaExp: new Date('5/09/1993'),
        isValid: true
      }
    ];
    component.dataSourceLotesAsignados.data = [
      {
        numeroLote: '1',
        cantidadSeleccionada: 1,
        sysNumber: 1,
        action: '',
        noidb: false,
        isValid: true
      }
    ];
    // component.getBatchesFromSelected(87987);
    // expect(component.getBatchesFromSelected()).toBeFalsy();
  });

  it('should setSelectedTr return false', () => {

    const iLotesFormulaReqq = undefined;
    component.setSelectedTr(iLotesFormulaReqq);
    expect(component.setSelectedTr).toBeTruthy();
    // component.getBatchesFromSelected(1);
    // expect(component.getBatchesFromSelected()).toBeFalsy();
  });

  it('should deleteLotes', () => {
    component.indexSelected = 0;
    const iLotesSelectReq = [
      {
        numeroLote: '1',
        cantidadSeleccionada: 1,
        sysNumber: 1,
        isValid: false,
        action: '',
        noidb: false
      }
    ];
    const iLotesReqq = component.dataSourceLotes.data = [
      {
        numeroLote: '1',
        cantidadDisponible: 1,
        cantidadAsignada: 1,
        cantidadSeleccionada: 1,
        sysNumber: 1,
        fechaExp: new Date('5/09/1993'),
        isValid: true
      }
    ];
    iLotesAsignadosReq.numeroLote = '1';
    iLotesAsignadosReq.sysNumber = 1;
    iLotesAsignadosReq.cantidadSeleccionada = 1;
    iLotesAsignadosReq.action = '';
    iLotesAsignadosReq.isValid = false;
    component.dataSourceDetails.data =
    [{
      codigoProducto: '',
      descripcionProducto: '',
      almacen: '',
      totalNecesario: 1,
      totalSeleccionado: 1,
      selected: true,
      lotesSeleccionados: iLotesSelectReq,
      lotes: iLotesReqq,
      lotesAsignados: [iLotesAsignadosReq]
    }];
    component.deleteLotes(iLotesAsignadosReq);
    expect(component.deleteLotes()).toBeFalsy();
  });

  it('should deleteDetails tomarEnCuenta = true', () => {
    const iLotesSelectReq = [
      {
        numeroLote: '1',
        cantidadSeleccionada: 1,
        sysNumber: 1,
        isValid: false,
        action: '',
        noidb: false
      }
    ];
    const iLotesReqq = component.dataSourceLotes.data = [
      {
        numeroLote: '1',
        cantidadDisponible: 1,
        cantidadAsignada: 1,
        cantidadSeleccionada: 1,
        sysNumber: 1,
        fechaExp: new Date('5/09/1993'),
        isValid: true
      }
    ];
    iLotesAsignadosReq[0] = [{
      numeroLote: '1',
      sysNumber: 1,
      cantidadSeleccionada: 1,
      action: '',
      isValid: false,
    }];
    component.dataSourceDetails.data =
    [{
      codigoProducto: '',
      descripcionProducto: '',
      almacen: '',
      totalNecesario: 1,
      totalSeleccionado: 1,
      selected: true,
      lotesSeleccionados: iLotesSelectReq,
      lotes: iLotesReqq,
      lotesAsignados: [iLotesAsignadosReq]
    }];
    // component.deleteDetails(iLotesAsignadosReq[0]);
    // expect(component.deleteDetails()).toBeFalsy();
  });

  it('should deleteDetails tomarEnCuenta = false', () => {
    const iLotesSelectReq = [
      {
        numeroLote: '1',
        cantidadSeleccionada: 1,
        sysNumber: 1,
        isValid: false,
        action: '',
        noidb: true
      }
    ];
    const iLotesReqq = component.dataSourceLotes.data = [
      {
        numeroLote: '1',
        cantidadDisponible: 1,
        cantidadAsignada: 1,
        cantidadSeleccionada: 1,
        sysNumber: 1,
        fechaExp: new Date('5/09/1993'),
        isValid: true
      }
    ];
    iLotesAsignadosReq.numeroLote = '1';
    iLotesAsignadosReq.sysNumber = 1;
    iLotesAsignadosReq.cantidadSeleccionada = 1;
    iLotesAsignadosReq.action = '';
    iLotesAsignadosReq.isValid = false;
    component.dataSourceDetails.data =
    [{
      codigoProducto: '',
      descripcionProducto: '',
      almacen: '',
      totalNecesario: 1,
      totalSeleccionado: 1,
      selected: true,
      lotesSeleccionados: iLotesSelectReq,
      lotes: iLotesReqq,
      lotesAsignados: [iLotesAsignadosReq]
    }];
    component.deleteDetails(iLotesAsignadosReq);
    // expect(component.deleteDetails()).toBeFalsy();
  });


  it('should return false', () => {
    expect(component.setSelectedQuantity()).toBeFalsy();
  });

  it('should return false', () => {
    expect(component.setTotales()).toBeFalsy();
  });

  it('should addLotes', () => {
    const iLotesSelectReq = [
      {
        numeroLote: '1',
        cantidadSeleccionada: 1,
        sysNumber: 1,
        isValid: false
      }
    ];
    const iLotesReqq = component.dataSourceLotes.data = [
      {
        numeroLote: '1',
        cantidadDisponible: 1,
        cantidadAsignada: 1,
        cantidadSeleccionada: 1,
        sysNumber: 1,
        fechaExp: new Date('5/09/1993'),
        isValid: true
      }
    ];

    iLotesAsignadosReq.numeroLote = '1';
    iLotesAsignadosReq.sysNumber = 1;
    iLotesAsignadosReq.cantidadSeleccionada = 1;
    iLotesAsignadosReq.action = '';
    iLotesAsignadosReq.isValid = false;

    component.dataSourceDetails.data =
      [{
        codigoProducto: '',
        descripcionProducto: '',
        almacen: '',
        totalNecesario: 1,
        totalSeleccionado: 1,
        selected: true,
        lotesSeleccionados: iLotesSelectReq,
        lotes: iLotesReqq,
        lotesAsignados: [iLotesAsignadosReq]
      }];
    component.addLotes(iLotesReqq[0]);
    expect(dataServiceSpy.setGeneralNotificationMessage).toBeTruthy();
  });

  it('should buildObjectToSap', () => {
    const iLotesSelectReq = [
      {
        numeroLote: '1',
        cantidadSeleccionada: 1,
        sysNumber: 1,
        isValid: false
      }
    ];
    const iLotesReqq = component.dataSourceLotes.data = [
      {
        numeroLote: '1',
        cantidadDisponible: 1,
        cantidadAsignada: 1,
        cantidadSeleccionada: 1,
        sysNumber: 1,
        fechaExp: new Date('5/09/1993'),
        isValid: true
      }
    ];

    iLotesAsignadosReq.numeroLote = '1';
    iLotesAsignadosReq.sysNumber = 1;
    iLotesAsignadosReq.cantidadSeleccionada = 1;
    iLotesAsignadosReq.action = '';
    iLotesAsignadosReq.isValid = false;

    component.dataSourceDetails.data =
      [{
        codigoProducto: '',
        descripcionProducto: '',
        almacen: '',
        totalNecesario: 1,
        totalSeleccionado: 1,
        selected: true,
        lotesSeleccionados: iLotesSelectReq,
        lotes: iLotesReqq,
        lotesAsignados: [iLotesAsignadosReq]
      }];
    component.buildObjectToSap();
    expect(component.buildObjectToSap).toBeTruthy();
    dataServiceSpy.presentToastCustom.and.callFake(() => {
      return Promise.resolve({
        isConfirmed: true
      });
    });
    // batchesServiceSpy.updateBatches();
    // expect(batchesServiceSpy.updateBatches).toHaveBeenCalled();
  });
  it('should goToDetailOrder', () => {
    component.goToDetailOrder([]);
    expect(component.goToDetailOrder).toBeTruthy();
  });

  it('should goToOrdenFab', () => {
    component.goToOrdenFab([]);
    expect(component.goToOrdenFab).toBeTruthy();
  });

  it('should goToOrders', () => {
    component.goToOrders([]);
    expect(component.goToOrders).toBeTruthy();
  });

  it('should isDue', () => {
    component.isDue(iLotesReq);
    expect(component.isDue).toBeTruthy();
  });

  it('should isDue return false', () => {
    iLotesReq.fechaExp = undefined;
    component.isDue(iLotesReq);
    expect(component.isDue).toBeTruthy();
  });

  it('should getIsValid', () => {
    iLotesReq.numeroLote = '1';
    component.dataSourceLotes.data = [
      {
        numeroLote: '1',
        cantidadDisponible: 1,
        cantidadAsignada: 1,
        cantidadSeleccionada: 1,
        sysNumber: 1,
        fechaExp: new Date('5/09/1993'),
        isValid: true
      }
    ];
    component.getIsValid(iLotesReq);
  });
});
