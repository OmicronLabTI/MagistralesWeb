import { Injectable } from '@angular/core';
import {ConsumeService} from './consume.service';
import {Endpoints} from '../../environments/endpoints';
import { IProductivityRes } from '../model/http/productivity';

@Injectable({
  providedIn: 'root'
})
export class ProductivityService {

  constructor(private consumeServie: ConsumeService) { }

  getProductivity(queryString: string) {
    return this.consumeServie.httpGet<IProductivityRes>(`${Endpoints.productivity.getProductivity}${queryString}`);
  }
}
