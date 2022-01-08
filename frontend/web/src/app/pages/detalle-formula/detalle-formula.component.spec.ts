import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { DetalleFormulaComponent } from './detalle-formula.component';
import { RouterTestingModule } from '@angular/router/testing';
import { MATERIAL_COMPONENTS } from 'src/app/app.material';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { DatePipe } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { PedidosService } from 'src/app/services/pedidos.service';
import { DetalleFormulaMock } from 'src/mocks/pedidosListMock';
import { Observable, of, Subject, throwError } from 'rxjs';
import { DataService } from '../../services/data.service';
import { CarouselOption, CONST_DETAIL_FORMULA } from '../../constants/const';
import { ErrorService } from '../../services/error.service';
import { ComponentsModule } from 'src/app/components/components.module';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ParamsPedidos } from 'src/app/model/http/pedidos';
import { ObservableService } from 'src/app/services/observable.service';
import { LocalStorageService } from 'src/app/services/local-storage.service';
import { DateService } from 'src/app/services/date.service';
import { MessagesService } from 'src/app/services/messages.service';
import { FiltersService } from '../../services/filters.service';

describe('DetalleFormulaComponent', () => {
  let component: DetalleFormulaComponent;
  let fixture: ComponentFixture<DetalleFormulaComponent>;
  let pedidosServiceSpy;
  let dataServiceSpy: jasmine.SpyObj<DataService>;
  let errorServiceSpy;
  let observableServiceSpy: jasmine.SpyObj<ObservableService>;
  let localStorageServiceSpy: jasmine.SpyObj<LocalStorageService>;
  let dateServiceSpy: jasmine.SpyObj<DateService>;
  let filtersServiceSpy: jasmine.SpyObj<FiltersService>;
  const paramsPedidos = new ParamsPedidos();
  let messagesServiceSpy: jasmine.SpyObj<MessagesService>;

  // let routerSpy: jasmine.SpyObj<ActivatedRoute>;

  beforeEach(async(() => {
    messagesServiceSpy = jasmine.createSpyObj<MessagesService>('MessagesService', [
      'presentToastCustom'
    ]);

    errorServiceSpy = jasmine.createSpyObj<ErrorService>('ErrorService',
      [
        'httpError'
      ]);
    localStorageServiceSpy = jasmine.createSpyObj<LocalStorageService>('LocalStorageService', [
        'getUserId',
        'getOrderIsolated',
        'removeOrderIsolated',
        'setFiltersActivesOrders',
        'getFiltersActivesOrders',
        'removeFiltersActiveOrders',
        'getFiltersActivesAsModelOrders',
      ]);
    pedidosServiceSpy = jasmine.createSpyObj<PedidosService>('PedidosService',
      [
        'getFormulaDetail',
        'getFormulaCarousel',
        'updateFormula'
      ]);
    dataServiceSpy = jasmine.createSpyObj<DataService>('DataService', [
      'getItemOnDataOnlyIds',
      'setIsToSaveAnything',
      'setIsToSaveAnything',
      'setIsToSaveAnything',
      'getFullStringForCarousel',
      'getIsToSaveAnything'
    ]);
    // routerSpy = jasmine.createSpyObj<ActivatedRoute>('ActivateRoute', [
    //   'paramMap'
    // ]);
    // routerSpy.paramMap.and.returnValue();
    localStorageServiceSpy.getFiltersActivesAsModelOrders.and.returnValue(paramsPedidos);
    pedidosServiceSpy.getFormulaDetail.and.callFake(() => {
      return of(DetalleFormulaMock);
    });
    pedidosServiceSpy.getFormulaCarousel.and.callFake(() => {
      return of(DetalleFormulaMock);
    });
    pedidosServiceSpy.updateFormula.and.callFake(() => {
      return of();
    });
    messagesServiceSpy.presentToastCustom.and.callFake(() => Promise.resolve([]));

    // --- Observable Service
    observableServiceSpy = jasmine.createSpyObj<ObservableService>('ObservableService',
      [
        'getCallHttpService',
        'setUrlActive',
        'setIsLoading',
        'setCallHttpService',
        'setMessageGeneralCallHttp',
        'getNewSearchOrdersModal',
        'setSearchComponentModal',
        'setCancelOrders',
        'setQbfToPlace',
        'getNewFormulaComponent',
        'setPathUrl',
      ]
    );
    observableServiceSpy.setMessageGeneralCallHttp.and.callFake(() => {
      return;
    });
    observableServiceSpy.getNewFormulaComponent.and.callFake(() => {
      return new Observable();
    });
    // --- Date Service
    dateServiceSpy = jasmine.createSpyObj<DateService>('DateService', [
      'transformDate',
    ]);
    dateServiceSpy.transformDate.and.returnValue('');
    // --- Filter Service
    filtersServiceSpy = jasmine.createSpyObj<FiltersService>('FiltersService', [
      'getIsThereOnData',
      'getItemOnDateWithFilter',
      'getNewDataToFilter',
    ]);
    filtersServiceSpy.getIsThereOnData.and.returnValue(true);
    filtersServiceSpy.getItemOnDateWithFilter.and.returnValue([]);
    filtersServiceSpy.getNewDataToFilter.and.returnValue([new ParamsPedidos(), '']);
  
    TestBed.configureTestingModule({
      imports: [
        RouterTestingModule,
        MATERIAL_COMPONENTS,
        HttpClientTestingModule,
        ReactiveFormsModule,
        FormsModule,
        BrowserAnimationsModule,
        ComponentsModule,
        RouterModule
      ],
      declarations: [DetalleFormulaComponent],
      providers: [
        DatePipe, {
          provide: PedidosService, useValue: pedidosServiceSpy
        },
        { provide: DataService, useValue: dataServiceSpy },
        { provide: ErrorService, useValue: errorServiceSpy },
        { provide: ObservableService, useValue: observableServiceSpy },
        { provide: LocalStorageService, useValue: localStorageServiceSpy},
        { provide: DateService, useValue: dateServiceSpy },
        { provide: MessagesService, useValue: messagesServiceSpy },
        { provide: FiltersService, useValue: filtersServiceSpy },
        { provide: ActivatedRoute, useValue: { paramMap: new Subject() } },
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DetalleFormulaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
    expect(component.displayedColumns).toEqual([
      'seleccion',
      'cons',
      'numero',
      'descripcion',
      'cantbase',
      'cantreq',
      'consumido',
      'disponible',
      'unidad',
      'almacen',
      'cantpend',
      'enstock',
      'cantalmacen'
    ]);
  });

  it('should call getDetalleFormula ok', () => {
    component.ordenFabricacionId = '1234';
    component.getDetalleFormula();
    expect(pedidosServiceSpy.getFormulaDetail).toHaveBeenCalledWith(component.ordenFabricacionId);
  });

  it('should call getFormulaDetail error', () => {
    pedidosServiceSpy.getFormulaDetail.and.callFake(() => {
      return throwError({ status: 500 });
    });
    component.getDetalleFormula();
    expect(errorServiceSpy.httpError).toHaveBeenCalled();
  });

  it('should updateAllComplete', () => {
    component.dataSource.data = DetalleFormulaMock.response.details;
    component.dataSource.data.forEach(element => element.isChecked = false);
    component.updateAllComplete();
    expect(component.allComplete).toBeFalsy();
    component.dataSource.data.forEach(element => element.isChecked = true);
    component.updateAllComplete();
    expect(component.allComplete).toBeTruthy();

  });
  it('should someComplete', () => {
    component.dataSource.data = [];
    component.dataSource.data = DetalleFormulaMock.response.details;
    component.dataSource.data.forEach(element => element.isChecked = false);
    component.allComplete = false;
    expect(component.someComplete()).toBeFalsy();
    component.dataSource.data.forEach(element => element.isChecked = true);
    expect(component.someComplete()).toBeTruthy();
  });
  it('should setAll', () => {
    component.dataSource.data = null;
    component.setAll(true);
    expect(component.allComplete).toBeTruthy();
    component.dataSource.data = DetalleFormulaMock.response.details;
    component.setAll(false);
    expect(component.allComplete).toBeFalsy();
    expect(component.dataSource.data.every(element => element.isChecked)).toBeFalsy();
  });
  it('should getOpenDialog()', () => {
    component.openDialog();
    expect(observableServiceSpy.setSearchComponentModal).toHaveBeenCalled();
  });
  it('should onBaseQuantityChange()', () => {
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
      hasBatches: false
    };
    component.onBaseQuantityChange(1, 0);
    expect(component.dataSource.data[0].action).toBeDefined();
    component.dataSource.data[0].action = CONST_DETAIL_FORMULA.update;
    component.onBaseQuantityChange(1, 0);
    expect(component.dataSource.data[0].action).toBeDefined();
  });
  it('should onRequiredQuantityChange()', () => {
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
      hasBatches: false
    };
    component.oldDataFormulaDetail.plannedQuantity = 2;
    component.onRequiredQuantityChange(1, 0);
    expect(component.dataSource.data[0].action).toBeDefined();
    expect(component.dataSource.data[0].requiredQuantity).toBeDefined();
  });

  it('should saveFormulaDetail is false', () => {
    component.saveFormulaDetail();
    expect(component.isPlannedQuantityError).toBe(false);
    // expect(dataServiceSpy.setMessageGeneralCallHttp).toHaveBeenCalled();
  });
  // it('should saveFormulaDetail is true', () => {
  //   dataServiceSpy.setMessageGeneralCallHttp.and.callFake(() => {
  //     return;
  //   });
  //   component.saveFormulaDetail();
  //   component.isPlannedQuantityError = false;
  //   // expect(component.isPlannedQuantityError).toBe(true);
  //   expect(dataServiceSpy.setMessageGeneralCallHttp).toHaveBeenCalled();
  // });


  it('should deleteComponentsToDelete', () => {
    pedidosServiceSpy.updateFormula.and.callFake(() => {
      return of();
    });
    messagesServiceSpy.presentToastCustom.and.callFake(() => {
      return Promise.resolve({
        isConfirmed: true
      });
    });
    component.deleteComponents();
    // expect(pedidosServiceSpy.updateFormula).toHaveBeenCalled();
  });

  it('should changeData', () => {
    component.changeData();
    expect(component.changeData).toBeTruthy();
  });

  it('should onSelectWareHouseChange', () => {
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
      hasBatches: false
    };
    component.onSelectWareHouseChange('', 0);

    expect(component.onSelectWareHouseChange).toBeTruthy();
  });
  it('should goToOrders', () => {

    component.goToOrders([]);
    expect(component.goToOrders).toBeTruthy();
  });

  it('should goToDetailOrder', () => {

    component.goToDetailOrder([]);
    expect(component.goToDetailOrder).toBeTruthy();
  });

  it('should getIsElementsToSave', () => {
    component.getIsElementsToSave();
    expect(component.getIsElementsToSave).toBeTruthy();
  });

  it('should changeFormulaByFIltersService', () => {
    pedidosServiceSpy.getFormulaCarousel.and.callFake(() => {
      return of(DetalleFormulaMock);
    });
    component.changeFormulaByFIltersService('');
    expect(component.changeFormulaByFIltersService).toBeTruthy();
  });

  it('should changeFormulaByFIltersService error', () => {
    pedidosServiceSpy.getFormulaCarousel.and.callFake(() => {
      return throwError({ error: true });
    });
    component.changeFormulaByFIltersService('');
    expect(errorServiceSpy.httpError).toHaveBeenCalled();
  });

  it('should createfilterDataOrdersForOrderIsolated', () => {
    component.createfilterDataOrdersForOrderIsolated();
    expect(localStorageServiceSpy.setFiltersActivesOrders).toHaveBeenCalled();
  });

  it('should changeFormulaValidate = 0', () => {
    component.changeFormulaValidate(0);
    // CarouselOption.backDetail;

  });
  it('should changeDetailFormula', () => {
    messagesServiceSpy.presentToastCustom.and.callFake(() => {
      return Promise.resolve({
        isConfirmed: true
      });
    });
    component.changeDetailFormula(0);
    // expect(dataServiceSpy.presentToastCustom).toHaveBeenCalled();
  });

  it('should changeFormulaByIndex', () => {
    component.changeFormulaByIndex(CarouselOption.backDetail);
  });
});
