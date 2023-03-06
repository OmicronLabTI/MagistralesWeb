import { HttpHeaders, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Endpoints } from 'src/environments/endpoints';
import { FileTypeContentEnum } from '../enums/FileTypeContentEnum';
import { ConsumeService } from './consume.service';
import { DownloadPDFResponse } from '../model/http/reporting';

@Injectable({
  providedIn: 'root'
})
export class ReportingService {
  constructor(private consumeServie: ConsumeService) { }

  public downloadPreviewRawMaterialRequest(data: any): Observable<HttpResponse<Blob>> {
    const headers = new HttpHeaders()
    .set('Accept', FileTypeContentEnum.PDF);

    return this.consumeServie.httpDownloadFilePost(`${Endpoints.reporting.getRawMaterialRequestFilePreview}`, data, headers);
  }

  downloadPreviewMaterial(data: any): Observable<DownloadPDFResponse> {
    return this.consumeServie.httpPost(`${Endpoints.reporting.getRawMaterialRequestFilePreview}`, data);
  }
}
