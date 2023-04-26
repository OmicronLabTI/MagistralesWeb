import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatTooltipModule } from '@angular/material';
import { TableCommonComponent } from './table-common.component';

describe('TableCommonComponent', () => {
  let component: TableCommonComponent;
  let fixture: ComponentFixture<TableCommonComponent>;
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TableCommonComponent],
      imports: [
        MatTooltipModule
      ],
      providers: [],
      schemas: []
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TableCommonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should handle action Cell', () => {
    spyOn(component.handleAction, 'emit');
  });
  it('should valueCell', () => {
    expect(component.valueCell({
      item: '1'
    }, 'item')).toBe('1');
  });
  it('should actionCell', () => {
    expect(component.actionCell({
      item: '1'
    }, 'item').label).toBe('');
  });
});
