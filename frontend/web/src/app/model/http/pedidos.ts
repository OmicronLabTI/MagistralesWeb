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
    docNum: number;
    fini: string;
    ffin: string;
    status: string;
    qfb: string;
    offset: number;
    limit: number;
}
