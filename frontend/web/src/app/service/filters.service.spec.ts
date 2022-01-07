import { TestBed } from '@angular/core/testing';
import { ConstStatus, FromToFilter } from '../constants/const';

import { FiltersService } from './filters.service';
import { DatePipe } from '@angular/common';
import { RouterTestingModule } from '@angular/router/testing';

describe('FiltersService', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [
      RouterTestingModule
    ],
    providers: [
      DatePipe,
    ]
  }));

  it('should be created', () => {
    const service: FiltersService = TestBed.get(FiltersService);
    expect(service).toBeTruthy();
  });
  it('should getIsThereOnData', () => {
    const service: FiltersService = TestBed.get(FiltersService);
    expect(service.getIsThereOnData([], ConstStatus.cancelado, FromToFilter.fromOrdersIsolatedCancel)).toBeFalsy();
  });
  it('should getItemOnDateWithFilter', () => {
    const service: FiltersService = TestBed.get(FiltersService);
    expect(service.getItemOnDateWithFilter([], FromToFilter.fromOrderIsolatedReassignItems, ConstStatus.cancelado).length).toEqual(0);
    expect(service.getItemOnDateWithFilter([], FromToFilter.fromOrdersReassign, ConstStatus.cancelado).length).toEqual(0);
    expect(service.getItemOnDateWithFilter([], FromToFilter.fromDefault, ConstStatus.cancelado).length).toEqual(0);

  });
});
