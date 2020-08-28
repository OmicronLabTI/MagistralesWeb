export class BaseResponseHttp {
    code: number;
    userError: any;
    exceptionMessage: any;
    success: boolean;
    comments?: number;
}

export class ErrorHttpInterface {
    error: MessageError;
    message: string;
    name: string;
    ok: boolean;
    status: number;
    statusText: string;
    url: string;
}
export class MessageError {
    errorCode: number;
    errorMessage: any;
    info: string;
    userError: string;
}
