import { Injectable } from '@angular/core';
import { Observable, from } from 'rxjs';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { DataService } from './data.service';

@Injectable({
  providedIn: 'root'
})
export class ConsumeService {

  constructor(private _http: HttpClient, private _dataService: DataService) { }

  httpGet(url: string, headers?) {
    let objHeaders = new HttpHeaders();
    if (headers) {
      Object.keys(headers).forEach((key) => {
        objHeaders = objHeaders.append(key, headers[key]);
      });
    }

    this._dataService.setIsLoading(true);
    return Observable.create(observer => {
      this._http.get<any>(url, { headers: objHeaders })
        .subscribe(response => {
          observer.next(response);
          observer.complete();
          this._dataService.setIsLoading(false);
        }, err => {
          observer.error(err);
          this._dataService.setIsLoading(false);
        });
    });
  }

  httpPost(url: string, body: any, headers?) {
    let objHeaders = new HttpHeaders();
    if (headers) {
      Object.keys(headers).forEach((key) => {
        objHeaders = objHeaders.append(key, headers[key]);
      });
    }

    this._dataService.setIsLoading(true);
    return Observable.create(observer => {
      this._http.post<any>(url, body, { headers: objHeaders })
        .subscribe(response => {
          observer.next(response);
          observer.complete();
          this._dataService.setIsLoading(false);
        }, err => {
          observer.error(err);
          this._dataService.setIsLoading(false);
        });
    });
  }

  httpPut(url: string, body: any, headers?) {
    let objHeaders = new HttpHeaders();
    if (headers) {
      Object.keys(headers).forEach((key) => {
        objHeaders = objHeaders.append(key, headers[key]);
      });
    }

    this._dataService.setIsLoading(true);
    return Observable.create(observer => {
      this._http.put<any>(url, body, { headers: objHeaders })
        .subscribe(response => {
          observer.next(response);
          observer.complete();
          this._dataService.setIsLoading(false);
        }, err => {
          observer.error(err);
          this._dataService.setIsLoading(false);
        });
    });
  }
}
