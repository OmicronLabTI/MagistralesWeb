export const IncidentsGraphMock = {
    code: 200,
    comments: 118,
    exceptionMessage: null,
    success: true,
    userError: null,
    response: [
        [
            {
                fieldKey: 'Producto derramado',
                totalCount: 1,
                graphType: 'IncidentReason'
            },
            {
                fieldKey: 'Nombre incorrecto',
                totalCount: 1,
                graphType: 'IncidentReason'
            },
            {
                fieldKey: 'Pedido incompleto',
                totalCount: 1,
                graphType: 'IncidentReason'
            },
            {
                fieldKey: 'Ingredientes incorrectos',
                totalCount: 1,
                graphType: 'IncidentReason'
            },
            {
                fieldKey: 'Envase incorrecto',
                totalCount: 0,
                graphType: 'IncidentReason'
            },
            {
                fieldKey: 'Producto dañado',
                totalCount: 0,
                graphType: 'IncidentReason'
            }
        ],
        [
            {
                fieldKey: 'Abierta',
                totalCount: 1,
                graphType: 'status'
            },
            {
                fieldKey: 'Atendiendo',
                totalCount: 1,
                graphType: 'status'
            },
            {
                fieldKey: 'Cerrada',
                totalCount: 0,
                graphType: 'status'
            }
        ]
    ],
};
