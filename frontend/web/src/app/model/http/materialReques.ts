import {BaseResponseHttp} from './commons';

export class IMaterialRequestRes extends BaseResponseHttp {
 response: RawRequest;
}
export class IMaterialPostRes extends BaseResponseHttp {
    response: MaterialPostRes;
}
export class MaterialPostRes {
    failed: RawRequestError[];
}
export class RawRequest {
    id?: number;
    productionOrderIds: number[];
    failedProductionOrderIds?: number[];
    signature: any;
    signingUserId?: any;
    signingUserName?: any;
    observations: string;
    orderedProducts: MaterialComponent[];
    creationUserId?: any;
    creationDate?: any;

}
export class MaterialComponent {
    id: number;
    requestId?: number;
    productId: string;
    description: string;
    requestQuantity: any;
    unit: string;
    isWithError?: boolean;
    isChecked?: boolean;
}

export class RawRequestPost {
    data: RawRequest;
    userId: string;

}
export class RawRequestError {
    productionOrderId: number;
    reason: string;
}
