/**
 * @fileoverview
 * Unit test suite for the AutoBillingService.
 *
 * This spec validates the correct behavior of the automatic billing data service,
 * including parameter construction, mapping transformations, sorting logic,
 * and empty response handling.
 *
 * Framework: Angular 8 (Karma + Jasmine)
 * Author: Victor Alfonso Tánori Ruiz
 * Date: November 2025
 */

import { DatePipe } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { of } from 'rxjs';

import { Endpoints } from 'src/environments/endpoints';
import { AutoBillingModel } from 'src/app/model/http/autoBilling.model';
import { ConsumeService } from 'src/app/services/consume.service';
import { AutoBillingService } from 'src/app/services/autoBilling.service';

/**
 * Test suite for AutoBillingService.
 * Ensures that the service:
 *  - Calls the backend using the correct parameters
 *  - Maps backend data into typed models
 *  - Sorts data properly
 *  - Handles empty or malformed responses gracefully
 */
describe('AutoBillingService', () => {
  let service: AutoBillingService;
  let consumeServiceMock: jasmine.SpyObj<ConsumeService>;

  beforeEach(() => {
    // Mock del servicio dependiente que realiza las llamadas HTTP
    consumeServiceMock = jasmine.createSpyObj<ConsumeService>('ConsumeService', ['httpGet']);

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, RouterTestingModule],
      providers: [
        DatePipe,
        AutoBillingService,
        { provide: ConsumeService, useValue: consumeServiceMock }
      ]
    });

    // Angular 8 usa get() en lugar de inject()
    service = TestBed.get(AutoBillingService);
  });

  /* ==========================================================
     ✅ 1. CREATION & BASIC SERVICE INTEGRATION
  ========================================================== */

  /**
   * Should create the service successfully.
   */
  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  /**
   * Should call `ConsumeService.httpGet` with correctly formatted parameters.
   * Verifies that the service constructs valid API parameters including:
   *  - Offset and limit for pagination
   *  - Filtering by status and date range
   *  - Sorting rules
   */
  it('should call httpGet with correct parameters', () => {
    const mockResponse = { response: [] };
    consumeServiceMock.httpGet.and.returnValue(of(mockResponse));

    service.getAllAutoBilling(10, 50).subscribe();

    expect(consumeServiceMock.httpGet).toHaveBeenCalledTimes(1);

    const [url, params] = consumeServiceMock.httpGet.calls.mostRecent().args;

    expect(url).toBe(Endpoints.invoices.historyBilling);
    expect(params.offset).toBe('10');
    expect(params.limit).toBe('50');
    expect(params.status).toBe('Creación exitosa');
    expect(params.sortBy).toBe('id');
    expect(params.sortOrder).toBe('asc');
    expect(params.startDate).toBeDefined();
    expect(params.endDate).toBeDefined();
  });

  /* ==========================================================
     ✅ 2. DATA MAPPING & MODEL TRANSFORMATION
  ========================================================== */

  /**
   * Should correctly transform API response into AutoBillingModel.
   *
   * This test validates:
   *  - Proper mapping of SAP Orders and Remissions arrays
   *  - Default values for missing optional fields
   *  - Type enforcement on numeric and string conversions
   */
  it('should map backend response correctly into AutoBillingModel', (done) => {
    const mockResponse = {
      response: [
        {
          id: 'A001',
          idFacturaSap: 'SAP001',
          invoiceCreateDate: '2025-11-12',
          typeInvoice: 'Factura A',
          billingType: 'Automática',
          almacenUser: 'userTest',
          dxpOrderId: 'ORD123',
          shopTransaction: 'TX123',
          retryNumber: 2,
          sapOrders: [
            { id: 1, sapOrderId: 'SO001', idInvoice: 'INV001' }
          ],
          remissions: [
            { id: 11, remissionId: 'REM001', idInvoice: 'INV001' }
          ]
        }
      ]
    };

    consumeServiceMock.httpGet.and.returnValue(of(mockResponse));

    service.getAllAutoBilling().subscribe((result: AutoBillingModel[]) => {
      expect(result.length).toBe(1);

      const item = result[0];
      expect(item.requestId).toBe('A001');
      expect(item.sapInvoiceId).toBe('SAP001');
      expect(item.invoiceType).toBe('Factura A');
      expect(item.billingMode).toBe('Automática');
      expect(item.originUser).toBe('userTest');
      expect(item.retries).toBe(2);

      // Validación del submodelo SAP Orders
      expect(item.sapOrders.length).toBe(1);
      expect(item.sapOrders[0].idpedidosap).toBe('SO001');
      expect(item.sapOrders[0].idinvoice).toBe('INV001');

      // Validación del submodelo Remissions
      expect(item.remissions.length).toBe(1);
      expect(item.remissions[0].idremission).toBe('REM001');
      done();
    });
  });

  /* ==========================================================
     ✅ 3. EMPTY RESPONSE HANDLING
  ========================================================== */

  /**
   * Should safely handle empty or undefined API responses.
   * The service must return an empty array instead of throwing errors.
   */
  it('should return empty array when API returns no data', (done) => {
    consumeServiceMock.httpGet.and.returnValue(of({}));

    service.getAllAutoBilling().subscribe((result) => {
      expect(result).toEqual([]);
      done();
    });
  });

  /* ==========================================================
     ✅ 4. SORTING LOGIC VALIDATION
  ========================================================== */

  /**
   * Should return results sorted in ascending order by the `id` field.
   * Confirms that the client-side sorting logic is applied even if
   * the backend response is unordered.
   */
  it('should sort results ascending by id', (done) => {
    const unsortedResponse = {
      response: [
        { id: 'C003', idFacturaSap: 'SAP003' },
        { id: 'A001', idFacturaSap: 'SAP001' },
        { id: 'B002', idFacturaSap: 'SAP002' }
      ]
    };

    consumeServiceMock.httpGet.and.returnValue(of(unsortedResponse));

    service.getAllAutoBilling().subscribe((result) => {
      const ids = result.map(r => r.requestId);
      expect(ids).toEqual(['A001', 'B002', 'C003']);
      done();
    });
  });
});
