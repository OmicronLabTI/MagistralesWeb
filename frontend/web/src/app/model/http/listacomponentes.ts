import {BaseResponseHttp} from './commons';

export class IMyListRes extends BaseResponseHttp {
    response: any;
}

export class IMyNewListReq {
    data: BaseComponent;
    userId: string;
}

export class Components {
    productId: string;
    description: string;
    baseQuantity: number;
}

export class BaseComponent {
    name: string;
    productId: string;
    components: Components[];
}
