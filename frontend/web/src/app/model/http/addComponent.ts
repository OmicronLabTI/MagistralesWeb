import { BaseResponseHttp } from './commons';
import { ILotesAsignadosReq, ILotesReq, ILotesSelectedReq } from './lotesformula';

export class IAddComponentsAndLotesTable {
    codigoProducto: string;
    description: string;
    baseQuantity: number;
    requiredQuantity: number;
    consumed: number;
    available: number;
    unit: string;
    warehouse: string;
    pendingQuantity: number;
    stock: number;
    warehouseQuantity: string;
    totalNecesario: number;
    totalSeleccionado: number;
    selected?: boolean;
    action?: string;

    lotes: ILotesReq[]; // Lotes disponibles para asignar
    lotesAsignados: ILotesAsignadosReq[]; // Lotes ya guardados (provienen del back)
    lotesSeleccionados?: ILotesSelectedReq[]; // Lotes que va a guardar
}

export class IComponentsLotesRes extends BaseResponseHttp {
    response: IComponentLotes[];
}

export class IComponentLotes {
    codigoProducto: string;
    almacen: string;
    isItemSelected?: boolean;
    lotes: Lotes[];
}

export class Lotes {
    numeroLote: string;
    cantidadDisponible: number;
    cantidadAsignada: number;
    sysNumber: number;
    fechaExp?: string;
    fechaExpDateTime?: string;
    itemCode: string;
    quantity: number;
}
