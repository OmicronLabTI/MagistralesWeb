import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FilterInvoiceTypeDialogComponent } from './filter-invoice-type-dialog.component';

describe('FilterInvoiceTypeDialogComponent', () => {
  let component: FilterInvoiceTypeDialogComponent;
  let fixture: ComponentFixture<FilterInvoiceTypeDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FilterInvoiceTypeDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FilterInvoiceTypeDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
