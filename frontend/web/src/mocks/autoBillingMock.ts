import { AutoBillingModel } from '../app/model/http/autoBilling.model';

const formatDate = (date: Date): string => {
  const two = (v: number) => (v < 10 ? '0' + v : v);
  return (
    two(date.getDate()) + '/' +
    two(date.getMonth() + 1) + '/' +
    String(date.getFullYear()).slice(-2) + ' ' +
    two(date.getHours()) + ':' +
    two(date.getMinutes()) + ':' +
    two(date.getSeconds())
  );
};

const now = new Date();

export const autoBillingMock: AutoBillingModel[] = [
  {
    id: '1',
    lastUpdateDate: now,
    status: 'created',
    requestId: 'INV-1001',
    sapInvoiceId: '1001',
    sapCreationDate: formatDate(now),
    invoiceType: 'Genérica',
    billingMode: 'Completa',
    originUser: 'klopezAlm',
    shopOrder: 'PED-00123',
    sapOrder: 2,
    shipments: 1,
    retries: 0,
    sapOrders: [
      { id: 1, idpedidosap: '700001', idinvoice: 'INV-1001' },
      { id: 2, idpedidosap: '700002', idinvoice: 'INV-1001' }
    ],
    remissions: [
      { id: 1, idremission: '500001', idinvoice: 'INV-1001' }
    ]
  },
  {
    id: '2',
    lastUpdateDate: now,
    status: 'created',
    requestId: 'INV-1002',
    sapInvoiceId: '1002',
    sapCreationDate: formatDate(now),
    invoiceType: 'No genérica',
    billingMode: 'Parcial',
    originUser: 'aespinosaAlm',
    shopOrder: 'PED-00987',
    sapOrder: 1,
    shipments: 3,
    retries: 1,
    sapOrders: [
      { id: 3, idpedidosap: '700003', idinvoice: 'INV-1002' }
    ],
    remissions: [
      { id: 4, idremission: '500010', idinvoice: 'INV-1002' },
      { id: 5, idremission: '500011', idinvoice: 'INV-1002' },
      { id: 6, idremission: '500012', idinvoice: 'INV-1002' }
    ]
  },
  {
    id: '3',
    lastUpdateDate: now,
    status: 'created',
    requestId: 'INV-1003',
    sapInvoiceId: '1003',
    sapCreationDate: formatDate(now),
    invoiceType: 'Genérica',
    billingMode: 'Completa',
    originUser: 'mhernandezAlm',
    shopOrder: 'PED-04567',
    sapOrder: 3,
    shipments: 2,
    retries: 0,
    sapOrders: [
      { id: 7, idpedidosap: '700004', idinvoice: 'INV-1003' },
      { id: 8, idpedidosap: '700005', idinvoice: 'INV-1003' },
      { id: 9, idpedidosap: '700006', idinvoice: 'INV-1003' }
    ],
    remissions: [
      { id: 10, idremission: '500020', idinvoice: 'INV-1003' },
      { id: 11, idremission: '500021', idinvoice: 'INV-1003' }
    ]
  },
  {
    id: '4',
    lastUpdateDate: now,
    status: 'created',
    requestId: 'INV-1004',
    sapInvoiceId: '1004',
    sapCreationDate: formatDate(now),
    invoiceType: 'No genérica',
    billingMode: 'Parcial',
    originUser: 'cperezAlm',
    shopOrder: '',
    sapOrder: 1,
    shipments: 1,
    retries: 2,
    sapOrders: [
      { id: 10, idpedidosap: '700007', idinvoice: 'INV-1004' }
    ],
    remissions: [
      { id: 12, idremission: '500030', idinvoice: 'INV-1004' }
    ]
  }
];
