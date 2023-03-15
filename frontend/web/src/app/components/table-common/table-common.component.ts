import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { BoolConst } from 'src/app/constants/const';
import { ActionTableCell, SettingsCommonTableClass, tableCellType, TRowTable, TRowTableHandler } from 'src/app/model/data/common.data';

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

  calculateTernary = (
    validation: boolean,
    firstValue: any,
    secondaValue: any
  ): any => (validation ? firstValue : secondaValue)

}


