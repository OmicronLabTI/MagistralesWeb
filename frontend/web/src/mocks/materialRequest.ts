import { IMaterialHistoryRes, IMaterialRequestRes } from 'src/app/model/http/materialReques';

export const MaterialRequestMock: IMaterialRequestRes = {
    code: 200,
    comments: 118,
    exceptionMessage: null,
    success: true,
    userError: null,
    response: {
        id: 2,
        productionOrderIds: [123, 456],
        failedProductionOrderIds: [123564, 75138],
        // tslint:disable-next-line: max-line-length
        signature: 'iVBORw0KGgoAAAANSUhEUgAAAfQAAAEsCAYAAAA1u0HIAAAAAXNSR0IArs4c6QAAFV1JREFUeF7t3U3IbdddBvCnOBKMadQEhUKSUlRaoQlFRCdpJqJSbYs4KBTSTtphEsSBnSQKBcFKkoGgILYZOLFIW8xAFEnqRBFtUrAOpJrqxJpYEiOlIkJl2fPCdue99573vmf9z15r',
        signingUserId: 'benny benny',
        signingUserName: '35642b3a-9471-4b89-9862-8bee6d98c361',
        observations: 'ASDFSDG',
        orderedProducts: [
            {
                id: 5,
                requestId: 6,
                productId: '1007   240 ML',
                description: 'CREMA FPS 15',
                requestQuantity: '1',
                unit: 'Pieza',
                isWithError: false,
                isChecked: true,
            }
        ],
        creationUserId: '12/12/21',
        creationDate: '12/12/21',
    }
};

export const MaterialRequestHistoryMock: IMaterialHistoryRes = {
    code: 200,
    userError: null,
    exceptionMessage: null,
    success: true,
    response: [
        {
            docEntry: 7750,
            itemCode: '1001   100 SB',
            description: 'Sulfato de Cobre 5 gr, Sobres',
            applicantName: 'benny benny',
            quantity: 1.8989,
            unit: 'Paquete',
            targetWarehosue: 'MG',
            docDate: '14/03/2023',
            status: 'Abierto'
        },
        {
            docEntry: 7750,
            itemCode: '1001   100 SB',
            description: 'Sulfato de Cobre 5 gr, Sobres',
            applicantName: 'benny benny',
            quantity: 1.8989,
            unit: 'Paquete',
            targetWarehosue: 'MG',
            docDate: '14/03/2023',
            status: 'Cerrado'
        },
        {
            docEntry: 7750,
            itemCode: '1001   100 SB',
            description: 'Sulfato de Cobre 5 gr, Sobres',
            applicantName: 'benny benny',
            quantity: 1.8989,
            unit: 'Paquete',
            targetWarehosue: 'MG',
            docDate: '2023-03-14T00:00:00',
            status: 'Cancelado'
        }
    ],
    comments: 477
};
