//import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/compiler';
import { ComponentFixture, TestBed } from '@angular/core/testing';
//import { CallBackEmptyConst } from 'src/app/constants/const';
//import { CommonService } from 'src/app/services/common.service';
//import { MockMatSpinnerComponent } from 'src/mocks/componentsHtmlMock';*/

import { TableCommonComponent } from './table-common.component';

describe('TableCommonComponent', () => {
  let component: TableCommonComponent;
  let fixture: ComponentFixture<TableCommonComponent>;
  beforeEach(async () => {
   
    await TestBed.configureTestingModule({
      declarations: [TableCommonComponent],
      providers: [
        
      ],
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
});
