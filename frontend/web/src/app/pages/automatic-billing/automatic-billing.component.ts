/**
 * ============================================================================
 * COMPONENT: AutomaticBillingComponent
 * ============================================================================
 * Purpose:
 *  This component manages the "Automatic Billing Dashboard" view, allowing
 *  administrators to track invoices that were automatically created by
 *  the warehouse system (Magistrales Web).
 *
 * Functional scope (HU OM-7160):
 *  - Displays historical automatic billing records (status: "Creación exitosa").
 *  - Lists invoices from the last 5 days (by default) with:
 *    • SAP invoice ID, creation date, invoice type, billing mode
 *    • Related SAP orders and remissions
 *    • Warehouse user, retry count, and error message (if any)
 *  - Allows user interactions:
 *    • Manual retry for failed invoices
 *    • View SAP orders and related remissions in modal dialogs
 *    • Confirm manual adjustments
 *
 *  Pagination and filters are supported for scalability.
 * ============================================================================
 */

import { Component, OnInit } from '@angular/core';
import {
  MatDialog,
  MatSelectionListChange,
  MatSlideToggleChange,
  MatTableDataSource,
  PageEvent
} from '@angular/material';
import {
  automaticBillingTableColumns,
  automaticBillingStatusConst,
  automaticBillingInvoiceTypeConst,
  automaticBillingBillingTypeConst,
  advanceFiltersTypes
} from 'src/app/constants/automatic_billing_constants';
import {
  CONST_NUMBER,
  CONST_STRING,
  HttpServiceTOCall
} from 'src/app/constants/const';
import { FilterInvoiceTypeDialogComponent } from 'src/app/dialogs/filter-invoice-type-dialog/filter-invoice-type-dialog.component';
import {
  ManualAdjustmentConfirmedDialogComponent
} from 'src/app/dialogs/manual-adjustment-confirmed-dialog/manual-adjustment-confirmed-dialog.component';
import {
  ViewSapOrdersDialogComponent
} from 'src/app/dialogs/view-sap-orders-dialog/view-sap-orders-dialog.component';
import {
  ViewShipmentsDialogComponent
} from 'src/app/dialogs/view-shipments-dialog/view-shipments-dialog.component';
import {
  RemissionModel,
  SapOrderModel
} from 'src/app/model/http/autoBilling.model';
import {
  AutomaticBilling,
  AutomaticBillingAdvanceFilters,
  ManualRetryequest
} from 'src/app/model/http/invoices';
import { DataService } from 'src/app/services/data.service';
import { ErrorService } from 'src/app/services/error.service';
import { InvoicesService } from 'src/app/services/invoices.service';
import { LocalStorageService } from 'src/app/services/local-storage.service';
import { ObservableService } from 'src/app/services/observable.service';

@Component({
  selector: 'app-automatic-billing',
  templateUrl: './automatic-billing.component.html',
  styleUrls: ['./automatic-billing.component.scss']
})
export class AutomaticBillingComponent implements OnInit {
  /** Columns to display in the Material table */
  displayedColumns = automaticBillingTableColumns;

  /** Data source for Angular Material Table */
  dataSource = new MatTableDataSource<AutomaticBilling>();

  /** Paginator settings */
  pageSize = CONST_NUMBER.ten;
  lengthPaginator = CONST_NUMBER.zero;
  pageIndex = CONST_NUMBER.zero;
  pageEvent: PageEvent;

  /** Pagination and query parameters */
  offset = CONST_NUMBER.zero;
  limit = CONST_NUMBER.ten;
  queryString = CONST_STRING.empty;
  statusFilter = CONST_STRING.empty;
  invoiceTypeFilter = CONST_STRING.empty;
  billingTypeFilter = CONST_STRING.empty;
  advanceFilter = CONST_STRING.empty;
  filtersObtained = new AutomaticBillingAdvanceFilters();

  /** Status constants and filters */
  automaticBillingStatus = automaticBillingStatusConst;
  automaticBillingInvoiceType = automaticBillingInvoiceTypeConst;
  automaticBillingBillingType = automaticBillingBillingTypeConst;

  filtersStatus = [
    automaticBillingStatusConst.error,
    automaticBillingStatusConst.sent,
    automaticBillingStatusConst.creating
  ];

  filterInvoiceType = [
    automaticBillingInvoiceTypeConst.generic,
    automaticBillingInvoiceTypeConst.non_generic
  ];

  filterBillingType = [
    automaticBillingBillingTypeConst.parcial,
    automaticBillingBillingTypeConst.completa
  ];

  /** Selected filter options for status */
  statusColumnSelectedOptions: string[] = [
    automaticBillingStatusConst.error,
    automaticBillingStatusConst.sent,
    automaticBillingStatusConst.creating
  ];

