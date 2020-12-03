import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { saveAs as importedSaveAs } from 'file-saver';
import {map} from 'rxjs/operators';
import {DataService} from './data.service';
import {Messages} from '../constants/messages';

@Injectable({
  providedIn: 'root'
})
export class DownloadImagesService {

  constructor(private http: HttpClient, private dataService: DataService) { }

  downloadImageFromUrl(urlImage: string, fileName: string): void {
    this.http
        .get(urlImage, { responseType: 'blob' })
        .pipe(map(res => res))
        .subscribe(blob => {
             importedSaveAs(blob, fileName);
        }, ()  => {
            this.dataService.presentToastCustom(Messages.errorOnDownloadImage, 'error', fileName, true, false );
        });
  }
}
