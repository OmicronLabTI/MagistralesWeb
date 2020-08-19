export const CONST_NUMBER = {
    zero: 0,
    one: 1,
    two: 2,
    three: 3,
    five: 5,
    ten: 10,
    timeToast: 2000

};
export const CONST_STRING = {
    empty: ''
};

export const CONST_USER_DIALOG = {
    defaultDefault: 'QFB',
    patternPassWord: /^(?=(?:.*\d){1})(?=(?:.*[A-Z]){1})(?=(?:.*[a-z]){1})(?=(?:.*[@$?¡\-_.+*;!?¿"%#&/]){0})\S{8,50}$/
};

export const MODAL_NAMES =  {
    addUserModal: 'addModal',
    editUser: 'editModal',
    placeOrders: 'Pedido',
    placeOrdersDetail: 'Orden'
};
export const MODAL_FIND_ORDERS = {
    thirtyDays: (24 * 60 * 60 * 1000) * 30,
    perDay: 1000 * 3600 * 24,
    ninetyDays: (24 * 60 * 60 * 1000) * 90,
    ninetyDaysDifference: 90
};
export const CONST_DETAIL_FORMULA = {
    update: 'update',
    insert: 'insert',
    delete: 'delete'
};
export enum HttpServiceTOCall {
    ORDERS,
    DETAIL_ORDERS,
    USERS,
    DETAIL_FORMULA
}

export const RoleQfbId = 2;

