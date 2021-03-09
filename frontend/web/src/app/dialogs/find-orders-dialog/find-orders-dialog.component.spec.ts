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
import {of, throwError} from 'rxjs';
import {UserListMock} from '../../../mocks/userListMock';
import {RolesMock} from '../../../mocks/rolesMock';
import {QfbWithNumberMock} from '../../../mocks/qfbWithNumberMock';
import {RouterTestingModule} from '@angular/router/testing';
import {ErrorService} from '../../services/error.service';

describe('FindOrdersDialogComponent', () => {
  let component: FindOrdersDialogComponent;
  let fixture: ComponentFixture<FindOrdersDialogComponent>;
  let userServiceSpy;
  let ordersServiceSpy;
  let errorServiceSpy;
  const filterData = {
    modalType: ConstOrders.modalOrders,
    filterOrdersData: {
    dateFull: '01/01/2020-02/02/2020',
    docNum: 11248
    }
  };
  const QfbSelectSpy = [
    {
      qfbId: '937b5cc5-78ef-49fb-9a6d-ebdd80f05873',
      qfbName: 'Vicente' + ' ' + 'Cantu'
    },
    {
      qfbId: '19340ad7-03e0-460e-b359-9180a70bf623',
      qfbName: 'test' + ' ' + 'test'
    },
    {
      qfbId: '66824b10-de9b-46e6-ab25-b43fe44e11d0',
      qfbName: 'Sutano' + ' ' + 'López Peréz'
    },
    {
      qfbId: 'f202e592-c0ca-4f3a-8922-0800bbe759ff',
      qfbName: 'QFBinactivo' + ' ' + 'QFBinactivo'
    }

  ];
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
    errorServiceSpy = jasmine.createSpyObj<ErrorService>('ErrorService', [
      'httpError'
    ]);

    TestBed.configureTestingModule({
      imports: [
        BrowserAnimationsModule,
        HttpClientTestingModule,
        MATERIAL_COMPONENTS,
        ReactiveFormsModule,
          RouterTestingModule
      ],
      declarations: [ FindOrdersDialogComponent ],
      providers: [
        DatePipe,
        { provide: MatDialogRef, useValue: {} },
        { provide: MAT_DIALOG_DATA, useValue: filterData},
        { provide: ErrorService, useValue: errorServiceSpy },
        { provide: PedidosService, useValue: ordersServiceSpy },
        { provide: UsersService, useValue: userServiceSpy },
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
    // expect(component.isFromSearchOrders).toBeTruthy();
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
     expect(component.findOrdersForm.get('label').value).toEqual('');
     expect(component.findOrdersForm.get('finlabel').value).toEqual('');
     expect(component.findOrdersForm.get('orderIncidents').value).toEqual('');
     expect(component.findOrdersForm.get('clasification').value).toEqual('');
     expect(component.findOrdersForm.get('docNumUntil').value).toEqual('');
  });
  it(' should reset params value', () => {
    component.resetParamsValue();
    expect(component.findOrdersForm.get('docNum').value).toEqual('');
    expect(component.findOrdersForm.get('dateType').value).toEqual(ConstOrders.defaultDateInit);
    expect(component.findOrdersForm.get('status').value).toEqual('');
    expect(component.findOrdersForm.get('qfb').value).toEqual('');
    expect(component.findOrdersForm.get('productCode').value).toEqual('');
    expect(component.findOrdersForm.get('clientName').value).toEqual('');
    expect(component.findOrdersForm.get('label').value).toEqual('');
    expect(component.findOrdersForm.get('finlabel').value).toEqual('');
    expect(component.findOrdersForm.get('orderIncidents').value).toEqual('');
    expect(component.findOrdersForm.get('clasification').value).toEqual('');
    expect(component.findOrdersForm.get('docNumUntil').value).toEqual('');
  });

  it('should reset Search Params', () => {
    component.resetSearchParams();
    expect(component.isToResetData).toBeTruthy();
    expect(component.isBeginInitForm).toBeTruthy();
    expect(component.findOrdersForm.get('docNum').value).toEqual('');
    expect(component.findOrdersForm.get('dateType').value).toEqual(ConstOrders.defaultDateInit);
    expect(component.findOrdersForm.get('status').value).toEqual('');
    expect(component.findOrdersForm.get('qfb').value).toEqual('');
    expect(component.findOrdersForm.get('productCode').value).toEqual('');
    expect(component.findOrdersForm.get('clientName').value).toEqual('');
    expect(component.findOrdersForm.get('label').value).toEqual('');
    expect(component.findOrdersForm.get('finlabel').value).toEqual('');
    expect(component.findOrdersForm.get('orderIncidents').value).toEqual('');
    expect(component.findOrdersForm.get('clasification').value).toEqual('');
    expect(component.findOrdersForm.get('docNumUntil').value).toEqual('');
  });

  it('should change Validators For DocNum', () => {
    component.changeValidatorsForDocNum();
    expect(component.isToResetData).toBeTruthy();
    expect(component.isBeginInitForm).toBeTruthy();
  });
});
