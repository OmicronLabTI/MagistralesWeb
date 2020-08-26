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
}

export class ILotesReq {
    numeroLote: string;
    cantidadDisponible: number;
    cantidadAsignada: number;
    cantidadSeleccionada?: number;
    sysNumber: number;
}

export class ILotesAsignadosReq {
    numeroLote: string;
    cantidadSeleccionada: number;
    sysNumber: number;
    action?: string;
    noidb?: boolean;
}

export class ILotesToSaveReq {
    orderId: number;
    assignedQty: number;
    batchNumber: string;
    itemCode: string;
    action: string;
}