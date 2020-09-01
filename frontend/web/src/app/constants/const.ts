export const CONST_NUMBER = {
    zero: 0,
    one: 1,
    two: 2,
    three: 3,
    five: 5,
    ten: 10,
    timeToast: 2000,
    nulo: null
};
export const CONST_STRING = {
    empty: ''
};

export const CONST_USER_DIALOG = {
    defaultQfb: 'QFB',
    patternPassWord: /^(?=(?:.*\d){1})(?=(?:.*[A-Z]){1})(?=(?:.*[a-z]){1})(?=(?:.*[@$?¡\-_.+*;!¿"%#&/]){0})\S{8,50}$/
};

export const MODAL_NAMES =  {
    addUserModal: 'addModal',
    editUser: 'editModal',
    placeOrders: 'Pedido',
    placeOrdersDetail: 'Orden',
    assignManual: 'manual',
    assignAutomatic: 'automatic'
};
export const MODAL_FIND_ORDERS = {
    thirtyDays: (24 * 60 * 60 * 1000) * 30,
    perDay: 1000 * 3600 * 24,
    ninetyDays: (24 * 60 * 60 * 1000) * 90,
    ninetyDaysDifference: 90,
    keyEnter: 'Enter'
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
export const ConstLogin = {
    defaultRedirectUri: 'asdad',
    defaultClientId2: '',
    defaultOrigin: 'web'
};
export enum MessageType {
    processOrder,
    processDetailOrder,
    placeOrder,
    placeDetailOrder,
    cancelOrder,
    cancelDetailOrder,
    finalizeOrder,
    saveBatches
}
export const ClassNames = {
    popupCustom: 'popup-custom'
};
export const ConstStatus = {
    abierto: 'Abierto',
    planificado: 'Planificado',
    finalizado: 'Finalizado',
    cancelado: 'Cancelado',
    liberado: 'Liberado',
    enProceso: 'En proceso',
    pendiente: 'Pendiente',
    asignado: 'Asignado',
    terminado: 'Terminado',
    reasingado: 'Reasingado',
};
export const HttpStatus = {
    ok: 200,
    created: 201,
    badRequest: 400,
    unauthorized: 401,
    forbidden: 403,
    notFound: 404,
    serverError: 500,
    timeOut: 504,
    connectionRefused: 0
};

export const ConstToken = {
    accessToken: 'token-omi',
    refreshToken: 'refresh-omi',
    rememberSession: 'session-re',
    userId: 'userId',
    userName: 'userName',
};
export const BOOLEANS = {
    verdadero: true,
    falso: false
};

export const ComponentSearch = {
    searchComponent: 'searchComponent'
};
