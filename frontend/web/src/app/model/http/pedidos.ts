import {BaseResponseHttp} from './commons';

export class IPedidosRes extends BaseResponseHttp {
    response: any;
}
export class IPedidosListRes extends BaseResponseHttp {
    response: IPedidoReq[];
}
export class IProcessOrdersRes extends BaseResponseHttp {
    response: string[];
}
export class IPlaceOrdersAutomaticRes extends BaseResponseHttp {
    response: string[];
}
export class ICancelOrdersRes extends BaseResponseHttp {
    response: ResponseCancel;
}
export class ICreateIsolatedOrderRes extends BaseResponseHttp {
    response: number;
}
export class IGetNewBachCodeRes extends BaseResponseHttp {
    response: string;
}
export class ParamsPedidos {
    dateType?: string;
    docNum?: any;
    fini?: Date;
    ffin?: Date;
    status?: string;
    qfb?: string;
    offset?: number;
    limit?: number;
    dateFull?: string;
    productCode?: string;
    isFromOrders?: boolean;

}

export class ProcessOrders {
    user: string;
    listIds: number[];
}
export class ProcessOrdersDetailReq {
    pedidoId: number;
    userId: string;
    productId: string[];
}
export class IPlaceOrdersAutomaticReq {
    userLogistic: string;
    docEntry: number[];
}
export class IPedidoReq {
    isChecked: boolean;
    docNum: number;
    codigo: string;
    cliente: string;
    medico: string;
    asesorName: string;
    fechaInicio: string;
    fechaFin: string;
    pedidoStatus: string;
    qfb?: string;
    class?: string;
}

export class CancelOrderReq {
    userId?: string;
    orderId: number;
    reason?: string;
    batches?: Batches[];
}

export class Batches {
    batchCode: string;
    quantity: string;
    expirationDate: string;
    manufacturingDate: string;
}
export class ResponseCancel {
    failed: CancelOrderReq[];
}
export class CreateIsolatedOrderReq {
    userId: string;
    productCode: string;
}
