import {BaseResponseHttp} from './commons';

export class IIncidentsGraphicRes extends BaseResponseHttp {
    response: IncidentsGraphicsMatrix[][];
}
export class IWarehouseGraphicRes extends BaseResponseHttp {
    response: IncidentsGraphicsMatrix[];
}

export class IIncidentsListRes extends BaseResponseHttp {
    response: IncidentItem[];
}

export class IncidentItem  {
    createDate: Date;
    saleOrder: number;
    delivery: number;
    invoice: number;
    itemCode: string;
    type: string;
    incident: string;
    batches: string;
    stage: string;
    comments: string;
    status: string;
}
export class IncidentsGraphicsMatrix {
    fieldKey: string;
    totalCount: number;
    graphType: string;
    color?: string;
}
