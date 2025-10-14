import {BaseResponseHttp} from './commons';
import { ChildrenOrders } from './detallepedidos.model';

export class IOrdersRes extends BaseResponseHttp {
    response: IOrdersReq[];
}

export class IChildrenOrdersListRes extends BaseResponseHttp {
    response: ChildrenOrdersFabOrderList[];
}

export class IOrdersReq {
    isChecked?: boolean;
    docNum: number;
    fabOrderId: number;
    itemCode: string;
    description: string;
    quantity: number;
    createDate: string;
    finishDate: string;
    qfb?: string;
    status?: string;
    class?: string;
    unit?: string;
    batche?: string;
    quantityFinish: number;
    endDate?: Date;
    fabDate?: Date;
    isWithError?: boolean;
    isWithErrorBatch?: boolean;
    hasMissingStock?: boolean;
    batch: string;
    onSplitProcess: boolean;

    orderRelationType: string;
    availablePieces: number;
    childOrders: number;
    childOrdersDetail: ChildrenOrdersFabOrderList[];

    style?: string;
}

export class ChildrenOrdersFabOrderList {
    isChecked: boolean;
    docNum: string;
    fabOrderId: number;
    quantity: number;
    createDate: string;
    userCreate: string;
    finishDate: string;
    qfb: string;
    status: string;
    batch: string;
}

export class ParamsOrders {
    dateType?: string;
    docNum: any;
    fini: Date;
    ffin: Date;
    status: string;
    qfb: string;
    offset?: number;
    limit?: number;
    dateFull: string;
}
