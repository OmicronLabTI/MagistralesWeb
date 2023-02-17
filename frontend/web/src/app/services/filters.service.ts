import { Injectable } from '@angular/core';
import { ConstOrders, ConstStatus, CONST_NUMBER, CONST_STRING, FromToFilter } from '../constants/const';
import { ParamsPedidos } from '../model/http/pedidos';
import { DateService } from '../services/date.service';
import { DataService } from '../services/data.service';

@Injectable({
  providedIn: 'root'
})
export class FiltersService {

  constructor(
    private dateService: DateService,
    private dataService: DataService,
  ) { }

  getIsThereOnData(
    dataToSearch: any[],
    status: string,
    fromToFilter: FromToFilter
  ) {
    switch (fromToFilter) {
      case FromToFilter.fromOrders:
        return (
          dataToSearch.filter((t) => this.dataService.calculateAndValueList([t.isChecked, t.pedidoStatus === status]))
            .length > 0
        );
      case FromToFilter.fromOrdersReassign:
        return (this.getValidationFromOrderReassign(dataToSearch, status).length > 0);
      case FromToFilter.fromOrdersCancel:
        return (
          dataToSearch.filter(
            (t) =>
              this.dataService.calculateAndValueList([
                t.isChecked,
                t.pedidoStatus !== status,
                t.pedidoStatus !== ConstStatus.cancelado,
                t.pedidoStatus !== ConstStatus.almacenado,
                t.pedidoStatus !== ConstStatus.rechazado
              ])
          ).length > 0
        );
      case FromToFilter.fromDetailOrder:
        return (
          dataToSearch.filter(
            (t) =>
              this.dataService.calculateAndValueList([
                t.isChecked,
                t.status !== status,
                t.status !== ConstStatus.cancelado,
                t.status !== ConstStatus.abierto,
                t.status !== ConstStatus.almacenado
              ])
          ).length > 0
        );
      case FromToFilter.fromOrderIsolatedReassign:
        return (
          dataToSearch.filter(
            (t) =>
              t.isChecked &&
              (t.status === status ||
                t.status === ConstStatus.asignado ||
                t.status.toLowerCase() ===
                ConstStatus.enProceso.toLowerCase() ||
                t.status === ConstStatus.pendiente ||
                t.status === ConstStatus.terminado)
          ).length > 0
        );
      case FromToFilter.fromOrdersIsolatedCancel:
        return (
          dataToSearch.filter(
            (t) =>
              this.dataService.calculateAndValueList([
                t.isChecked,
                t.status !== status,
                t.status !== ConstStatus.cancelado,
                t.status !== ConstStatus.almacenado
              ])
          ).length > 0
        );
      case FromToFilter.fromOrderDetailLabel:
        return (
          dataToSearch.filter(
            (t) =>
              this.dataService.calculateAndValueList([
                t.isChecked,
                t.status !== status,
                t.status !== ConstStatus.cancelado,
                t.finishedLabel !== 1
              ])
          ).length > 0
        );
      default:
        return (
          dataToSearch.filter((t) => this.dataService.calculateAndValueList([t.isChecked, t.status === status]))
            .length > 0
        );
    }
  }
  getItemOnDateWithFilter(
    dataToSearch: any[],
    fromToFilter: FromToFilter,
    status?: string
  ) {
    switch (fromToFilter) {
      case FromToFilter.fromOrderIsolatedReassignItems:
        return dataToSearch.filter(
          (t) =>
            t.isChecked &&
            (t.status === ConstStatus.reasingado ||
              t.status === ConstStatus.asignado ||
              t.status.toLowerCase() === ConstStatus.enProceso.toLowerCase() ||
              t.status === ConstStatus.pendiente ||
              t.status === ConstStatus.terminado)
        );
      case FromToFilter.fromOrdersReassign:
        return this.getValidationFromOrderReassign(dataToSearch, status);
      default:
        return dataToSearch.filter((t) => t.isChecked && t.status === status);
    }
  }

  getValidationFromOrderReassign(dataToSearch: any[], status: string): any[] {
    return dataToSearch.filter(
      (t) =>
        t.isChecked &&
        (t.pedidoStatus === status ||
          t.pedidoStatus === ConstStatus.terminado)
    );
  }

  getIsWithFilter(resultSearchOrderModal: ParamsPedidos) {
    if (resultSearchOrderModal) {
      return this.validateIsWithFilter(resultSearchOrderModal);
    }
    return false;
  }

  validateIsWithFilter(resultSearchOrderModal: ParamsPedidos): boolean {
    let isSearchWithFilter = false;
    const isSomeEmpty = this.isSomeWithValue(resultSearchOrderModal, (key) => resultSearchOrderModal[key] === '');
    const isSomeNotEmpty = this.isSomeWithValue(resultSearchOrderModal, (key) => resultSearchOrderModal[key] !== '');

    if (this.dataService.calculateAndValueList([resultSearchOrderModal.dateType === ConstOrders.defaultDateInit,
    (isSomeEmpty || !resultSearchOrderModal.orderIncidents)])) {
      isSearchWithFilter = false;
    }
    if (resultSearchOrderModal.dateType === ConstOrders.defaultDateInit &&
      (isSomeNotEmpty || resultSearchOrderModal.orderIncidents)) {
      isSearchWithFilter = true;
    }
    if (resultSearchOrderModal.dateType === ConstOrders.dateFinishType &&
      (isSomeNotEmpty || resultSearchOrderModal.orderIncidents)) {
      isSearchWithFilter = true;
    }
    if (this.dataService.calculateAndValueList([resultSearchOrderModal.dateType === ConstOrders.dateFinishType,
    (isSomeEmpty || !resultSearchOrderModal.orderIncidents)])) {
      isSearchWithFilter = true;
    }
    isSearchWithFilter = this.dataService.calculateTernary(resultSearchOrderModal.docNum !== '',
      true,
      isSearchWithFilter);
    return isSearchWithFilter;
  }

