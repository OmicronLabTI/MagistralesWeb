import { Directive, ElementRef, HostListener, Input } from '@angular/core';

@Directive({
  selector: '[appLimitEntires]'
})
export class LimitEntiresDirective {
  @Input() entiresPlaces = 10;

  private specialKeys: Array<string> = ['Backspace', 'Tab', 'End', 'Home', 'ArrowLeft', 'ArrowRight', 'Del', 'Delete', 'Alt', '+', '.'];
  constructor(private el: ElementRef) { }

  @HostListener('keydown', ['$event'])
  onKeyDown(event: KeyboardEvent) {
    this.processEvent(event);
  }

  processEvent(event: KeyboardEvent): string {
    const current: string = this.el.nativeElement.value;
    const entires = current.split('.');
    if (this.specialKeys.indexOf(event.key) !== -1 || entires[1] !== undefined) {
      return current;
    }
    if (entires[0].length > this.entiresPlaces) {
      event.preventDefault();
      return current;
    }
    return `${current}${event.key}`;
  }

}