  invoiceTypeColumnSelectedOptions: string[] = [
    automaticBillingInvoiceTypeConst.generic,
    automaticBillingInvoiceTypeConst.non_generic
  ];

  billingTypeColumSelectedOptions: string[] = [
    automaticBillingBillingTypeConst.parcial,
    automaticBillingBillingTypeConst.completa
  ];

  /** Tracks the last selected status option */
  lastOptionStatus: string | null = null;
  lastOptionInvoiceType: string | null = null;
  lastOptionBillingType: string | null = null;

  constructor(
    private invoicesService: InvoicesService,
    private errorService: ErrorService,
    private observableService: ObservableService,
    public dataService: DataService,
    private dialog: MatDialog,
    private localStorageService: LocalStorageService
  ) {
    /** Register this module as active for navigation breadcrumbs */
    this.observableService.setUrlActive(HttpServiceTOCall.AUTOMATIC_BILLING);
  }

  /**
   * Initializes the component and loads billing data.
   */
  ngOnInit(): void {
    this.getData();
  }

  /**
   * Fetches automatic billing data from the backend using current filters.
   */
  getData(): void {
    this.setFilters();
    this.getQueryString();

    this.invoicesService.getAutomaticBillingTableData(this.queryString).subscribe({
      next: (res) => {
        this.lengthPaginator = res.comments;
        this.dataSource.data = res.response;
        this.shapeData();
      },
      error: (err) => this.errorService.httpError(err)
    });
  }

  /**
   * Ensures "errorMessage" field displays "NO APLICA" when empty or invalid.
   */
  private shapeData(): void {
    this.dataSource.data.forEach(row => {
      row.errorMessage = this.dataService.calculateTernary(
        this.dataService.validateValidString(row.errorMessage),
        row.errorMessage,
        'NO APLICA'
      );
      row.typeInvoice = this.dataService.calculateTernary(
        row.typeInvoice === automaticBillingInvoiceTypeConst.non_generic,
        'Con datos fiscales',
        row.typeInvoice
      );
    });
  }

  /**
   * Combines selected status filters into a comma-separated string.
   */
  private setFilters(): void {
    this.statusFilter = this.statusColumnSelectedOptions.join(',');
    this.invoiceTypeFilter = this.invoiceTypeColumnSelectedOptions.join(',');
    this.billingTypeFilter = this.billingTypeColumSelectedOptions.join(',');
  }


  clearFilters(): void {
    this.statusColumnSelectedOptions = [...this.filtersStatus];
    this.invoiceTypeColumnSelectedOptions = [...this.filterInvoiceType];
    this.billingTypeColumSelectedOptions = [...this.filterBillingType];
    this.advanceFilter = '';
    this.onSelectionChangeStatus();
    this.onSelectionChangeInvoiceType();
    this.onSelectionChangeBillingType();
    this.applyFilters();
  }

  /**
   * Builds the query string for backend request.
   */
  private getQueryString(): void {
    const inTableFilters = `&status=${this.statusFilter}&invoiceType=${this.invoiceTypeFilter}&billingType=${this.billingTypeFilter}`;
    const isAdvanceSearch = this.dataService.calculateTernary(
      this.dataService.validateValidString(this.advanceFilter),
      this.advanceFilter,
      inTableFilters
    );
    this.queryString = `&offset=${this.offset}&limit=${this.limit}` + isAdvanceSearch;
  }

  /**
   * Handles paginator changes (page index and size).
   * Reloads data from backend accordingly.
   */
  changeDataEvent(event: PageEvent): PageEvent {
    this.pageSize = event.pageSize;
    this.offset = (event.pageSize * event.pageIndex);
    this.limit = event.pageSize;
    this.pageIndex = event.pageIndex;
    this.getData();
    return event;
  }

  applyFilters(): void {
    this.pageIndex = 0;
    this.offset = 0;
    this.getData();
  }

  /**
   * Opens confirmation dialog for manual adjustment.
   * If confirmed, sends backend update.
   */
  manualChangesApplied(row: AutomaticBilling): void {
    this.dialog.open(ManualAdjustmentConfirmedDialogComponent, {
      panelClass: 'custom-dialog-container',
      data: ''
    }).afterClosed().subscribe(confirmadjustment => {
      if (confirmadjustment) {
        this.sendManualChangesConfirmation(row);
      } else {
        row.manualChangeApplied = false;
      }
    });
  }

  /**
   * Sends manual adjustment confirmation to backend.
   */
  sendManualChangesConfirmation(row: AutomaticBilling): void {
    const query = `${row.id}`;
    this.invoicesService.adjustmentMade(query).subscribe({
      next: (res) => {
        if (res.code !== 200) {
          row.manualChangeApplied = false;
        }
      },
      error: (err) => this.errorService.httpError(err)
    });
  }