  isSomeWithValue(resultSearchOrderModal: ParamsPedidos, validation: (key: string) => boolean): boolean {
    const keys = ['status', 'qfb', 'productCode', 'clientName', 'label', 'finlabel'];
    return keys.some((key) => resultSearchOrderModal[key] && validation(key));
  }
  getNewDataToFilter(
    resultSearchOrderModal: ParamsPedidos
  ): [ParamsPedidos, string] {
    let queryString = CONST_STRING.empty;
    let rangeDate = CONST_STRING.empty;

    const filterDataOrders = new ParamsPedidos();
    filterDataOrders.isFromOrders = resultSearchOrderModal.isFromOrders;
    filterDataOrders.isFromIncidents = resultSearchOrderModal.isFromIncidents;

    if (resultSearchOrderModal.docNum) {
      filterDataOrders.docNum = resultSearchOrderModal.docNum;
      filterDataOrders.dateFull = this.dateService.getDateFormatted(
        new Date(),
        new Date(),
        true
      );
      filterDataOrders.docNumUntil = resultSearchOrderModal.docNumUntil;
      queryString = this.dataService.getRangeOrders(
        resultSearchOrderModal.docNum,
        resultSearchOrderModal.docNumUntil
      );
    } else if (resultSearchOrderModal.docNumDxp) {
      filterDataOrders.docNumDxp = resultSearchOrderModal.docNumDxp;
      filterDataOrders.dateFull = this.dateService.getDateFormatted(
        new Date(),
        new Date(),
        true
      );
      queryString = `?docNumDxp=${resultSearchOrderModal.docNumDxp}`;
    } else {
      if (resultSearchOrderModal.dateType) {
        filterDataOrders.dateType = resultSearchOrderModal.dateType;
        if (resultSearchOrderModal.fini || resultSearchOrderModal.ffin) {
          rangeDate = this.dateService.getDateFormatted(
            resultSearchOrderModal.fini,
            resultSearchOrderModal.ffin,
            false
          );
          queryString = this.dataService.getfiniOrffin(resultSearchOrderModal, rangeDate);
          filterDataOrders.dateFull = rangeDate;
        } else {
          queryString = this.dataService.getfiniOrffin(
            resultSearchOrderModal,
            resultSearchOrderModal.dateFull
          );
          filterDataOrders.dateFull = resultSearchOrderModal.dateFull;
        }
      }
      queryString = this.continueGetDataOrders(resultSearchOrderModal, queryString, filterDataOrders);
    }
    return [filterDataOrders, queryString];
  }

  getValidateHasValue(
    resultSearchOrderModal: ParamsPedidos,
    key: string): boolean {
    return Object.keys(resultSearchOrderModal).includes(key) &&
      resultSearchOrderModal[key] !== '' &&
      resultSearchOrderModal[key] !== undefined;
  }

  getQueryStringAndAssingValue(
    query: string,
    resultSearchOrderModal: ParamsPedidos,
    filterDataOrders: ParamsPedidos,
    key: string): string {
    const dictOptions: { [key: string]: string } = {
      status: 'status',
      qfb: 'qfb',
      productCode: 'code',
      label: 'label',
      finlabel: 'finlabel',
      clasification: 'ordtype',
      '': ''
    };
    if (this.getValidateHasValue(resultSearchOrderModal, key)) {
      filterDataOrders[key] = resultSearchOrderModal[key];
      return `${query}&${dictOptions[key]}=${resultSearchOrderModal[key]}`;
    }
    return query;
  }

  continueGetDataOrders(
    resultSearchOrderModal: ParamsPedidos,
    queryString: string,
    filterDataOrders: ParamsPedidos): string {
    const keys = ['status', 'qfb', 'productCode', 'label', 'finlabel', 'clasification'];
    const completeQuery = keys.reduce((query, key) =>
      this.getQueryStringAndAssingValue(
        query,
        resultSearchOrderModal,
        filterDataOrders,
        key), CONST_STRING.empty);
    queryString = `${queryString}${completeQuery}`;
    if (
      resultSearchOrderModal.clientName !== '' &&
      resultSearchOrderModal.clientName
    ) {
      queryString = `${queryString}&cliente=${resultSearchOrderModal.clientName.replace(
        /\s+/g,
        ','
      )}`;
      filterDataOrders.clientName = resultSearchOrderModal.clientName;
    }
    if (
      resultSearchOrderModal.orderIncidents !== CONST_NUMBER.zero &&
      resultSearchOrderModal.orderIncidents
    ) {
      queryString = `${queryString}&docnum=${resultSearchOrderModal.orderIncidents}`;
      filterDataOrders.orderIncidents = resultSearchOrderModal.orderIncidents;
    }
    return queryString;
  }
}
