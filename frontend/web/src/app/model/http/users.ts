import {BaseResponseHttp} from './commons';
export class IUserRes extends BaseResponseHttp {
    response: UserRes;
}
export class IUserListRes extends BaseResponseHttp {
    response: IUserReq[];
}

export class IRolesRes extends BaseResponseHttp {
    response: RoleUser[];
}
export class IDeleteRes extends BaseResponseHttp {
    response: boolean;

}
export class IQfbWithNumberRes extends BaseResponseHttp {
    response: QfbWithNumber[];

}
export class IUserReq {
    id?: string;
    userName: string;
    firstName: string;
    lastName: string;
    role: number;
    password?: string;
    activo: number;
    piezas?: any;
    asignable?: number;
    userTypeR?: string;
    isChecked?: boolean = false;
    classification?: string;
}
export class UserRes {
    activo: number;
    firstName: string;
    id: string;
    lastName: string;
    role: number;
    userName: string;
}
export class IPlaceOrdersReq {
    userLogistic: string;
    userId: string;
    docEntry: number[];
    orderType: string;
}
export class RoleUser {
    id: number;
    description: string;
}

export class QfbSelect {
    qfbId: string;
    qfbName: string;
}
export  class QfbWithNumber {
    userId?: string;
    userName?: string;
    countTotalFabOrders?: any;
    countTotalOrders?: any;
    countTotalPieces?: any;
    modalType?: string;
    list?: number[];
    assignType?: string;
    isFromOrderIsolated?: boolean;
    isFromReassign?: boolean;
    asignable?: number;
    qfbClassification?: string;
}
export const QfbClassification = {
    mg: 'mg',
    mn: 'mn',
    be: 'be'
};

export class SearchUsersData {
    userNameSe: string;
    firstNameSe: string;
    lastNameSe: string;
    userTypeRSe: string;
    activoSe: string;
    asignableSe: string;
    classificationQFBSe: string;
}
