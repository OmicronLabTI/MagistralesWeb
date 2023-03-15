import { CONST_NUMBER, CONST_STRING } from 'src/app/constants/const';
import { BaseResponseHttp } from './commons';

export class IMaterialRequestRes extends BaseResponseHttp {
    response: RawRequest;
}

export class IMaterialHistoryRes extends BaseResponseHttp {
    response: IMaterialHistoryItem[]
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
    isLabel?: boolean;
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
    warehouse?: string;
    isLabel?: boolean;
}

export class RawRequestPost {
    data: RawRequest;
    userId: string;

}
export class RawRequestError {
    productionOrderId: number;
    reason: string;
}

export interface DestinationStoreResponse extends BaseResponseHttp {
    response: DestinationStore[];
}

export class DestinationStore {
    id: number;
    value: string;
    type: string;
    field: string;
    constructor() {
        this.id = CONST_NUMBER.zero;
        this.value = CONST_STRING.empty;
        this.type = CONST_STRING.empty;
        this.field = CONST_STRING.empty;
    }
}

export interface IMaterialHistoryItem {
    docEntry: number;
    itemCode: string;
    description: string;
    applicantName: string;
    quantity: number;
    unit: string;
    targetWarehosue: string;
    docDate: string;
    status: string;

}
