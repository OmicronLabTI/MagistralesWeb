import { TestBed } from '@angular/core/testing';
import { DatePipe } from '@angular/common';
import { DataService } from './data.service';
import { Catalogs } from '../model/http/pedidos';
import { RouterTestingModule } from '@angular/router/testing';

describe('DataService', () => {
  const catalogs = new Catalogs();
  catalogs.id = 74;
  catalogs.value = 'DZ';
  catalogs.type = 'string';
  catalogs.field = 'ProductNoLabel';

  beforeEach(() => TestBed.configureTestingModule({
    imports: [RouterTestingModule],
    providers: [DatePipe]
  }));

  it('should be created', () => {
    const service: DataService = TestBed.get(DataService);
    expect(service).toBeTruthy();
  });

  it('should getIsToSaveAnything', () => {
    const service: DataService = TestBed.get(DataService);
    service.setIsToSaveAnything(false);
    expect(service.getIsToSaveAnything()).toBeFalsy();
    service.setIsToSaveAnything(true);
    expect(service.getIsToSaveAnything()).toBeTruthy();
  });
});
