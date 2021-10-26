import { HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { FileTypeContentEnum } from '../enums/FileTypeContentEnum';

@Injectable({
  providedIn: 'root'
})
export class FileDownloaderService {
  constructor() { }
  public downloadFile(requestResult: Observable<HttpResponse<Blob>>, fileType: FileTypeContentEnum, fileName: string): void {
    requestResult
      .subscribe(response => {
        try {
          this.downloadFileResult(response.body, fileType, fileName);
        } catch (err) {
          return;
        }
      }, errorResponse => {
      });
  }
  public downloadFileResult(content: Blob, fileType, fileName: string): void {
    const blob = new Blob([content], { type: fileType });
    const dwlLink = document.createElement('a');
    const url = URL.createObjectURL(blob);
    dwlLink.setAttribute('href', url);
    dwlLink.setAttribute('download', fileName.toString());
    dwlLink.style.visibility = 'hidden';
    document.body.appendChild(dwlLink);
    dwlLink.click();
    document.body.removeChild(dwlLink);
  }
}
