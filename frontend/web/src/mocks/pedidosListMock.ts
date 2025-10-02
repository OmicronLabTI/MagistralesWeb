import { ChildrenOrders } from 'src/app/model/http/detallepedidos.model';
import { ChildrenOrdersFabOrderList, IChildrenOrdersListRes } from 'src/app/model/http/ordenfabricacion';

export const DetalleFormulaMock = {
    code: 200,
    comments: 118,
    exceptionMessage: null,
    success: true,
    userError: null,
    response: {
        productionOrderId: 89098,
        code: '150   240 ML',
        productDescription: 'Hialuronico 3%  Vit C 4%  crema',
        type: 'Estandar',
        status: 'Cancelado',
        plannedQuantity: 1,
        unit: 'Pieza',
        warehouse: 'PT',
        number: 60024,
        fabDate: '18/08/2020',
        dueDate: '31/08/2020',
        startDate: '18/08/2020',
        endDate: '18/08/2020',
        user: 'manager',
        origin: 'Manual',
        baseDocument: 60024,
        client: 'C00141',
        completeQuantity: 0,
        realEndDate: '',
        productLabel: 'PERSONALIZADA',
        container: 'AMBAR',
        isChecked: false,
        comments: '',
        hasBatches: false,
        // tslint:disable-next-line: max-line-length
        destinyAddress: 'HEB ESTANZUELA CARR. NACIONAL KM 271  \rLOMAS DEL VALLE ALTO, MONTERREY ,MONTERREY\rNuevo León, Mexico , C.P. 64988',
        details: [
            {
                isChecked: false,
                orderFabId: 89098,
                productId: 'EN-075',
                description: 'Pomadera 8 Oz c/ Tapa  R-89 Bonita',
                baseQuantity: 210.000000,
                requiredQuantity: 210.000000,
                consumed: 0.000000,
                available: 0.000000,
                unit: 'Pieza',
                warehouse: 'PROD',
                pendingQuantity: 210.000000,
                stock: 1606.000000,
                warehouseQuantity: 0.000000,
                hasBatches: false
            }
        ]
    }
};

export const PedidosListMock = {
    code: 200,
    comments: 118,
    exceptionMessage: null,
    success: true,
    userError: null,
    response: [
        {
            docNum: 60021,
            docNumDxp: 'OM120I',
            cliente: 'VENTA AL PUBLICO',
            codigo: 'C00100',
            medico: 'VENTA AL PUBLICO',
            asesorName: 'OBIOTIX ONLINE',
            fechaInicio: '30/07/2020',
            fechaFin: '09/08/2020',
            pedidoStatus: 'Abierto',
            qfb: 'Armando Hoyos',
            isChecked: false,
            labelType: 'G',
            finishedLabel: 0,
            orderType: 'MN',
            clientType: 'general',
            onSplitProcess: false,
        },
        {
            docNum: 60022,
            docNumDxp: 'OM120I',
            cliente: 'ABELARDO LEAL DUMONT',
            codigo: 'C00826',
            medico: 'ABELARDO LEAL DUMONT',
            asesorName: 'CC1 - SELENE ESCAMILLA',
            fechaInicio: '13/08/2020',
            fechaFin: '23/08/2020',
            pedidoStatus: 'Planificado',
            qfb: 'Armando Hoyos',
            isChecked: false,
            labelType: 'P',
            finishedLabel: 0,
            orderType: 'MN',
            clientType: 'general',
            onSplitProcess: false,
        },
        {
            docNum: 60023,
            docNumDxp: 'OM120I',
            cliente: 'ABELARDO LEAL DUMONT',
            codigo: 'C00826',
            medico: 'ABELARDO LEAL DUMONT',
            asesorName: 'CC1 - SELENE ESCAMILLA',
            fechaInicio: '13/08/2020',
            fechaFin: '23/08/2020',
            pedidoStatus: 'Liberado',
            qfb: 'Armando Hoyos',
            isChecked: false,
            labelType: 'M',
            finishedLabel: 0,
            orderType: 'MG',
            clientType: 'general',
            onSplitProcess: false,
        },
        {
            docNum: 60024,
            docNumDxp: 'OM120I',
            cliente: 'ABELARDO LEAL DUMONT',
            codigo: 'C00826',
            medico: 'ABELARDO LEAL DUMONT',
            asesorName: 'CC1 - SELENE ESCAMILLA',
            fechaInicio: '13/08/2020',
            fechaFin: '23/08/2020',
            pedidoStatus: 'Cancelado',
            qfb: 'Armando Hoyos',
            isChecked: false,
            labelType: 'G',
            finishedLabel: 1,
            orderType: 'BE',
            clientType: 'general',
            onSplitProcess: false,
        },
        {
            docNum: 60025,
            docNumDxp: 'OM120I',
            cliente: 'ABELARDO LEAL DUMONT',
            codigo: 'C00826',
            medico: 'ABELARDO LEAL DUMONT',
            asesorName: 'CC1 - SELENE ESCAMILLA',
            fechaInicio: '13/08/2020',
            fechaFin: '23/08/2020',
            pedidoStatus: 'En proceso',
            qfb: 'Armando Hoyos',
            isChecked: false,
            labelType: 'P',
            finishedLabel: 1,
            orderType: 'MX',
            clientType: 'general',
            onSplitProcess: false,
        }
    ]
};

