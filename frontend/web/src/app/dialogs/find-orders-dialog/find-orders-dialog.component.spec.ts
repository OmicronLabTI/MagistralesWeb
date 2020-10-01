import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FindOrdersDialogComponent } from './find-orders-dialog.component';
import { MATERIAL_COMPONENTS } from 'src/app/app.material';
import {  ReactiveFormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import {ConstOrders} from '../../constants/const';
import {UsersService} from '../../services/users.service';
import {PedidosService} from '../../services/pedidos.service';
import {of} from 'rxjs';
import {UserListMock} from '../../../mocks/userListMock';
import {RolesMock} from '../../../mocks/rolesMock';
import {QfbWithNumberMock} from '../../../mocks/qfbWithNumberMock';

describe('FindOrdersDialogComponent', () => {
  let component: FindOrdersDialogComponent;
  let fixture: ComponentFixture<FindOrdersDialogComponent>;
  let userServiceSpy;
  let ordersServiceSpy;
  beforeEach(async(() => {
    userServiceSpy = jasmine.createSpyObj<UsersService>('UsersService', [
      'getUsers', 'getRoles'
    ]);
    userServiceSpy.getRoles.and.callFake(() => {
      return of(RolesMock);
    });
    userServiceSpy.getUsers.and.callFake(() => {
      return of(UserListMock);
    });
    ordersServiceSpy = jasmine.createSpyObj<PedidosService>('PedidosService', [
      'getQfbs'
    ]);
    ordersServiceSpy.getQfbs.and.callFake(() => {
      return of(QfbWithNumberMock);
    });

    TestBed.configureTestingModule({
      imports: [
        BrowserAnimationsModule,
        HttpClientTestingModule,
        MATERIAL_COMPONENTS,
        ReactiveFormsModule
      ],
      declarations: [ FindOrdersDialogComponent ],
      providers: [
        DatePipe,
        { provide: MatDialogRef, useValue: {} },
        { provide: MAT_DIALOG_DATA, useValue: {
            modalType: ConstOrders.modalOrders, filterOrdersData: {dateFull: '01/01/2020-02/02/2020'}} },
        /*{ provide: PedidosService, useValue: ordersServiceSpy },
        { provide: UsersService, useValue: userServiceSpy },*/
          DatePipe
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FindOrdersDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
   //  expect(component.isFromSearchOrders).toBeTruthy();
    expect(component.fullDate.length).toEqual(2);
    expect(component.findOrdersForm).toBeDefined();
    expect(component.isToResetData).toBeTruthy();
  });
  it('should call ngOnInit()', () => {
     component.ngOnInit();
     expect(component.findOrdersForm.get('docNum').value).toEqual('');
     expect(component.findOrdersForm.get('fini').value).toBeDefined();
     expect(component.findOrdersForm.get('ffin').value).toBeDefined();
     expect(component.findOrdersForm.get('dateType').value).toEqual('');
     expect(component.findOrdersForm.get('status').value).toEqual('');
     expect(component.findOrdersForm.get('productCode').value).toEqual('');
     expect(component.findOrdersForm.get('clientName').value).toEqual('');


  });
});
