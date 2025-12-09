import {
  Component,
  OnInit,
  AfterViewInit,
  ViewChild
} from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';

import { AutoBillingService } from 'src/app/services/autoBilling.service';
import { AutoBillingModel } from 'src/app/model/http/autoBilling.model';
import { ViewSapOrdersDialogComponent } from 'src/app/dialogs/view-sap-orders-dialog/view-sap-orders-dialog.component';
import { ViewShipmentsDialogComponent } from 'src/app/dialogs/view-shipments-dialog/view-shipments-dialog.component';

import { ObservableService } from 'src/app/services/observable.service';
import { HttpServiceTOCall } from 'src/app/constants/const';
import { FilterInvoiceTypeDialogComponent } from 'src/app/dialogs/filter-invoice-type-dialog/filter-invoice-type-dialog.component';

@Component({
  selector: 'app-auto-billing',
  templateUrl: './auto-billing.component.html',
  styleUrls: ['./auto-billing.component.scss']
})
export class AutoBillingComponent implements OnInit, AfterViewInit {

  displayedColumns: string[] = [
    'sapInvoiceId',
    'sapCreationDate',
    'invoiceType',
    'billingMode',
    'originUser',
    'shopOrder',
    'sapOrder',
    'shipments',
    'retries',
    'actions'
  ];

  dataSource = new MatTableDataSource<AutoBillingModel>([]);
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;

  // ================================================
  // OPCIONES DE FILTRO (con disabled dinámico)
  // ================================================
  invoiceOptions = [
    { label: 'Genérica', selected: true, disabled: false },
    { label: 'Con datos fiscales', selected: true, disabled: false }
  ];

  billingOptions = [
    { label: 'Completa', selected: true, disabled: false },
    { label: 'Parcial', selected: true, disabled: false }
  ];

  idtype = '';
  id = '';

  showFilter = false;
  popupPosition: any = {};
  currentOptions: any[] = [];

  startDate: Date;
  endDate: Date;

  isAdvancedSearchActive = false;

  FILTER_STORAGE_KEY = 'historyBillingFilters';
  DATE_STORAGE_KEY = 'historyBillingDates';

  constructor(
    private autoBillingService: AutoBillingService,
    private dialog: MatDialog,
    private observableService: ObservableService
  ) {
    this.observableService.setUrlActive(HttpServiceTOCall.HISTORY_BILLING);
  }

  // ============================================================
  // INIT
  // ============================================================
  ngOnInit(): void {
    const storedFilters = localStorage.getItem(this.FILTER_STORAGE_KEY);
    const storedDates = localStorage.getItem(this.DATE_STORAGE_KEY);

    if (storedFilters) {
      const f = JSON.parse(storedFilters);
      this.invoiceOptions = f.invoiceOptions;
      this.billingOptions = f.billingOptions;
      this.id = f.id;
      this.idtype = f.idtype;
    }

    if (storedDates) {
      const d = JSON.parse(storedDates);
      this.startDate = new Date(d.startDate);
      this.endDate = new Date(d.endDate);
    } else {
      const today = new Date();
      const past5 = new Date();
      past5.setDate(today.getDate() - 5);

      this.startDate = past5;
      this.endDate = today;

      localStorage.setItem(
        this.DATE_STORAGE_KEY,
        JSON.stringify({
          startDate: this.startDate,
          endDate: this.endDate
        })
      );
    }

    // Estado de filtros avanzados
    this.updateAdvancedSearchState();
    this.loadPageData(0, 10);

    // Cerrar popup con clic fuera
    document.addEventListener('click', (event: any) => {
      if (!this.showFilter) {
        return;
      }

      const inside =
        event.target.closest('.filter-box') ||
        event.target.closest('.header-filter');

      if (!inside) {
        this.showFilter = false;
      }
    });
  }

  ngAfterViewInit(): void {
    this.paginator.page.subscribe(event => {
      const changed = event.pageSize !== this.paginator.pageSize;

      if (changed) {
        this.paginator.pageIndex = 0;
      }

      const offset = this.paginator.pageIndex * event.pageSize;
      const limit = event.pageSize;

      this.loadPageData(offset, limit);
    });
  }

  // ============================================================
  // FILTROS
  // ============================================================
  getSelectedInvoiceTypes(): string {
    return this.invoiceOptions
      .filter(x => x.selected)
      .map(x => x.label)
      .join(',');
  }

