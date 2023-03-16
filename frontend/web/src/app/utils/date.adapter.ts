import { NativeDateAdapter } from '@angular/material/core';

export class AppDateAdapter extends NativeDateAdapter {

    format(date: Date, displayFormat: object): string {
        const d = new Date(date);
        let month = '' + (d.getMonth() + 1);
        let day = '' + d.getDate();
        if (month.length < 2) {
            month = '0' + month;
        }
        if (day.length < 2) {
            day = '0' + day;
        }
        return [day, month, d.getFullYear()].join('/');
    }
}
