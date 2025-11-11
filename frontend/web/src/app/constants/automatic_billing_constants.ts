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
    error: 'Error al crear'
};
