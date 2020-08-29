import { Injectable } from '@angular/core';
import {Observable} from 'rxjs';
import {HttpHeaders, HttpClient, HttpParams} from '@angular/common/http';
import { DataService } from './data.service';

@Injectable({
  providedIn: 'root'
})
export class ConsumeService {

  constructor(private http: HttpClient, private dataService: DataService) { }

  httpGet<T>(url: string, params?, headers?) {
    let objHeaders = new HttpHeaders();
    if (headers) {
      Object.keys(headers).forEach((key) => {
        objHeaders = objHeaders.append(key, headers[key]);
      });
    }

    let objParams = new HttpParams();
    if (params) {
      Object.keys(params).forEach((key) => {
        objParams = objParams.append(key, params[key]);
      });
    }

    this.dataService.setIsLoading(true);
    return new Observable<T>(observer => {
      this.http.get<any>(url, { headers: objHeaders, params: objParams })
        .subscribe(response => {
          this.successObserver(observer, response);
        }, err => {
          this.onErrorObserver(observer, err);
        });
    });
  }

  httpPost<T>(url: string, body: any, headers?) {
    let objHeaders = new HttpHeaders();
    if (headers) {
      Object.keys(headers).forEach((key) => {
        objHeaders = objHeaders.append(key, headers[key]);
      });
    }

    this.dataService.setIsLoading(true);
    return new Observable<T>(observer => {
      this.http.post<any>(url, body, { headers: objHeaders })
        .subscribe(response => {
          this.successObserver(observer, response);
        }, err => {
          this.onErrorObserver(observer, err);
        });
    });
  }

  httpPut<T>(url: string, body: any, headers?) {
    let objHeaders = new HttpHeaders();
    if (headers) {
      Object.keys(headers).forEach((key) => {
        objHeaders = objHeaders.append(key, headers[key]);
      });
    }

    this.dataService.setIsLoading(true);
    return new Observable<T>(observer => {
      this.http.put<any>(url, body, { headers: objHeaders })
        .subscribe(response => {
          this.successObserver(observer, response);
        }, err => {
          this.onErrorObserver(observer, err);
        });
    });
  }

  httpPatch<T>(url: string, body: any, headers?) {
    let objHeaders = new HttpHeaders();
    if (headers) {
      Object.keys(headers).forEach((key) => {
        objHeaders = objHeaders.append(key, headers[key]);
      });
    }

    this.dataService.setIsLoading(true);
    return new Observable<T>(observer => {
      this.http.patch<any>(url, body, { headers: objHeaders })
          .subscribe(response => {
            this.successObserver(observer, response);
          }, err => {
            this.onErrorObserver(observer, err);
          });
    });
  }

  private successObserver(observer: any, response: any) {
    observer.next(response);
    observer.complete();
    this.dataService.setIsLoading(false);
  }

  private onErrorObserver(observer: any, err: any) {
    observer.error(err);
    this.dataService.setIsLoading(false);
  }
}
