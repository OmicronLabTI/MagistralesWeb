import { TestBed } from '@angular/core/testing';
import { PedidosService } from './pedidos.service';
import {DatePipe} from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import {Observable} from 'rxjs';
import {IPlaceOrdersReq} from '../model/http/users';
import {IComponentsSaveReq} from '../model/http/detalleformula';
import {
    CancelOrderReq,
    CreateIsolatedOrderReq,
    IPlaceOrdersAutomaticReq,
    ProcessOrdersDetailReq
} from '../model/http/pedidos';
import {LabelToFinish} from '../model/http/detallepedidos.model';

describe('PedidosService', () => {
  beforeEach(() => {
        TestBed.configureTestingModule({
          imports: [HttpClientTestingModule],
          providers: [DatePipe]
        });
      });

  it('should be created', () => {
    const service: PedidosService = TestBed.get(PedidosService);
    expect(service).toBeTruthy();
  });
  it('should getPedidos', () => {
    const service: PedidosService = TestBed.get(PedidosService);
    expect(service.getPedidos('anyQueryString') instanceof Observable).toBeTruthy();
  });
  it('should getDetallePedido', () => {
        const service: PedidosService = TestBed.get(PedidosService);
        expect(service.getDetallePedido('anyDocumentNum') instanceof Observable).toBeTruthy();
    });
  it('should getFormulaDetail', () => {
        const service: PedidosService = TestBed.get(PedidosService);
        expect(service.getFormulaDetail('anyOrderNum') instanceof Observable).toBeTruthy();
    });
  it('should processOrders', () => {
        const service: PedidosService = TestBed.get(PedidosService);
        expect(service.processOrders('anyOrdersToProcess') instanceof Observable).toBeTruthy();
    });
  it('should getQfbs', () => {
        const service: PedidosService = TestBed.get(PedidosService);
        expect(service.getQfbs(2) instanceof Observable).toBeTruthy();
    });
  it('should getQfbsWithOrders', () => {
        const service: PedidosService = TestBed.get(PedidosService);
        expect(service.getQfbsWithOrders() instanceof Observable).toBeTruthy();
    });
  it('should postPlaceOrders', () => {
        const service: PedidosService = TestBed.get(PedidosService);
        const placeOlderReq = new IPlaceOrdersReq();
        placeOlderReq.docEntry = [1, 2, 3, 4];
        placeOlderReq.orderType = 'Type order [Pedido, Orden]';
        placeOlderReq.userId = 'idUser to place orders';
        placeOlderReq.userLogistic = 'Id user logueado';
        expect(service.postPlaceOrders(placeOlderReq , true) instanceof Observable).toBeTruthy();
        expect(service.postPlaceOrders(placeOlderReq , false) instanceof Observable).toBeTruthy();
    });
  it('should getComponents', () => {
        const service: PedidosService = TestBed.get(PedidosService);
        expect(service.getComponents('anyQueryStringToGetComponents', false) instanceof Observable).toBeTruthy();
        expect(service.getComponents('anyQueryStringToGetComponents', true) instanceof Observable).toBeTruthy();
    });
  it('should updateFormula', () => {
        const service: PedidosService = TestBed.get(PedidosService);
        const componentsToSaveReq = new IComponentsSaveReq();
        componentsToSaveReq.components = []; // components to insert, delete, update
        componentsToSaveReq.fechaFin = '01/12/2020';
        componentsToSaveReq.comments = 'anyComments';
        componentsToSaveReq.plannedQuantity = 30;
        componentsToSaveReq.fabOrderId = 12;
        expect(service.updateFormula(componentsToSaveReq) instanceof Observable).toBeTruthy();
    });
  it('should postPlaceOrdersDetail', () => {
        const service: PedidosService = TestBed.get(PedidosService);
        const processOrdersDetailReq = new ProcessOrdersDetailReq();
        processOrdersDetailReq.productId = []; // productsIds
        processOrdersDetailReq.userId = 'user logueado';
        processOrdersDetailReq.pedidoId = 1234;
        expect(service.postPlaceOrdersDetail(processOrdersDetailReq) instanceof Observable).toBeTruthy();
    });
  it('should postPlaceOrderAutomatic', () => {
        const service: PedidosService = TestBed.get(PedidosService);
        const placeOrdersAutomaticReq = new IPlaceOrdersAutomaticReq();
        placeOrdersAutomaticReq.docEntry = []; // ids to place automatic
        placeOrdersAutomaticReq.userLogistic = 'user logueado';
        expect(service.postPlaceOrderAutomatic(placeOrdersAutomaticReq) instanceof Observable).toBeTruthy();
    });
  it('should putCancelOrders', () => {
        const service: PedidosService = TestBed.get(PedidosService);
        const cancelOrderReq: CancelOrderReq[] = []; // {idOrder: 234, userId: 'user logueado'} // to cancel
        expect(service.putCancelOrders(cancelOrderReq, true) instanceof Observable).toBeTruthy();
        expect(service.putCancelOrders(cancelOrderReq, false) instanceof Observable).toBeTruthy();
    });
  it('should putFinalizeOrders', () => {
        const service: PedidosService = TestBed.get(PedidosService);
        const cancelOrderReq: CancelOrderReq[] = []; // {idOrder: 234, userId: 'user logueado'} // to finalize
        expect(service.putFinalizeOrders(cancelOrderReq, true) instanceof Observable).toBeTruthy();
        expect(service.putFinalizeOrders(cancelOrderReq, false) instanceof Observable).toBeTruthy();
    });
  it('should createIsolatedOrder', () => {
        const service: PedidosService = TestBed.get(PedidosService);
        const createIsolatedOrder = new CreateIsolatedOrderReq(); // {idOrder: 234, userId: 'user logueado'} // to finalize
        expect(service.createIsolatedOrder(createIsolatedOrder) instanceof Observable).toBeTruthy();
    });
  it('should getNextBatchCode', () => {
        const service: PedidosService = TestBed.get(PedidosService);
        expect(service.getNextBatchCode('anyProductCode') instanceof Observable).toBeTruthy();
    });
  it('should getIfExistsBatchCode', () => {
        const service: PedidosService = TestBed.get(PedidosService);
        expect(service.getIfExistsBatchCode('anyProductCode', 'anyBatchCode') instanceof Observable).toBeTruthy();
    });
  it('should createPdfOrders', () => {
      const service: PedidosService = TestBed.get(PedidosService);
      expect(service.createPdfOrders([]) instanceof Observable).toBeTruthy();
  });
  it('should createPdfOrders', () => {
      const service: PedidosService = TestBed.get(PedidosService);
      expect(service.qrByEachOrder([123, 2345]) instanceof Observable).toBeTruthy();
  });
  it('should getWorkLoad', () => {
        const service: PedidosService = TestBed.get(PedidosService);
        expect(service.getWorLoad('querystring') instanceof Observable).toBeTruthy();
    });
  it('should getOrdersPdfsView', () => {
        const service: PedidosService = TestBed.get(PedidosService);
        expect(service.getOrdersPdfViews([123, 123, 1234]) instanceof Observable).toBeTruthy();
    });
  it('should putOrdersToDelivered', () => {
        const service: PedidosService = TestBed.get(PedidosService);
        expect(service.putOrdersToDelivered([]) instanceof Observable).toBeTruthy();
    });
  it('should getRecipesByOrder', () => {
        const service: PedidosService = TestBed.get(PedidosService);
        expect(service.getRecipesByOrder(1234) instanceof Observable).toBeTruthy();
    });
  it('should savedComments', () => {
        const service: PedidosService = TestBed.get(PedidosService);
        expect(service.savedComments(1234, 'anyComments') instanceof Observable).toBeTruthy();
    });
  it('should finishLabels', () => {
        const service: PedidosService = TestBed.get(PedidosService);
        expect(service.finishLabels({
            designerSignature: 'sign', details: [{} as LabelToFinish], userId: ''}) instanceof Observable).toBeTruthy();
    });
});
