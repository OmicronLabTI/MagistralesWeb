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
    fechaFin: string;
    comments: string;
    components: IFormulaDetalleReq[];
}

export class IFormulaReq {
    fabDate: string;
    comments: string;
    productionOrderId: string;
    code: string;
    productionDescription: string;
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
    details: IFormulaDetalleReq[];
}

export class IFormulaDetalleReq {
    isChecked: boolean;
    orderFabId?: number;
    productId: string;
    description: string;
    baseQuantity: number;
    requiredQuantity: number;
    consumed: number;
    available: number;
    unit: string;
    warehouse: string;
    pendingQuantity: number;
    stock: number;
    warehouseQuantity: number;
    action?: string;
}



