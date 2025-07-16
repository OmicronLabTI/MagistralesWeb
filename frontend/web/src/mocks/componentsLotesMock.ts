import { IAddComponentsAndLotesTable, IComponentLotes } from 'src/app/model/http/addComponent';
import { IFormulaDetalleReq, IFormulaReq } from 'src/app/model/http/detalleformula';
import { ILotesAsignadosReq, ILotesFormulaReq, ILotesReq } from 'src/app/model/http/lotesformula';

export const mockIFormulaReqResponse: IFormulaReq = {
    details: [{
        isChecked: true,
        orderFabId: 12345,
        productId: 'PROD001',
        description: 'Producto de prueba',
        productoName: 'Nombre del producto',
        baseQuantity: 100,
        requiredQuantity: 120,
        consumed: 80,
        available: 150,
        unit: 'kg',
        warehouse: 'WH01',
        pendingQuantity: 40,
        stock: 100,
        warehouseQuantity: 100,
        hasBatches: true,
        isInDb: false,
        isItemSelected: true,
        productoId: 'PROD001',
        isContainer: false,
        isLabel: false,
        assignedBatches: [
            {
                assignedQty: 60,
                batchNumber: 'BATCH001',
                areBatchesComplete: 1,
                sysNumber: 101
            },
            {
                assignedQty: 20,
                batchNumber: 'BATCH002',
                areBatchesComplete: 0,
                sysNumber: 102
            }
        ]
    }],
    dueDate: '03/06/2025',
    plannedQuantity: 100,
    warehouse: 'WH01',
    comments: 'Comentario de prueba',
    catalogGroupName: 'Grupo A',
    fabDate: '',
    productionOrderId: '',
    code: '',
    type: '',
    status: '',
    unit: '',
    number: 0,
    startDate: '',
    endDate: '',
    user: '',
    origin: '',
    baseDocument: 0,
    client: '',
    completeQuantity: 0,
    realEndDate: '',
    hasMissingStock: false
};

export const mockIFormulaDetalleReq: IFormulaDetalleReq[] = [
    {
        isChecked: true,
        orderFabId: 12345,
        productId: 'PROD001',
        description: 'Producto de prueba',
        productoName: 'Nombre del producto',
        baseQuantity: 100,
        requiredQuantity: 120,
        consumed: 80,
        available: 150,
        unit: 'kg',
        warehouse: 'WH01',
        pendingQuantity: 40,
        stock: 100,
        warehouseQuantity: 100,
        hasBatches: true,
        isInDb: false,
        isItemSelected: true,
        productoId: 'PROD001',
        isContainer: false,
        isLabel: false,
        action: 'insert',
        assignedBatches: [
            {
                assignedQty: 60,
                batchNumber: 'BATCH001',
                areBatchesComplete: 1,
                sysNumber: 101
            },
            {
                assignedQty: 20,
                batchNumber: 'BATCH002',
                areBatchesComplete: 0,
                sysNumber: 102
            }
        ],
        availableWarehouses: ['MG']
    }
];

export const lotesComponentMock: IComponentLotes = {
    codigoProducto: 'MP-109',
    almacen: 'PROD',
    isItemSelected: true,
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

export const ILotesFormulaReqMock: ILotesFormulaReq[] = [
    {
        descripcionProducto: 'Aceite Mineral',
        totalNecesario: 0.0002,
        totalSeleccionado: 0.0002,
        lotesAsignados: [
            {
                numeroLote: '1524/250785',
                cantidadSeleccionada: 0.0002,
                sysNumber: 10,
            }
        ],
        codigoProducto: 'MP-001',
        almacen: 'MG',
        lotes: [
            {
                numeroLote: 'PT-2-21145',
                cantidadDisponible: 0.460699,
                cantidadAsignada: 1.0544,
                sysNumber: 9,
                fechaExp: new Date(),
            },
            {
                numeroLote: '1524/250785',
                cantidadDisponible: 0.1107,
                cantidadAsignada: 0.0004,
                sysNumber: 10,
                fechaExp: null,
            }
        ]
    }
];

export const dataSourceComponentsMock: IAddComponentsAndLotesTable[] = [
    {
        codigoProducto: 'MP-001',
        description: 'Aceite Mineral',
        baseQuantity: 0.0004,
        requiredQuantity: 0.0004,
        consumed: 0,
        available: 123828628.626659,
        unit: 'KG',
        warehouse: 'MG',
        pendingQuantity: 0.0004,
        stock: 53.899619,
        warehouseQuantity: '1.626199',
        totalNecesario: 4.0002,
        totalSeleccionado: 1.0002,
        lotes: [
            {
                numeroLote: 'PT-2-21145',
                cantidadDisponible: 0.460699,
                cantidadAsignada: 1.0544,
                sysNumber: 9,
                fechaExp: new Date(),
                cantidadSeleccionada: 0.0002,
                isValid: false
            },
            {
                numeroLote: '1524/250785',
                cantidadDisponible: 0.1107,
                cantidadAsignada: 0.0004,
                sysNumber: 10,
                fechaExp: null,
                cantidadSeleccionada: 0.0002,
                isValid: true
            }
        ],
        lotesAsignados: [
            {
                numeroLote: '1524/250785',
                cantidadSeleccionada: 0.0002,
                sysNumber: 10,
                isValid: true
            }
        ],
        lotesSeleccionados: [
            {
                numeroLote: '1524/250785',
                cantidadSeleccionada: 0.0002,
                sysNumber: 10,
                isValid: true
            }
        ],
        selected: true,
        availableWarehouses: ['MG']
    },
    {
        codigoProducto: 'MP-002',
        description: 'Acetona',
        baseQuantity: 3,
        requiredQuantity: 3,
        consumed: 0,
        available: -123456583.049202,
        unit: 'Litro',
        warehouse: 'MG',
        pendingQuantity: 3,
        stock: 9.409586,
        warehouseQuantity: '1.409586',
        totalNecesario: 1.923747,
        totalSeleccionado: 1.076253,
        lotes: [
            {
                numeroLote: '46231',
                cantidadDisponible: 0,
                cantidadAsignada: 0.409586,
                sysNumber: 8,
                fechaExp: new Date(),
            },
            {
                numeroLote: 'Axity-01',
                cantidadDisponible: 0,
                cantidadAsignada: 1,
                sysNumber: 10,
                fechaExp: null,
            }
        ],
        lotesAsignados: [
            {
                numeroLote: '46231',
                cantidadSeleccionada: 0.409586,
                sysNumber: 8,
            },
            {
                numeroLote: 'Axity-01',
                cantidadSeleccionada: 0.666667,
                sysNumber: 10,
            }
        ],
        lotesSeleccionados: [
            {
                numeroLote: '46231',
                cantidadSeleccionada: 0.409586,
                sysNumber: 8,
            },
            {
                numeroLote: 'Axity-01',
                cantidadSeleccionada: 0.666667,
                sysNumber: 10,
            }
        ],
        availableWarehouses: ['MG']
    }
];

export const lotesAsignadosReqMock: ILotesAsignadosReq = {
    numeroLote: 'PT-2-21145',
    sysNumber: 9,
    cantidadSeleccionada: 0.0002,
    isValid: false
};

// export const dataSourceLotesMock: ILotesReq = {

// }