  /**
   * Sends a manual retry request to reprocess invoice creation.
   */
  manualRetry(operationId: string): void {
    const userId = this.localStorageService.getUserId();

    const request: ManualRetryequest = {
      invoiceIds: [operationId],
      requestingUser: userId,
      offset: 0,
      limit: 0
    };

    this.invoicesService.sendManualRetry(request).subscribe({
      next: (res) => {
        if (res.code === 200) {
          const processedIdsList = res.response.processedIds;
          const skippedIdsList = res.response.skippedIds;
          // TODO: Implement user feedback for processed/skipped IDs
        }
      },
      error: (err) => this.errorService.httpError(err)
    });
  }

  /**
   * Determines whether the manual retry button should be disabled.
   * Rules:
   *  - Disabled if the status is not 'Error'
   *  - Disabled if manual adjustment is required but not yet applied
   */
  disableManualRetry(element: AutomaticBilling): boolean {
    return this.dataService.calculateOrValueList([
      element.status !== automaticBillingStatusConst.error,
      !element.manualChangeApplied && element.manualChangeApplied !== null
    ]);
  }

  /**
   * Opens dialog to view SAP Orders or Remissions depending on the flag.
   * @param row Invoice row data
   * @param isForSAPorders Whether the dialog is for SAP orders (true) or remissions (false)
   */
  seeDetail(row: AutomaticBilling, isForSAPorders: boolean): void {
    const dataToTransform = isForSAPorders ? row.sapOrderId : row.remissionId;
    const dataToModal = isForSAPorders
      ? this.buildObjectForSAPOrders(dataToTransform)
      : this.buildObjectForRemissions(dataToTransform);

    if (isForSAPorders) {
      this.dialog.open(ViewSapOrdersDialogComponent, {
        width: '800px',
        panelClass: 'custom-dialog-container',
        data: {
          invoiceId: row.id,
          orders: dataToModal,
          status: row.status,
          updateDate: row.updateDate,
          isFromAutomaticBilling: true,
        }
      });
    } else {
      this.dialog.open(ViewShipmentsDialogComponent, {
        width: '800px',
        panelClass: 'custom-dialog-container',
        data: {
          invoiceId: row.id,
          remissions: dataToModal,
          status: row.status,
          updateDate: row.updateDate,
          isFromAutomaticBilling: true,
        }
      });
    }
  }

  openAdvancedFiltersDialog(): void {
    const dialogRef = this.dialog.open(FilterInvoiceTypeDialogComponent, {
      panelClass: 'advanced-filter-dialog',
      disableClose: true,
      width: 'auto',
      maxWidth: '95vw',
      data: true,
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.filtersObtained.type = result.type;
        this.filtersObtained.value = result.value;
        this.getTypeOfFilter(this.filtersObtained);
      }
    });
  }

  getTypeOfFilter(filter: AutomaticBillingAdvanceFilters): void {
    Object.entries(advanceFiltersTypes).forEach(([key, value]) => {
      if (value === filter.type) {
        this.advanceFilter = `&idtype=${key}&id=${filter.value}`;
        this.applyFilters();
      }
    });
  }

  /**
   * Transforms numeric SAP order IDs into structured model objects.
   */
  private buildObjectForSAPOrders(list: number[]): SapOrderModel[] {
    return list.map((element, index) => ({
      id: index + 1,
      idpedidosap: `${element}`,
      idinvoice: ''
    }));
  }

  /**
   * Transforms numeric remission IDs into structured model objects.
   */
  private buildObjectForRemissions(list: number[]): RemissionModel[] {
    return list.map((element, index) => ({
      id: index + 1,
      idremission: `${element}`,
      idinvoice: ''
    }));
  }

  /**
   * Handles selection changes for the status filter menu.
   */
  onSelectionChangeStatus(): void {
    this.lastOptionStatus = this.statusColumnSelectedOptions.length === 1
      ? this.statusColumnSelectedOptions[0]
      : null;
  }

  onSelectionChangeInvoiceType(): void {
    this.lastOptionInvoiceType = this.invoiceTypeColumnSelectedOptions.length === 1
      ? this.invoiceTypeColumnSelectedOptions[0]
      : null;
  }

  onSelectionChangeBillingType(): void {
    this.lastOptionBillingType = this.billingTypeColumSelectedOptions.length === 1
      ? this.billingTypeColumSelectedOptions[0]
      : null;
  }

  /**
   * Checks if a given filter is the last remaining active one.
   */
  isLastStatusFilter(filter: string): boolean {
    return this.lastOptionStatus === filter || this.dataService.validateValidString(this.advanceFilter);
  }

  isLastInvoiceFilter(filter: string): boolean {
    return this.lastOptionInvoiceType === filter || this.dataService.validateValidString(this.advanceFilter);
  }

  isLastBillingTypeFilter(filter: string): boolean {
    return this.lastOptionBillingType === filter || this.dataService.validateValidString(this.advanceFilter);
  }
}
