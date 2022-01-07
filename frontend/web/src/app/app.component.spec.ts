import { TestBed, async } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { AppComponent } from './app.component';
import { MATERIAL_COMPONENTS } from './app.material';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {DatePipe} from '@angular/common';
import {HttpClientModule} from '@angular/common/http';
import {CUSTOM_ELEMENTS_SCHEMA} from '@angular/core';
import { PedidosService } from './services/pedidos.service';
import { DataService } from './services/data.service';
import { LocalStorageService } from './services/local-storage.service';
describe('AppComponent', () => {
  let pedidosServiceSpy: jasmine.SpyObj<PedidosService>;
  let dataServiceSpy: jasmine.SpyObj<DataService>;
  let localStorageServiceSpy: jasmine.SpyObj<LocalStorageService>;



  beforeEach(async(() => {
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
    ]);
    dataServiceSpy = jasmine.createSpyObj<DataService>('DataService', [
      'getUserRole',
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
      'getOrderIsolated',
      'removeOrderIsolated',
      'getIsToSaveAnything',
      'presentToastCustom',
      'setCallHttpService',
      'getUserName',
      'removeFiltersActiveOrders',
      'removeFiltersActive',
      'getMessageTitle',
      'setNewFormulaComponent',
      'setNewMaterialComponent',
      'getFiltersActivesAsModelOrders',
      'setFiltersActivesOrders',
      'setNewSearchOrderModal',
      'setNewCommentsResult'
    ]);
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
      providers: [DatePipe],
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
