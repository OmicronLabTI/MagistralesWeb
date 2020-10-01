import { Directive, ElementRef, HostListener, Input } from '@angular/core';

@Directive({
    selector: '[appDigitDecimalNumber]'
})
export class DigitDecimalNumberDirective {

    // Number of decimal places
    @Input() decimalPlaces: number;

    // Accept negative values
    @Input() negative: boolean = false;

    // Allow decimal numbers and negative values
    private regex: RegExp;

    // Allow key codes for special events. Reflect :
    // Backspace, tab, end, home
    private specialKeys: Array<string> = ['Backspace', 'Tab', 'End', 'Home', '-', 'ArrowLeft', 'ArrowRight', 'Del', 'Delete', 'Alt'];

    constructor(private el: ElementRef) {
    }

    @HostListener('keydown', ['$event'])
    onKeyDown(event: KeyboardEvent) {
        this.buildRegex();

        if (!this.negative) {
            if (event.key == '-') {
                event.preventDefault();
                return;
            }   
        }

        if (this.specialKeys.indexOf(event.key) !== -1) {
            return;
        }

        let current: string = this.el.nativeElement.value;
        const next: string = `${current}${event.key}`;

        if (next && !String(next).match(this.regex)) {
            event.preventDefault();
        }
    }

    buildRegex() {
        if (this.regex){
            return;
        }
        let regexAsString = `^-?\\d*\\.?\\d{0,${this.decimalPlaces}}$`
        this.regex = new RegExp(regexAsString, 'g');
    }
}