import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkLoadComponent } from './work-load.component';
import { MATERIAL_COMPONENTS } from 'src/app/app.material';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { DatePipe } from '@angular/common';
import { DataService } from 'src/app/services/data.service';
import { ErrorService } from 'src/app/services/error.service';
import { IWorkLoadRes, WorkLoad } from 'src/app/model/http/pedidos';
import { of, throwError } from 'rxjs';
import { PedidosService } from 'src/app/services/pedidos.service';
import { ObservableService } from 'src/app/services/observable.service';
import { DateService } from 'src/app/services/date.service';

describe('WorkLoadComponent', () => {
  let component: WorkLoadComponent;
  let fixture: ComponentFixture<WorkLoadComponent>;
  let dataServiceSpy: any;
  let errorServiceSpy: any;
  let pedidosServiceSpy: any;
  let observableServiceSpy: jasmine.SpyObj<ObservableService>;
  let dateServiceSpy: jasmine.SpyObj<DateService>;

  const load = new WorkLoad();
  load.assigned = '1';
  load.finalized = '1';
  load.finished = '1';
  load.pending = '1';
  load.processed = '1';
  load.reassigned = '1';
  load.totalFabOrders = '1';
  load.totalOrders = '10';
  load.totalPieces = '1';
  load.totalPossibleAssign = '1';
  load.user = 'Total';
  const loadTwo = new WorkLoad();
  loadTwo.assigned = '1';
  loadTwo.finalized = '1';
  loadTwo.finished = '1';
  loadTwo.pending = '1';
  loadTwo.processed = '1';
  loadTwo.reassigned = '1';
  loadTwo.totalFabOrders = '1';
  loadTwo.totalOrders = '10';
  loadTwo.totalPieces = '1';
  loadTwo.totalPossibleAssign = '1';
  loadTwo.user = 'Daniel';
  const list: WorkLoad[] = [];
  list.push(load, loadTwo);
  const workResponse: IWorkLoadRes = {
    response: list,
  };
  beforeEach(async(() => {
    dataServiceSpy = jasmine.createSpyObj('DataService',
    [
      'setUrlActive',
      'transformDate',
      'getDateFormatted',
      'getFormattedNumber',
      'getMaxMinDate'
    ]);
    errorServiceSpy = jasmine.createSpyObj('ErrorService',
    [
      'httpError'
    ]);
    pedidosServiceSpy = jasmine.createSpyObj('PedidosService',
    [
      'getWorLoad'
    ]);

    dataServiceSpy.transformDate.and.callFake(() => {
      return '13/10/2021';
    });
    pedidosServiceSpy.getWorLoad.and.callFake(() => {
      return of(workResponse);
    });
    // --- Observable Service
    observableServiceSpy = jasmine.createSpyObj<ObservableService>('ObservableService',
      [
        'setUrlActive'
      ]);
      // --- Date Service
    dateServiceSpy = jasmine.createSpyObj<DateService>('DateService', [
      'transformDate',
      'getMaxMinDate',
      'getDateFormatted'
    ]);
    dateServiceSpy.transformDate.and.returnValue('');
    dateServiceSpy.getMaxMinDate.and.returnValue(new Date());
    dateServiceSpy.getDateFormatted.and.returnValue('');
    TestBed.configureTestingModule({
      declarations: [WorkLoadComponent],
      imports: [
        RouterTestingModule,
        MATERIAL_COMPONENTS,
        HttpClientTestingModule,
        ReactiveFormsModule,
        FormsModule,
        BrowserAnimationsModule
      ],
      providers: [
        DatePipe,
        { provide: DataService, useValue: dataServiceSpy },
        { provide: ErrorService, useValue: errorServiceSpy },
        { provide: PedidosService, useValue: pedidosServiceSpy },
        { provide: ObservableService, useValue: observableServiceSpy },
        { provide: DateService, useValue: dateServiceSpy },
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkLoadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should onDataChange', () => {
    component.onDataChange();
    expect(pedidosServiceSpy.getWorLoad).toHaveBeenCalled();
  });

  it('should getWorkLoad with error', () => {
    pedidosServiceSpy.getWorLoad.and.callFake(() => {
      return throwError({ status: 500 });
    });
    component.onDataChange();
    expect(errorServiceSpy.httpError).toHaveBeenCalled();
  });
});
