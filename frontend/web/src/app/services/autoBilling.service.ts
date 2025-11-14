import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AutoBillingModel } from '../model/http/autoBilling.model';
import { ConsumeService } from './consume.service';
import { Endpoints } from 'src/environments/endpoints';

@Injectable({
  providedIn: 'root'
})
export class AutoBillingService {

  /**
   * API endpoint for retrieving billing records.
   */
  private readonly API_URL = Endpoints.invoices.historyBilling;

  constructor(private consume: ConsumeService) {}

  /**
   * Retrieves paginated AutoBilling data from the backend.
   * ConsumeService already handles all HTTP errors globally.
   *
   * @param offset The start index.
   * @param limit Number of items per page.
   */
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

    return this.consume.httpGet<any>(this.API_URL, params).pipe(
      map((api) => {

        const rows = api && api.response ? api.response : [];
        const total = api && api.comments && api.comments.total
          ? api.comments.total
          : 0;

        const mapped = rows.map((item) => {

          const sapOrders = item.sapOrders
            ? item.sapOrders.map((s) => {
                return {
                  id: Number(s.id),
                  idpedidosap: String(s.sapOrderId),
                  idinvoice: String(s.idInvoice)
                };
              })
            : [];

          const remissions = item.remissions
            ? item.remissions.map((r) => {
                return {
                  id: Number(r.id),
                  idremission: String(r.remissionId),
                  idinvoice: String(r.idInvoice)
                };
              })
            : [];

          return {
            requestId: item.id,
            sapInvoiceId: item.idFacturaSap,
            sapCreationDate: item.invoiceCreateDate,
            invoiceType: item.typeInvoice,
            billingMode: item.billingType,
            originUser: item.almacenUser,

            /**
             * Returns only the last 6 characters of the shop order ID.
             */
            shopOrder: item.dxpOrderId
              ? String(item.dxpOrderId).slice(-6)
              : '',

            sapOrder: item.sapOrdersCount,
            shipments: item.remissionsCount,
            retries: item.retryNumber,
            sapOrders,
            remissions
          };
        });

        return {
          items: mapped,
          total
        };
      })
    );
  }
}
