import {CancelOrderReq} from '../http/pedidos';

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
}
export class SearchComponentModal {
    modalType: string;
}

