import { CONST_ARRAY, CONST_NUMBER } from 'src/app/constants/const';
import { IncidentsGraphicsMatrix } from '../http/incidents.model';

export class ConfigurationGraphic {
    isPie = true;
    titleForGraph: string;
    dataGraph: IncidentsGraphicsMatrix[] = CONST_ARRAY.empty;
    isSmall: boolean;
    isWithFullTooltip: boolean;
    isWithNumberTooltip?: boolean;
}
export class ItemIndicator {
    nameItem: string;
    background: string;
    percentage: string;
    count: number;
}
export class CommentsConfig {
    comments: string;
    isReadOnly?: boolean;
    isForClose?: boolean;
    isForRefuseOrders?: boolean;
}

