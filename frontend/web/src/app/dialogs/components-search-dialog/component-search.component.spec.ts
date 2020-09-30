import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import {MAT_DIALOG_DATA, MatDialogModule, MatDialogRef} from '@angular/material/dialog';
import { ComponentSearchComponent } from './component-search.component';
import {CUSTOM_ELEMENTS_SCHEMA} from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {MatChipsModule} from '@angular/material/chips';
import {
  MatTableModule,
  MatCheckboxModule,
  MatFormFieldModule,
  MatInputModule,
} from '@angular/material';
import {DatePipe} from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import {PedidosService} from '../../services/pedidos.service';
import {of} from 'rxjs';
import {ComponentSearchMock} from '../../../mocks/componentsMock';

describe('ComponentSearchComponent', () => {
  let component: ComponentSearchComponent;
  let fixture: ComponentFixture<ComponentSearchComponent>;
  let ordersServiceSpy;
  beforeEach(async(() => {
    ordersServiceSpy = jasmine.createSpyObj<PedidosService>('PedidosService', [
      'getComponents'
    ]);
    ordersServiceSpy.getComponents.and.callFake(() => {
      return of(ComponentSearchMock);
    });
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, MatTableModule,
        MatDialogModule,
        MatCheckboxModule, MatFormFieldModule, MatInputModule, BrowserAnimationsModule, MatChipsModule],
      declarations: [ ComponentSearchComponent ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA],
      providers: [DatePipe,
        {
          provide: MatDialogRef,
          useValue: {}
        },
        { provide: MAT_DIALOG_DATA, useValue: {modalType: 'searchComponent',
                                                chips: ['crema']} }]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ComponentSearchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
    expect(component.isFromSearchComponent).toBeTruthy();
  });
});
