import {Component, OnDestroy, OnInit} from '@angular/core';
import { DataService } from './services/data.service';
import {Observable, Subscription} from 'rxjs';
import { MatSnackBar } from '@angular/material';
import { AppConfig } from './constants/app-config';
import { Router} from '@angular/router';
import {CONST_NUMBER, CONST_STRING} from './constants/const';
import {PlaceOrderDialogComponent} from './dialogs/place-order-dialog/place-order-dialog.component';
import {MatDialog} from '@angular/material/dialog';
import {Messages} from './constants/messages';
import {IPlaceOrdersReq, QfbWithNumber} from './model/http/users';
import {PedidosService} from './services/pedidos.service';
import {ErrorService} from './services/error.service';



@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnDestroy , OnInit {
  iconMenuActive: number = CONST_NUMBER.one;
  title = 'omicron';
  now = new Date();
  isLoading: Observable<boolean>;
  isLogin = false;
  subscriptionQfbToPlace = new Subscription();
  fullName = '';
  constructor(private dataService: DataService, private snackBar: MatSnackBar,
              private router: Router,  private dialog: MatDialog,
              private pedidosService: PedidosService, private errorService: ErrorService) {
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
    this.subscriptionQfbToPlace =
        this.dataService.getQfbToPlace().subscribe( qfbToPlace => {
          this.onSuccessPlaceOrder(qfbToPlace);
        });
  }
  logoutSession() {
    this.dataService.setIsLogin(false);
    this.dataService.clearToken();
    this.router.navigate(['/login']);
  }

  changeIconActive(newMeuActive: number) {
    this.iconMenuActive = newMeuActive;
  }

  onSuccessPlaceOrder(qfbToPlace: QfbWithNumber) {
    if (qfbToPlace.userId) {
      this.dataService.presentToastCustom(`${Messages.placeOrder} ${qfbToPlace.userName} ?`,
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
              this.pedidosService.postPlaceOrders( placeOrder).subscribe( resultPlaceOrder => {
                console.log('resultPlaceOrder: ', resultPlaceOrder);
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
}
