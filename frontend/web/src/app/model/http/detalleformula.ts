import {BaseResponseHttp} from './commons';

export class IFormulaRes extends BaseResponseHttp {
    response: IFormulaReq;
}
export class IComponentsRes extends BaseResponseHttp {
    response: IFormulaDetalleReq[];
}

export class IComponentsSaveReq {
    fabOrderId: number;
    plannedQuantity: number;
    warehouse: string;
    fechaFin: string;
    comments: string;
    components: IFormulaDetalleReq[];
    isInDb?: boolean;
}

export class IFormulaReq {
    fabDate: string;
    comments: string;
    productionOrderId: string;
    code: string;
    productDescription?: string;
    type: string;
    status: string;
    plannedQuantity: number;
    unit: string;
    warehouse: string;
    number: number;
    dueDate: string;
    startDate: string;
    endDate: string;
    user: string;
    origin: string;
    baseDocument: number;
    client: string;
    completeQuantity: number;
    realEndDate: string;
    productLabel?: string;
    container?: string;
    hasBatches?: boolean;
    details: IFormulaDetalleReq[];
    isInDb?: boolean;
    hasMissingStock: boolean;
    catalogGroupName: string;
}

export class IFormulaDetalleReq {
    isChecked: boolean;
    orderFabId?: number;
    productId: string;
    description: string;
    productoName?: string;
    baseQuantity: number;
    requiredQuantity: number;
    consumed: number;
    available: number;
    unit: string;
    warehouse: string;
    pendingQuantity: number;
    stock: any;
    warehouseQuantity: any;
    action?: string;
    hasBatches?: boolean;
    isInDb?: boolean;
    isItemSelected?: boolean;
    productoId?: string;
    isContainer?: boolean;
}



