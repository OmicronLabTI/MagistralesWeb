import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { saveAs as importedSaveAs } from 'file-saver';
import {map} from 'rxjs/operators';
// import {DataService} from './data.service';
import {Messages} from '../constants/messages';
import { MessagesService } from './messages.service';

@Injectable({
  providedIn: 'root'
})
export class DownloadImagesService {

  constructor(private http: HttpClient, private messagesService: MessagesService) { }

  downloadImageFromUrl(urlImage: string, fileName: string): void {
    this.http
        .get(urlImage, { responseType: 'blob' })
        .pipe(map(res => res))
        .subscribe(blob => {
             importedSaveAs(blob, fileName);
        }, ()  => {
            this.messagesService.presentToastCustom(Messages.errorOnDownloadImage, 'error', fileName, true, false );
        });
  }
}
