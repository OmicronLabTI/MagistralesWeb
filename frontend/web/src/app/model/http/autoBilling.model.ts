export interface SapOrderModel {
  id: number;
  idpedidosap: string;
  idinvoice: string;
}

export interface RemissionModel {
  id: number;
  idremission: string;
  idinvoice: string;
}

export interface AutoBillingModel {
  id: string;
  requestId: string;
  sapInvoiceId: string;
  sapCreationDate: string;
  invoiceType: string;
  billingMode: string;
  originUser: string;
  shopOrder: string;
  sapOrder: number;
  shipments: number;
  retries: number;
  sapOrders: SapOrderModel[];
  remissions: RemissionModel[];
  lastUpdateDate: Date;
  status: string;
}
