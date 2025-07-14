import {BaseResponseHttp} from './commons';

export class IMyListRes extends BaseResponseHttp {
    response: any;
}

export class IMyCustomListRes extends BaseResponseHttp {
    response: BaseComponent[];
}

export class IMyNewListReq {
    data: BaseComponent;
    userId: string;
}

export class Components {
    id?: number;
    customListId?: number;
    productId: string;
    description: string;
    baseQuantity: number;
    unit: string;
}

export class BaseComponent {
    id?: number;
    name: string;
    productId: string;
    components: Components[];
    creationUserId?: string;
    creatinDate?: Date;
    unit: string;
}

export class ICustomListDelete {
    name: string;
    productId: string;
}
