import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DetalleFormulaComponent } from './detalle-formula.component';

describe('DetalleFormulaComponent', () => {
  let component: DetalleFormulaComponent;
  let fixture: ComponentFixture<DetalleFormulaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DetalleFormulaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DetalleFormulaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
