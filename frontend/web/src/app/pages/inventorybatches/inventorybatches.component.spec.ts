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
import { ILotesAsignadosReq, ILotesFormulaRes, ILotesReq, 
  ILotesSaveRes, ILotesSelectedReq } from 'src/app/model/http/lotesformula';
import { of } from 'rxjs';

describe('InventorybatchesComponent', () => {
  let component: InventorybatchesComponent;
  let fixture: ComponentFixture<InventorybatchesComponent>;
  let dataServiceSpy: jasmine.SpyObj<DataService>;
  let batchesServiceSpy: jasmine.SpyObj<BatchesService>;
  let errorServiceSpy;
  const iLotesFormulaRes = new ILotesFormulaRes();
  const iLotesSaveRes = new ILotesSaveRes();
  const iLotesReq = new ILotesReq();
  const iLotesSelectedReq = new ILotesSelectedReq();

  beforeEach(async(() => {
    errorServiceSpy = jasmine.createSpyObj<ErrorService>('ErrorService', [
      'httpError'
    ]);
    batchesServiceSpy = jasmine.createSpyObj<BatchesService>('BatchesService', [
      'getInventoryBatches',
      'updateBatches',
    ]);
    batchesServiceSpy.getInventoryBatches.and.returnValue(of(iLotesFormulaRes));
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
    expect(component.getInventoryBatches()).toBeTruthy();
    component.getInventoryBatches();
    // expect(batchesServiceSpy.getInventoryBatches).toHaveBeenCalled();
  });

  it('should return true', () => {
    component.setSelectedTr();
    expect(component.setSelectedTr()).toBeTruthy();
  });

  it('should return false', () => {
    component.getBatchesFromSelected();
    expect(component.getBatchesFromSelected()).toBeFalsy();
  });

  it('should return false', () => {
    expect(component.deleteLotes()).toBeFalsy();
  });

  it('should return false', () => {
    expect(component.deleteDetails()).toBeFalsy();
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

    const iLotesAsignadosReq = component.dataSourceLotesAsignados.data = [
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
        codigoProducto: '',
        descripcionProducto: '',
        almacen: '',
        totalNecesario: 1,
        totalSeleccionado: 1,
        selected: true,
        lotesSeleccionados: iLotesSelectReq,
        lotes: iLotesReqq,
        lotesAsignados: iLotesAsignadosReq
      }];
    // expect(component.addLotes(iLotesReq[0])).toBeTruthy();
  });
});
