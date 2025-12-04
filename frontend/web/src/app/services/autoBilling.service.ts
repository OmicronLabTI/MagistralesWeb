import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AutoBillingModel } from '../model/http/autoBilling.model';
import { ConsumeService } from './consume.service';
import { Endpoints } from 'src/environments/endpoints';
import {
  AutoBillingApiResponse,
  AutoBillingApiItem
} from '../model/http/autoBilling-response.model';

@Injectable({
  providedIn: 'root'
})
export class AutoBillingService {
  private readonly API_URL = Endpoints.invoices.historyBilling;

  constructor(private consume: ConsumeService) {}

  getAllAutoBilling(
    offset: number,
    limit: number,
    startDate: Date,
    endDate: Date,
    billingType: string,
    invoiceType: string,
    idtype?: string,
    id?: string
  ): Observable<{ items: AutoBillingModel[]; total: number }> {

    const formatDate = (d: Date): string => {
      return new Date(d).toISOString().split('T')[0];
    };

    invoiceType = invoiceType.replace('Con datos fiscales', 'No genérica');

    const params = {
      offset: String(offset),
      limit: String(limit),
      status: 'Creación exitosa',
      startDate: formatDate(startDate),
      endDate: formatDate(endDate),
      billingType,
      invoiceType,
      idtype,
      id
    };

    return this.consume.httpGet<AutoBillingApiResponse>(this.API_URL, params).pipe(
      map((api: AutoBillingApiResponse) => {
        const rows = api && api.response ? api.response : [];
        const total = api && api.comments && api.comments.total ? api.comments.total : 0;

        const mapped: AutoBillingModel[] = rows.map((item: AutoBillingApiItem) => {
          const sapOrders = item.sapOrders.map(s => ({
            id: s.id,
            idpedidosap: s.sapOrderId,
            idinvoice: s.idInvoice
          }));

          const remissions = item.remissions.map(r => ({
            id: r.id,
            idremission: r.remissionId,
            idinvoice: r.idInvoice
          }));

          return {
            requestId: item.id,
            sapInvoiceId: item.idFacturaSap,
            sapCreationDate: item.invoiceCreateDate,
            invoiceType: item.typeInvoice,
            billingMode: item.billingType,
            originUser: item.almacenUser,
            shopOrder: item.dxpOrderId,
            sapOrder: item.sapOrdersCount,
            shipments: item.remissionsCount,
            retries: item.retryNumber,
            sapOrders,
            remissions
          };
        });

        mapped.sort((a, b) => Number(a.sapInvoiceId) - Number(b.sapInvoiceId));

        return {
          items: mapped,
          total
        };
      })
    );
  }
}
