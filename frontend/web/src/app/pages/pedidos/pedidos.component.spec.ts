import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { PedidosComponent } from './pedidos.component';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { DatePipe } from '@angular/common';
import { MATERIAL_COMPONENTS } from 'src/app/app.material';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { PedidosService } from '../../services/pedidos.service';
import { of, throwError } from 'rxjs';
import { PedidosListMock } from '../../../mocks/pedidosListMock';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ConstStatus, FromToFilter } from '../../constants/const';
import { PageEvent } from '@angular/material/paginator';
import { DataService } from '../../services/data.service';
import Swal from 'sweetalert2';
import { Catalogs, ICreatePdfOrdersRes, IProcessOrdersRes, IRecipesRes, ParamsPedidos } from '../../model/http/pedidos';
import { PipesModule } from '../../pipes/pipes.module';
import { RangeDateMOck } from '../../../mocks/rangeDateMock';
import { LocalStorageService } from 'src/app/services/local-storage.service';
import { ObservableService } from 'src/app/services/observable.service';
import { DateService } from 'src/app/services/date.service';
import { MessagesService } from 'src/app/services/messages.service';
import { FiltersService } from 'src/app/services/filters.service';
import { ErrorService } from 'src/app/services/error.service';
import { Router } from '@angular/router';
import { CommentsConfig } from 'src/app/model/device/incidents.model';
import { IOrdersRefuseReq, IPedidoRefuseRes, ReasonRefuse } from 'src/app/model/http/detallepedidos.model';

