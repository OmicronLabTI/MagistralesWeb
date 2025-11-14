/**
 * Mock data for AutoBilling dashboard (OM-7160).
 *
 * This file provides realistic, structured mock data representing automatic
 * billing history records as returned by the backend.
 *
 * Each record includes:
 *  - SAP invoice ID and creation date (dd/MM/yy HH:mm:ss)
 *  - Invoice type and billing mode
 *  - Warehouse user responsible for triggering the invoice
 *  - Related SAP orders and remissions
 *  - Retry count and transaction identifiers
 *
 * These examples simulate successful invoices (“Creación exitosa”) generated
 * automatically from warehouse operations for both Shop and SAP orders.
 */

import { AutoBillingModel } from '../app/model/http/autoBilling.model';

/**
 * Returns the current date formatted as dd/MM/yy HH:mm:ss.
 */
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

// Simulated current time for record generation
const now = new Date();

/**
 * List of mock automatic billing records.
 *
 * Each record reflects the data contract defined in AutoBillingModel.
 */
export const autoBillingMock: AutoBillingModel[] = [
  {
    /** Unique internal invoice ID */
    requestId: 'INV-1001',

    /** SAP-assigned invoice ID */
    sapInvoiceId: '1001',

    /** Date and time when the invoice was created in SAP */
    sapCreationDate: formatDate(now),

    /** Type of invoice: Genérica (with general RFC) or No genérica (with fiscal data) */
    invoiceType: 'Genérica',

    /** Billing mode: Completa or Parcial */
    billingMode: 'Completa',

    /** Warehouse user who triggered automatic billing */
    originUser: 'klopezAlm',

    /** Shop order identifier, if applicable */
    shopOrder: 'PED-00123',

    /** Number of related SAP orders */
    sapOrder: 2,

    /** Number of related remissions */
    shipments: 1,

    /** Retry count (0 = created successfully on first attempt) */
    retries: 0,

    /** Related SAP orders */
    sapOrders: [
      { id: 1, idpedidosap: '700001', idinvoice: 'INV-1001' },
      { id: 2, idpedidosap: '700002', idinvoice: 'INV-1001' }
    ],

    /** Related remissions */
    remissions: [
      { id: 1, idremission: '500001', idinvoice: 'INV-1001' }
    ]
  },

  {
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
