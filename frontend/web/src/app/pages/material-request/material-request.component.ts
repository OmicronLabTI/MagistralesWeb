import { Component, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { MaterialHistoryQuery, MaterialRequestData } from 'src/app/model/http/materialReques';
import { LocalStorageService } from 'src/app/services/local-storage.service';

@Component({
  selector: 'app-material-request',
  templateUrl: './material-request.component.html',
  styleUrls: ['./material-request.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class MaterialRequestComponent implements OnInit, OnDestroy {

  constructor(private localStorageService: LocalStorageService) {
  }

  ngOnInit(): void {}

  ngOnDestroy(): void {
    this.localStorageService.setMaterialRequestData(new MaterialRequestData());
    this.localStorageService.setMaterialHistoryQuery(new MaterialHistoryQuery());
  }
}
