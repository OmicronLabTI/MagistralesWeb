import {Component, OnDestroy, OnInit} from '@angular/core';
import { DataService } from './services/data.service';
import {Observable, Subscription} from 'rxjs';
import { MatSnackBar } from '@angular/material';
import { AppConfig } from './constants/app-config';
import { Router} from '@angular/router';
import { CONST_STRING, HttpServiceTOCall, MODAL_NAMES} from './constants/const';
import {PlaceOrderDialogComponent} from './dialogs/place-order-dialog/place-order-dialog.component';
import {MatDialog} from '@angular/material/dialog';
import {Messages} from './constants/messages';
import {IPlaceOrdersReq, QfbWithNumber} from './model/http/users';
import {PedidosService} from './services/pedidos.service';
import {ErrorService} from './services/error.service';
import {GeneralMessage} from './model/device/general';



@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnDestroy , OnInit {
  iconMenuActive = HttpServiceTOCall.ORDERS;
  title = 'omicron';
  now = new Date();
  isLoading: Observable<boolean>;
  isLogin = false;
  subscriptionQfbToPlace = new Subscription();
  fullName = '';
  constructor(private dataService: DataService, private snackBar: MatSnackBar,
              private router: Router,  private dialog: MatDialog,
              private pedidosService: PedidosService, private errorService: ErrorService,
              ) {
    this.getFullName();
    this.isLoading = this.dataService.getIsLoading();
    this.isLogin = this.dataService.userIsAuthenticated();
    this.dataService.getIsLogin().subscribe( isLoginS => {
      this.getFullName();
      this.isLogin = isLoginS;
    });

    this.dataService
      .getGeneralNotificationMessage()
      .subscribe(msg => {
        this.snackBar.open(msg, 'OK', {
          duration: AppConfig.generalMessageTimeout
        });
      });
  }
  ngOnInit() {
    this.subscriptionQfbToPlace.add(this.dataService.getUrlActive().subscribe(url => {
      this.iconMenuActive = url;
    }));
    this.subscriptionQfbToPlace.add(this.dataService.getQfbToPlace().subscribe( qfbToPlace => {
      this.onSuccessPlaceOrder(qfbToPlace);
    }));
    this.subscriptionQfbToPlace.add(this.dataService.getMessageGeneralCalHttp()
        .subscribe(generalMessage => this.onSuccessGeneralMessage(generalMessage)));
  }
  logoutSession() {
    this.dataService.setIsLogin(false);
    this.dataService.clearToken();
    this.router.navigate(['/login']);
  }

  goToPage(url: string[]) {
    if (!this.dataService.getIsToSaveAnything()) {
      this.navigatePage(url);
    } else {
      /*this.dataService.presentToastCustom(Messages.saveFormulaDetail, 'question', '', true, true)
          .then((savedResult: any) => {
            if (savedResult.isConfirmed) {

            } else {
              this.navigatePage(url)
            }
          });*/
    }
  }
  navigatePage(url: string[]) {
    this.router.navigate(url);
  }

  onSuccessPlaceOrder(qfbToPlace: QfbWithNumber) {
    if (qfbToPlace.userId) {
      this.dataService.presentToastCustom(
          qfbToPlace.modalType === MODAL_NAMES.placeOrders ? `${Messages.placeOrder} ${qfbToPlace.userName} ?` :
              `${Messages.placeOrderDetail} ${qfbToPlace.userName} ?`,
          'warning',
          CONST_STRING.empty,
          true, true)
          .then((result: any) => {
            if (result.isConfirmed) {
              const placeOrder = new IPlaceOrdersReq();
              placeOrder.userLogistic = this.dataService.getUserId();
              placeOrder.userId = qfbToPlace.userId;
              placeOrder.docEntry = qfbToPlace.list;
              placeOrder.orderType = qfbToPlace.modalType;
              this.pedidosService.postPlaceOrders( placeOrder).subscribe( () => {
                if (qfbToPlace.modalType === MODAL_NAMES.placeOrders) {
                  this.dataService.setCallHttpService(HttpServiceTOCall.ORDERS);
                  this.onSuccessGeneralMessage({title: Messages.success, isButtonAccept: false, icon: 'success'});
                } else {
                  this.dataService.setCallHttpService(HttpServiceTOCall.DETAIL_ORDERS);
                  this.onSuccessGeneralMessage({title: Messages.success, isButtonAccept: false, icon: 'success'});
                }
              }, error => this.errorService.httpError(error));
            } else {
              this.createPlaceOrderDialog(qfbToPlace);
            }
          });
    } else {
      this.createPlaceOrderDialog(qfbToPlace);
    }
  }
  createPlaceOrderDialog(placeOrdersData: any) {
    this.dialog.open(PlaceOrderDialogComponent, {
      panelClass: 'custom-dialog-container',
      data: {
        placeOrdersData
      }
    });
  }

  getFullName() {
    this.fullName = this.dataService.getUserName();
  }

  ngOnDestroy() {
    this.subscriptionQfbToPlace.unsubscribe();
  }
  onSuccessGeneralMessage(generalMessage: GeneralMessage) {
    this.dataService.presentToastCustom(generalMessage.title,
        generalMessage.icon, CONST_STRING.empty, generalMessage.isButtonAccept, false);

  }
}
