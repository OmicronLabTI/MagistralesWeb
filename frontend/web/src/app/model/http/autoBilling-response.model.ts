export interface AutoBillingApiResponse {
  response: AutoBillingApiItem[];
  comments: {
    total: number;
  };
}

export interface AutoBillingApiItem {
  id: string;
  idFacturaSap: string;
  invoiceCreateDate: string;
  typeInvoice: string;
  billingType: string;
  almacenUser: string;
  dxpOrderId: string;
  sapOrders: Array<{
    id: number;
    sapOrderId: string;
    idInvoice: string;
  }>;
  remissions: Array<{
    id: number;
    remissionId: string;
    idInvoice: string;
  }>;
  sapOrdersCount: number;
  remissionsCount: number;
  retryNumber: number;
  lastUpdateDate: Date;
  status: string;
}
