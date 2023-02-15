import { TestBed, async, ComponentFixture } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { AppComponent } from './app.component';
import { MATERIAL_COMPONENTS } from './app.material';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { DatePipe } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { PedidosService } from './services/pedidos.service';
import { DataService } from './services/data.service';
import { ObservableService } from './services/observable.service';
import { of } from 'rxjs';
import { HttpServiceTOCall, MODAL_NAMES } from './constants/const';
import { QfbWithNumber } from './model/http/users';
import { GeneralMessage } from './model/device/general';
import { CancelOrders, SearchComponentModal } from './model/device/orders';
import { CommentsConfig } from './model/device/incidents.model';
import { LocalStorageService } from './services/local-storage.service';
import { MessagesService } from './services/messages.service';
import { Router } from '@angular/router';
import { ComponentsModule } from './components/components.module';
import { PlaceOrderDialogComponent } from './dialogs/place-order-dialog/place-order-dialog.component';
import { AppModule } from './app.module';
import { OrdersService } from './services/orders.service';

describe('AppComponent', () => {
  let component: AppComponent;
  let fixture: ComponentFixture<AppComponent>;
  let pedidosServiceSpy: jasmine.SpyObj<PedidosService>;
  let dataServiceSpy: jasmine.SpyObj<DataService>;
  let observableServiceSpy: jasmine.SpyObj<ObservableService>;
  let localStorageServiceSpy: jasmine.SpyObj<LocalStorageService>;
  let messagesServiceSpy: jasmine.SpyObj<MessagesService>;
  let routerSpy: jasmine.SpyObj<Router>;
  const httpServiceTOCall = HttpServiceTOCall.ORDERS;
  const qfbWithNumber = new QfbWithNumber();
  const generalMessage = new GeneralMessage();
  const cancelOrders = new CancelOrders();
  cancelOrders.list = [{ orderId: 231 }, { orderId: 123 }]
  const searchComponentModal = new SearchComponentModal();
  const commentsConfig = new CommentsConfig();

  beforeEach(async(() => {
    messagesServiceSpy = jasmine.createSpyObj<MessagesService>('MessagesService', [
      'presentToastCustom',
      'getMessageTitle'
    ]);

    messagesServiceSpy.presentToastCustom.and.returnValue(new Promise((resolve, reject) => {
      resolve({
        isConfirmed: true
      })
    }))

    pedidosServiceSpy = jasmine.createSpyObj<PedidosService>('PedidosService', [
      'postPlaceOrders',
      'postPlaceOrderAutomatic',
      'putCancelOrders',
      'putFinalizeOrders',
      'createIsolatedOrder',
      'getQfbsWithOrders'
    ]);

    pedidosServiceSpy.getQfbsWithOrders.and.callFake(() => of({
      response: []
    }))

    pedidosServiceSpy.postPlaceOrders.and.callFake(() => of({
      response: []
    }))

    pedidosServiceSpy.postPlaceOrderAutomatic.and.callFake(() => of({
      response: []
    }))

    pedidosServiceSpy.putCancelOrders.and.callFake(() => of({
      response: {
        failed: []
      }
    }))

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

    routerSpy = jasmine.createSpyObj<Router>('Router', [
      'navigate'
    ])

    localStorageServiceSpy.removeOrderIsolated.and.returnValue();
    localStorageServiceSpy.getOrderIsolated.and.returnValue('orderTest');
    dataServiceSpy = jasmine.createSpyObj<DataService>('DataService', [
      'getIsToSaveAnything',
      'calculateTernary'
    ]);
    dataServiceSpy.calculateTernary.and.callFake(<T, U>(validation: boolean, firstValue: T, secondaValue: U): T | U => {
      return validation ? firstValue : secondaValue;
    });
    dataServiceSpy.getIsToSaveAnything.and.returnValue(false)
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
          'setNewCommentsResult'
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
        { provide: Router, useValue: routerSpy }
      ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AppComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  })

  it('should create', () => {
    expect(component).toBeTruthy();
  });


  it('should go to Page and remove isolate', () => {
    component.goToPage(['ordenes'])
    expect(routerSpy.navigate).toHaveBeenCalled();
  });

  it('should go message service', () => {
    dataServiceSpy.getIsToSaveAnything.and.returnValue(true)
    component.goToPage(['ordenes'])
    expect(routerSpy.navigate).toHaveBeenCalled();
  });

  it('should end session', () => {
    component.endSession()
    expect(localStorageServiceSpy.clearSession).toHaveBeenCalled();

  });

  it('should getSucessMessage', () => {
    let message = component.getSucessMessage({})
    expect(component.getSucessMessage({})).toBe(message)
  });

  it('should getSucessMessage', () => {
    component.showModalPlaceOrderss({})
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


});
