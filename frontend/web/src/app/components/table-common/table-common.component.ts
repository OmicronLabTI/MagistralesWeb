import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { BoolConst, CallBackEmptyConst, CONST_STRING } from 'src/app/constants/const';

export class SettingsCommonTableClass {
  className: string = CONST_STRING.empty;
  emptyLabel: string = "No Hay resultados";
  stickyColumns: Array<SettingTableItem> = [];
  columns: Array<SettingTableItem> = [];
}

type SettingTableItem = {
  className: string;
  title: string;
  attr: string;
  short?: boolean;
  type?: TTableCellType;
  hide?: boolean;
};

type TTableCellType = 'text' | 'action' | 'date';

export type TRowTable = { [key in string]: string | number | ActionTableCell };

export type TRowTableHandler = {
  action: string;
  row: TRowTable;
  index: number;
};
export const tableCellType: { [key in TTableCellType]: TTableCellType } = {
  text: 'text',
  action: 'action',
  date: 'date',
};

export class ActionTableCell {
  label: string = CONST_STRING.empty;
  className: string = CONST_STRING.empty;
  action: CallableFunction = CallBackEmptyConst;
}

@Component({
  selector: 'app-table-common',
  templateUrl: './table-common.component.html',
  styleUrls: ['./table-common.component.scss']
})
export class TableCommonComponent implements OnInit {
  @Input() settings: SettingsCommonTableClass = new SettingsCommonTableClass();
  @Input() data: Array<TRowTable> = [];
  @Input() loading: boolean = BoolConst.false;
  @Output() handleAction: EventEmitter<TRowTableHandler> = new EventEmitter<TRowTableHandler>();
  CellTypes = tableCellType;
  constructor() { }

  ngOnInit(): void { }

  valueCell = (row: TRowTable, attr: string): string => {
    return this.calculateTernary(typeof row[attr] === 'object', '--', row[attr]);
  }

  actionCell = (row: TRowTable, attr: string): ActionTableCell => {
    return this.calculateTernary(typeof row[attr] !== 'object', new ActionTableCell(), row[attr]);
  }

  handleActionCell = (row: TRowTable, action: string, index: number): void => {
    /*const handle = this.actionCell(row, action).action ?? CallBackEmptyConst;
    //handle(row);
    this.handleAction.emit({ row, action, index });*/
  }

  calculateTernary = (
    validation: boolean,
    firstValue: any,
    secondaValue: any
  ): any => (validation ? firstValue : secondaValue)

}


