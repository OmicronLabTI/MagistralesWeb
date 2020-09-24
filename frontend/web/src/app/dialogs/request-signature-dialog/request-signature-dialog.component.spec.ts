import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { ImageBase64Mock } from 'src/mocks/imageBase64Mock';
import {MatTableModule} from '@angular/material/table';
import {MatCheckboxModule} from '@angular/material/checkbox';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {CUSTOM_ELEMENTS_SCHEMA} from '@angular/core';
import { RequestSignatureDialogComponent } from './request-signature-dialog.component';
import { MatDialogModule, MatDialogRef } from '@angular/material';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

describe('RequestSignatureDialogComponent', () => {
  let component: RequestSignatureDialogComponent;
  let fixture: ComponentFixture<RequestSignatureDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [
        MatTableModule,
        MatDialogModule,
        MatCheckboxModule,
        MatFormFieldModule, MatInputModule,
        BrowserAnimationsModule],
      declarations: [ RequestSignatureDialogComponent ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
      providers: [
        {
          provide: MatDialogRef,
          useValue: {}
        }
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RequestSignatureDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('Validate signature with draw signature', () => {
    component.signaturePad.fromDataURL(ImageBase64Mock.src);
    component.validateSignature();
    expect(component.isValidSignature).toBeTruthy();

  });

  it('Validate signature with empty signature', () => {
    component.validateSignature();
    expect(component.isValidSignature).toBeFalsy();
  });

  it('Reset canvas correctry', () => {
    component.signaturePad.fromDataURL(ImageBase64Mock.src);
    component.reset();
    expect(component.isValidSignature).toBeFalsy();
  });

  it('Get signature value with out base 64 prefix', () => {
    component.signaturePad.fromDataURL(ImageBase64Mock.src);
    expect(component.getSignatureValue()).toEqual(ImageBase64Mock.srcWithoutPrefix);
  });
});
