import {BaseResponseHttp} from './commons';

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
export class RoleUser {
    id: number;
    description: string;
}

export class QfbSelect {
    qfbId: string;
    qfbName: string;
}
