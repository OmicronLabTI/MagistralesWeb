import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Component, DebugElement } from '@angular/core';
import { DigitDecimalNumberDirective } from './digit-decima-number.directive';
import { By } from "@angular/platform-browser";
import { FormsModule } from '@angular/forms';

@Component({
    template: `<hr\>`
})
class TestComponent {
}

describe('DigitDecimalNumberDirective', () => {
    let directive: DigitDecimalNumberDirective;
    let fixture: ComponentFixture<TestComponent>;
    let elements: DebugElement[];

    beforeEach(() => 
    {
        fixture = TestBed.configureTestingModule({
            declarations: [TestComponent, DigitDecimalNumberDirective],
            imports: [FormsModule],
            providers: []
        })
        .overrideComponent(TestComponent, {
        set: {
            template: '<input matInput type="number" appDigitDecimalNumber >'
        }})
        .createComponent(TestComponent)

        fixture.detectChanges();
        fixture = TestBed.createComponent(TestComponent);
        elements = fixture.debugElement.queryAll(By.directive(DigitDecimalNumberDirective));
        directive = elements[0].injector.get(DigitDecimalNumberDirective) as DigitDecimalNumberDirective;
    });

    it('should be press valid value.', async () => {
        // arrange
        const event = new KeyboardEvent("keydown", { 'key': '1' });

        // act
        let result = directive.processEvent(event);

        // assert
        expect(result).toEqual('1');
    });

    it('should be on press negative sign with false negative flag.', async () => {
        // arrange
        const event = new KeyboardEvent("keydown", { 'key': '-' });

        // act
        let result = directive.processEvent(event);

        // assert
        expect(directive.negative).toBeFalsy();
        expect(result).toEqual('');
    });

    it('should be on press negative sign with true negative flag.', async () => {
        // arrange
        const event = new KeyboardEvent("keydown", { 'key': '-' });
        directive.negative = true;

        // act
        let result = directive.processEvent(event);

        // assert
        expect(directive.negative).toBeTruthy();
        expect(result).toEqual('-');
    });

    it('should be on press special key.', async () => {
        // arrange
        const event = new KeyboardEvent("keydown", { 'key': 'Tab' });

        // act
        let result = directive.processEvent(event);

        // assert
        expect(result).toEqual('');
    });
});
