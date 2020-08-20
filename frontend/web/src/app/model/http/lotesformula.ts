import {BaseResponseHttp} from './commons';

export class ILotesFormulaRes extends BaseResponseHttp {
    response: ILotesFormulaReq[];
}

export class ILotesFormulaReq {
    codigoProducto: string;
    descripcionProducto: string;
    almacen: string;
    totalNecesario: number;
    totalSeleccionado: number;
    lotesSeleccionados: ILotesSelectedReq[];
    selected?: boolean;
}

export class ILotesReq {
    numeroLote: number;
    cantidadDisponible: number;
}

export class ILotesRes extends BaseResponseHttp {
    response: ILotesReq[];
}

export class ILotesSelectedReq {
    numeroLote: number;
    cantidadSeleccionada: number;
}

