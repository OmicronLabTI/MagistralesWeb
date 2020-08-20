import { TestBed } from '@angular/core/testing';
import { HttpClientModule } from '@angular/common/http';
import { UsersService } from './users.service';
import {DatePipe} from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
describe('UsersService', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [
      HttpClientTestingModule
    ],
    providers: [DatePipe]
  }));

  it('should be created', () => {
    const service: UsersService = TestBed.get(UsersService);
    expect(service).toBeTruthy();
  });
});
