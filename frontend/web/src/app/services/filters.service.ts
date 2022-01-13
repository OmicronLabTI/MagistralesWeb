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
          dataToSearch.filter((t) => t.isChecked && t.pedidoStatus === status)
            .length > 0
        );
      case FromToFilter.fromOrdersReassign:
        return (
          dataToSearch.filter(
            (t) =>
              t.isChecked &&
              (t.pedidoStatus === status ||
                t.pedidoStatus === ConstStatus.terminado)
          ).length > 0
        );
      case FromToFilter.fromOrdersCancel:
        return (
          dataToSearch.filter(
            (t) =>
              t.isChecked &&
              t.pedidoStatus !== status &&
              t.pedidoStatus !== ConstStatus.cancelado &&
              t.pedidoStatus !== ConstStatus.almacenado &&
              t.pedidoStatus !== ConstStatus.rechazado
          ).length > 0
        );
      case FromToFilter.fromDetailOrder:
        return (
          dataToSearch.filter(
            (t) =>
              t.isChecked &&
              t.status !== status &&
              t.status !== ConstStatus.cancelado &&
              t.status !== ConstStatus.abierto &&
              t.status !== ConstStatus.almacenado
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
              t.isChecked &&
              t.status !== status &&
              t.status !== ConstStatus.cancelado &&
              t.status !== ConstStatus.almacenado
          ).length > 0
        );
      case FromToFilter.fromOrderDetailLabel:
        return (
          dataToSearch.filter(
            (t) =>
              t.isChecked &&
              t.status !== status &&
              t.status !== ConstStatus.cancelado &&
              t.finishedLabel !== 1
          ).length > 0
        );
      default:
        return (
          dataToSearch.filter((t) => t.isChecked && t.status === status)
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
        return dataToSearch.filter(
          (t) =>
            t.isChecked &&
            (t.pedidoStatus === status ||
              t.pedidoStatus === ConstStatus.terminado)
        );
      default:
        return dataToSearch.filter((t) => t.isChecked && t.status === status);
    }
  }
  getIsWithFilter(resultSearchOrderModal: ParamsPedidos) {
    let isSearchWithFilter = false;
    if (
      resultSearchOrderModal &&
      resultSearchOrderModal.dateType === ConstOrders.defaultDateInit &&
      ((resultSearchOrderModal && resultSearchOrderModal.status === '') ||
        resultSearchOrderModal.qfb === '' ||
        resultSearchOrderModal.productCode === '' ||
        resultSearchOrderModal.clientName === '' ||
        resultSearchOrderModal.label === '' ||
        resultSearchOrderModal.finlabel === '' ||
        !resultSearchOrderModal.orderIncidents)
    ) {
      isSearchWithFilter = false;
    }
    if (
      resultSearchOrderModal &&
      resultSearchOrderModal.dateType === ConstOrders.defaultDateInit &&
      ((resultSearchOrderModal && resultSearchOrderModal.status !== '') ||
        resultSearchOrderModal.qfb !== '' ||
        resultSearchOrderModal.productCode !== '' ||
        resultSearchOrderModal.clientName !== '' ||
        resultSearchOrderModal.label !== '' ||
        resultSearchOrderModal.finlabel !== '' ||
        resultSearchOrderModal.orderIncidents)
    ) {
      isSearchWithFilter = true;
    }
    if (
      resultSearchOrderModal &&
      resultSearchOrderModal.dateType === ConstOrders.dateFinishType &&
      ((resultSearchOrderModal && resultSearchOrderModal.status !== '') ||
        resultSearchOrderModal.qfb !== '' ||
        resultSearchOrderModal.productCode !== '' ||
        resultSearchOrderModal.clientName !== '' ||
        resultSearchOrderModal.label !== '' ||
        resultSearchOrderModal.finlabel !== '' ||
        resultSearchOrderModal.orderIncidents)
    ) {
      isSearchWithFilter = true;
    }
    if (
      resultSearchOrderModal &&
      resultSearchOrderModal.dateType === ConstOrders.dateFinishType &&
      ((resultSearchOrderModal && resultSearchOrderModal.status === '') ||
        resultSearchOrderModal.qfb === '' ||
        resultSearchOrderModal.productCode === '' ||
        resultSearchOrderModal.clientName === '' ||
        resultSearchOrderModal.label === '' ||
        resultSearchOrderModal.finlabel === '' ||
        !resultSearchOrderModal.orderIncidents)
    ) {
      isSearchWithFilter = true;
    }
    if (resultSearchOrderModal && resultSearchOrderModal.docNum !== '') {
      isSearchWithFilter = true;
    }

    return isSearchWithFilter;
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
      if (
        resultSearchOrderModal.status !== '' &&
        resultSearchOrderModal.status
      ) {
        queryString = `${queryString}&status=${resultSearchOrderModal.status}`;
        filterDataOrders.status = resultSearchOrderModal.status;
      }
      if (resultSearchOrderModal.qfb !== '' && resultSearchOrderModal.qfb) {
        queryString = `${queryString}&qfb=${resultSearchOrderModal.qfb}`;
        filterDataOrders.qfb = resultSearchOrderModal.qfb;
      }
      if (
        resultSearchOrderModal.productCode !== '' &&
        resultSearchOrderModal.productCode
      ) {
        queryString = `${queryString}&code=${resultSearchOrderModal.productCode}`;
        filterDataOrders.productCode = resultSearchOrderModal.productCode;
      }
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
      if (resultSearchOrderModal.label !== '' && resultSearchOrderModal.label) {
        queryString = `${queryString}&label=${resultSearchOrderModal.label}`;
        filterDataOrders.label = resultSearchOrderModal.label;
      }
      if (
        resultSearchOrderModal.finlabel !== '' &&
        resultSearchOrderModal.finlabel
      ) {
        queryString = `${queryString}&finlabel=${resultSearchOrderModal.finlabel}`;
        filterDataOrders.finlabel = resultSearchOrderModal.finlabel;
      }
      if (
        resultSearchOrderModal.orderIncidents !== CONST_NUMBER.zero &&
        resultSearchOrderModal.orderIncidents
      ) {
        queryString = `${queryString}&docnum=${resultSearchOrderModal.orderIncidents}`;
        filterDataOrders.orderIncidents = resultSearchOrderModal.orderIncidents;
      }
      if (
        resultSearchOrderModal.clasification !== '' &&
        resultSearchOrderModal.clasification
      ) {
        queryString = `${queryString}&ordtype=${resultSearchOrderModal.clasification}`;
        filterDataOrders.clasification = resultSearchOrderModal.clasification;
      }
    }

    return [filterDataOrders, queryString];
  }
}
