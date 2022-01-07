import { TestBed, async } from '@angular/core/testing';
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
import { HttpServiceTOCall } from './constants/const';
import { QfbWithNumber } from './model/http/users';
import { GeneralMessage } from './model/device/general';
import { CancelOrders, SearchComponentModal } from './model/device/orders';
import { CommentsConfig } from './model/device/incidents.model';
import { LocalStorageService } from './services/local-storage.service';
import { MessagesService } from './services/messages.service';
describe('AppComponent', () => {
  let pedidosServiceSpy: jasmine.SpyObj<PedidosService>;
  let dataServiceSpy: jasmine.SpyObj<DataService>;
  let observableServiceSpy: jasmine.SpyObj<ObservableService>;
  let localStorageServiceSpy: jasmine.SpyObj<LocalStorageService>;
  let messagesServiceSpy: jasmine.SpyObj<MessagesService>;

  const httpServiceTOCall = HttpServiceTOCall.ORDERS;
  const qfbWithNumber = new QfbWithNumber();
  const generalMessage = new GeneralMessage();
  const cancelOrders = new CancelOrders();
  const searchComponentModal = new SearchComponentModal();
  const commentsConfig = new CommentsConfig();

  beforeEach(async(() => {
    messagesServiceSpy = jasmine.createSpyObj<MessagesService>('MessagesService', [
      'presentToastCustom',
      'getMessageTitle'
    ]);

    pedidosServiceSpy = jasmine.createSpyObj<PedidosService>('PedidosService', [
      'postPlaceOrders',
      'postPlaceOrderAutomatic',
      'putCancelOrders',
      'putFinalizeOrders',
      'createIsolatedOrder'
    ]);

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
    dataServiceSpy = jasmine.createSpyObj<DataService>('DataService', [
      'getIsToSaveAnything',
    ]);
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


    TestBed.configureTestingModule({
      imports: [
        RouterTestingModule,
        MATERIAL_COMPONENTS,
        BrowserAnimationsModule,
        HttpClientModule
      ],
      declarations: [
        AppComponent
      ],
      providers: [
        DatePipe,
        { provide: ObservableService, useValue: observableServiceSpy },
        { provide: DataService, useValue: dataServiceSpy },
        { provide: PedidosService, useValue: pedidosServiceSpy },
        { provide: LocalStorageService, useValue: localStorageServiceSpy},
        { provide: MessagesService, useValue: messagesServiceSpy },
      ],
      schemas: [CUSTOM_ELEMENTS_SCHEMA]
    }).compileComponents();
  }));

  it('should create the app', () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.debugElement.componentInstance;
    expect(app).toBeTruthy();
  });

  it(`should have as title 'omicron'`, () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.debugElement.componentInstance;
    expect(app.title).toEqual('omicron');
  });

  /* it('should render title', () => {
     const fixture = TestBed.createComponent(AppComponent);
     fixture.detectChanges();
     const compiled = fixture.debugElement.nativeElement;
     expect(compiled.querySelector('span').textContent).toContain('Hola ,');
   });*/
});
