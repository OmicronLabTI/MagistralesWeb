export class BaseResponseHttp {
    code: number;
    userError: any;
    exceptionMessage: any;
    success: boolean;
    comments?: number;
}

export class ErrorHttpInterface {
    error: string;
    message: string;
    name: string;
    ok: boolean;
    status: number;
    statusText: string;
    url: string;
}
