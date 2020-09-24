import { Component, OnInit } from '@angular/core';
import {Router} from '@angular/router';
import {MatTableDataSource} from '@angular/material/table';
import {MaterialComponent} from '../../model/http/materialReques';
import {MaterialRequestService} from '../../services/material-request.service';
import {CONST_STRING} from '../../constants/const';
import {ErrorService} from '../../services/error.service';
import {MatDialog} from '@angular/material';
import { RequestSignatureDialogComponent } from 'src/app/dialogs/request-signature-dialog/request-signature-dialog.component';

@Component({
  selector: 'app-material-request',
  templateUrl: './material-request.component.html',
  styleUrls: ['./material-request.component.scss']
})
export class MaterialRequestComponent implements OnInit {
  dataToRequest = {};
  displayedColumns: string[] = [
    'code', 'component', 'requestQuantity', 'unit'
  ];
  dataSource = new MatTableDataSource<MaterialComponent>();
  comments = CONST_STRING.empty;
  isOrder = false;
  constructor(private router: Router,
              private dialog: MatDialog,
              private materialReService: MaterialRequestService,
              private errorService: ErrorService) {
    this.dataToRequest = this.router.getCurrentNavigation().extras.state;
    this.isOrder = this.router.getCurrentNavigation().extras.replaceUrl;
  }

  ngOnInit() {
    this.getPreMaterialRequestH();
  }
  getPreMaterialRequestH() {
    this.materialReService.getPreMaterialRequest(this.dataToRequest, this.isOrder).subscribe( resultMaterialRequest => {
      console.log('resultRequest: ', resultMaterialRequest)
      this.dataSource.data = resultMaterialRequest.response.orderedProducts;
      console.log('dataSource: ', this.dataSource.data[0].description)
    }, error => this.errorService.httpError(error));
  }

    addNewComponent() {
        console.log('added component: ')
    }

  signUser() {
    this.dialog.open(RequestSignatureDialogComponent, { panelClass: 'custom-dialog-container' })
      .afterClosed().subscribe(result => {
        if (result) {
          console.log('Set signature.. ', result);
        }
      });
  }

  sendRequest() {
    console.log('sendingRequest')
  }

  cancelRequest() {
    console.log('cancelRequest')
  }
}
