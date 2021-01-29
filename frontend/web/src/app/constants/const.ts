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
    oneHundred: 100,
    oneThousand: 999
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
    delete: 'delete',
    none: 'none'
};
export enum HttpServiceTOCall {
    ORDERS,
    DETAIL_ORDERS,
    USERS,
    DETAIL_FORMULA,
    ORDERS_ISOLATED,
    PRODUCTIVITY,
    INCIDENTS_LIST,
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
    default,
    ordersWithoutQr
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
    almacenado: 'Almacenado',
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
    fromOrderDetailLabel,
    fromOrdersIsolated,
    fromOrdersIsolatedCancel,
    fromOrderIsolatedReassign,
    fromOrderIsolatedReassignItems,
    fromDefault,
    fromDetailOrderQr
}

export const MaterialRequestPage = {
    onlyNumberPatter: /^([0-9.])$/
};

export const Colors = [
    'rgb(70,61,242)',
    'rgb(23,119,246)',
    'rgb(74,185,196)',
    'rgb(82,189,144)',
    'rgb(3,172,12)',
    'rgb(248,126,204)',
    'rgb(136,6,146)',
    'rgb(246,114,62)',
    'rgb(232,236,112)',
    'rgb(123,83,132)',
    'rgb(11,77,158)',
    'rgb(230,92,175)',
    'rgb(69,58,55)',
    'rgb(127,112,208)',
    'rgb(52,4,149)',
    'rgb(224,168,125)',
    'rgb(224,25,64)',
    'rgb(62,90,63)'
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
    ],
    design: [
        'pedidos',
        'pdetalle',
        '**'
    ]
};
export const CONST_PRODUCTIVITY = {
  titleTotal: 'total',
};

export const RouterPaths = {
    materialRequest: 'materialRequest',
    incidents: 'incidents',
    incidentsList: 'incidentsList',
    warehousePage: 'warehouse'
};
export enum TypeProperty {
    code,
    unit,
    description,
    requestQuantity
}

export enum TypeInitialRange {
    onlyDefault,
    monthCalendar,
    mont
}

export enum TypeToSeeTap {
    order,
    receipt,
    system
}

export const RolesType = {
    logistic: '3',
    admin: '1',
    design: '4',
    warehouse: '5'
};
export const GraphType = {
    statusGraph: 'status',
    incidentGraph: 'IncidentReason'
};
