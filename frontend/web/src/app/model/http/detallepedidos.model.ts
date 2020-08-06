import {BaseResponseHttp} from "./commons";

export class IPedidoDetalleReq {
    isChecked: boolean;
    ordenFabricacionId: string;
    codigoProducto: string;
    descripcionProducto: string;
    qtyPlanned: number;
    fechaOf: string;
    fechaOfFin: string;
    qfb: string;
    status: string;
}
export class IPedidoDetalleRes extends BaseResponseHttp{
    response: any;
}
export class IPedidoDetalleListRes extends BaseResponseHttp{
    response: IPedidoDetalleReq[];
}