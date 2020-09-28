import { HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { FileTypeContentEnum } from '../enums/FileTypeContentEnum';

@Injectable({
  providedIn: 'root'
})
export class FileDownloaderService {
  constructor() { }
  /**
    * Download file from request result
    * @param {Observable<HttpResponse<Blob>>} requestResult - Observable for request.
    * @param {FileTypeContentEnum} fileType - File type content
    * @param {string} fileName - File name
 */
  public downloadFile(requestResult: Observable<HttpResponse<Blob>>, fileType: FileTypeContentEnum, fileName: string): void {
    requestResult
    .subscribe(response => {
      try {
        this.downloadFileResult(response.body, fileType, fileName);
      }
      catch(err) {
        console.log(err);
      }
    }, errorResponse => {
      console.log(errorResponse);
    })
  }

  /**
    * Download file from blob
    * @param {Blob} content - Blob content.
    * @param {FileTypeContentEnum} fileType - File type content
    * @param {string} fileName - File name
 */
  public downloadFileResult(content: Blob, fileType, fileName: string): void {
    console.log(content);
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
