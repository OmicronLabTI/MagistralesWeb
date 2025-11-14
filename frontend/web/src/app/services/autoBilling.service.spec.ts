import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AutoBillingModel, SapOrderModel, RemissionModel } from '../model/http/autoBilling.model';
import { ConsumeService } from './consume.service';
import { Endpoints } from 'src/environments/endpoints';

@Injectable({
  providedIn: 'root'
})
export class AutoBillingService {

  private readonly API_URL = Endpoints.invoices.historyBilling;

  constructor(private consume: ConsumeService) {}

  getAllAutoBilling(
    offset: number,
    limit: number
  ): Observable<{ items: AutoBillingModel[]; total: number }> {

    const params = {
      offset: String(offset),
      limit: String(limit),
      status: 'Creación exitosa',
      startDate: '2025-10-01',
      endDate: '2025-11-15'
    };

    return this.consume.httpGet<{
      response: AutoBillingModel[];
      comments: { total: number };
    }>(this.API_URL, params).pipe(
      map((api) => ({
        items: api.response.map(item => ({
          requestId: item.requestId,
          sapInvoiceId: item.sapInvoiceId,
          sapCreationDate: item.sapCreationDate,
          invoiceType: item.invoiceType,
          billingMode: item.billingMode,
          originUser: item.originUser,
          shopOrder: item.shopOrder,
          sapOrder: item.sapOrder,
          shipments: item.shipments,
          retries: item.retries,

          sapOrders: item.sapOrders.map((s: SapOrderModel) => ({
            id: s.id,
            idpedidosap: s.idpedidosap,
            idinvoice: s.idinvoice
          })),

          remissions: item.remissions.map((r: RemissionModel) => ({
            id: r.id,
            idremission: r.idremission,
            idinvoice: r.idinvoice
          }))
        })),
        total: api.comments.total
      }))
    );
  }
}
