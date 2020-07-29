import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { DataService } from './data.service';

@Injectable({
  providedIn: 'root'
})
export class ConsumeService {

  constructor(private http: HttpClient, private dataService: DataService) { }

  httpGet<T>(url: string, headers?) {
    let objHeaders = new HttpHeaders();
    if (headers) {
      Object.keys(headers).forEach((key) => {
        objHeaders = objHeaders.append(key, headers[key]);
      });
    }

    this.dataService.setIsLoading(true);
    return new Observable<T>(observer => {
      this.http.get<any>(url, { headers: objHeaders })
        .subscribe(response => {
          observer.next(response);
          observer.complete();
          this.dataService.setIsLoading(false);
        }, err => {
          observer.error(err);
          this.dataService.setIsLoading(false);
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
          observer.next(response);
          observer.complete();
          this.dataService.setIsLoading(false);
        }, err => {
          observer.error(err);
          this.dataService.setIsLoading(false);
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
          observer.next(response);
          observer.complete();
          this.dataService.setIsLoading(false);
        }, err => {
          observer.error(err);
          this.dataService.setIsLoading(false);
        });
    });
  }

  httpDelete<T>(url: string, body: any, headers?) {
    let objHeaders = new HttpHeaders();
    if (headers) {
      Object.keys(headers).forEach((key) => {
        objHeaders = objHeaders.append(key, headers[key]);
      });
    }

    this.dataService.setIsLoading(true);
    return new Observable<T>(observer => {
      /*this.http.delete<any>(url, body, { headers: objHeaders })
          .subscribe(response => {
            observer.next(response);
            observer.complete();
            this.dataService.setIsLoading(false);
          }, err => {
            observer.error(err);
            this.dataService.setIsLoading(false);
          });*/
    });
  }
}
