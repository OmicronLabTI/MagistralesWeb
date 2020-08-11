import {BaseResponseHttp} from './commons';

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
export class IPedidosRes extends BaseResponseHttp {
    response: any;
}
export class IPedidosListRes extends BaseResponseHttp {
    response: IPedidoReq[];
}
export class ParamsPedidos {
    dateType?: string;
    docNum: number;
    fini: Date;
    ffin: Date;
    status: string;
    qfb: string;
    offset?: number;
    limit?: number;
    dateFull: string;
}

export class ProcessOrders {
    user: string;
    listIds: number[];
}
