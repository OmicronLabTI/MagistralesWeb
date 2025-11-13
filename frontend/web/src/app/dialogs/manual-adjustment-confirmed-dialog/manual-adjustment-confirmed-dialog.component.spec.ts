import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ManualAdjustmentConfirmedDialogComponent } from './manual-adjustment-confirmed-dialog.component';
import { MatButtonModule, MatDialogModule, MatDialogRef } from '@angular/material';

describe('ManualAdjustmentConfirmedDialogComponent', () => {
  let component: ManualAdjustmentConfirmedDialogComponent;
  let fixture: ComponentFixture<ManualAdjustmentConfirmedDialogComponent>;
  const close = () => { };
  const MockDialogRef = { close: jasmine.createSpy('close') };

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ManualAdjustmentConfirmedDialogComponent],
      imports: [MatDialogModule, MatButtonModule],
      providers: [
        {
          provide: MatDialogRef,
          useValue: MockDialogRef
        },
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ManualAdjustmentConfirmedDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
  it('should continue', () => {
    component.confirm();
    expect(MockDialogRef.close).toHaveBeenCalledWith(true);
  });
});
