import {IncidentsGraphicsMatrix} from '../http/incidents.model';

export class ConfigurationGraphic {
    isPie: boolean;
    titleForGraph: string;
    dataGraph: IncidentsGraphicsMatrix[];
    isSmall: boolean;
    isWithFullTooltip: boolean;
}
export class ItemIndicator {
    nameItem: string;
    background: string;
    percentage: string;
    count: number;
}

