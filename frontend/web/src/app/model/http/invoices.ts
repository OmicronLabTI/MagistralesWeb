import { BaseResponseHttp } from './commons';

export class AutomaticBilling {
    id: string;
    createDate: string;
    status: string;
    typeInvoice: string;
    billingType: string;
    almacenUser: string;
    dxpOrderId: string;
    sapOrderId: number[];
    remissionId: number[];
    updateDate: string;
    errorMessage: string;
    retryNumber: number;
    manualChangeApplied: boolean | null;
    isProcessing: boolean;
}

export class IAutomaticBillingRes extends BaseResponseHttp {
    response: AutomaticBilling[];
}

export class IUpdateManualChangeRes extends BaseResponseHttp {
    response: string;
}

export class ManualRetryequest {
    invoiceIds: string[];
    requestingUser: string;
    offset: number;
    limit: number;
}
export class IManualRetrieResponse extends BaseResponseHttp {
    response: IManualRetryResponseIds;
}

export class IManualRetryResponseIds {
    processedIds: string[];
    skippedIds: string[];
}

export class AutomaticBillingAdvanceFilters {
    type: string;
    value: string;
}
