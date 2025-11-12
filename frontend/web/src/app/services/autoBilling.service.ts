import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AutoBillingModel, SapOrderModel, RemissionModel } from '../model/http/autoBilling.model';
import { ConsumeService } from './consume.service';
import { Endpoints } from 'src/environments/endpoints';

/**
 * @fileoverview
 * Service responsible for retrieving and transforming automatic billing (Auto-Billing)
 * history data from the backend according to user story OM-7160.
 *
 * Responsibilities:
 * - Retrieve automatic billing records from the last 5 days (including today).
 * - Filter by status "Creación exitosa".
 * - Sort records in ascending order by invoice ID.
 * - Map backend responses into strongly typed models
 *   (AutoBillingModel, SapOrderModel, RemissionModel).
 *
 * @see Endpoints.invoices.historyBilling
 * @since November 2025
 * @version 1.5
 * @author
 * Victor Alfonso Tánori Ruiz
 */
@Injectable({
  providedIn: 'root'
})
export class AutoBillingService {
  private readonly API_URL = Endpoints.invoices.historyBilling;

  constructor(private consume: ConsumeService) {}

  /**
   * Retrieves Auto-Billing records with pagination and filtering.
   * @param offset current page index * pageSize
   * @param limit number of records per page
   */
  getAllAutoBilling(offset = 0, limit = 20): Observable<AutoBillingModel[]> {
    const today = new Date();
    const fiveDaysAgo = new Date(today);
    fiveDaysAgo.setDate(today.getDate() - 5);

    const format = (d: Date): string => d.toISOString().split('T')[0];

    const params = {
      offset: String(offset),
      limit: String(limit),
      status: 'Creación exitosa',
      startDate: format(fiveDaysAgo),
      endDate: format(today),
      sortBy: 'id',
      sortOrder: 'asc'
    };

    // ✅ No manejamos errores locales; ConsumeService los captura automáticamente.
    return this.consume.httpGet<{ response: any[] }>(this.API_URL, params).pipe(
      map(apiResponse => {
        const data = apiResponse && apiResponse.response ? apiResponse.response : [];
        data.sort((a, b) => (a.id || '').localeCompare(b.id || ''));

        return data.map((item): AutoBillingModel => {
          const sapOrders: SapOrderModel[] = (item.sapOrders || []).map((s: any) => ({
            id: Number(s.id),
            idpedidosap: String(s.sapOrderId),
            idinvoice: String(s.idInvoice)
          }));

          const remissions: RemissionModel[] = (item.remissions || []).map((r: any) => ({
            id: Number(r.id),
            idremission: String(r.remissionId),
            idinvoice: String(r.idInvoice)
          }));

          return {
            requestId: String(item.id || ''),
            sapInvoiceId: String(item.idFacturaSap || 'N/A'),
            sapCreationDate: String(item.invoiceCreateDate || 'N/A'),
            invoiceType: String(item.typeInvoice || 'N/A'),
            billingMode: String(item.billingType || 'N/A'),
            originUser: String(item.almacenUser || 'Unknown'),
            shopOrder: String(item.dxpOrderId || ''),
            shopTransaction: String(item.shopTransaction || ''),
            sapOrder: Number(item.sapOrdersCount || sapOrders.length),
            shipments: Number(item.remissionsCount || remissions.length),
            retries: Number(item.retryNumber != null ? item.retryNumber : 0),
            sapOrders,
            remissions
          };
        });
      })
    );
  }
}
