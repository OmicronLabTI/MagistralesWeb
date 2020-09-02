import {BaseResponseHttp} from './commons';

export class IOrdersRes extends BaseResponseHttp {
    response: IOrdersReq[];
}

export class IOrdersReq {
    isChecked?: boolean;
    docNum: number;
    fabOrderId: string;
    itemCode: string;
    description: string;
    quantity: number;
    createDate: Date;
    finishDate: Date;
    qfb?: string;
    status?: string;
    class?: string;
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