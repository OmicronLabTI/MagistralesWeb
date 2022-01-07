import { Injectable } from '@angular/core';
import Swal, { SweetAlertIcon } from 'sweetalert2';
import {
  Colors,
  ColorsBarGraph,
  CONST_NUMBER,
  CONST_STRING,
  CONST_USER_DIALOG,
  ConstOrders,
  ConstStatus,
  FromToFilter,
  MessageType,
  RouterPaths,
  TypeToSeeTap,
} from '../constants/const';
import { CancelOrderReq, ParamsPedidos } from '../model/http/pedidos';
import { IncidentsGraphicsMatrix } from '../model/http/incidents.model';
import { Router } from '@angular/router';
import { DateService } from './date.service';

@Injectable({
  providedIn: 'root',
})
export class DataService {
  private isToSaVeAnything = false;
  constructor(private router: Router, private dateService: DateService) {}

  setIsToSaveAnything(isToSave: boolean) {
    this.isToSaVeAnything = isToSave;
  }
  getIsToSaveAnything() {
    return this.isToSaVeAnything;
  }
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

  getfiniOrffin(resultSearchOrderModal: ParamsPedidos, date: string) {
    if (resultSearchOrderModal.dateType === ConstOrders.defaultDateInit) {
      return `?fini=${date}`;
    } else {
      return `?ffin=${date}`;
    }
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
      queryString = this.getRangeOrders(
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
          queryString = this.getfiniOrffin(resultSearchOrderModal, rangeDate);
          filterDataOrders.dateFull = rangeDate;
        } else {
          queryString = this.getfiniOrffin(
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
  getFormattedNumber(numberToFormatted: any) {
    return new Intl.NumberFormat().format(Number(numberToFormatted));
  }


  openNewTapByUrl(url: string, typeToSeeTap: TypeToSeeTap, orderId?: number) {
    // let tapTitle = CONST_STRING.empty;
    // switch (typeToSeeTap) {
    //  case TypeToSeeTap.order:
    //    tapTitle = `Pedido ${orderId}`;
    //    break;
    //  case TypeToSeeTap.receipt:
    //    tapTitle = `Receta pedido ${orderId}`;
    //    break;
    // }
    // const prntWin = window.open();
    // prntWin.document.write('<html><head><title>' + tapTitle + '</title></head><body style="background-color: rgb(60, 61, 62)">'
    //     + '<embed width="100%" height="100%" name="plugin" src="' + url + '" '
    //     + 'type="application/pdf" internalinstanceid="21" /></body></html>');
    // prntWin.document.close();
    const link = document.createElement('a');
    link.target = '_blank';
    link.href = url;
    link.click();
    link.remove();
  }

  getItemOnDataOnlyIds(dataToSearch: any[], type: FromToFilter) {
    switch (type) {
      case FromToFilter.fromOrders:
        return dataToSearch
          .filter(
            (t) => t.isChecked && t.pedidoStatus === ConstStatus.planificado
          )
          .map((t) => t.docNum);
      case FromToFilter.fromDetailOrder:
        return dataToSearch
          .filter((t) => t.isChecked && t.status === ConstStatus.planificado)
          .map((order) => order.ordenFabricacionId);
      case FromToFilter.fromDetailOrderQr:
        return dataToSearch
          .filter(
            (t) =>
              t.isChecked &&
              t.status !== ConstStatus.abierto &&
              t.status !== ConstStatus.cancelado
          )
          .map((order) => order.ordenFabricacionId);
      case FromToFilter.fromOrdersIsolated:
        return dataToSearch
          .filter((t) => t.isChecked && t.status === ConstStatus.planificado)
          .map((order) => Number(order.fabOrderId));
    }
  }
  getNormalizeString(valueToNormalize: string) {
    return valueToNormalize.normalize('NFD').replace(/[\u0300-\u036f]/g, '');
  }
  getOptionsGraphToShow = (isPie: boolean, titleForGraph: string) => ({
    tooltips: {
      callbacks: {
        label: (tooltipItem, data) => {
          if (Boolean(isPie)) {
            return `${
              data.labels[tooltipItem.index]
            }: ${this.getPercentageByItem(
              data.datasets[0].data[tooltipItem.index],
              data.datasets[0].data
            )}`;
          } else {
            return `${data.datasets[0].data[tooltipItem.index]}`;
          }
        },
      },
    },
    legend: { display: false },
    title: {
      display: true,
      text: titleForGraph,
    },
    plugins: {
      labels: isPie
        ? [
            {
              render: 'label',
              precision: 2,
              position: 'outside',
            },
          ]
        : [],
    },
    scales: {
      yAxes: !isPie
        ? [
            {
              ticks: {
                beginAtZero: true,
              },
            },
          ]
        : [],
    },
  })
  getPercentageByItem(
    valueItem: number,
    valuesArray: number[],
    isOnlyNumberPercent: boolean = false
  ) {
    if (!isOnlyNumberPercent) {
      return `${Math.round(
        (valueItem / valuesArray.reduce((a, b) => a + b, 0)) * 100
      )} %`;
    } else {
      return Math.round(
        (valueItem / valuesArray.reduce((a, b) => a + b, 0)) * 100
      );
    }
  }
  getDataForGraphic = (
    itemsArray: IncidentsGraphicsMatrix[],
    isBarGraph: boolean
  ) => ({
    labels: itemsArray.map((item) => item.fieldKey),
    datasets: [
      {
        backgroundColor: this.getRandomColorsArray(
          itemsArray.length,
          isBarGraph
        ),
        data: itemsArray.map((item) => item.totalCount),
        borderColor: '#fff',
        borderWidth: 3,
        hoverBorderWidth: 10,
        hoverBorderColor: '#c0c8ce',
      },
    ],
  })
  getRandomColorsArray(lengthArrayForGraph: number, isBarGraph: boolean) {
    let countIndex = CONST_NUMBER.zero;
    const range = Colors.length;
    const colorsArray = isBarGraph ? ColorsBarGraph : Colors;

    let colorsString: string[] = [];
    for (let i = 0; i < lengthArrayForGraph; i++) {
      if (range === countIndex) {
        countIndex = CONST_NUMBER.zero;
      }
      colorsString = [...colorsString, colorsArray[countIndex]];
      countIndex++;
    }
    return colorsString;
  }

  changeRouterForFormula(
    ordenFabricacionId: string,
    ordersIds: string,
    isFromOrders: number
  ) {
    this.router.navigate([
      RouterPaths.detailFormula,
      ordenFabricacionId,
      ordersIds,
      isFromOrders,
    ]);
  }
  getFullStringForCarousel(
    baseQueryString: string,
    currentOrder: string,
    optionsCarousel: string
  ) {
    return `${baseQueryString}&current=${currentOrder}&advance=${optionsCarousel}`;
  }

  getRangeOrders(docNum: any, docNumUntil: any) {
    if (
      docNum === docNumUntil ||
      docNumUntil === CONST_STRING.empty ||
      !docNumUntil
    ) {
      return `?docNum=${docNum}-${docNum}`;
    } else {
      return `?docNum=${docNum}-${docNumUntil}`;
    }
  }
  inputNumbersOnly(event): boolean {
    return CONST_USER_DIALOG.patternOnlyNumbers.test(event.key);
  }
}
