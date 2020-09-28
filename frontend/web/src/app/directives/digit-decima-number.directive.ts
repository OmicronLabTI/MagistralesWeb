import { Directive, ElementRef, HostListener, Input } from '@angular/core';

@Directive({
    selector: '[appDigitDecimaNumber]'
})
export class DigitDecimaNumberDirective {

    // Number of decimal places
    @Input() decimalPlaces: number;
    
    // Allow decimal numbers and negative values
    private regex: RegExp;

    // Allow key codes for special events. Reflect :
    // Backspace, tab, end, home
    private specialKeys: Array<string> = ['Backspace', 'Tab', 'End', 'Home', '-', 'ArrowLeft', 'ArrowRight', 'Del', 'Delete'];

    constructor(private el: ElementRef) {
    }

    @HostListener('keydown', ['$event'])
    onKeyDown(event: KeyboardEvent) {
        this.buildRegex();

        if (this.specialKeys.indexOf(event.key) !== -1) {
            return;
        }

        let current: string = this.el.nativeElement.value;
        const position = this.el.nativeElement.selectionStart;
        const next: string = [current.slice(0, position), event.key == 'Decimal' ? '.' : event.key, current.slice(position)].join('');
        
        if (next && !String(next).match(this.regex)) {
            event.preventDefault();
        }
    }

    buildRegex() {
        if (this.regex){
            return;
        }
        let regexAsString = `^\\d*\\.?\\d{0,${this.decimalPlaces}}$`
        this.regex = new RegExp(regexAsString, 'g');
    }
}