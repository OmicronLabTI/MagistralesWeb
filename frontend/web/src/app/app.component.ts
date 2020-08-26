import {Component, OnDestroy, OnInit} from '@angular/core';
import {DataService} from './services/data.service';
import {Observable, Subscription} from 'rxjs';
import {MatSnackBar} from '@angular/material';
import {AppConfig} from './constants/app-config';
import {Router} from '@angular/router';
import {
    ClassNames,
    CONST_NUMBER,
    CONST_STRING,
    HttpServiceTOCall,
    HttpStatus,
    MessageType,
    MODAL_NAMES
} from './constants/const';
import {PlaceOrderDialogComponent} from './dialogs/place-order-dialog/place-order-dialog.component';
import {MatDialog} from '@angular/material/dialog';
import {Messages} from './constants/messages';
import {IPlaceOrdersReq, QfbWithNumber} from './model/http/users';
import {PedidosService} from './services/pedidos.service';
import {ErrorService} from './services/error.service';
import {GeneralMessage} from './model/device/general';
import {ICancelOrdersRes, IPlaceOrdersAutomaticReq, IPlaceOrdersAutomaticRes} from './model/http/pedidos';
import {CancelOrders} from './model/device/orders';
import {ErrorHttpInterface} from './model/http/commons';


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
  subscriptionObservables = new Subscription();
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
    this.subscriptionObservables.add(this.dataService.getUrlActive().subscribe(url => this.iconMenuActive = url));
    this.subscriptionObservables.add(this.dataService.getQfbToPlace().subscribe(qfbToPlace =>
      this.onSuccessPlaceOrder(qfbToPlace)));
    this.subscriptionObservables.add(this.dataService.getMessageGeneralCalHttp()
        .subscribe(generalMessage => this.onSuccessGeneralMessage(generalMessage)));
    this.subscriptionObservables.add(this.dataService.getCancelOrder().subscribe(resultCancel =>
        this.onSuccessCancelOrder(resultCancel)));
    this.subscriptionObservables.add(this.dataService.getFinalizeOrders().subscribe(resultFinalize =>
        this.onSuccessFinalizeOrders(resultFinalize)));
    this.subscriptionObservables.add(this.dataService.getPathUrl().subscribe(resultPath =>
        this.goToPageEvaluate(resultPath)));
    this.subscriptionObservables.add(this.dataService.getIsLogout().subscribe(() => this.logoutSession(false)));
  }
  endSession() {
      this.logoutSession(true);

  }
  logoutSession(isFromEndSession: boolean) {
      this.dataService.setIsLogin(false);
      this.dataService.clearToken();
      this.onSuccessGeneralMessage({title: isFromEndSession ? Messages.endSession : Messages.expiredSession ,
      icon: isFromEndSession ? 'success' : 'info', isButtonAccept: true});
      this.router.navigate(['/login']);
  }

  goToPage(url: string[]) {
      this.goToPageEvaluate(url);
  }
  goToPageEvaluate(url: any[]) {
      if (!this.dataService.getIsToSaveAnything()) {
          this.navigatePage(url);
      } else {
          console.log('there anything to save');
          this.dataService.presentToastCustom(Messages.leftWithoutSave, 'question', '', true, true)
              .then((savedResult: any) => {
                  if (savedResult.isConfirmed) {
                      this.navigatePage(url);
                  }
              });
      }
  }
  navigatePage(url: any[]) {
    this.router.navigate(url);
  }

  onSuccessPlaceOrder(qfbToPlace: QfbWithNumber) {
    if (qfbToPlace.userId) {
      this.dataService.presentToastCustom(
          qfbToPlace.modalType === MODAL_NAMES.placeOrders ? `${Messages.placeOrder} ${qfbToPlace.userName} ?` :
              `${Messages.placeOrderDetail} ${qfbToPlace.userName} ?`,
          'question',
          CONST_STRING.empty,
          true, true)
          .then((result: any) => {
            if (result.isConfirmed) {
                const placeOrder = new IPlaceOrdersReq();
                placeOrder.userLogistic = this.dataService.getUserId();
                placeOrder.userId = qfbToPlace.userId;
                placeOrder.docEntry = qfbToPlace.list;
                placeOrder.orderType = qfbToPlace.modalType;
                this.pedidosService.postPlaceOrders( placeOrder).subscribe( resPlaceManual => {
                    this.onSuccessPlaceOrdersHttp(resPlaceManual, qfbToPlace.modalType);
                    }, error => this.errorService.httpError(error));
            } else {
              this.createPlaceOrderDialog(qfbToPlace);
            }
          });
    } else if (qfbToPlace.assignType === MODAL_NAMES.assignAutomatic) {
      this.dataService.presentToastCustom(
          Messages.placeOrderAutomatic,
          'question',
          CONST_STRING.empty,
          true, true)
          .then((result: any) => {
            if (result.isConfirmed) {
              const placeOrdersAutomaticReq = new IPlaceOrdersAutomaticReq();
              placeOrdersAutomaticReq.userLogistic = this.dataService.getUserId();
              placeOrdersAutomaticReq.docEntry = qfbToPlace.list;
              this.pedidosService.postPlaceOrderAutomatic(placeOrdersAutomaticReq).subscribe( resultAutomatic => {
                  this.onSuccessPlaceOrdersHttp(resultAutomatic, qfbToPlace.modalType);
              }, (error: ErrorHttpInterface) => {
                  if (error.status === HttpStatus.badRequest) {
                      this.onSuccessGeneralMessage({title: Messages.errorToAssignOrderAutomatic, icon: 'error', isButtonAccept: true});
                  } else {
                      this.errorService.httpError(error);
                  }
              });
            } else {
              this.createPlaceOrderDialog(qfbToPlace);
            }
          });

    }  else {
      this.createPlaceOrderDialog(qfbToPlace);
    }
  }
  createDialogHttpOhAboutTypePlace(modalType: string) {
    if (modalType === MODAL_NAMES.placeOrders) {
      this.dataService.setCallHttpService(HttpServiceTOCall.ORDERS);
      this.onSuccessGeneralMessage({title: Messages.success, isButtonAccept: false, icon: 'success'});
    } else {
      this.dataService.setCallHttpService(HttpServiceTOCall.DETAIL_ORDERS);
      this.onSuccessGeneralMessage({title: Messages.success, isButtonAccept: false, icon: 'success'});
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
    this.subscriptionObservables.unsubscribe();
  }
  onSuccessGeneralMessage(generalMessage: GeneralMessage) {
    this.dataService.presentToastCustom(generalMessage.title,
        generalMessage.icon, CONST_STRING.empty, generalMessage.isButtonAccept, false);

  }
  onSuccessPlaceOrdersHttp(resPlaceOrders: IPlaceOrdersAutomaticRes, modalType: string) {
      if (resPlaceOrders.success && resPlaceOrders.response.length > CONST_NUMBER.zero) {
          const titleItemsWithError = this.dataService.getMessageTitle(resPlaceOrders.response, MessageType.placeOrder);
          this.callHttpAboutModalFrom(modalType);
          this.dataService.presentToastCustom(titleItemsWithError, 'error',
              Messages.errorToAssignOrderAutomaticSubtitle , true, false, ClassNames.popupCustom);
      } else {
          this.createDialogHttpOhAboutTypePlace(modalType);
      }
  }

    private onSuccessCancelOrder(resultCancel: CancelOrders) {
        this.dataService.presentToastCustom(resultCancel.cancelType === MODAL_NAMES.placeOrders ?
            Messages.cancelOrders : Messages.placeOrderDetail,
            'question', CONST_STRING.empty, true, true)
            .then((result: any) => {
                if (result.isConfirmed) {
                    const cancelOrders = [...resultCancel.list];
                    cancelOrders.forEach(order => order.userId = this.dataService.getUserId());
                    this.pedidosService.putCancelOrders(cancelOrders, resultCancel.cancelType === MODAL_NAMES.placeOrders)
                        .subscribe(resultCancelHttp => {
                            this.onSuccessFinalizeHttp(resultCancelHttp, resultCancel.cancelType);
                    }, error => this.errorService.httpError(error));
                }
            });
    }
    callHttpAboutModalFrom(modalType: string) {
        if (modalType === MODAL_NAMES.placeOrders) {
            this.dataService.setCallHttpService(HttpServiceTOCall.ORDERS);
        } else {
            this.dataService.setCallHttpService(HttpServiceTOCall.DETAIL_ORDERS);
        }
    }

    onSuccessFinalizeOrders(resultFinalize: CancelOrders) {
        this.dataService.presentToastCustom(resultFinalize.cancelType === MODAL_NAMES.placeOrders ?
            Messages.finalizeOrders : Messages.finalizeOrdersDetail,
            'question', CONST_STRING.empty, true, true)
            .then((result: any) => {
                if (result.isConfirmed) {
                    const finalizeOrders = [...resultFinalize.list];
                    const userId = this.dataService.getUserId();
                    finalizeOrders.forEach(order => order.userId = userId);
                    console.log('result true modal: ', finalizeOrders)
                    this.pedidosService.putFinalizeOrders(finalizeOrders, resultFinalize.cancelType === MODAL_NAMES.placeOrders)
                        .subscribe(resultFinalizeHttp => {
                           this.onSuccessFinalizeHttp(resultFinalizeHttp, resultFinalize.cancelType);
                        }, error => this.errorService.httpError(error));
                }
            });
    }
    onSuccessFinalizeHttp(resultCancelHttp: ICancelOrdersRes, fromCall: string) {
        if (resultCancelHttp.success && resultCancelHttp.response.failed.length > 0) {
            const titleFinalizeWithError = this.dataService.getMessageTitle(
                resultCancelHttp.response.failed, MessageType.finalizeOrder, true);
            this.callHttpAboutModalFrom(fromCall);
            this.dataService.presentToastCustom(titleFinalizeWithError, 'error',
                Messages.errorToAssignOrderAutomaticSubtitle, true, false, ClassNames.popupCustom);
        } else {
            this.createDialogHttpOhAboutTypePlace(fromCall);
        }
    }
}
