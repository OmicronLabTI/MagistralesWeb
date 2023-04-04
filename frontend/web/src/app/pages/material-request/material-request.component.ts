import { Component, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatTableDataSource } from '@angular/material/table';
import { DestinationStore, IMaterialRequestRes, MaterialComponent, RawRequest, RawRequestPost } from '../../model/http/materialReques';
import { MaterialRequestService } from '../../services/material-request.service';
import {
  ClassNames,
  ComponentSearch,
  CONST_ARRAY,
  CONST_NUMBER,
  CONST_STRING,
  MessageType,
  TypeToSeeTap
} from '../../constants/const';
import { ErrorService } from '../../services/error.service';
import { DataService } from '../../services/data.service';
import { Messages } from '../../constants/messages';
import { Location } from '@angular/common';
import { Subscription } from 'rxjs';
import { ReportingService } from 'src/app/services/reporting.service';
import { LocalStorageService } from 'src/app/services/local-storage.service';
import { ObservableService } from '../../services/observable.service';
import { MessagesService } from 'src/app/services/messages.service';


@Component({
  selector: 'app-material-request',
  templateUrl: './material-request.component.html',
  styleUrls: ['./material-request.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class MaterialRequestComponent implements OnInit, OnDestroy {
  dataToRequest = {};
  displayedColumns: string[] = [
    'check', 'code', 'component', 'requestQuantity', 'destinationStore', 'unit'
  ];
  dataSource = new MatTableDataSource<MaterialComponent>();
  comments = CONST_STRING.empty;
  isOrder = false;
  isCorrectData = false;
  oldData = new RawRequest();
  subscription = new Subscription();
  allComplete = false;
  isThereToDelete = false;
  isToDownload = false;
  isFreeRequest = false;
  listStore: DestinationStore[] = [];
  constructor(
    private materialReService: MaterialRequestService,
    private errorService: ErrorService,
    private activeRoute: ActivatedRoute,
    private dataService: DataService,
    private reportingService: ReportingService,
    private location: Location,
    private router: Router,
    private localStorageService: LocalStorageService,
    private observableService: ObservableService,
    private messagesService: MessagesService) {
  }

  ngOnInit(): void {
    this.activeRoute.paramMap.subscribe(params => {
      this.dataToRequest = params.get('requests');
      this.isOrder = Number(params.get('isOrder')) === CONST_NUMBER.one;
      this.isFreeRequest = Number(this.dataToRequest) === CONST_NUMBER.zero;
      this.getDestination();
      this.validateRequest();
    });
    this.dataSource.data = this.localStorageService.getMaterialRequestData();
    this.subscription.add(this.observableService.getNewMaterialComponent().subscribe(resultNewMaterialComponent => {
      this.dataSource.data = [...this.dataSource.data, {
        ...resultNewMaterialComponent,
        id: CONST_NUMBER.zero, requestQuantity: CONST_NUMBER.one,
        warehouse: CONST_STRING.empty,
        isLabel: resultNewMaterialComponent.isLabel,
        isWithError: true
      }];
      this.checkIsCorrectData();
      this.checkToDownload();
      this.registerChanges();
    }));
    this.subscription.add(this.observableService.getNewDataSignature().subscribe(newDataSignature => {
      this.oldData.signature = newDataSignature;
      this.checkIsCorrectData();
    }));
  }

  getDestination(): void {
    this.materialReService.getDestinationStore().subscribe((res) => {
      this.listStore = res.response;
    }, error => this.errorService.httpError(error));
  }
  getPreMaterialRequestH(): void {
    let titleStatusOrders = CONST_STRING.empty;
    this.materialReService.getPreMaterialRequest(this.dataToRequest, this.isOrder).subscribe(resultMaterialRequest => {
      const lengthFailOrders = resultMaterialRequest.response.failedProductionOrderIds.length;
      const lengthOkOrders = resultMaterialRequest.response.productionOrderIds.length;
      if (lengthFailOrders > CONST_NUMBER.zero) {
        titleStatusOrders = `${Messages.existRequest} ${this.dataService.calculateTernary(lengthFailOrders === CONST_NUMBER.one,
          Messages.nextOrder, Messages.nextOrders)} \n\n`;
        titleStatusOrders = `${titleStatusOrders} ${this.getIdsLit(resultMaterialRequest.response.failedProductionOrderIds)} \n\n`;
      }
      if (lengthOkOrders > CONST_NUMBER.zero) {
        titleStatusOrders = this.getTitleStatusOrder(
          titleStatusOrders,
          lengthFailOrders,
          lengthOkOrders,
          resultMaterialRequest
        );
      }

      this.messagesService.presentToastCustom(titleStatusOrders === CONST_STRING.empty ? Messages.thereNoOrderProcess :
        titleStatusOrders, 'info', '', true, false, ClassNames.popupCustom);

      if (resultMaterialRequest.response.productionOrderIds.length !== 0) {
        this.oldData = resultMaterialRequest.response;
        this.dataSource.data = resultMaterialRequest.response.orderedProducts;
        this.dataService.setIsToSaveAnything(false);
        this.checkIsCorrectData();
        this.checkToDownload();
      } else {
        this.goBack();
      }
    }, error => this.errorService.httpError(error));
  }

  getTitleStatusOrder(
    titleStatusOrders: string,
    lengthFailOrders: number,
    lengthOkOrders: number,
    resultMaterialRequest: IMaterialRequestRes): string {
    titleStatusOrders = `${titleStatusOrders} ${lengthFailOrders >= CONST_NUMBER.one ?
      `${Messages.requestOrderWithFailOrders}${lengthOkOrders === CONST_NUMBER.one ? Messages.nextOrder : Messages.nextOrders} `
      : `${Messages.requestOrdersOnlyOk} ${lengthOkOrders === CONST_NUMBER.one ? Messages.nextOrder : Messages.nextOrders}`}`;
    titleStatusOrders = `${titleStatusOrders} \n\n ${this.getIdsLit(resultMaterialRequest.response.productionOrderIds).toString()}`;
    return titleStatusOrders;
  }

  addNewComponent(): void {
    this.observableService.setSearchComponentModal({ modalType: ComponentSearch.addComponent, data: this.dataSource.data });
  }

  updateAllComplete(): void {
    this.allComplete = this.dataSource.data != null && this.dataSource.data.every(t => t.isChecked);
    this.checkToDelete();
  }

  someComplete(): boolean {
    if (this.dataSource.data == null) {
      return false;
    }
    return this.dataSource.data.filter(t => t.isChecked).length > CONST_NUMBER.zero && !this.allComplete;
  }

  setAll(completed: boolean): void {
    this.allComplete = completed;
    if (this.dataSource.data == null) {
      return;
    }
    this.dataSource.data.forEach(t => t.isChecked = completed);
    this.checkToDelete();
  }

  signUser(): void {
    this.observableService.setOpenSignatureDialog(this.oldData.signature || CONST_STRING.empty);
  }

  sendRequest(): void {
    const newComponentsToSend = new RawRequestPost();
    this.setModelData();
    newComponentsToSend.data = this.oldData;
    newComponentsToSend.data.productionOrderIds = this.oldData.productionOrderIds || [];
    newComponentsToSend.userId = this.localStorageService.getUserId();

    this.materialReService.postMaterialRequest(newComponentsToSend).subscribe(resultMaterialPost => {
      this.dataSource.data = CONST_ARRAY.empty;
      if (!resultMaterialPost.response && resultMaterialPost.userError) {
        this.messagesService.presentToastCustom(CONST_STRING.empty, 'error',
          resultMaterialPost.userError,
          true, false, ClassNames.popupCustom);
        return;
      }
      if (resultMaterialPost.success && resultMaterialPost.response.failed.length > CONST_NUMBER.zero) {
        this.onDataError(resultMaterialPost.response.failed);
      } else {
        this.goBack();
        this.observableService.setMessageGeneralCallHttp({ title: Messages.success, icon: 'success', isButtonAccept: false });
      }
    }, error => this.errorService.httpError(error));
  }

  cancelRequest(): void {
    this.messagesService.presentToastCustom(Messages.cancelMaterialRequest, 'question', '', true
      , true).then((resultCancel: any) => {
        if (resultCancel.isConfirmed) {
          this.goBack();
        }
      });
  }


  onRequestQuantityChange(requestQuantity: number, index: number): void {
    this.validateRow(index);
    this.checkIsCorrectData();
    this.registerChanges();
  }

  onChangeStore(value: string, index: number): void {
    this.validateRow(index);
    this.checkIsCorrectData();
  }

  validateRow(index: number): void {
    this.dataSource.data[index].isWithError = !Number(this.dataSource.data[index].requestQuantity) ||
      this.dataSource.data[index].warehouse === CONST_STRING.empty;
  }

  checkIsCorrectData(): void {
    this.isCorrectData = this.dataSource.data.filter(order => order.productId === CONST_STRING.empty
      || order.requestQuantity === null || order.description === CONST_STRING.empty
      || order.isWithError).length === CONST_NUMBER.zero && this.oldData.signature && this.dataSource.data.length > 0;
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
    this.dataService.setIsToSaveAnything(false);
    this.localStorageService.setMaterialRequestData(this.dataSource.data);
  }

  checkToDelete(): void {
    this.isThereToDelete = this.dataSource.data.filter(t => t.isChecked).length > CONST_NUMBER.zero;
  }
  checkToDownload(): void {
    this.isToDownload = this.dataSource.data.length > CONST_NUMBER.zero;
  }

  deleteComponents(): void {
    this.messagesService.presentToastCustom(Messages.deleteComponents, 'question', '', true, true)
      .then((resultDeleteMessage: any) => {
        if (resultDeleteMessage.isConfirmed) {
          this.dataSource.data = this.dataSource.data.filter(order => !order.isChecked);
          this.checkToDownload();
          this.registerChanges();
          this.checkIsCorrectData();
        }
      });
  }

  private goBack(): void {
    const route = this.isOrder ? 'ordenes' : 'pedidos';
    this.router.navigate([route]);
  }

  onDataError(errorData: any[], isOnInitError: boolean = false): void {
    this.generateMessage(this.messagesService.getMessageTitle(errorData,
      isOnInitError ? MessageType.materialRequest : MessageType.default, !isOnInitError));
  }

  generateMessage(title: string): void {
    this.messagesService.presentToastCustom(title, 'error',
      Messages.errorToAssignOrderAutomaticSubtitle,
      true, false, ClassNames.popupCustom);
  }

  downloadPreview(): void {
    this.setModelData();
    this.reportingService.downloadPreviewMaterial(this.oldData).subscribe((res) => {
      const listOfBlobs = res.response;
      listOfBlobs.forEach((url) => {
        this.dataService.openNewTapByUrl(url, TypeToSeeTap.order);
      });
    });
  }

  private setModelData(): void {
    this.oldData.observations = this.comments || '';
    this.dataSource.data.forEach(order => order.requestQuantity = Number(Number(order.requestQuantity).toFixed(CONST_NUMBER.seven)));
    this.oldData.orderedProducts = this.dataSource.data;
    this.oldData.signature = this.oldData.signature || '';
    this.oldData.signingUserName = this.localStorageService.getUserName();
  }

  private getFileNamePreview(): string {
    const date = new Date();
    const fileName =
      `Solicitud_MP_${this.getStringNumberTwoDigits(date.getDate())}-
      ${this.getStringNumberTwoDigits(date.getMonth() + 1)}-
      ${date.getFullYear()}_${date.getHours()}_
      ${date.getTime()}_PREVIEW.pdf`;
    return fileName;
  }

  private getStringNumberTwoDigits(numberConverter: number): string {
    if (numberConverter < 10) {
      return `0${numberConverter}`;
    }
    return `${numberConverter}`;
  }
  registerChanges() {
    this.dataService.setIsToSaveAnything(true);
  }

  getIdsLit(idToMessage: number[]): any[] {
    let newIdsStrings = [];
    idToMessage.forEach(id => newIdsStrings = [...newIdsStrings, ` ${id.toString()}`]);
    return newIdsStrings;
  }
  validateRequest(): void {
    if (!this.isFreeRequest) {
      this.getPreMaterialRequestH();
    }
  }
}