describe('PedidosComponent', () => {
  let component: PedidosComponent;
  let fixture: ComponentFixture<PedidosComponent>;
  let localStorageServiceSpy: jasmine.SpyObj<LocalStorageService>;
  let pedidosServiceSpy;
  let dataServiceSpy: jasmine.SpyObj<DataService>;
  let observableServiceSpy: jasmine.SpyObj<ObservableService>;
  let messagesServiceSpy: jasmine.SpyObj<MessagesService>;
  let dateServiceSpy: jasmine.SpyObj<DateService>;
  let filtersServiceSpy: jasmine.SpyObj<FiltersService>;
  let errorServiceSpy;
  const routerSpy = {navigate: jasmine.createSpy('navigate')};

  const paramsPedidos = new ParamsPedidos();
  const iRecipesRes = new IRecipesRes();
  const iCreatePdfOrdersRes = new ICreatePdfOrdersRes();
  const iPedidoRefuseRes = new IPedidoRefuseRes();
  beforeEach(async(() => {
    messagesServiceSpy = jasmine.createSpyObj<MessagesService>('MessagesService', [
      'presentToastCustom',
      'getMessageTitle'
    ]);

    localStorageServiceSpy = jasmine.createSpyObj<LocalStorageService>('LocalStorageService', [
      'getUserId', 'setRefreshToken', 'getUserRole',
      'setProductNoLabel',
      'setFiltersActives',
      'getFiltersActives',
      'removeFiltersActive',
      'getFiltersActivesAsModel'
    ]);
    // localStorageServiceSpy.getUserRole.and.returnValue('');
    dataServiceSpy = jasmine.createSpyObj<DataService>('DataService',
      [
        'getItemOnDataOnlyIds',
        'openNewTapByUrl',
      ]);
    dataServiceSpy.getItemOnDataOnlyIds.and.returnValue([]);
    messagesServiceSpy.presentToastCustom.and.returnValue(Promise.resolve(true));
    messagesServiceSpy.getMessageTitle.and.returnValue('');

    localStorageServiceSpy.getFiltersActives.and.returnValue('');
    localStorageServiceSpy.getFiltersActivesAsModel.and.returnValue(paramsPedidos);
    pedidosServiceSpy = jasmine.createSpyObj<PedidosService>('PedidosService', [
      'getPedidos',
      'processOrders',
      'getInitRangeDate',
      'getRecipesByOrder',
      'createPdfOrders',
      'getOrdersPdfViews',
      'putRefuseOrders'
    ]);
    pedidosServiceSpy.processOrders.and.callFake(() => {
      return of({ success: true, response: ['id'] } as IProcessOrdersRes);
    });
    pedidosServiceSpy.getInitRangeDate.and.callFake(() => {
      return of(RangeDateMOck);
    });
    pedidosServiceSpy.getPedidos.and.callFake(() => {
      return of(PedidosListMock);
    });
    pedidosServiceSpy.getRecipesByOrder .and.callFake(() => {
      return of(iRecipesRes);
    });
    pedidosServiceSpy.createPdfOrders.and.callFake(() => {
      return of(iCreatePdfOrdersRes);
    });
    pedidosServiceSpy.getOrdersPdfViews.and.callFake(() => {
      return of(iCreatePdfOrdersRes);
    });
    pedidosServiceSpy.putRefuseOrders.and.callFake(() => {
      return of(iPedidoRefuseRes);
    });

    errorServiceSpy = jasmine.createSpyObj<ErrorService>('ErrorService', [
      'httpError'
    ]);
    // -- Observable Service
    observableServiceSpy = jasmine.createSpyObj<ObservableService>
      ('ObservableService',
        [
          'getCallHttpService',
          'setMessageGeneralCallHttp',
          'setUrlActive',
          'setQbfToPlace',
          'setIsLoading',
          'setFinalizeOrders',
          'setCancelOrders',
          'getNewCommentsResult',
          'getNewSearchOrdersModal',
          'setSearchOrdersModal',
          'setOpenCommentsDialog'
        ]
      );
    observableServiceSpy.getCallHttpService.and.returnValue(of());
    observableServiceSpy.getNewSearchOrdersModal.and.returnValue(of());
    observableServiceSpy.getNewCommentsResult.and.returnValue(of());
    // --- Date Service
    dateServiceSpy = jasmine.createSpyObj<DateService>('DateService',
      [
        'transformDate',
        'getDateFormatted',
      ]);
    dateServiceSpy.transformDate.and.returnValue('');
    dateServiceSpy.getDateFormatted.and.returnValue('');
    // --- Filter Service
    filtersServiceSpy = jasmine.createSpyObj<FiltersService>('FiltersService', [
      'getIsThereOnData',
      'getItemOnDateWithFilter',
      'getNewDataToFilter',
      'getIsWithFilter',
    ]);

    filtersServiceSpy.getIsThereOnData.and.returnValue(true);
    filtersServiceSpy.getItemOnDateWithFilter.and.returnValue([]);
    filtersServiceSpy.getIsWithFilter.and.returnValue(true);
    filtersServiceSpy.getNewDataToFilter.and.returnValue([paramsPedidos, '']);
    TestBed.configureTestingModule({
      declarations: [PedidosComponent],
      imports: [RouterTestingModule, MATERIAL_COMPONENTS,
        HttpClientTestingModule, BrowserAnimationsModule, PipesModule],
      providers: [
        DatePipe,
        { provide: PedidosService, useValue: pedidosServiceSpy },
        { provide: DataService, useValue: dataServiceSpy },
        { provide: ObservableService, useValue: observableServiceSpy },
        { provide: LocalStorageService, useValue: localStorageServiceSpy },
        { provide: DateService, useValue: dateServiceSpy },
        { provide: MessagesService, useValue: messagesServiceSpy },
        { provide: FiltersService, useValue: filtersServiceSpy },
        { provide: ErrorService, useValue: errorServiceSpy },
        { provide: Router, useValue: routerSpy }
      ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PedidosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
    expect(component.displayedColumns)
      .toEqual(['seleccion', 'codigo', 'codigoDxp', 'cliente', 'medico', 'asesor',
        'orderType', 'f_inicio', 'f_fin', 'qfb_asignado', 'status', 'actions']);
    expect(component.limit).toEqual(10);
    expect(component.offset).toEqual(0);
  });
  it('should createInitRage error', () => {
    pedidosServiceSpy.getInitRangeDate.and.callFake(() => {
      return throwError({ error: true });
    });
    component.createInitRage();
    expect(errorServiceSpy.httpError).toHaveBeenCalled();
  });
  it('should call getPedidos ok', () => {
    component.offset = 0;
    component.limit = 10;
    component.queryString = 'rango de fechas';
    component.getFullQueryString();
    expect(component.fullQueryString).toEqual(`${component.queryString}&offset=0&limit=10`);
    component.getPedidos();
    expect(pedidosServiceSpy.getPedidos).toHaveBeenCalledWith(`${component.queryString}&offset=0&limit=10`);
    expect(component.lengthPaginator).toEqual(PedidosListMock.comments);
    expect(component.dataSource.data).toEqual(PedidosListMock.response);
    component.dataSource.data.filter(pedido => pedido.pedidoStatus === ConstStatus.abierto)
      .forEach(pedido => expect(pedido.class).toEqual('abierto'));
    component.dataSource.data.filter(pedido => pedido.pedidoStatus === ConstStatus.planificado)
      .forEach(pedido => expect(pedido.class).toEqual('planificado'));
    component.dataSource.data.filter(pedido => pedido.pedidoStatus === ConstStatus.liberado)
      .forEach(pedido => expect(pedido.class).toEqual('liberado'));
    component.dataSource.data.filter(pedido => pedido.pedidoStatus === ConstStatus.cancelado)
      .forEach(pedido => expect(pedido.class).toEqual('cancelado'));
    component.dataSource.data.filter(pedido => pedido.pedidoStatus === ConstStatus.enProceso)
      .forEach(pedido => expect(pedido.class).toEqual('proceso'));
    expect(component.isThereOrdersToPlan).toBeFalsy();
    expect(component.isThereOrdersToPlace).toBeFalsy();
    expect(component.isThereOrdersToCancel).toBeFalsy();
    expect(component.isThereOrdersToFinalize).toBeFalsy();
  });
  it('should call getPedidos error', () => {
    pedidosServiceSpy.getPedidos.and.callFake(() => {
      return throwError({ status: 500 });
    });
    component.getPedidos();
    expect(component.dataSource.data.length).toEqual(0);
  });
  // only Locally
  it('should updateAllComplete', () => {
    component.dataSource.data = PedidosListMock.response;
    component.dataSource.data.forEach(user => user.isChecked = false);
    component.updateAllComplete();
    expect(component.allComplete).toBeFalsy();
    component.dataSource.data.forEach(user => user.isChecked = true);
    component.updateAllComplete();
    expect(component.allComplete).toBeTruthy();

  });
  it('should someComplete', () => {
    component.dataSource.data = [];
    component.dataSource.data = PedidosListMock.response;
    component.dataSource.data.forEach(user => user.isChecked = false);
    component.allComplete = false;
    expect(component.someComplete()).toBeFalsy();
    component.dataSource.data.forEach(user => user.isChecked = true);
    expect(component.someComplete()).toBeTruthy();
  });
  it('should setAll', () => {
    component.dataSource.data = null;
    component.setAll(true);
    expect(component.allComplete).toBeTruthy();
    component.dataSource.data = PedidosListMock.response;
    component.setAll(true);
    expect(component.allComplete).toBeTruthy();
    expect(component.dataSource.data.every(user => user.isChecked)).toBeTruthy();
    component.setAll(false);
    expect(component.allComplete).toBeFalsy();
    expect(component.dataSource.data.every(user => user.isChecked)).toBeFalsy();
  });
  it('should changeDataEvent', () => {
    expect(component.changeDataEvent({ pageIndex: 0, pageSize: 5 } as PageEvent)).toEqual({ pageIndex: 0, pageSize: 5 } as PageEvent);
    expect(component.pageIndex).toEqual(0);
    expect(component.offset).toEqual(0);
    expect(component.limit).toEqual(5);
  });
  it('should openPlaceOrdersDialog', () => {
    component.dataSource.data = [];
    component.openPlaceOrdersDialog();
    component.dataSource.data = PedidosListMock.response;
    component.dataSource.data.filter(order => order.pedidoStatus === ConstStatus.planificado)
      .forEach(order => order.isChecked = true);
    expect(observableServiceSpy.setQbfToPlace).toHaveBeenCalled();
  });
  it('should cancelOrders', () => {
    component.dataSource.data = [{
      isChecked: true,
      docNum: 1,
      docNumDxp: '1',
      codigo: '1',
      cliente: '1',
      medico: '1',
      asesorName: 'qwerty',
      fechaInicio: '12-12-12',
      fechaFin: '13-12-12',
      pedidoStatus: 'Planificado',
      labelType: 'string',
      finishedLabel: 2,
      orderType: 'string'
    }];
    component.cancelOrders();
    component.dataSource.data = PedidosListMock.response;
    component.dataSource.data.filter(order => order.pedidoStatus !== ConstStatus.finalizado)
      .forEach(order => order.isChecked = true);
    expect(component.cancelOrders).toBeTruthy();
  });
  it('should finalizeOrders', () => {
    component.dataSource.data = [{
      isChecked: true,
      docNum: 1,
      docNumDxp: '1',
      codigo: '1',
      cliente: '1',
      medico: '1',
      asesorName: 'qwerty',
      fechaInicio: '12-12-12',
      fechaFin: '13-12-12',
      pedidoStatus: 'Terminado',
      labelType: 'string',
      finishedLabel: 2,
      orderType: 'string'
    }];
    component.finalizeOrders();
    component.dataSource.data = PedidosListMock.response;
    component.dataSource.data.filter(order => order.pedidoStatus === ConstStatus.terminado)
      .forEach(order => order.isChecked = true);
    expect(component.finalizeOrders).toBeTruthy();
  });
  it('should processOrders', () => {
    messagesServiceSpy.presentToastCustom.and.callFake(() => {
      return Promise.resolve({
        isConfirmed: true
      });
    });
    component.dataSource.data = [];
    component.processOrdersService();
    expect(messagesServiceSpy.presentToastCustom).toHaveBeenCalled();
    // setTimeout(() => {
    //   expect(Swal.isVisible()).toBeFalsy();
    //   Swal.clickConfirm();
    //   // expect(pedidosServiceSpy.processOrders).toHaveBeenCalled();
    //   done();
    // });
  });
  it('should processOrdersService error', () => {
    messagesServiceSpy.presentToastCustom.and.callFake(() => {
      return Promise.resolve({
        isConfirmed: true
      });
    });
    pedidosServiceSpy.processOrders();
    pedidosServiceSpy.processOrders.and.callFake(() => {
      return throwError({ error: true });
    });
    component.processOrdersService();
    expect(messagesServiceSpy.presentToastCustom).toHaveBeenCalled();
    expect(pedidosServiceSpy.processOrders).toHaveBeenCalled();
    // setTimeout(() => {
    //   expect(Swal.isVisible()).toBeFalsy();
    //   Swal.clickConfirm();
    //   // expect(pedidosServiceSpy.processOrders).toHaveBeenCalled();
    //   done();
    // });
  });


  it('should showMessagesAndRefresh', () => {
    component.showMessagesAndRefresh();
    expect(observableServiceSpy.setMessageGeneralCallHttp).toHaveBeenCalled();
  });
  it('should openFindOrdersDialog', () => {
    component.openFindOrdersDialog();
    expect(observableServiceSpy.setSearchOrdersModal).toHaveBeenCalled();
  });

  // it('should onSuccessSearchOrderModal', () => {
  //   paramsPedidos.dateType = '';
  //   paramsPedidos.pageIndex = 1;
  //   paramsPedidos.offset = 1;
  //   paramsPedidos.limit = 1;
  //   component.onSuccessSearchOrderModal(paramsPedidos);
  // });

  it('should reassignOrders', () => {
    component.dataSource.data = [{
      isChecked: true,
      docNum: 1,
      docNumDxp: '1',
      codigo: '1',
      cliente: '1',
      medico: '1',
      asesorName: 'qwerty',
      fechaInicio: '12-12-12',
      fechaFin: '13-12-12',
      pedidoStatus: 'Terminado',
      labelType: 'string',
      finishedLabel: 2,
      orderType: 'string'
    }];
    filtersServiceSpy.getItemOnDateWithFilter(component.dataSource.data, FromToFilter.fromOrdersReassign, ConstStatus.liberado);
    component.reassignOrders();
    expect(filtersServiceSpy.getItemOnDateWithFilter).toHaveBeenCalled();
  });

  it('should toSeeRecipes', () => {
    iRecipesRes.response = [{
      order: 1,
      recipe: ''
    }];
    component.toSeeRecipes(1);
    expect(pedidosServiceSpy.getRecipesByOrder).toHaveBeenCalled();
  });

  it('should toSeeRecipes error', () => {
    pedidosServiceSpy.getRecipesByOrder.and.callFake(() => {
      return throwError({ error: true });
    });
    component.toSeeRecipes(1);
    expect(errorServiceSpy.httpError).toHaveBeenCalled();
  });

  // it('should onSuccessHttpGetRecipes.lenght === CONST_NUMBER.zero', () => {
  //   iRecipesRes.response = [];
  //   component.onSuccessHttpGetRecipes(iRecipesRes);
  //   expect(dataServiceSpy.openNewTapByUrl).toHaveBeenCalled();
  // });

  it('should onSuccessHttpGetRecipes.lenght !== CONST_NUMBER.zero', () => {
    iRecipesRes.response = [{
      order: 1,
      recipe: ''
    }];
    component.onSuccessHttpGetRecipes(iRecipesRes);
    expect(dataServiceSpy.openNewTapByUrl).toHaveBeenCalled();
  });

  // it('should requestMaterial', () => {
  //   component.requestMaterial();
  //   expect (routerSpy.navigate).toHaveBeenCalledWith(['/materialRequest']);
  // });

  it('should printOrderAsPdfFile', () => {
    // iCreatePdfOrdersRes.response;
    component.isCheckedOrders = true;
    component.dataSource.data = [{
      isChecked: true,
      docNum: 1,
      docNumDxp: '1',
      codigo: '1',
      cliente: '1',
      medico: '1',
      asesorName: 'qwerty',
      fechaInicio: '12-12-12',
      fechaFin: '13-12-12',
      pedidoStatus: 'Terminado',
      labelType: 'string',
      finishedLabel: 2,
      orderType: 'string'
    }];
    messagesServiceSpy.presentToastCustom.and.callFake(() => {
      return Promise.resolve({
        isConfirmed: true
      });
    });
    component.printOrderAsPdfFile();
    expect(messagesServiceSpy.presentToastCustom).toHaveBeenCalled();
  });


  it('should printOrderAsPdfFileConfirmedAction error', () => {
    pedidosServiceSpy.createPdfOrders.and.callFake(() => {
      return throwError({ error: true });
    });
    component.printOrderAsPdfFileConfirmedAction();
    expect(errorServiceSpy.httpError).toHaveBeenCalled();
  });

  it('should openNewTabByOrder', () => {
    component.openNewTabByOrder(0);
    // expect (routerSpy.navigate).toHaveBeenCalledWith(['/pdetalle']);
    expect(component.openNewTabByOrder).toBeTruthy();
  });


  it('should viewPedidosWithPdf error', () => {
    pedidosServiceSpy.getOrdersPdfViews.and.callFake(() => {
      return throwError({ error: true });
    });
    component.viewPedidosWithPdf();
    expect(errorServiceSpy.httpError).toHaveBeenCalled();
  });

  it('should ordersToRefuse', () => {
    messagesServiceSpy.presentToastCustom.and.callFake(() => {
      return Promise.resolve({
        isConfirmed: true
      });
    });
    component.ordersToRefuse();
    expect(messagesServiceSpy.presentToastCustom).toHaveBeenCalled();
  });

  it('should successNewComments error', () => {
    const commentsConfig = new CommentsConfig();
    commentsConfig.comments = '';
    // commentsConfig.
    const iOrdersRefuseReq = new IOrdersRefuseReq();

    pedidosServiceSpy.putRefuseOrders(iOrdersRefuseReq);
    pedidosServiceSpy.putRefuseOrders.and.callFake(() => {
      return throwError({ error: true });
    });
    component.successNewComments(commentsConfig);
    expect(errorServiceSpy.httpError).toHaveBeenCalled();
  });

  it('should successRefuseResult', () => {
    messagesServiceSpy.presentToastCustom.and.callFake(() => {
      return Promise.resolve({
        isConfirmed: true
      });
    });
    component.successRefuseResult([]);
    expect(component.successRefuseResult).toBeTruthy();
  });

  it('should successRefuseResult failed.lenght != 0', () => {
    messagesServiceSpy.presentToastCustom.and.callFake(() => {
      return Promise.resolve({
        isConfirmed: true
      });
    });
    const reasonRefuse = new ReasonRefuse();
    reasonRefuse.reason = '';
    component.successRefuseResult([reasonRefuse]);
    expect(messagesServiceSpy.presentToastCustom).toHaveBeenCalled();
  });

  it('should createProductoNoLabel error', () => {
    pedidosServiceSpy.getInitRangeDate.and.callFake(() => {
      return throwError({ error: true });
    });
    component.createProductoNoLabel();
    expect(errorServiceSpy.httpError).toHaveBeenCalled();
  });


  it('should setProductNoLabel', () => {
    const catalogs = new Catalogs();
    component.setProductNoLabel(catalogs);
    expect(localStorageServiceSpy.setProductNoLabel).toHaveBeenCalled();
  });
});
