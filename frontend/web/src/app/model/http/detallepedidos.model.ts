import {BaseResponseHttp} from './commons';

export class IPedidoDetalleLabelReq {
    details: LabelToFinish[];
    userId: string;
    designerSignature: any;
}
export class LabelToFinish {
    orderId: number;
    checked: boolean;
}
export class IPedidoDetalleReq {
    isChecked: boolean;
    ordenFabricacionId: number;
    codigoProducto: string;
    descripcionProducto: string;
    qtyPlanned: number;
    fechaOf: string;
    fechaOfFin: string;
    hasMissingStock: boolean;
    qfb: string;
    status: string;
    class?: string;
    pedidoStatus: string;
    comments: string;
    label: string;
    finishedLabel: number;
    pedidoId?: number;
}
export class IPedidoDetalleRes extends BaseResponseHttp {
    response: any;
}
export class IPedidoDetalleListRes extends BaseResponseHttp {
    response: IPedidoDetalleReq[];
}

