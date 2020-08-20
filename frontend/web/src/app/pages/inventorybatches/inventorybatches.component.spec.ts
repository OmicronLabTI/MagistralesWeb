import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InventorybatchesComponent } from './inventorybatches.component';

describe('InventorybatchesComponent', () => {
  let component: InventorybatchesComponent;
  let fixture: ComponentFixture<InventorybatchesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InventorybatchesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InventorybatchesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
