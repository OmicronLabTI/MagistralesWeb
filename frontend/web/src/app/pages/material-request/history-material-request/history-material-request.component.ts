import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { PageEvent } from '@angular/material';
import { BoolConst, CONST_NUMBER } from 'src/app/constants/const';
import { SettingsCommonTableClass } from 'src/app/model/data/common.data';
import { MaterialRequestHistoryTableSettings } from 'src/app/model/data/materialRequestHistory';
import { IMaterialHistoryItem } from 'src/app/model/http/materialReques';
import { DataService } from 'src/app/services/data.service';
import { ErrorService } from 'src/app/services/error.service';
import { LocalStorageService } from 'src/app/services/local-storage.service';
import { MaterialRequestService } from 'src/app/services/material-request.service';

@Component({
  selector: 'app-history-material-request',
  templateUrl: './history-material-request.component.html',
  styleUrls: ['./history-material-request.component.scss']
})
export class HistoryMaterialRequestComponent implements OnInit, OnDestroy {
  historyMaterialRequestSettings: SettingsCommonTableClass = MaterialRequestHistoryTableSettings;
  dataHistory: IMaterialHistoryItem[] = [];
  lengthPaginator = CONST_NUMBER.zero;
  offset: number = CONST_NUMBER.zero;
  pageIndex: number = CONST_NUMBER.zero;
  limit: number = CONST_NUMBER.ten;
  statusControl: FormControl = new FormControl(['Abierto']);
  loading: boolean = BoolConst.false;
  minDate: Date = null;
  maxDate: Date = new Date();
  today: Date = new Date();
  date: FormControl;
  @ViewChild('picker2', { static: true }) picker;
  constructor(
    private materialRequest: MaterialRequestService,
    private errorService: ErrorService,
    private dataService: DataService,
    private localStorageService: LocalStorageService
  ) {
    this.date = new FormControl({ begin: this.getWeekOneWeekDate(this.maxDate, -6), end: this.maxDate });
  }

  ngOnInit() {
    this.picker.openedStream.subscribe(this.onOpenDate);
    this.picker.setBeginDateSelected = this.updateMaxDate;
    this.loadFilters();
    this.historyMaterialRequest();
  }

  ngOnDestroy(): void {
    this.localStorageService.setMaterialHistoryQuery({
      start: this.date.value.begin,
      end: this.date.value.end,
      offset: this.offset,
      limit: this.limit,
      status: this.statusControl.value.toString()
    });
  }

  loadFilters = () => {
    const { limit, offset, status, start, end } = this.localStorageService.getMaterialHistoryQuery();
    this.limit = limit;
    this.statusControl.setValue(status.split(','));
    this.offset = offset;
    if (start !== '') {
      this.date.setValue({
        begin: new Date(start), end: new Date(end)
      });
    }
  }

  filterChange = () => {
    if (this.date.value) {
      this.offset = 0;
      this.pageIndex = 0;
      this.historyMaterialRequest();
    }
  }

  onOpenDate = () => {
    this.minDate = null;
    this.maxDate = this.today;
  }
  getIsDisabled(status: string): boolean {
    if ((this.statusControl.value as string[]).length <= 1) {
      return status.toUpperCase() === String(this.statusControl.value[0]).toUpperCase();
    }
    return false;
  }

  updateMaxDate = (dateStart: Date) => {
    this.minDate = dateStart;
    const maxDateAux = this.getWeekOneWeekDate(dateStart, 6);
    this.maxDate = maxDateAux > this.today ? this.today : maxDateAux;
  }

  getQuery = () => {
    const status = this.statusControl.value.toString();
    const start = this.date.value.begin;
    const end = this.date.value.end;
    return `?offset=${this.offset}&limit=${this.limit}
      &fini=${this.dataService.getDateFilterFormat(start)}&ffin=${this.dataService.getDateFilterFormat(end)}&status=${status}`;
  }

  changeDataEvent(event: PageEvent) {
    this.pageIndex = event.pageIndex;
    this.offset = (event.pageSize * (event.pageIndex));
    this.limit = event.pageSize;
    this.historyMaterialRequest();
    return event;
  }

  getWeekOneWeekDate = (date: Date, days: number) => {
    const dateAux = new Date(date);
    dateAux.setDate(dateAux.getDate() + days);
    return dateAux;
  }

  historyMaterialRequest = () => {
    this.loading = true;
    this.materialRequest.gethistoryMaterial(this.getQuery()).subscribe(res => {
      this.dataHistory = res.response.map(({ quantity, ...others }, index) =>
      ({
        ...others,
        order: this.offset + index + 1,
        quantity: String(quantity.toLocaleString('en-US'))
      }));
      this.lengthPaginator = res.comments;
      this.loading = false;
    },
      err => {
        this.errorService.httpError(err);
        this.loading = false;
      });
  }
}
