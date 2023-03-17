import { CONST_STRING } from 'src/app/constants/const';
import { SettingsCommonTableClass } from './common.data';

export const MaterialRequestHistoryTableSettings: SettingsCommonTableClass = {
    className: 'metrics-promotions-table',
    emptyLabel: CONST_STRING.notResults,
    stickyColumns: [],
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
            className: ' badge ',
            type: 'badge',
            title: 'ESTATUS',
            attr: 'status',
        },
    ]
};
