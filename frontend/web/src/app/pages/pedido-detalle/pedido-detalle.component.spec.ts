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
import { Catalogs, ICreatePdfOrdersRes, ParamsPedidos } from 'src/app/model/http/pedidos';
import { HttpServiceTOCall } from 'src/app/constants/const';
import { CommentsConfig } from '../../model/device/incidents.model';
import { LocalStorageService } from 'src/app/services/local-storage.service';
import { ObservableService } from 'src/app/services/observable.service';
import { FiltersService } from 'src/app/service/filters.service';

describe('PedidoDetalleComponent', () => {
  let component: PedidoDetalleComponent;
  let fixture: ComponentFixture<PedidoDetalleComponent>;
  let pedidosServiceSpy: jasmine.SpyObj<PedidosService>;
  let downloadImagesServiceSpy;
  let dataServiceSpy: jasmine.SpyObj<DataService>;
  let localStorageServiceSpy: jasmine.SpyObj<LocalStorageService>;
  let errorServiceSpy;
  let observableServiceSpy: jasmine.SpyObj<ObservableService>;
  let filtersServiceSpy: jasmine.SpyObj<FiltersService>;

  const catalogs = new Catalogs();
  const iPedidoDetalleRes = new IPedidoDetalleListRes();
  const iPedidoDetalleReq: IPedidoDetalleReq[] = [];

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
      'presentToastCustom',
      'getMessageTitle',
      'openNewTapByUrl',
      'getItemOnDataOnlyIds',
      'getFullStringForCarousel',
    ]);
    localStorageServiceSpy.getProductNoLabel.and.returnValue(catalogs);
    localStorageServiceSpy.getUserRole.and.returnValue('');
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
    filtersServiceSpy.getItemOnDateWithFilter.and.returnValue([]);
    filtersServiceSpy.getIsWithFilter.and.returnValue(true)
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
});
