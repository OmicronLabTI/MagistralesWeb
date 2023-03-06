import { BaseResponseHttp } from './commons';

export interface DownloadPDFResponse extends BaseResponseHttp {
    response: Blob[];
}
