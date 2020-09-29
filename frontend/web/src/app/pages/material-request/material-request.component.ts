import {Component, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {MatTableDataSource} from '@angular/material/table';
import {MaterialComponent, RawRequest, RawRequestPost} from '../../model/http/materialReques';
import {MaterialRequestService} from '../../services/material-request.service';
import {
  ClassNames,
  ComponentSearch,
  CONST_NUMBER,
  CONST_STRING,
  MaterialRequestPage,
  MessageType
} from '../../constants/const';
import {ErrorService} from '../../services/error.service';
import {MatDialog} from '@angular/material';
import {RequestSignatureDialogComponent} from 'src/app/dialogs/request-signature-dialog/request-signature-dialog.component';
import {DataService} from '../../services/data.service';
import {Messages} from '../../constants/messages';
import {Location} from '@angular/common';
import {Subscription} from 'rxjs';
import { FileDownloaderService } from 'src/app/services/file.downloader.service';
import { ReportingService } from 'src/app/services/reporting.service';
import { FileTypeContentEnum } from 'src/app/enums/FileTypeContentEnum';


@Component({
  selector: 'app-material-request',
  templateUrl: './material-request.component.html',
  styleUrls: ['./material-request.component.scss']
})
export class MaterialRequestComponent implements OnInit, OnDestroy {
  dataToRequest = {};
  displayedColumns: string[] = [
    'check', 'code', 'component', 'requestQuantity', 'unit'
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
  constructor(private router: Router,
              private dialog: MatDialog,
              private materialReService: MaterialRequestService,
              private errorService: ErrorService,
              private activeRoute: ActivatedRoute,
              private dataService: DataService,
              private fileDownloaderServie: FileDownloaderService,
              private reportingService: ReportingService,
              private  location: Location) {
  }

  ngOnInit() {
    this.activeRoute.paramMap.subscribe(params => {
      this.dataToRequest = params.get('requests');
      this.isOrder = Number(params.get('isOrder')) === CONST_NUMBER.one;
      this.getPreMaterialRequestH();
    });
    this.subscription.add(this.dataService.getNewMaterialComponent().subscribe( resultNewMaterialComponent => {
      this.dataSource.data = [...this.dataSource.data, {...resultNewMaterialComponent,
                                                          id: CONST_NUMBER.zero, requestQuantity: CONST_NUMBER.one}];
      this.checkIsCorrectData();
      this.checkToDelete();
    }));
  }
  getPreMaterialRequestH() {
    this.materialReService.getPreMaterialRequest(this.dataToRequest, this.isOrder).subscribe( resultMaterialRequest => {
      if (resultMaterialRequest.response.failedProductionOrderIds.length > CONST_NUMBER.zero) {
        this.onDataError(resultMaterialRequest.response.failedProductionOrderIds, true);
      }
      if (resultMaterialRequest.response.orderedProducts.length !== 0) {
        this.oldData = resultMaterialRequest.response;
        this.dataSource.data = resultMaterialRequest.response.orderedProducts;
        this.dataService.setIsToSaveAnything(false);
        this.checkIsCorrectData();
        this.checkToDelete();
      } else {
        this.goBack();
      }
    }, error => this.errorService.httpError(error));
  }

  addNewComponent() {
      this.dataService.setSearchComponentModal({ modalType: ComponentSearch.addComponent});
  }

  updateAllComplete() {
    this.allComplete = this.dataSource.data != null && this.dataSource.data.every(t => t.isChecked);
    this.checkToDelete();
  }

  someComplete(): boolean {
    if (this.dataSource.data == null) {
      return false;
    }
    return this.dataSource.data.filter(t => t.isChecked).length > CONST_NUMBER.zero && !this.allComplete;
  }

  setAll(completed: boolean) {
    this.allComplete = completed;
    if (this.dataSource.data == null) {
      return;
    }
    this.dataSource.data.forEach(t => t.isChecked = completed);
    this.checkToDelete();
  }

  signUser() {
    this.dialog.open(RequestSignatureDialogComponent, 
      { 
        panelClass: 'custom-dialog-container',
        data: this.oldData.signature 
      })
      .afterClosed().subscribe(result => {
        if (result) {
          this.oldData.signature = result;
          this.checkIsCorrectData();
        }
      });
  }

  sendRequest() {
    const newComponentsToSend = new RawRequestPost();
    this.setModelData();
    newComponentsToSend.data = this.oldData;
    newComponentsToSend.data.productionOrderIds = this.oldData.productionOrderIds;
    newComponentsToSend.userId = this.dataService.getUserId();
    this.materialReService.postMaterialRequest(newComponentsToSend).subscribe( resultMaterialPost => {
      if (resultMaterialPost.success && resultMaterialPost.response.failed.length > CONST_NUMBER.zero) {
        this.onDataError(resultMaterialPost.response.failed);
      } else {
        this.goBack();
        this.dataService.setMessageGeneralCallHttp({title: Messages.success, icon: 'success', isButtonAccept: false});
      }
    }, error => this.errorService.httpError(error));
  }

  cancelRequest() {
    this.dataService.presentToastCustom(Messages.cancelMaterialRequest, 'question', '', true
    , true).then((resultCancel: any) => {
      if (resultCancel.isConfirmed) {
        this.goBack();
      }
    });
  }


  onRequestQuantityChange(requestQuantity: number, index: number) {
    this.dataSource.data[index].isWithError = !Number(requestQuantity);
    this.checkIsCorrectData();
  }

  checkIsCorrectData() {
    this.isCorrectData = this.isCorrectData = this.dataSource.data.filter(order => order.productId === CONST_STRING.empty
        || order.requestQuantity === null || order.description === CONST_STRING.empty
        || order.isWithError).length === CONST_NUMBER.zero && this.oldData.signature;
    this.dataService.setIsToSaveAnything(true);
  }

  numericOnly(event): boolean {
    return MaterialRequestPage.onlyNumberPatter.test(event.key);
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
    this.dataService.setIsToSaveAnything(false);
  }

  checkToDelete() {
    this.isThereToDelete = this.dataSource.data.filter(t => t.isChecked).length > CONST_NUMBER.zero;
    this.isToDownload = this.dataSource.data.length > CONST_NUMBER.zero;
  }

  deleteComponents() {
    this.dataSource.data = this.dataSource.data.filter( order => !order.isChecked);
    this.checkToDelete();
  }

  private goBack() {
    this.location.back();
  }
  onDataError(errorData: any[], isOnInitError: boolean = false) {
    this.generateMessage(this.dataService.getMessageTitle(errorData,
        isOnInitError ? MessageType.materialRequest : MessageType.default, !isOnInitError));
  }
  generateMessage(title: string) {
    this.dataService.presentToastCustom(title, 'error',
        Messages.errorToAssignOrderAutomaticSubtitle ,
        true, false, ClassNames.popupCustom);
  }
  
  downloadPreview() {
    this.setModelData();
    this.fileDownloaderServie.downloadFile(this.reportingService.downloadPreviewRawMaterialRequest(this.oldData), FileTypeContentEnum.PDF, this.getFileNamePreview());
  }

  private setModelData() {
    this.oldData.observations = this.comments || '';
    this.dataSource.data.forEach(order => order.requestQuantity = Number(Number(order.requestQuantity).toFixed(CONST_NUMBER.seven)));
    this.oldData.orderedProducts = this.dataSource.data;
    this.oldData.signature = this.oldData.signature || '';
    this.oldData.signingUserName = this.dataService.getUserName();
  }
  private getFileNamePreview(): string {
    var date = new Date();
    let fileName = `Solicitud_MP_${this.getStringNumberTwoDigits(date.getDate())}-${this.getStringNumberTwoDigits(date.getMonth())}-${date.getFullYear()}_${date.getHours()}_${date.getMinutes()}_PREVIEW.pdf`;
    return fileName;
  }

  private getStringNumberTwoDigits(number: number): string {
    if (number < 10) {
      return `0${number}`;
    }
    return `${number}`;
  }
}
