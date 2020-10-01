import { Directive, ElementRef, HostListener, Input } from '@angular/core';

@Directive({
    selector: '[appDigitDecimalNumber]'
})
export class DigitDecimalNumberDirective {

    // Number of decimal places
    @Input() decimalPlaces: number = 2;

    // Accept negative values
    @Input() negative: boolean = false;

    // Allow decimal numbers and negative values
    private regex: RegExp;

    // Allow key codes for special events. Reflect :
    // Backspace, tab, end, home
    private specialKeys: Array<string> = ['Backspace', 'Tab', 'End', 'Home', 'ArrowLeft', 'ArrowRight', 'Del', 'Delete', 'Alt', '+' ];

    constructor(private el: ElementRef) {
    }

    @HostListener('keydown', ['$event'])
    onKeyDown(event: KeyboardEvent) {
        this.processEvent(event);
    }

    processEvent(event: KeyboardEvent): string {
        this.buildRegex();
        let current: string = this.el.nativeElement.value;

        if (!this.negative) {
            if (event.key == '-') {
                event.preventDefault();
                return current;
            }   
        }

        if (this.specialKeys.indexOf(event.key) !== -1) {
            return current;
        }

        const next: string = `${current}${event.key}`;

        if (next && !String(next).match(this.regex)) {
            event.preventDefault();
            return current;
        }

        return next;
    }

    buildRegex() {
        if (this.regex){
            return;
        }
        let regexAsString = `^-?\\d*\\.?\\d{0,${this.decimalPlaces}}$`
        this.regex = new RegExp(regexAsString, 'g');
    }
}