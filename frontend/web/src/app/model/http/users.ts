import {BaseResponseHttp} from './commons';
export class IUserRes extends BaseResponseHttp {
    response: any;
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
    password: string;
    activo: number;
    isChecked?: boolean = false;
}
export class IPlaceOrdersReq {
    userLogistic: string;
    userId: string;
    list: number[];
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
    countTotal?: number;
    modalType?: string;
    list?: number[];
}
