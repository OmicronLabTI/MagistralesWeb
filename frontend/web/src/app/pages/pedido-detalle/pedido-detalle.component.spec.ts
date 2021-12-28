import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PedidoDetalleComponent } from './pedido-detalle.component';
import {RouterTestingModule} from '@angular/router/testing';
import { IPedidoDetalleListRes, IPedidoDetalleReq } from 'src/app/model/http/detallepedidos.model';
import { MATERIAL_COMPONENTS } from 'src/app/app.material';
import { FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {DatePipe} from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import {PedidosService} from '../../services/pedidos.service';
import {of, throwError} from 'rxjs';
import {DetailOrderMock} from '../../../mocks/detailOrder.Mock';
import {DownloadImagesService} from '../../services/download-images.service';
import {UrlsOfQrEachOrderMock} from '../../../mocks/urlsOfQrEachOrderMock';
import {ErrorService} from '../../services/error.service';
import {error} from 'util';
import {CUSTOM_ELEMENTS_SCHEMA} from '@angular/core';
import { DataService } from 'src/app/services/data.service';
import { Catalogs, ParamsPedidos } from 'src/app/model/http/pedidos';
import { HttpServiceTOCall } from 'src/app/constants/const';
import { CommentsConfig } from '../../model/device/incidents.model';

describe('PedidoDetalleComponent', () => {
  let component: PedidoDetalleComponent;
  let fixture: ComponentFixture<PedidoDetalleComponent>;
  let pedidosServiceSpy: jasmine.SpyObj<PedidosService>;
  let downloadImagesServiceSpy;
  let dataServiceSpy: jasmine.SpyObj<DataService>;
  let errorServiceSpy;
  let catalogs = new Catalogs();
  let iPedidoDetalleRes = new IPedidoDetalleListRes();
  let iPedidoDetalleReq: IPedidoDetalleReq[] = [];

  let httpServiceTOCallRes: HttpServiceTOCall = HttpServiceTOCall.DETAIL_ORDERS;
  let comentsConfig = new CommentsConfig();
  let parametrosPedidos = new ParamsPedidos();
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
      'savedComments'
    ]);
    dataServiceSpy = jasmine.createSpyObj<DataService>('DataService', [
    'getProductNoLabel', 
    'setUrlActive', 
    'getIsThereOnData', 
    'presentToastCustom', 
    'getUserId', 
    'getMessageTitle', 
    'setCancelOrders', 
    'setFinalizeOrders', 
    'setQbfToPlace', 
    'setPathUrl', 
    'setCurrentDetailOrder', 
    'setOpenCommentsDialog', 
    'setMessageGeneralCallHttp',
    'setOpenSignatureDialog', 
    'getUserRole', 
    'openNewTapByUrl', 
    'getItemOnDataOnlyIds', 
    'getFiltersActives', 
    'getNewDataToFilter', 
    'getFullStringForCarousel', 
    'getCurrentDetailOrder', 
    'getCallHttpService',
    'getNewDataSignature',
    'getNewCommentsResult',
    'removeCurrentDetailOrder'
    ]);
    dataServiceSpy.getProductNoLabel.and.returnValue(catalogs);
    dataServiceSpy.getCallHttpService.and.returnValue(of(httpServiceTOCallRes))
    dataServiceSpy.getFiltersActives.and.returnValue(JSON.stringify(parametrosPedidos));
    dataServiceSpy.getCurrentDetailOrder.and.returnValue('')
    downloadImagesServiceSpy = jasmine.createSpyObj<DownloadImagesService>('DownloadImagesService', ['downloadImageFromUrl']);
    pedidosServiceSpy.qrByEachOrder.and.callFake( () => {
      return of(UrlsOfQrEachOrderMock);
    });
    pedidosServiceSpy.finishLabels.and.returnValue(of(iPedidoDetalleRes))
    dataServiceSpy.getNewDataSignature.and.returnValue(of({}));
    dataServiceSpy.getNewCommentsResult.and.returnValue(of(comentsConfig));
    dataServiceSpy.getNewDataToFilter.and.returnValue([parametrosPedidos, '']);
    // pedidosServiceSpy.getDetallePedido.and.callFake(() => {
    //   return of(DetailOrderMock);
    // });
    pedidosServiceSpy.getDetallePedido.and.returnValue(of(DetailOrderMock));
    pedidosServiceSpy.savedComments.and.returnValue(of(iPedidoDetalleRes))
    TestBed.configureTestingModule({
      imports: [
        MATERIAL_COMPONENTS,
        HttpClientTestingModule,
        RouterTestingModule,
        FormsModule,
        BrowserAnimationsModule],
      declarations: [ PedidoDetalleComponent ],
      providers: [DatePipe,
        { provide: PedidosService, useValue: pedidosServiceSpy },
        { provide: DownloadImagesService, useValue: downloadImagesServiceSpy },
        { provide: ErrorService, useValue: errorServiceSpy },
        { provide: DataService, useValue: dataServiceSpy}],
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
  // it('should getProductoNoLabel', () => {
  //   component.getProductoNoLabel();
  //   expect(component.ProductNoLabel.value).toEqual(catalogs.value);
    
  // });
  // it('should getDetallePedido', () => {
  //   /*component.getDetallePedido();
  //   expect(pedidosServiceSpy.getDetallePedido).toHaveBeenCalled();
  //   expect(component.isThereOrdersDetailToPlan).toBeFalsy();
  //   expect(component.isThereOrdersDetailToPlace).toBeFalsy();
  //   expect(component.isThereOrdersDetailToCancel).toBeFalsy();
  //   expect(component.isThereOrdersDetailToFinalize).toBeFalsy();*/
  // });

  // it('should someComplete return false', () => {
  //   component.allComplete = true;
  //   expect(component.someComplete()).toBeFalsy();
  // });
  // it('should someComplete return true', () => {
  //   component.dataSource.data = [
  //     {
  //       isChecked: true,
  //     } as IPedidoDetalleReq
  //   ];
  //   expect(component.someComplete()).toBeTruthy();
  // });

  // it('should someCompletereturn false when allcomplete is true', () => {
  //   component.dataSource.data = [
  //     {
  //       isChecked: false,
  //     } as IPedidoDetalleReq
  //   ];
  //   component.allComplete = true;
  //   expect(component.someComplete()).toBeFalsy();
  // });
  // it('should updateAllComplete', () => {
  //   component.dataSource.data = DetailOrderMock.response;
  //   component.updateAllComplete();
  //   expect(component.allComplete).toBeFalsy();
  //   // component.dataSource.data = DetailOrderMock.response;
  //   component.dataSource.data.forEach(detail => detail.isChecked = true);
  //   component.updateAllComplete();
  //   expect(component.allComplete).toBeTruthy();
  // });
  // it('should setAll', () => {
  //   component.dataSource.data.forEach(detail => detail.isChecked = false);
  //   component.dataSource.data = null;
  //   component.setAll(false);
  //   expect(component.allComplete).toBe(false);
  //   component.dataSource.data = DetailOrderMock.response;
  //   component.setAll(true);
  //   expect(component.allComplete).toBe(true);
  //   expect(component.dataSource.data.every(detail => detail.isChecked)).toBeTruthy();
  //   component.setAll(false);
  //   expect(component.allComplete).toBe(false);
  //   expect(component.dataSource.data.every(detail => !detail.isChecked)).toBeTruthy();
  // });

  // it('should call ordersToDownloadQr()', () => {
  //   component.dataSource.data = [
  //     {
  //       status: 'Planificado',
  //       pedidoStatus: 'Planificado',
  //       isChecked: true,
  //     } as IPedidoDetalleReq
  //   ];
  //   component.ordersToDownloadQr();
  //   expect(pedidosServiceSpy.qrByEachOrder).toHaveBeenCalled();
  //   expect(downloadImagesServiceSpy.downloadImageFromUrl).toHaveBeenCalledTimes(1);
  // });
  // it('should call ordersToDownloadQr() error', () => {
  //   pedidosServiceSpy.qrByEachOrder.and.callFake(() => {
  //     return throwError({ error: true });
  //   });
  //   component.dataSource.data = [
  //     {
  //       status: 'Planificado',
  //       pedidoStatus: 'Planificado',
  //       isChecked: true,
  //     } as IPedidoDetalleReq
  //   ];
  //   component.ordersToDownloadQr();
  //   expect(pedidosServiceSpy.qrByEachOrder).toHaveBeenCalled();
  //   expect(errorServiceSpy.httpError).toHaveBeenCalled();
  // });
});
