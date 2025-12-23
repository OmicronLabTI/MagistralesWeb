export const automaticBillingTableColumns: string[] = [
    'id',
    'requestDate',
    'status',
    'invoiceType',
    'invoiceForm',
    'almacenUser',
    'orderShop',
    'orderSAP',
    'remissions',
    'updateDate',
    'errorMessage',
    'retries',
    'manualAdjustment',
    'actions',
];

export const automaticBillingStatusConst = {
    sent: 'Enviada a crear',
    creating: 'Creando factura',
    error: 'Error al crear',
    created: 'Creación exitosa'
};

export const automaticBillingInvoiceTypeConst = {
    generic: 'Genérica',
    non_generic: 'No genérica'
};

export const automaticBillingBillingTypeConst = {
    parcial: 'Parcial',
    completa: 'Completa'
};

export const advanceFiltersTypes: Record<string, string> = {
    invoice: 'ID Factura SAP',
    pedidosap: 'Pedido SAP',
    pedidodxp: 'Pedido shop'
};
