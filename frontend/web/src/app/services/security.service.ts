import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Endpoints } from 'src/environments/endpoints';
import { ILoginRes, ILoginReq } from '../model/http/security.model';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { DataService } from './data.service';
import { ConsumeService } from './consume.service';

@Injectable({
  providedIn: 'root'
})
export class SecurityService {

  constructor(private http: HttpClient, private dataService: DataService, private consumeService: ConsumeService) { }

  login(req: ILoginReq): Observable<ILoginRes> {
    return this.consumeService.httpPost(Endpoints.security.login, req);
  }
}
