import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { PageEvent } from '@angular/material';
import { BoolConst, CONST_NUMBER } from 'src/app/constants/const';
import { SettingsCommonTableClass } from 'src/app/model/data/common.data';
import { MaterialRequestHistoryTableSettings } from 'src/app/model/data/materialRequestHistory';
import { IMaterialHistoryItem } from 'src/app/model/http/materialReques';
import { ErrorService } from 'src/app/services/error.service';
import { MaterialRequestService } from 'src/app/services/material-request.service';

@Component({
  selector: 'app-history-material-request',
  templateUrl: './history-material-request.component.html',
  styleUrls: ['./history-material-request.component.scss']
})
export class HistoryMaterialRequestComponent implements OnInit {
  historyMaterialRequestSettings: SettingsCommonTableClass = MaterialRequestHistoryTableSettings;
  dataHistory: IMaterialHistoryItem[];
  lengthPaginator = CONST_NUMBER.zero;
  offset: number = CONST_NUMBER.zero;
  pageIndex: number = CONST_NUMBER.zero;
  limit: number = CONST_NUMBER.ten;
  statusControl: FormControl = new FormControl([]);
  loading: boolean = BoolConst.false;
  constructor(
    private materialRequest: MaterialRequestService,
    private errorService: ErrorService
  ) { }

  ngOnInit() {
    this.historyMaterialRequest();
  }

  filterChange = () => {
    this.historyMaterialRequest();
  }

  getQuery = () => {
    const status = this.statusControl.value.toString();
    return `?offset=${this.offset}&limit=${this.limit}&fini=08/03/2023&ffin=14/03/2023&status=${status}`;
  }
  changeDataEvent(event: PageEvent) {
    this.pageIndex = event.pageIndex;
    this.offset = (event.pageSize * (event.pageIndex));
    this.limit = event.pageSize;
    this.historyMaterialRequest();
    return event;
  }

  historyMaterialRequest = () => {
    this.loading = true;
    this.materialRequest.gethistoryMaterial(this.getQuery()).subscribe(res => {
      this.dataHistory = res.response.map((item, index) => ({ ...item, order: index + 1 }));
      this.lengthPaginator = res.comments;
    },
      this.errorService.httpError, () => {
        this.loading = false;
      });
  }

}
