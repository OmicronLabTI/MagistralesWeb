import { SettingsCommonTableClass } from "src/app/components/table-common/table-common.component";

export const MaterialRequestHistoryTableSettings: SettingsCommonTableClass = {
    className: 'metrics-promotions-table',
    emptyLabel: "No se Encontraron resultados",
    stickyColumns: [
        // {
        //     className: '',
        //     title: 'Código de cliente',
        //     attr: 'cardCode',
        // },
    ],
    columns: [
        {
            className: ' text-center ',
            title: '#',
            attr: 'order',
        },
        {
            className: ' text-center ',
            title: 'NO.',
            attr: 'docEntry',
        },
        {
            className: ' text-center ',
            title: 'CÓDIGO',
            attr: 'itemCode',
        },
        {
            className: ' text-center ',
            title: 'DESCRIPCIÓN',
            attr: 'description',
        },
        {
            className: ' text-center ',
            title: 'SOLICITANTE',
            attr: 'applicantName',
        },
        {
            className: ' text-center ',
            title: 'CANTIDAD',
            attr: 'quantity',
        },
        {
            className: ' text-center ',
            title: 'UNIDAD',
            attr: 'unit',
        },
        {
            className: ' text-center ',
            title: 'ALMACÉN DESTINO',
            attr: 'targetWarehosue',
        },
        {
            className: ' text-center ',
            title: 'FECHA DE SOLICITUD',
            attr: 'docDate',
        },
        {
            className: ' text-center ',
            title: 'ESTATUS',
            attr: 'status',
        },
    ]
};