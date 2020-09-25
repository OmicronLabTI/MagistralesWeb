import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {MatTableDataSource} from '@angular/material/table';
import {MaterialComponent} from '../../model/http/materialReques';
import {MaterialRequestService} from '../../services/material-request.service';
import {CONST_NUMBER, CONST_STRING, TypeProperty} from '../../constants/const';
import {ErrorService} from '../../services/error.service';
import {MatDialog} from '@angular/material';
import { RequestSignatureDialogComponent } from 'src/app/dialogs/request-signature-dialog/request-signature-dialog.component';
import {isNumber} from "util";
import { ReportingService } from 'src/app/services/reporting.service';
import { FileTypeContentEnum } from 'src/app/enums/FileTypeContentEnum';
import { FileDownloaderService } from 'src/app/services/file.downloader.service';


@Component({
  selector: 'app-material-request',
  templateUrl: './material-request.component.html',
  styleUrls: ['./material-request.component.scss']
})
export class MaterialRequestComponent implements OnInit {
  dataToRequest = {};
  displayedColumns: string[] = [
    'code', 'component', 'unit', 'requestQuantity'
  ];
  dataSource = new MatTableDataSource<MaterialComponent>();
  comments = CONST_STRING.empty;
  isOrder = false;
  isCorrectData = false;

  onlyNumbers = /[0-9]/;
  @ViewChild('quantityField', {static: false}) quantityField: ElementRef;
  constructor(private router: Router,
              private dialog: MatDialog,
              private materialReService: MaterialRequestService,
              private reportingService: ReportingService,
              private fileDownloaderServie: FileDownloaderService,
              private errorService: ErrorService,
              private activeRoute: ActivatedRoute) {
  }

  ngOnInit() {
    this.activeRoute.paramMap.subscribe(params => {
      this.dataToRequest = params.get('requests');
      this.isOrder = Number(params.get('isOrder')) === CONST_NUMBER.one;
      this.getPreMaterialRequestH();
    });
  }
  getPreMaterialRequestH() {
    this.materialReService.getPreMaterialRequest(this.dataToRequest, this.isOrder).subscribe( resultMaterialRequest => {
      this.dataSource.data = resultMaterialRequest.response.orderedProducts;
      this.checkIsCorrectData();
    }, error => this.errorService.httpError(error));
  }

    addNewComponent() {
      this.dataSource.data = [...this.dataSource.data, {
          id: CONST_NUMBER.zero,
        requestId: CONST_NUMBER.zero,
        productId: CONST_STRING.empty,
        description: CONST_STRING.empty,
        requestQuantity: CONST_NUMBER.zero,
        unit: CONST_STRING.empty
        }];
    }

  signUser() {
    this.dialog.open(RequestSignatureDialogComponent, { panelClass: 'custom-dialog-container' })
      .afterClosed().subscribe(result => {
        if (result) {
          console.log('Set signature.. ', result);
        }
      });
  }

  downloadPreview() {
    console.log('download preview')
    // var data = {};
    // this.fileDownloaderServie.downloadFile(this.reportingService.downloadPreviewRawMaterialRequest(data), FileTypeContentEnum.PDF, this.getFileNamePreview());
  }

  sendRequest() {
    console.log('sendingRequest')
  }

  cancelRequest() {
    console.log('cancelRequest')
    this.setFocus()
  }

  onProductIdChange(productId: string, index: number) {
    this.checkStrings(productId, TypeProperty.code, index);
  }

  onDescriptionChange(description: string, index: number) {
    this.checkStrings(description, TypeProperty.description, index);
  }

  onUnitChange(unit: string, index: number) {
    this.checkStrings(unit, TypeProperty.unit, index);
  }

  onRequestQuantityChange(requestQuantity: number, index: number) {

    console.log('quantity: ', Number(requestQuantity), 'boolean: ', isNumber(Number(requestQuantity)))
    console.log('data: ', Number(this.dataSource.data[index].requestQuantity).toFixed(CONST_NUMBER.seven))
    this.dataSource.data[index].isWithError = !Number(requestQuantity);
    // console.log('quantity: ', requestQuantity, 'index: ', Number(requestQuantity), ' isNumber: ', isNumber(Number(requestQuantity)))
    this.checkIsCorrectData();
  }

  checkIsCorrectData() {
    this.isCorrectData = this.isCorrectData = this.dataSource.data.filter(order => order.productId === CONST_STRING.empty
        || order.productId.trim().length > CONST_NUMBER.fifty || order.requestQuantity <= CONST_NUMBER.zero
        || order.requestQuantity === null || order.description === CONST_STRING.empty
        || order.description.trim().length > CONST_NUMBER.oneHundred
        || order.unit === CONST_STRING.empty || order.unit.trim().length > CONST_NUMBER.fifty
        || order.isWithError).length === CONST_NUMBER.zero;
  }
  checkStrings(wordToCheck: string, typeProperty: TypeProperty, index: number) {
    if (wordToCheck.trim().length < (typeProperty === TypeProperty.unit || typeProperty === TypeProperty.code ?
        CONST_NUMBER.fifty : CONST_NUMBER.oneHundred) && wordToCheck.trim() !== CONST_STRING.empty) {
      this.assignData(wordToCheck, typeProperty, index);
      this.dataSource.data[index].isWithError = false;
    } else {
      this.dataSource.data[index].isWithError = true;
    }
    this.checkIsCorrectData();
  }
  assignData(value: string, typeProperty: TypeProperty, index: number) {
    switch (typeProperty) {
      case TypeProperty.code:
        this.dataSource.data[index].productId = value;
        break;
      case  TypeProperty.unit:
        this.dataSource.data[index].unit = value;
        break;
      case TypeProperty.description:
        this.dataSource.data[index].description = value;
        break;
      case TypeProperty.requestQuantity:
        console.log('caseQuantity')
        this.dataSource.data[index].requestQuantity = value;
        break;
    }
  }

  private forEachNumber(newRequestQuantity: string, numberString: string, isForDecimal: boolean = false) {
    newRequestQuantity = isForDecimal ? `${newRequestQuantity}.` : newRequestQuantity;
    for (let i = 0; i < numberString.length; i++) {
      newRequestQuantity += this.onlyNumbers.test(numberString.charAt(i)) ? numberString.charAt(i) : CONST_STRING.empty;
      console.log('localQuantity: ', newRequestQuantity)
    }
    return newRequestQuantity;
  }
  setFocus() {
    this.quantityField.nativeElement.focus();
  }
  numericOnly(event): boolean {
   // const pattern = /^\d{1,9}(\.\d{0,6})?$/;
    const pattern = /^([0-9.])$/;
    return pattern.test(event.key);
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
