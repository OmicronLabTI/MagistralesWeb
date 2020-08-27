import {BaseResponseHttp} from './commons';

export class IPedidoDetalleReq {
    isChecked: boolean;
    ordenFabricacionId: number;
    codigoProducto: string;
    descripcionProducto: string;
    qtyPlanned: number;
    fechaOf: string;
    fechaOfFin: string;
    qfb: string;
    status: string;
    class?: string;
}
export class IPedidoDetalleRes extends BaseResponseHttp {
    response: any;
}
export class IPedidoDetalleListRes extends BaseResponseHttp {
    response: IPedidoDetalleReq[];
}
