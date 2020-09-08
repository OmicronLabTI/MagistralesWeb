import {CancelOrderReq, ParamsPedidos} from '../http/pedidos';

export class PlaceOrders {
    placeOrdersData: {
        list: [],
        modalType: '',
        userId: '',
     };
}
export class CancelOrders {
    list: CancelOrderReq[];
    cancelType: string;
    isFromCancelIsolated?: boolean;
}
export class SearchComponentModal {
    modalType: string;
    chips?: string[];
    filterOrdersData?: ParamsPedidos;
}