  getSelectedBillingTypes(): string {
    return this.billingOptions
      .filter(x => x.selected)
      .map(x => x.label)
      .join(',');
  }

  // ============================================================
  // HABILITAR / DESHABILITAR FILTROS
  // ============================================================
  updateAdvancedSearchState(): void {
    this.isAdvancedSearchActive =
      this.id && this.id.trim() !== '' &&
      this.idtype && this.idtype.trim() !== '';

    // Bloquear filtros pero mostrarlos
    this.invoiceOptions.forEach(x => { x.disabled = this.isAdvancedSearchActive; });
    this.billingOptions.forEach(x => { x.disabled = this.isAdvancedSearchActive; });
  }

  // ============================================================
  // CARGA DE DATOS
  // ============================================================
  loadPageData(offset: number, limit: number): void {
    const invoices = this.getSelectedInvoiceTypes();
    const billing = this.getSelectedBillingTypes();

    localStorage.setItem(
      this.FILTER_STORAGE_KEY,
      JSON.stringify({
        invoiceOptions: this.invoiceOptions,
        billingOptions: this.billingOptions,
        id: this.id,
        idtype: this.idtype
      })
    );

    this.updateAdvancedSearchState();

    this.autoBillingService
      .getAllAutoBilling(
        offset,
        limit,
        this.startDate,
        this.endDate,
        billing,
        invoices,
        this.idtype,
        this.id
      )
      .subscribe(response => {
        this.dataSource.data = response.items;

        if (this.paginator) {
          this.paginator.length = response.total;
        }
      });
  }

  // ============================================================
  // POPUP DE FILTROS (BLOQUEADO EN BÚSQUEDA AVANZADA)
  // ============================================================
  openFilter(event: MouseEvent, type: string): void {
    if (this.isAdvancedSearchActive) {
      return; // NO ABRIR POPUP
    }

    const target = event.target as HTMLElement;
    const rect = target.getBoundingClientRect();

    this.popupPosition = {
      top: rect.bottom + 'px',
      left: rect.left + 'px'
    };

    this.currentOptions =
      type === 'invoice' ? this.invoiceOptions : this.billingOptions;

    this.showFilter = true;
  }

  toggleOption(option: any): void {
    if (option.disabled) {
      return;
    }
    option.selected = !option.selected;
  }

  // ============================================================
  // DIALOGS
  // ============================================================
  openSapOrdersDialog(row: AutoBillingModel): void {
    if (!row.sapOrders || row.sapOrders.length === 0) {
      return;
    }

    this.dialog.open(ViewSapOrdersDialogComponent, {
      width: '800px',
      panelClass: 'custom-dialog-container',
      data: row
    });
  }

  openShipmentsDialog(row: AutoBillingModel): void {
    if (!row.remissions || row.remissions.length === 0) {
      return;
    }

    this.dialog.open(ViewShipmentsDialogComponent, {
      width: '800px',
      panelClass: 'custom-dialog-container',
      data: row
    });
  }

  // ============================================================
  // CLEAR - REACTIVAR FILTROS
  // ============================================================
  onClear(): void {
    this.invoiceOptions.forEach(x => {
      x.selected = true;
      x.disabled = false;
    });

    this.billingOptions.forEach(x => {
      x.selected = true;
      x.disabled = false;
    });

    this.id = '';
    this.idtype = '';
    this.isAdvancedSearchActive = false;

    // Reset fechas
    const today = new Date();
    const past5 = new Date();
    past5.setDate(today.getDate() - 5);

    this.startDate = past5;
    this.endDate = today;

    localStorage.removeItem(this.FILTER_STORAGE_KEY);
    localStorage.removeItem(this.DATE_STORAGE_KEY);

    this.loadPageData(0, 10);
  }

  // ============================================================
  // BÚSQUEDA AVANZADA
  // ============================================================
  openAdvancedFiltersDialog(): void {
    const dialogRef = this.dialog.open(FilterInvoiceTypeDialogComponent, {
      panelClass: 'advanced-filter-dialog',
      disableClose: true,
      width: 'auto',
      maxWidth: '95vw'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result) {
        return;
      }

      this.idtype = result.type
        .replace('ID Factura SAP', 'invoice')
        .replace('Pedido SAP', 'pedidosap')
        .replace('Pedido shop', 'pedidodxp');

      this.id = result.value;

      this.updateAdvancedSearchState();
      this.loadPageData(0, 10);
    });
  }
}
