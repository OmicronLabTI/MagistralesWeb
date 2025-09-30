import { Injectable } from '@angular/core';
import { ConstOrders, ConstStatus, CONST_NUMBER, CONST_STRING, FromToFilter, BoolConst } from '../constants/const';
import { ParamsPedidos } from '../model/http/pedidos';
import { DateService } from '../services/date.service';
import { DataService } from '../services/data.service';
import { ChildrenOrders } from '../model/http/detallepedidos.model';

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
    const childrenOrdersChecked = this.getChildrenOrdersChecked(dataToSearch);
    const dataChecked = this.getDataChecked(dataToSearch, t => t.isChecked);
    let evaluateChildrenOrders = false;
    let enableButton = BoolConst.false;
    switch (fromToFilter) {
      case FromToFilter.fromOrders:
        const evaluateStatusAndInProcessfromOrders = dataChecked.every(t =>
          this.dataService.calculateAndValueList([
            t.pedidoStatus === status,
            !t.onSplitProcess
          ])
        );
        enableButton = this.dataService.calculateAndValueList([
          evaluateStatusAndInProcessfromOrders,
          dataChecked.length !== 0
        ]);
        return enableButton;
      case FromToFilter.fromOrdersReassign:
        const evaluateStatusAndInProcessOrdersReassign = dataChecked.every(t =>
          this.dataService.calculateAndValueList([
            t.isChecked && (
              this.dataService.calculateOrValueList([
                t.pedidoStatus === status,
                t.pedidoStatus === ConstStatus.terminado
              ])
            ),
            !t.onSplitProcess
          ])
        );
        enableButton = this.dataService.calculateAndValueList([
          evaluateStatusAndInProcessOrdersReassign,
          dataChecked.length !== 0
        ]);
        return enableButton;
      case FromToFilter.fromOrdersCancel:
        const evaluateStatusAndInProcessOrdersCancel = dataChecked.every(t =>
          this.dataService.calculateAndValueList([
            t.isChecked,
            t.pedidoStatus !== status,
            t.pedidoStatus !== ConstStatus.cancelado,
            t.pedidoStatus !== ConstStatus.almacenado,
            t.pedidoStatus !== ConstStatus.rechazado,
            !t.onSplitProcess
          ])
        );
        enableButton = this.dataService.calculateAndValueList([
          evaluateStatusAndInProcessOrdersCancel,
          dataChecked.length !== 0
        ]);
        return enableButton;
      case FromToFilter.fromDetailOrder:
        const evaluateStatusAndInProcessDetailOrder = dataChecked.every(t =>
          this.dataService.calculateAndValueList([
            t.isChecked,
            t.status !== status,
            t.status !== ConstStatus.cancelado,
            t.status !== ConstStatus.abierto,
            t.status !== ConstStatus.almacenado,
            !t.onSplitProcess
          ])
        );
        evaluateChildrenOrders = this.evaluateChildrenForfromDetailOrderCase(childrenOrdersChecked, status);
        enableButton = this.dataService.calculateOrValueList([
          this.dataService.calculateAndValueList([
            evaluateStatusAndInProcessDetailOrder,
            dataChecked.length !== 0
          ]),
          this.dataService.calculateAndValueList([
            evaluateStatusAndInProcessDetailOrder,
            evaluateChildrenOrders,
            childrenOrdersChecked.length !== 0
          ])
        ]);
        return enableButton;
      case FromToFilter.fromOrderIsolatedReassign:
        const evaluateStatusAndInProcessIsolatedReassig = dataChecked.every(t =>
          this.dataService.calculateAndValueList([
            this.dataService.calculateOrValueList([
              t.status === status,
              t.status === ConstStatus.asignado,
              t.status.toLowerCase() === ConstStatus.enProceso.toLowerCase(),
              t.status === ConstStatus.pendiente,
              t.status === ConstStatus.terminado,
              this.getValidParentOrderToReasign(t)
            ]),
            !t.onSplitProcess
          ])
        );
        evaluateChildrenOrders = this.evaluateChildrenForfromOrderIsolatedReassignCase(childrenOrdersChecked, status);
        enableButton = this.dataService.calculateOrValueList([
          this.dataService.calculateAndValueList([
            evaluateStatusAndInProcessIsolatedReassig,
            dataChecked.length !== 0
          ]),
          this.dataService.calculateAndValueList([
            evaluateStatusAndInProcessIsolatedReassig,
            evaluateChildrenOrders,
            childrenOrdersChecked.length !== 0
          ])
        ]);
        return enableButton;
      case FromToFilter.fromOrdersIsolatedCancel:
        const evaluateStatusAndInProcessIsolatedCancel = dataChecked.every(
          (t) =>
            this.dataService.calculateAndValueList([
              t.status !== status,
              t.status !== ConstStatus.cancelado,
              t.status !== ConstStatus.almacenado,
              !t.onSplitProcess
            ])
        );
        enableButton = this.dataService.calculateAndValueList([
          evaluateStatusAndInProcessIsolatedCancel,
          dataChecked.length !== 0
        ]);
        return enableButton;
      case FromToFilter.fromOrderDetailLabel:
        const evaluateStatusAndInProcessDetailLabel = dataChecked.every(
          (t) =>
            this.dataService.calculateAndValueList([
              t.isChecked,
              t.status !== status,
              t.status !== ConstStatus.cancelado,
              t.finishedLabel !== 1,
              !t.onSplitProcess
            ])
        );
        evaluateChildrenOrders = this.evaluateChildrenForfromOrderDetailLabelCase(childrenOrdersChecked, status);
        enableButton = this.dataService.calculateOrValueList([
          this.dataService.calculateAndValueList([
            evaluateStatusAndInProcessDetailLabel,
            dataChecked.length !== 0
          ]),
          this.dataService.calculateAndValueList([
            evaluateStatusAndInProcessDetailLabel,
            evaluateChildrenOrders,
            childrenOrdersChecked.length !== 0
          ]),
        ]);
        console.log(enableButton, evaluateChildrenOrders, evaluateStatusAndInProcessDetailLabel);
        return enableButton;
      default:
        const evaluateStatusAndInProcessDefault = dataChecked.every(
          (t) =>
            this.dataService.calculateAndValueList([
              t.status === status,
              !t.onSplitProcess
            ])
        );
        evaluateChildrenOrders = this.evaluateChildrenForDefaultCase(childrenOrdersChecked, status);
        enableButton = this.dataService.calculateOrValueList([
          this.dataService.calculateAndValueList([
            evaluateStatusAndInProcessDefault,
            dataChecked.length !== 0
          ]),
          this.dataService.calculateAndValueList([
            evaluateStatusAndInProcessDefault,
            evaluateChildrenOrders,
            childrenOrdersChecked.length !== 0
          ]),
        ]);
        return enableButton;
    }
  }

  evaluateChildrenForfromOrderDetailLabelCase(orders: ChildrenOrders[], statusToCompare: string): boolean {
    return orders.every(order =>
      this.dataService.calculateAndValueList([
        order.status !== statusToCompare,
        order.status !== ConstStatus.cancelado,
        order.finishedLabel !== 1
      ])
    );
  }

  evaluateChildrenForfromDetailOrderCase(orders: ChildrenOrders[], statusToCompare: string): boolean {
    return orders.every(order =>
      this.dataService.calculateAndValueList([
        order.status !== statusToCompare,
        order.status !== ConstStatus.cancelado,
        order.status !== ConstStatus.abierto,
        order.status !== ConstStatus.almacenado,
      ])
    );
  }

  evaluateChildrenForfromOrderIsolatedReassignCase(orders: ChildrenOrders[], statusToCompare: string): boolean {
    return orders.every(order =>
      this.dataService.calculateOrValueList([
        order.status === statusToCompare,
        order.status === ConstStatus.asignado,
        order.status.toLowerCase() === ConstStatus.enProceso.toLowerCase(),
        order.status === ConstStatus.pendiente,
        order.status === ConstStatus.terminado,
      ])
    );
  }

  evaluateChildrenForDefaultCase(orders: ChildrenOrders[], statusToCompare: string): boolean {
    return orders.every(order => this.compareStatus(order.status, statusToCompare));
  }

  compareStatus(orderStatus: string, statusToCompare: string): boolean {
    return orderStatus === statusToCompare;
  }

  getChildrenOrdersChecked(dataToSearch: any[]): ChildrenOrders[] {
    const childrenOrdersChecked = dataToSearch.map(parentOrder => parentOrder.childOrders
      .filter(childrenOrders => childrenOrders.isChecked))
      .reduce((acc, childrenOrders) => acc.concat(childrenOrders), []);
    return childrenOrdersChecked;
  }

  getValidParentOrderToReasign(data: any): boolean {
    const props = ['status', 'availablePieces'];
    const containsAllProps = props.every(x => data.hasOwnProperty(x));
    if (containsAllProps) {
      return data.status === ConstStatus.cancelado && data.availablePieces > 0;
    }

    return false;
  }

  getDataChecked = <T>(data: T[], checked: (prop: T) => boolean): T[] => data.filter(checked);

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
      (t) => this.dataService.calculateAndValueList([
        t.isChecked && (t.pedidoStatus === status || t.pedidoStatus === ConstStatus.terminado),
        !t.onSplitProcess
      ])
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
