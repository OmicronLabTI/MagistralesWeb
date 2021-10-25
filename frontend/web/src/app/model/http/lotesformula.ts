import {BaseResponseHttp} from './commons';

export class ILotesFormulaRes extends BaseResponseHttp {
    response: ILotesFormulaReq[];
}

export class ILotesSaveRes extends BaseResponseHttp {
    response: string[];
}

export class ILotesFormulaReq {
    codigoProducto: string;
    descripcionProducto: string;
    almacen: string;
    totalNecesario: number;
    totalSeleccionado: number;
    lotesSeleccionados?: ILotesSelectedReq[];
    selected?: boolean;
    lotes: ILotesReq[];
    lotesAsignados: ILotesAsignadosReq[];

}

export class ILotesSelectedReq {
    numeroLote: string;
    cantidadSeleccionada: number;
    sysNumber: number;
    action?: string;
    noidb?: boolean;
    isValid?: boolean;
}

export class ILotesReq {
    numeroLote: string;
    cantidadDisponible: number;
    cantidadAsignada: number;
    cantidadSeleccionada?: number;
    sysNumber: number;
    fechaExp: Date;
    isValid?: boolean;
}

export class ILotesAsignadosReq {
    numeroLote: string;
    cantidadSeleccionada: number;
    sysNumber: number;
    action?: string;
    noidb?: boolean;
    isValid?: boolean;
}

export class ILotesToSaveReq {
    orderId: number;
    assignedQty: number;
    batchNumber: string;
    itemCode: string;
    action: string;
    areBatchesComplete: number;
}
