import {BaseResponseHttp} from "./commons";

export class IFormulaReq {
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
    completedQuantity: number;
    realEndDate: string;
    productLabel?: string;
    container?: string;
    details: IFormulaDetalleReq[];
}

export class IFormulaDetalleReq {
    isChecked: boolean;
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
}

export class IFormulaRes extends BaseResponseHttp{
    response: any;
}

export class IFormulaDetalleRes extends BaseResponseHttp{
    response: IFormulaReq;
}