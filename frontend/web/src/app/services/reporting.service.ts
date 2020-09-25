import { HttpHeaders, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Endpoints } from 'src/environments/endpoints';
import { FileTypeContentEnum } from '../enums/FileTypeContentEnum';
import { ConsumeService } from './consume.service';

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
}
