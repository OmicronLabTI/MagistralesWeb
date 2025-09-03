import { IComponentLotes, IComponentsLotesRes } from 'src/app/model/http/addComponent';

export const ComponentSearchMock = {
    code: 200,
    userError: null,
    exceptionMessage: null,
    success: true,
    comments: 5086,
    response: [
        {
            orderFabId: 0,
            productId: '1007   120 ML',
            description: 'Crema FPS 15',
            baseQuantity: 0,
            requiredQuantity: 0,
            consumed: 0,
            available: -80.699991,
            unit: 'Pieza',
            warehouse: 'MN',
            pendingQuantity: 0,
            stock: 0.000000,
            warehouseQuantity: 0.000000,
            hasBatches: false,
            availableWarehouses: []
        },
        {
            orderFabId: 0,
            productId: '1008   120 ML',
            description: 'Crema FPS 15',
            baseQuantity: 0,
            requiredQuantity: 0,
            consumed: 0,
            available: -80.699991,
            unit: 'Pieza',
            warehouse: 'MN',
            pendingQuantity: 0,
            stock: 0.000000,
            warehouseQuantity: 0.000000,
            hasBatches: false,
            availableWarehouses: []
        },
        {
            orderFabId: 0,
            productId: '1009   120 ML',
            description: 'Crema FPS 15',
            baseQuantity: 0,
            requiredQuantity: 0,
            consumed: 0,
            available: -80.699991,
            unit: 'Pieza',
            warehouse: 'MN',
            pendingQuantity: 0,
            stock: 0.000000,
            warehouseQuantity: 0.000000,
            hasBatches: false,
            availableWarehouses: []
        }
    ]
};

export const searchComponentsLotesMock: IComponentsLotesRes = {
    code: 200,
    userError: null,
    exceptionMessage: null,
    success: true,
    response:
        {
            codigoProducto: 'MP-109',
            almacen: 'PROD',
            lotes: [
                {
                    numeroLote: '88-22',
                    cantidadDisponible: 1.405394,
                    cantidadAsignada: 2.673856,
                    sysNumber: 158,
                    fechaExp: '29/03/2023',
                    fechaExpDateTime: new Date(),
                    itemCode: 'MP-109',
                    quantity: 4.07925
                },
                {
                    numeroLote: 'Axity-01',
                    cantidadDisponible: 97.33999,
                    cantidadAsignada: 1.74501,
                    sysNumber: 160,
                    fechaExp: null,
                    fechaExpDateTime: null,
                    itemCode: 'MP-109',
                    quantity: 99.085
                }
            ]
        },
    comments: null
};

export const componentLoteMock: IComponentLotes = {
    codigoProducto: 'MP-109',
    almacen: 'PROD',
    lotes: [
        {
            numeroLote: '88-22',
            cantidadDisponible: 1.405394,
            cantidadAsignada: 2.673856,
            sysNumber: 158,
            fechaExp: '29/03/2023',
            fechaExpDateTime: new Date(),
            itemCode: 'MP-109',
            quantity: 4.07925
        },
        {
            numeroLote: 'Axity-01',
            cantidadDisponible: 97.33999,
            cantidadAsignada: 1.74501,
            sysNumber: 160,
            fechaExp: null,
            fechaExpDateTime: null,
            itemCode: 'MP-109',
            quantity: 99.085
        }
    ]
};
