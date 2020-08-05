import { MatPaginatorIntl } from '@angular/material';

export function CustomPaginator() {
    const customPaginatorIntl = new MatPaginatorIntl();

    customPaginatorIntl.itemsPerPageLabel = 'Registros por página:';
    customPaginatorIntl.firstPageLabel = 'Primera página';
    customPaginatorIntl.lastPageLabel = 'Ultima página';
    customPaginatorIntl.nextPageLabel = 'Siguiente página';
    customPaginatorIntl.previousPageLabel = 'Página anterior';
    return customPaginatorIntl;
}
