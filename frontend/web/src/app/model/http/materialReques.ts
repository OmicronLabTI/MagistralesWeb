import { BoolConst, CONST_ARRAY, CONST_NUMBER, CONST_STRING } from 'src/app/constants/const';
import { BaseResponseHttp } from './commons';

export class IMaterialRequestRes extends BaseResponseHttp {
    response: RawRequest;
}

export class IMaterialHistoryRes extends BaseResponseHttp {
    response: IMaterialHistoryItem[];
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
    quantity: string | number;
    unit: string;
    targetWarehosue: string;
    docDate: string;
    status: string;
}

export class MaterialHistoryQuery {
    start: string;
    offset: number;
    limit: number;
    status: string;
    constructor() {
        this.start = CONST_STRING.empty;
        this.offset = CONST_NUMBER.zero;
        this.limit = CONST_NUMBER.ten;
        this.status = 'Abierto';
    }
}

export class MaterialRequestData {
    products: Array<MaterialComponent>;
    sign: string;
    comments: string;
    isValid: boolean;
    constructor() {
        this.products = CONST_ARRAY.empty;
        this.sign = CONST_STRING.empty;
        this.comments = CONST_STRING.empty;
        this.isValid = BoolConst.false;
    }
}
