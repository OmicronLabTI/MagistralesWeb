import { APP_BASE_HREF, DatePipe } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed, async } from '@angular/core/testing';
import { MatDialog, MatDialogRef } from '@angular/material';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { of } from 'rxjs';
import { AppComponent } from './app.component';
import { MATERIAL_COMPONENTS } from './app.material';
import { AppModule } from './app.module';
import { ComponentsModule } from './components/components.module';
import { HttpServiceTOCall, MODAL_NAMES } from './constants/const';
import { FindOrdersDialogComponent } from './dialogs/find-orders-dialog/find-orders-dialog.component';
import { GeneralMessage } from './model/device/general';
import { CommentsConfig } from './model/device/incidents.model';
import { CancelOrders, SearchComponentModal } from './model/device/orders';
import {
  ParamsPedidos
} from './model/http/pedidos';
import { QfbWithNumber } from './model/http/users';
import { DataService } from './services/data.service';
import { ErrorService } from './services/error.service';
import { LocalStorageService } from './services/local-storage.service';
import { MessagesService } from './services/messages.service';
import { ObservableService } from './services/observable.service';
import { PedidosService } from './services/pedidos.service';
describe('AppComponent', () => {
  let component: AppComponent;
  let fixture: ComponentFixture<AppComponent>;
  let pedidosServiceSpy: jasmine.SpyObj<PedidosService>;
  let dataServiceSpy: jasmine.SpyObj<DataService>;
  let observableServiceSpy: jasmine.SpyObj<ObservableService>;
  let localStorageServiceSpy: jasmine.SpyObj<LocalStorageService>;
  let messagesServiceSpy: jasmine.SpyObj<MessagesService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let errorServiceSpy: jasmine.SpyObj<ErrorService>;
  let matDialog: jasmine.SpyObj<MatDialog>;

  const httpServiceTOCall = HttpServiceTOCall.ORDERS;
  const qfbWithNumber = new QfbWithNumber();
  const generalMessage = new GeneralMessage();
  const cancelOrders = new CancelOrders();
  cancelOrders.list = [{ orderId: 231 }, { orderId: 123 }];
  const searchComponentModal = new SearchComponentModal();
  const commentsConfig = new CommentsConfig();

  beforeEach(async(() => {
    matDialog = jasmine.createSpyObj<MatDialog>('MatDialog', ['open']);

    matDialog.open.and.returnValue({
      afterClosed: () => of(true),
    } as MatDialogRef<FindOrdersDialogComponent>);

    messagesServiceSpy = jasmine.createSpyObj<MessagesService>('MessagesService', [
      'presentToastCustom',
      'getMessageTitle'
    ]);

    messagesServiceSpy.presentToastCustom.and.returnValue(new Promise((resolve, reject) => {
      resolve({
        isConfirmed: true
      });
    }));

    pedidosServiceSpy = jasmine.createSpyObj<PedidosService>('PedidosService', [
      'postPlaceOrders',
      'postPlaceOrderAutomatic',
      'putCancelOrders',
      'putFinalizeOrders',
      'createIsolatedOrder',
      'getQfbsWithOrders'
    ]);
    errorServiceSpy = jasmine.createSpyObj<ErrorService>('ErrorService', ['httpError']);

    pedidosServiceSpy.createIsolatedOrder.and.returnValue(of({
      response: 1
    }));
    pedidosServiceSpy.getQfbsWithOrders.and.callFake(() => of({
      response: []
    }));

    pedidosServiceSpy.postPlaceOrders.and.callFake(() => of({
      response: []
    }));

    pedidosServiceSpy.postPlaceOrderAutomatic.and.callFake(() => of({
      response: []
    }));

    pedidosServiceSpy.putCancelOrders.and.callFake(() => of({
      response: {
        failed: []
      }
    }));
    pedidosServiceSpy.putFinalizeOrders.and.callFake(() => of({
      response: {
        failed: []
      }
    }));

    localStorageServiceSpy = jasmine.createSpyObj<LocalStorageService>('LocalStorageService', [
      'clearSession',
      'getUserId',
      'userIsAuthenticated',
      'getUserName',
      'getUserRole',
      'getOrderIsolated',
      'removeOrderIsolated',
      'removeFiltersActive',
      'getFiltersActivesAsModelOrders',
      'setFiltersActivesOrders',
      'removeFiltersActiveOrders',
    ]);
    localStorageServiceSpy.getFiltersActivesAsModelOrders.and.returnValue(new ParamsPedidos());

    routerSpy = jasmine.createSpyObj<Router>('Router', [
      'navigate'
    ]);

    localStorageServiceSpy.removeOrderIsolated.and.returnValue();
    localStorageServiceSpy.getOrderIsolated.and.returnValue('orderTest');
    dataServiceSpy = jasmine.createSpyObj<DataService>('DataService', [
      'getIsToSaveAnything',
      'calculateTernary'
    ]);
    dataServiceSpy.calculateTernary.and.callFake(<T, U>(validation: boolean, firstValue: T, secondaValue: U): T | U => {
      return validation ? firstValue : secondaValue;
    });
    dataServiceSpy.getIsToSaveAnything.and.returnValue(false);
    // ------------ Observable Service
    observableServiceSpy = jasmine.createSpyObj<ObservableService>
      ('ObservableService',
        [
          'getIsLoading',
          'getIsLogin',
          'getGeneralNotificationMessage',
          'getUrlActive',
          'getQfbToPlace',
          'getMessageGeneralCalHttp',
          'getCancelOrder',
          'getFinalizeOrders',
          'getPathUrl',
          'getIsLogout',
          'getSearchComponentModal',
          'getSearchOrdersModal',
          'getOpenSignatureDialog',
          'getOpenCommentsDialog',
          'setIsLogin',
          'setCallHttpService',
          'setNewFormulaComponent',
          'setNewMaterialComponent',
          'setNewSearchOrderModal',
          'setNewCommentsResult',
          'setNewDataSignature'
        ]
      );

    observableServiceSpy.getIsLoading.and.returnValue(of(true));
    observableServiceSpy.getIsLogin.and.returnValue(of(true));
    observableServiceSpy.getGeneralNotificationMessage.and.returnValue(of(''));
    observableServiceSpy.getUrlActive.and.returnValue(of(httpServiceTOCall));
    observableServiceSpy.getQfbToPlace.and.returnValue(of(qfbWithNumber));
    observableServiceSpy.getMessageGeneralCalHttp.and.returnValue(of(generalMessage));
    observableServiceSpy.getCancelOrder.and.returnValue(of(cancelOrders));
    observableServiceSpy.getFinalizeOrders.and.returnValue(of(cancelOrders));
    observableServiceSpy.getPathUrl.and.returnValue(of([]));
    observableServiceSpy.getIsLogout.and.returnValue(of(true));
    observableServiceSpy.getSearchComponentModal.and.returnValue(of(searchComponentModal));
    observableServiceSpy.getSearchOrdersModal.and.returnValue(of(searchComponentModal));
    observableServiceSpy.getOpenSignatureDialog.and.returnValue(of(''));
    observableServiceSpy.getOpenCommentsDialog.and.returnValue(of(commentsConfig));
    observableServiceSpy.setCallHttpService.and.returnValue();

    TestBed.configureTestingModule({
      imports: [
        RouterTestingModule,
        MATERIAL_COMPONENTS,
        BrowserAnimationsModule,
        HttpClientModule,
        ComponentsModule,
        AppModule,
      ],
      declarations: [

        /* PlaceOrderDialogComponent,
         ComponentSearchComponent,
         FindOrdersDialogComponent,
         RequestSignatureDialogComponent,
         AddCommentsDialogComponent*/
      ],
      providers: [
        DatePipe,
        { provide: ObservableService, useValue: observableServiceSpy },
        { provide: DataService, useValue: dataServiceSpy },
        { provide: PedidosService, useValue: pedidosServiceSpy },
        { provide: LocalStorageService, useValue: localStorageServiceSpy },
        { provide: MessagesService, useValue: messagesServiceSpy },
        { provide: Router, useValue: routerSpy },
        { provide: ErrorService, useValue: errorServiceSpy },
        { provide: MatDialog, useValue: matDialog },
        { provide: APP_BASE_HREF, useValue: '/' }
      ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AppComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });


  it('should go to Page and remove isolate', () => {
    component.goToPage(['ordenes']);
    expect(routerSpy.navigate).toHaveBeenCalled();
  });

  it('should go message service', () => {
    dataServiceSpy.getIsToSaveAnything.and.returnValue(true);
    component.goToPage(['ordenes']);
    expect(routerSpy.navigate).toHaveBeenCalled();
  });

  it('should end session', () => {
    component.endSession();
    expect(localStorageServiceSpy.clearSession).toHaveBeenCalled();

  });

  it('should getSucessMessage', () => {
    const message = component.getSucessMessage({});
    expect(component.getSucessMessage({})).toBe(message);
  });

  it('should getSucessMessage', () => {
    component.showModalPlaceOrderss({});
  });

  it('should createDialogHttpOhAboutTypePlace', () => {
    component.createDialogHttpOhAboutTypePlace(MODAL_NAMES.placeOrders, true);
    expect(observableServiceSpy.setCallHttpService).toHaveBeenCalled();
  });

  it('should createDialogHttpOhAboutTypePlace PlaceOrders', () => {
    component.createDialogHttpOhAboutTypePlace(MODAL_NAMES.placeOrders, false);
    expect(observableServiceSpy.setCallHttpService).toHaveBeenCalled();
  });

  it('should createDialogHttpOhAboutTypePlace another modal', () => {
    component.createDialogHttpOhAboutTypePlace(MODAL_NAMES.placeOrdersDetail, false);
    expect(observableServiceSpy.setCallHttpService).toHaveBeenCalled();
  });

  it('should showAutomaticModalOrder ', () => {
    component.showAutomaticModalOrder({});
    expect(messagesServiceSpy.presentToastCustom).toHaveBeenCalled();
  });


});
