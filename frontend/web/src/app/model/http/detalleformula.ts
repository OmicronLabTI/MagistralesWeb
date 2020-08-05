import {BaseResponseHttp} from "./commons";

export class IFormulaReq {
    formula: string;
    tipo: string;
    estatus: string;
    cantidadPlanificada: number;
    unidad: string;
    almacen: string;
    numero: number;
    fechaVencimiento: string;
    fechaInicio: string;
    fechaFinalizacion: string;
    usuario: string;
    origen: string;
    pedidoCliente: number;
    cliente: string;
    cantidadCompletada: number;
    fechaCierreReal: string;
    detalle: IFormulaDetalleReq[];
}

export class IFormulaDetalleReq {
    numero: string;
    descripcion: string;
    cantidadBase: number;
    cantidadRequerida: number;
    consumido: number;
    disponible: number;
    unidad: string;
    almacen: string;
    cantidadPendiente: number;
    stock: number;
    cantidadAlmacen: number;
}

export class IFormulaRes extends BaseResponseHttp{
    response: any;
}

export class IFormulaDetalleRes extends BaseResponseHttp{
    response: IFormulaReq;
}