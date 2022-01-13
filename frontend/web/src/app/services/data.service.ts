import { Injectable } from '@angular/core';
import {
  Colors,
  ColorsBarGraph,
  CONST_NUMBER,
  CONST_STRING,
  CONST_USER_DIALOG,
  ConstOrders,
  ConstStatus,
  FromToFilter,
  RouterPaths,
  TypeToSeeTap,
} from '../constants/const';
import { ParamsPedidos } from '../model/http/pedidos';
import { IncidentsGraphicsMatrix } from '../model/http/incidents.model';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class DataService {
  private isToSaVeAnything = false;
  constructor(private router: Router) { }

  setIsToSaveAnything(isToSave: boolean) {
    this.isToSaVeAnything = isToSave;
  }
  getIsToSaveAnything() {
    return this.isToSaVeAnything;
  }

  getfiniOrffin(resultSearchOrderModal: ParamsPedidos, date: string) {
    if (resultSearchOrderModal.dateType === ConstOrders.defaultDateInit) {
      return `?fini=${date}`;
    } else {
      return `?ffin=${date}`;
    }
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
            return `${data.labels[tooltipItem.index]
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
