import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PedidoDetalleComponent } from './pedido-detalle.component';
import { RouterTestingModule } from '@angular/router/testing';
import { IPedidoDetalleListRes, IPedidoDetalleReq } from 'src/app/model/http/detallepedidos.model';
import { MATERIAL_COMPONENTS } from 'src/app/app.material';
import { FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { DatePipe } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { PedidosService } from '../../services/pedidos.service';
import { of, throwError } from 'rxjs';
import { DetailOrderMock } from '../../../mocks/detailOrder.Mock';
import { DownloadImagesService } from '../../services/download-images.service';
import { UrlsOfQrEachOrderMock } from '../../../mocks/urlsOfQrEachOrderMock';
import { ErrorService } from '../../services/error.service';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { DataService } from 'src/app/services/data.service';
import { Catalogs, ICreatePdfOrdersRes, IProcessOrdersRes, ParamsPedidos } from 'src/app/model/http/pedidos';
import { FromToFilter, HttpServiceTOCall } from 'src/app/constants/const';
import { CommentsConfig } from '../../model/device/incidents.model';
import { LocalStorageService } from 'src/app/services/local-storage.service';
import { ObservableService } from 'src/app/services/observable.service';
import { MessagesService } from 'src/app/services/messages.service';
import { FiltersService } from 'src/app/services/filters.service';

describe('PedidoDetalleComponent', () => {
  let component: PedidoDetalleComponent;
  let fixture: ComponentFixture<PedidoDetalleComponent>;
  let pedidosServiceSpy: jasmine.SpyObj<PedidosService>;
  let downloadImagesServiceSpy;
  let dataServiceSpy: jasmine.SpyObj<DataService>;
  let localStorageServiceSpy: jasmine.SpyObj<LocalStorageService>;
  let errorServiceSpy;
  let observableServiceSpy: jasmine.SpyObj<ObservableService>;
  let messagesServiceSpy: jasmine.SpyObj<MessagesService>;
  let filtersServiceSpy: jasmine.SpyObj<FiltersService>;

  const catalogs = new Catalogs();
  const iPedidoDetalleRes = new IPedidoDetalleListRes();
  const iPedidoDetalleReq: IPedidoDetalleReq[] = [];
  const iProcessOrdersRes = new IProcessOrdersRes();
  const iPedidoDetalleListRes = new IPedidoDetalleListRes();

  const iCreatePdfOrdersRes = new ICreatePdfOrdersRes();

  const httpServiceTOCallRes: HttpServiceTOCall = HttpServiceTOCall.DETAIL_ORDERS;
  const comentsConfig = new CommentsConfig();
  const parametrosPedidos = new ParamsPedidos();
  parametrosPedidos.offset = 0;
  parametrosPedidos.limit = 10;
  parametrosPedidos.pageIndex = 0;

  iPedidoDetalleRes.response = iPedidoDetalleReq;
  catalogs.id = 74;
  catalogs.value = 'DZ';
  catalogs.type = 'string';
  catalogs.field = 'ProductNoLabel';
  beforeEach(async(() => {
    messagesServiceSpy = jasmine.createSpyObj<MessagesService>('MessagesService', [
      'presentToastCustom',
      'getMessageTitle',
    ]);

    errorServiceSpy = jasmine.createSpyObj<ErrorService>('ErrorService', [
      'httpError'
    ]);
    pedidosServiceSpy = jasmine.createSpyObj<PedidosService>('PedidosService', [
      'getPedidos',
      'processOrders',
      'getDetallePedido',
      'qrByEachOrder',
      'finishLabels',
      'savedComments',
      'putOrdersToDelivered',
      'postPlaceOrdersDetail',
      'getOrdersPdfViews',
      'getDetailCarousel'
    ]);
    localStorageServiceSpy = jasmine.createSpyObj<LocalStorageService>('LocalStorageService', [
      'getProductNoLabel',
      'getUserId',
      'getUserRole',
      'getFiltersActives',
      'getCurrentDetailOrder',
      'removeCurrentDetailOrder',
      'setCurrentDetailOrder',
    ]);
    dataServiceSpy = jasmine.createSpyObj<DataService>('DataService', [
      'openNewTapByUrl',
      'getItemOnDataOnlyIds',
      'getFullStringForCarousel',
      'changeRouterForFormula'
    ]);
    localStorageServiceSpy.getProductNoLabel.and.returnValue(catalogs);
    localStorageServiceSpy.getUserRole.and.returnValue('4');
    localStorageServiceSpy.getFiltersActives.and.returnValue(JSON.stringify(parametrosPedidos));
    localStorageServiceSpy.getCurrentDetailOrder.and.returnValue('');
    dataServiceSpy.getItemOnDataOnlyIds.and.returnValue([]);
    downloadImagesServiceSpy = jasmine.createSpyObj<DownloadImagesService>('DownloadImagesService', ['downloadImageFromUrl']);
    pedidosServiceSpy.qrByEachOrder.and.callFake(() => {
      return of(UrlsOfQrEachOrderMock);
    });
    pedidosServiceSpy.finishLabels.and.returnValue(of(iPedidoDetalleRes));
    pedidosServiceSpy.putOrdersToDelivered.and.returnValue(of(iCreatePdfOrdersRes));
    pedidosServiceSpy.getDetallePedido.and.returnValue(of(DetailOrderMock));
    pedidosServiceSpy.savedComments.and.returnValue(of(iPedidoDetalleRes));
    pedidosServiceSpy.postPlaceOrdersDetail.and.callFake(() => {
      return of(iProcessOrdersRes);
    });
    pedidosServiceSpy.getOrdersPdfViews.and.callFake(() => {
      return of(iCreatePdfOrdersRes);
    });
    pedidosServiceSpy.getDetailCarousel .and.callFake(() => {
      return of(iPedidoDetalleListRes);
    });

    // --- ObservableService
    observableServiceSpy = jasmine.createSpyObj<ObservableService>('ObservableService',
      [
        'setUrlActive',
        'setCancelOrders',
        'setFinalizeOrders',
        'setQbfToPlace',
        'setPathUrl',
        'setOpenCommentsDialog',
        'setMessageGeneralCallHttp',
        'setOpenSignatureDialog',
        'getCallHttpService',
        'getNewDataSignature',
        'getNewCommentsResult',
      ]
    );
    observableServiceSpy.getNewDataSignature.and.returnValue(of({}));
    observableServiceSpy.getNewCommentsResult.and.returnValue(of(comentsConfig));
    observableServiceSpy.getCallHttpService.and.returnValue(of(httpServiceTOCallRes));

    // --- Filter Service
    filtersServiceSpy = jasmine.createSpyObj<FiltersService>('FiltersService', [
      'getIsThereOnData',
      'getItemOnDateWithFilter',
      'getNewDataToFilter',
      'getIsWithFilter',
    ]);

    filtersServiceSpy.getIsThereOnData.and.returnValue(true);
    filtersServiceSpy.getItemOnDateWithFilter.and.callFake(() => {
      return [];
    });
    filtersServiceSpy.getIsWithFilter.and.returnValue(true);
    filtersServiceSpy.getNewDataToFilter.and.returnValue([new ParamsPedidos(), '']);

    TestBed.configureTestingModule({
      imports: [
        MATERIAL_COMPONENTS,
        HttpClientTestingModule,
        RouterTestingModule,
        FormsModule,
        BrowserAnimationsModule],
      declarations: [PedidoDetalleComponent],
      providers: [DatePipe,
        { provide: PedidosService, useValue: pedidosServiceSpy },
        { provide: DownloadImagesService, useValue: downloadImagesServiceSpy },
        { provide: ErrorService, useValue: errorServiceSpy },
        { provide: DataService, useValue: dataServiceSpy },
        { provide: LocalStorageService, useValue: localStorageServiceSpy},
        { provide: ObservableService, useValue: observableServiceSpy },
        { provide: MessagesService, useValue: messagesServiceSpy },
        { provide: FiltersService, useValue: filtersServiceSpy },
      ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PedidoDetalleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });
  it('should create', () => {
    expect(component).toBeTruthy();
  });
  it('should getProductoNoLabel', () => {
    component.getProductoNoLabel();
    expect(component.ProductNoLabel.value).toEqual(catalogs.value);
  });

  it('should someComplete return false', () => {
    component.allComplete = true;
    expect(component.someComplete()).toBeFalsy();
  });
  it('should someComplete return true', () => {
    component.dataSource.data = [
      {
        isChecked: true,
      } as IPedidoDetalleReq
    ];
    expect(component.someComplete()).toBeTruthy();
  });

  it('should someCompletereturn false when allcomplete is true', () => {
    component.dataSource.data = [
      {
        isChecked: false,
      } as IPedidoDetalleReq
    ];
    component.allComplete = true;
    expect(component.someComplete()).toBeFalsy();
  });
  it('should updateAllComplete', () => {
    component.dataSource.data = DetailOrderMock.response;
    component.updateAllComplete();
    expect(component.allComplete).toBeFalsy();
    component.dataSource.data.forEach(detail => detail.isChecked = true);
    expect(component.OrderToGenerateQR = component.dataSource.data.some(detail => detail.isChecked)).toBeTruthy();
    component.updateAllComplete();
    expect(component.OrderToGenerateQR = component.dataSource.data.some(detail => detail.isChecked)).toBeTruthy();
    expect(component.allComplete).toBeTruthy();
  });
  it('should setAll', () => {
    component.dataSource.data.forEach(detail => detail.isChecked = false);
    component.dataSource.data = null;
    component.setAll(false);
    expect(component.allComplete).toBe(false);
    component.dataSource.data = DetailOrderMock.response;
    component.setAll(true);
    expect(component.allComplete).toBe(true);
    expect(component.dataSource.data.every(detail => detail.isChecked)).toBeTruthy();
    expect(component.OrderToGenerateQR = component.dataSource.data.some(detail => detail.isChecked)).toBeTruthy();
    component.setAll(false);
    expect(component.allComplete).toBe(false);
    expect(component.dataSource.data.every(detail => !detail.isChecked)).toBeTruthy();
    expect(component.OrderToGenerateQR = component.dataSource.data.some(detail => !detail.isChecked)).toBeTruthy();
  });

  it('should call ordersToDownloadQr()', () => {
    component.dataSource.data = [
      {
        status: 'Planificado',
        pedidoStatus: 'Planificado',
        isChecked: true,
      } as IPedidoDetalleReq
    ];

    component.ordersToDownloadQr();
    expect(pedidosServiceSpy.qrByEachOrder).toHaveBeenCalled();
    expect(downloadImagesServiceSpy.downloadImageFromUrl).toHaveBeenCalledTimes(1);
  });
  it('should call ordersToDownloadQr() error', () => {
    pedidosServiceSpy.qrByEachOrder.and.callFake(() => {
      return throwError({ error: true });
    });
    component.dataSource.data = [
      {
        status: 'Planificado',
        pedidoStatus: 'Planificado',
        isChecked: true,
      } as IPedidoDetalleReq
    ];
    component.ordersToDownloadQr();
    expect(pedidosServiceSpy.qrByEachOrder).toHaveBeenCalled();
    expect(errorServiceSpy.httpError).toHaveBeenCalled();
  });
  it('should processOrdersDetail', () => {
    messagesServiceSpy.presentToastCustom.and.callFake(() => {
    return Promise.resolve({
      isConfirmed: true
    });
  });
    component.dataSource.data = [
      {
        isChecked: true,
        status: 'Abierto',
        codigoProducto: '1'
      } as IPedidoDetalleReq
    ];
    iProcessOrdersRes.response = ['1'];
    pedidosServiceSpy.postPlaceOrdersDetail();
    component.processOrdersDetail();
    expect(messagesServiceSpy.presentToastCustom).toHaveBeenCalled();
  });

  it('should processOrdersDetail error', () => {
    messagesServiceSpy.presentToastCustom.and.callFake(() => {
      return Promise.resolve({
        isConfirmed: true
      });
    });
    component.dataSource.data = [
      {
        isChecked: true,
        status: 'Abierto',
        codigoProducto: '1'
      } as IPedidoDetalleReq
    ];
    pedidosServiceSpy.postPlaceOrdersDetail();
    pedidosServiceSpy.postPlaceOrdersDetail.and.callFake(() => {
      return throwError({ error: true });
    });
    component.processOrdersDetail();
    expect(pedidosServiceSpy.postPlaceOrdersDetail).toHaveBeenCalled();
  });

  it('should cancelOrders', () => {
    component.cancelOrders();
    expect(observableServiceSpy.setCancelOrders).toHaveBeenCalled();
  });

  it('should finalizeOrdersDetail', () => {
    component.finalizeOrdersDetail();
    expect(observableServiceSpy.setFinalizeOrders).toHaveBeenCalled();
  });

  it('should goToOrders', () => {
    component.goToOrders(['']);
    expect(observableServiceSpy.setPathUrl).toHaveBeenCalled();
  });

  it('should addCommentsDialog', () => {
    component.isCorrectToAddComments = false;
    component.addCommentsDialog();
    expect(observableServiceSpy.setOpenCommentsDialog).toHaveBeenCalled();
    // expect(observableServiceSpy.setPathUrl).toHaveBeenCalled();
  });

  it('should addCommentsOnService error', () => {
    pedidosServiceSpy.savedComments.and.callFake(() => {
      return throwError({ error: true });
    });
    component.addCommentsOnService('');
    expect(pedidosServiceSpy.savedComments).toHaveBeenCalled();
    expect(errorServiceSpy.httpError).toHaveBeenCalled();
  });

  it('should finishOrdersLabels', () => {
    component.finishOrdersLabels();
    expect(observableServiceSpy.setOpenSignatureDialog).toHaveBeenCalled();
  });

  it('should createConsumeService error', () => {
    pedidosServiceSpy.finishLabels.and.callFake(() => {
      return throwError({ error: true });
    });
    component.createConsumeService();
    expect(pedidosServiceSpy.finishLabels).toHaveBeenCalled();
    expect(errorServiceSpy.httpError).toHaveBeenCalled();
  });

  it('should getArrayToFinishLabel', () => {
    component.ProductNoLabel.value = 'DZ';
    component.dataSource.data = [
      {
        isChecked: true,
        status: 'Planificado',
        codigoProducto: 'oj 6',
        finishedLabel: 0,
      } as IPedidoDetalleReq
    ];
    component.getArrayToFinishLabel(true, 0);
    expect(component.getArrayToFinishLabel).toBeTruthy();
  });

  it('should removeSignature', () => {
    messagesServiceSpy.presentToastCustom.and.callFake(() => {
      return Promise.resolve({
        isConfirmed: true
      });
    });
    component.removeSignature(0);
    expect(messagesServiceSpy.presentToastCustom).toHaveBeenCalled();
  });

  // it('should viewOrdersWithPdf', () => {
  //   component.viewOrdersWithPdf();
  //   expect(pedidosServiceSpy.getOrdersPdfViews).toHaveBeenCalled();
  // });

  it('should viewOrdersWithPdf error', () => {
    pedidosServiceSpy.getOrdersPdfViews.and.callFake(() => {
      return throwError({ error: true });
    });
    component.viewOrdersWithPdf();
    expect(errorServiceSpy.httpError).toHaveBeenCalled();
  });

  it('should createMessageWithOrdersWithoutQr', () => {
    messagesServiceSpy.presentToastCustom.and.callFake(() => {
      return Promise.resolve({
        isConfirmed: true
      });
    });
    component.createMessageWithOrdersWithoutQr(['']);
    expect(messagesServiceSpy.presentToastCustom).toHaveBeenCalled();
  });

  it('should goToDetailFormula', () => {
    component.goToDetailFormula('0');
    expect(dataServiceSpy.changeRouterForFormula).toHaveBeenCalled();
  });

  it('should reassignOrderDetail', () => {
    component.dataSource.data = [
      {
        isChecked: true,
        status: 'Planificado',
        codigoProducto: 'oj 6',
        finishedLabel: 0,
        ordenFabricacionId: 1
      } as IPedidoDetalleReq
    ];
    // FromToFilter.fromOrderIsolatedReassignItems;
    dataServiceSpy.getItemOnDataOnlyIds(component.dataSource.data, FromToFilter.fromOrderIsolatedReassignItems);
    component.reassignOrderDetail();
    expect(component.reassignOrderDetail).toBeTruthy();
    // expect(dataServiceSpy.changeRouterForFormula).toHaveBeenCalled();
  });
  it('should ordersToDelivered', () => {
    messagesServiceSpy.presentToastCustom.and.callFake(() => {
      return Promise.resolve({
        isConfirmed: true
      });
    });
    component.ordersToDelivered();
    expect(messagesServiceSpy.presentToastCustom).toHaveBeenCalled();
  });

  // it('should ordersToDelivered error', () => {
  //   filtersServiceSpy.getItemOnDateWithFilter.and.callFake(() => {
  //     return [throwError({ error: true })];
  //   });
  //   component.ordersToDelivered();
  //   // expect(messagesServiceSpy.presentToastCustom).toHaveBeenCalled();
  // });
});
