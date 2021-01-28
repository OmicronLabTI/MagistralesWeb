import {BaseResponseHttp} from './commons';

export class IIncidentsGraphicRes extends BaseResponseHttp {
    response: IncidentsGraphicsMatrix[][];
}
export class IncidentsGraphicsMatrix {
    fieldKey: string;
    totalCount: number;
    graphType: string;
}
