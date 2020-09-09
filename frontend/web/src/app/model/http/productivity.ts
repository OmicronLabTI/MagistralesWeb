import {BaseResponseHttp} from './commons';

export class IProductivityRes extends BaseResponseHttp {
    response: IProductivityReq;
}

export class IProductivityReq {
    matrix: string[][];
}