export const CONST_NUMBER = {
    lessOne: -1,
    zero: 0,
    one: 1,
    two: 2,
    three: 3,
    four: 4,
    five: 5,
    ten: 10,
    timeToast: 2000,
    nulo: null,
    six: 6,
    seven: 7,
    fifty: 50,
    oneHundred: 100
};
export const CONST_STRING = {
    empty: ''
};

export const CONST_USER_DIALOG = {
    defaultQfb: 'QFB',
    defaultNumberOfPieces: '200',
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
    ninetyDays: (24 * 60 * 60 * 1000) * 91,
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
    DETAIL_FORMULA,
    ORDERS_ISOLATED,
    PRODUCTIVITY
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
    saveBatches,
    materialRequest,
    default
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
    enProceso: 'En Proceso',
    pendiente: 'Pendiente',
    asignado: 'Asignado',
    terminado: 'Terminado',
    reasingado: 'Reasignado',
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
    userRole: 'role',
    isolatedOrder: 'istOrder',
};
export const BOOLEANS = {
    verdadero: true,
    falso: false
};
export const ConstOrders = {
    modalOrders: 'orders',
    modalOrdersIsolated: 'ordersIsolated',
    defaultDateInit: '0',
    dateFinishType: '1'
};

export const ComponentSearch = {
    searchComponent: 'searchComponent',
    createOrderIsolated: 'createOrder',
    addComponent: 'addComponent'
};

export enum FromToFilter {
    fromOrders,
    fromOrdersReassign,
    fromOrdersCancel,
    fromDetailOrder,
    fromOrdersIsolated,
    fromOrdersIsolatedCancel,
    fromOrderIsolatedReassign,
    fromOrderIsolatedReassignItems,
    fromDefault
}

export const MaterialRequestPage = {
    onlyNumberPatter: /^([0-9.])$/
};

export const Colors = [
    'rgba(70, 61, 242, 95)',
    'rgba(5, 112, 255, 100)',
    'rgba(69, 216, 230, 90)',
    'rgba(63, 252, 171, 99)',
    'rgba(61, 242, 70, 95)',
    'rgba(242, 141, 206, 95)',
    'rgba(160, 230, 146, 90)',
    'rgba(146, 200, 252, 99)',
    'rgba(242, 141, 225, 95)',
    'rgba(242, 99, 85, 95)',
    'rgba(255, 223, 107, 100)',
    'rgba(92, 230, 106, 90)',
    'rgba(122, 179, 255, 100)',
    'rgba(242, 85, 236, 95)',
    'rgba(82, 233, 242, 95)',
    'rgba(82, 100, 242, 95)',
    'rgba(230, 176, 90, 90)',
    'rgba(71, 255, 78, 100)'
];

export const pathRoles = {
    admin: [
        'userList',
        'login',
        '**'
    ],
    logistica: [
        'pedidos',
        'ordenes',
        'productividad',
        'ordenfabricacion',
        'pdetalle',
        'lotes',
        'login',
        'workLoad',
        'materialRequest',
        '**'
    ]
};
export const CONST_PRODUCTIVITY = {
  titleTotal: 'total',
};

export const RouterPaths = {
  materialRequest: 'materialRequest'
};
export enum TypeProperty {
    code,
    unit,
    description,
    requestQuantity
}
