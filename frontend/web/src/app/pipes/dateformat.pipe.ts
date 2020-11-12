import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'dateformat'
})
export class DateformatPipe implements PipeTransform {

  transform(value: string): any {
    const datesSections = value.split('-');
    return `${datesSections[0]} - ${datesSections[1]}`;
  }

}
