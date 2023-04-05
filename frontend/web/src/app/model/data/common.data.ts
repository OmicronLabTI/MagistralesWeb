import { CallBackEmptyConst, CONST_STRING } from 'src/app/constants/const';

export class SettingsCommonTableClass {
    className: string = CONST_STRING.empty;
    emptyLabel: string = CONST_STRING.notResults;
    stickyColumns: Array<SettingTableItem> = [];
    columns: Array<SettingTableItem> = [];
}

interface SettingTableItem {
    className: string;
    title: string;
    attr: string;
    short?: boolean;
    type?: TTableCellType;
    hide?: boolean;
}

type TTableCellType = 'text' | 'action' | 'date' | 'badge';

export type TRowTable = { [key in string]: string | number | ActionTableCell };

export interface TRowTableHandler {
    action: string;
    row: TRowTable;
    index: number;
}
export const tableCellType: { [key in TTableCellType]: TTableCellType } = {
    text: 'text',
    action: 'action',
    date: 'date',
    badge: 'badge'
};

export class ActionTableCell {
    label: string = CONST_STRING.empty;
    className: string = CONST_STRING.empty;
    action: CallableFunction = CallBackEmptyConst;
}
