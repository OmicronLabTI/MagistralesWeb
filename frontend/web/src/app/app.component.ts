import {Component, OnDestroy, OnInit} from '@angular/core';
import {DataService} from './services/data.service';
import {Observable, Subscription} from 'rxjs';
import {MatSnackBar} from '@angular/material';
import {AppConfig} from './constants/app-config';
import {Router} from '@angular/router';
import {
    ClassNames,
    ComponentSearch,
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
import {
    CreateIsolatedOrderReq,
    ICancelOrdersRes,
    IPlaceOrdersAutomaticReq,
    IPlaceOrdersAutomaticRes,
    ParamsPedidos
} from './model/http/pedidos';
import {CancelOrders, SearchComponentModal} from './model/device/orders';
import {ErrorHttpInterface} from './model/http/commons';
import {ComponentSearchComponent} from './dialogs/components-search-dialog/component-search.component';
import {FindOrdersDialogComponent} from './dialogs/find-orders-dialog/find-orders-dialog.component';

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
    role = '';
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
      this.role = this.dataService.getUserRole();
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
    this.subscriptionObservables.add(this.dataService.getSearchComponentModal().subscribe(resultSearchComponentModal =>
         this.onSuccessSearchComponentModal(resultSearchComponentModal)));
    this.subscriptionObservables.add(this.dataService.getSearchOrdersModal().subscribe(resultSearchOrdersModal =>
        this.onSuccessSearchOrders(resultSearchOrdersModal)));
  }
  endSession() {
      this.logoutSession(true);

  }
  logoutSession(isFromEndSession: boolean) {
      this.dataService.setIsLogin(false);
      this.dataService.clearSession();
      this.onSuccessGeneralMessage({title: isFromEndSession ? Messages.endSession : Messages.expiredSession ,
      icon: isFromEndSession ? 'success' : 'info', isButtonAccept: false});
      this.router.navigate(['/login']);
  }

  goToPage(url: string[]) {
      this.goToPageEvaluate(url);
  }
  goToPageEvaluate(url: any[]) {
      if (!this.dataService.getIsToSaveAnything()) {
          this.navigatePage(url);
      } else {
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
          qfbToPlace.modalType === MODAL_NAMES.placeOrders ? !qfbToPlace.isFromReassign ?
              `${Messages.placeOrder} ${qfbToPlace.userName} ?` :
              `${Messages.reassignOrder} ${qfbToPlace.userName} ?` :
              !qfbToPlace.isFromReassign ?
                  `${Messages.placeOrderDetail} ${qfbToPlace.userName} ?` :
                  `${Messages.reassignOrderDetail} ${qfbToPlace.userName} ?`,
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
                this.pedidosService.postPlaceOrders( placeOrder, qfbToPlace.isFromReassign).subscribe( resPlaceManual => {
                    this.onSuccessPlaceOrdersHttp(resPlaceManual, qfbToPlace.modalType, qfbToPlace.isFromOrderIsolated);
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
                  this.onSuccessPlaceOrdersHttp(resultAutomatic, qfbToPlace.modalType, qfbToPlace.isFromOrderIsolated);
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
  createDialogHttpOhAboutTypePlace(modalType: string, isFromOrderIsolated: boolean) {
    if (isFromOrderIsolated) {
        this.dataService.setCallHttpService(HttpServiceTOCall.ORDERS_ISOLATED);
        this.onSuccessGeneralMessage({title: Messages.success, isButtonAccept: false, icon: 'success'});
    } else {
        if (modalType === MODAL_NAMES.placeOrders) {
            this.dataService.setCallHttpService(HttpServiceTOCall.ORDERS);
            this.onSuccessGeneralMessage({title: Messages.success, isButtonAccept: false, icon: 'success'});
        } else {
            this.dataService.setCallHttpService(HttpServiceTOCall.DETAIL_ORDERS);
            this.onSuccessGeneralMessage({title: Messages.success, isButtonAccept: false, icon: 'success'});
        }
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
  onSuccessPlaceOrdersHttp(resPlaceOrders: IPlaceOrdersAutomaticRes, modalType: string, isFromOrderIsolated: boolean) {
      if (resPlaceOrders.success && resPlaceOrders.response !== null && resPlaceOrders.response.length > CONST_NUMBER.zero) {
          const titleItemsWithError = this.dataService.getMessageTitle(resPlaceOrders.response, MessageType.placeOrder);
          this.callHttpAboutModalFrom(modalType, isFromOrderIsolated);
          this.dataService.presentToastCustom(titleItemsWithError, 'error',
              Messages.errorToAssignOrderAutomaticSubtitle , true, false, ClassNames.popupCustom);
      } else {
          this.createDialogHttpOhAboutTypePlace(modalType, isFromOrderIsolated);
      }
  }

    private onSuccessCancelOrder(resultCancel: CancelOrders) {
        this.dataService.presentToastCustom(resultCancel.cancelType === MODAL_NAMES.placeOrders ?
            Messages.cancelOrders : Messages.cancelOrdersDetail,
            'question', CONST_STRING.empty, true, true)
            .then((result: any) => {
                if (result.isConfirmed) {
                    const cancelOrders = [...resultCancel.list];
                    cancelOrders.forEach(order => order.userId = this.dataService.getUserId());
                    this.pedidosService.putCancelOrders(cancelOrders, resultCancel.cancelType === MODAL_NAMES.placeOrders)
                        .subscribe(resultCancelHttp => {
                            this.onSuccessFinalizeHttp(resultCancelHttp, resultCancel.cancelType, resultCancel.isFromCancelIsolated);
                    }, error => this.errorService.httpError(error));
                }
            });
    }
    callHttpAboutModalFrom(modalType: string, isFromOrdersIsolated: boolean) {
       if (isFromOrdersIsolated) {
           this.dataService.setCallHttpService(HttpServiceTOCall.ORDERS_ISOLATED);
       } else {
           if (modalType === MODAL_NAMES.placeOrders) {
               this.dataService.setCallHttpService(HttpServiceTOCall.ORDERS);
           } else {
               this.dataService.setCallHttpService(HttpServiceTOCall.DETAIL_ORDERS);
           }
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
                    this.pedidosService.putFinalizeOrders(finalizeOrders, resultFinalize.cancelType === MODAL_NAMES.placeOrders)
                        .subscribe(resultFinalizeHttp => {
                           this.onSuccessFinalizeHttp(resultFinalizeHttp, resultFinalize.cancelType, resultFinalize.isFromCancelIsolated);
                        }, error => this.errorService.httpError(error));
                }
            });
    }
    onSuccessFinalizeHttp(resultCancelHttp: ICancelOrdersRes, fromCall: string, isFromOrderIsolated: boolean) {
        if (resultCancelHttp.success && resultCancelHttp.response.failed.length > 0) {
            const titleFinalizeWithError = this.dataService.getMessageTitle(
                resultCancelHttp.response.failed, MessageType.finalizeOrder, true);
            this.callHttpAboutModalFrom(fromCall, isFromOrderIsolated);
            this.dataService.presentToastCustom(titleFinalizeWithError, 'error',
                Messages.errorToAssignOrderAutomaticSubtitle, true, false, ClassNames.popupCustom);
        } else {
            this.createDialogHttpOhAboutTypePlace(fromCall, isFromOrderIsolated);
        }
    }

    private onSuccessSearchComponentModal(resultSearchComponentModal: SearchComponentModal) {
        const dialogRef = this.dialog.open(ComponentSearchComponent, {
            panelClass: 'custom-dialog-container',
            data: {
                modalType: resultSearchComponentModal.modalType,
                chips: resultSearchComponentModal.chips
            }
        });

        dialogRef.afterClosed().subscribe((resultComponents: any) => {
            if (resultComponents && (resultSearchComponentModal.modalType === ComponentSearch.searchComponent)) {
                this.dataService.setNewFormulaComponent(resultComponents);
            } else if (resultComponents) {
                this.dataService.presentToastCustom(Messages.createIsolatedOrder + resultComponents.productoId + '?',
                    'question', CONST_STRING.empty, true, true)
                    .then((resultCreateIsolated: any) => {
                        if (resultCreateIsolated.isConfirmed) {
                            this.onSuccessDialogClosed(resultComponents);
                        } else {
                            this.onSuccessSearchComponentModal({
                                modalType: resultSearchComponentModal.modalType,
                                chips: resultComponents.chips
                            });
                        }
                    });

            }
        });
    }

    onSuccessDialogClosed(resultComponents: any) {
        const createIsolatedReq = new CreateIsolatedOrderReq();
        createIsolatedReq.productCode = resultComponents.productoId;
        createIsolatedReq.userId = this.dataService.getUserId();
        this.pedidosService.createIsolatedOrder(createIsolatedReq).subscribe( resultCreateIsolated => {
            if (resultCreateIsolated.response !== 0) {// 0 = with error
                this.onSuccessGeneralMessage({title: Messages.success, icon: 'success', isButtonAccept: false});
                this.navigatePage(['/ordenfabricacion', resultCreateIsolated.response.toString()]);
            } else {
                this.dataService.presentToastCustom(resultCreateIsolated.userError, 'error',
                    Messages.errorToAssignOrderAutomaticSubtitle, true, false, ClassNames.popupCustom);
            }
        }, error => this.errorService.httpError(error));
    }

    private onSuccessSearchOrders(resultSearchOrdersModal: SearchComponentModal) {
        this.dialog.open(FindOrdersDialogComponent, {
            panelClass: 'custom-dialog-container',
            data: {
                modalType: resultSearchOrdersModal.modalType,
                filterOrdersData: resultSearchOrdersModal.filterOrdersData
            }
        }).afterClosed().subscribe((result: ParamsPedidos) => {
           if (result) {
               this.dataService.setNewSearchOrderModal(result);
           }
        });
    }
}
