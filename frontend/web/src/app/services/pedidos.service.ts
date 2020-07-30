import { Injectable } from '@angular/core';
import {ConsumeService} from "./consume.service";
import {Endpoints} from "../../environments/endpoints";

@Injectable({
  providedIn: 'root'
})
export class PedidosService {

  constructor(private consumeService: ConsumeService) { }

  getPedidos(params){
    let queryString: string = "?";    
    queryString = queryString+"date="+params['date']+"&";
    queryString = queryString+"offset="+params['offset']+"&";
    queryString = queryString+"limit="+params['limit']+"&";
    return this.consumeService.httpGet(Endpoints.pedidos.getPedidos+queryString);
  }
}