export const productWarehousesResponseMock = {
    code: 200,
    comments: 118,
    exceptionMessage: null,
    success: true,
    userError: null,
    response: ['MN']
};

export const childrenOrdersMock: ChildrenOrdersFabOrderList[] = [
    {
        docNum: '176693',
        fabOrderId: 227323,
        quantity: 3,
        createDate: '07/09/2025 23:25:31',
        userCreate: 'Miranda Garfias',
        finishDate: '08/09/2025',
        qfb: 'Miranda Garfias',
        status: 'Terminado',
        batch: null,
        isChecked: false
    },
    {
        docNum: '176693',
        fabOrderId: 227324,
        quantity: 3,
        createDate: '08/09/2025 00:07:23',
        userCreate: 'Miranda Garfias',
        finishDate: '08/09/2025',
        qfb: 'Miranda Garfias',
        status: 'Terminado',
        batch: null,
        isChecked: false
    },
    {
        docNum: '176693',
        fabOrderId: 227322,
        quantity: 3,
        createDate: '07/09/2025 23:24:58',
        userCreate: 'Miranda Garfias',
        finishDate: '08/09/2025',
        qfb: 'Miranda Garfias',
        status: 'Terminado',
        batch: '',
        isChecked: false
    }
];

export const childrenOrdersDetailMock: ChildrenOrders[] = [
    {
        ordenFabricacionId: 227308,
        codigoProducto: 'BQ 250',
        // tslint:disable-next-line:max-line-length
        descripcionProducto: 'Biest (66.6/33.3) 3 mg (2 mg estriol + 1 mg estradiol)/g, DHEA 10 mg/g, Testosterona 4 mg/g, Crema base csp 30 g',
        assignedPieces: 1,
        fechaOf: '05/09/2025',
        fechaOfFin: '05/09/2025',
        qfb: 'josue castillo',
        status: 'Abierto',
        userCreate: 'josue castillo',
        createDate: '05/09/2025 17:10:26',
        label: 'Genérica',
        realLabel: 'NA',
        finishedLabel: 0,
        isChecked: true
    },
    {
        ordenFabricacionId: 227309,
        codigoProducto: 'BQ 250',
        // tslint:disable-next-line:max-line-length
        descripcionProducto: 'Biest (66.6/33.3) 3 mg (2 mg estriol + 1 mg estradiol)/g, DHEA 10 mg/g, Testosterona 4 mg/g, Crema base csp 30 g',
        assignedPieces: 1,
        fechaOf: '05/09/2025',
        fechaOfFin: '05/09/2025',
        qfb: 'josue castillo',
        status: 'Terminado',
        userCreate: 'josue castillo',
        createDate: '05/09/2025 17:12:24',
        label: 'Genérica',
        realLabel: 'NA',
        finishedLabel: 0,
        isChecked: true
    },
    {
        ordenFabricacionId: 227309,
        codigoProducto: 'BQ 250',
        // tslint:disable-next-line:max-line-length
        descripcionProducto: 'Biest (66.6/33.3) 3 mg (2 mg estriol + 1 mg estradiol)/g, DHEA 10 mg/g, Testosterona 4 mg/g, Crema base csp 30 g',
        assignedPieces: 1,
        fechaOf: '05/09/2025',
        fechaOfFin: '05/09/2025',
        qfb: 'josue castillo',
        status: 'Planificado',
        userCreate: 'josue castillo',
        createDate: '05/09/2025 17:12:24',
        label: 'Genérica',
        realLabel: 'NA',
        finishedLabel: 1,
        isChecked: true
    }
];

export const getChildrenResponseMock: IChildrenOrdersListRes = {
    response: childrenOrdersMock
};
