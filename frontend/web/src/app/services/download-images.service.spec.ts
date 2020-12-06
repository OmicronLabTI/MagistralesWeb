import { TestBed } from '@angular/core/testing';

import { DownloadImagesService } from './download-images.service';
import {HttpClientTestingModule} from '@angular/common/http/testing';
import {DatePipe} from '@angular/common';

describe('DownloadImagesService', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [
      HttpClientTestingModule,
    ],
    providers: [DatePipe]
  }));

  it('should be created', () => {
    const service: DownloadImagesService = TestBed.get(DownloadImagesService);
    expect(service).toBeTruthy();
  });

});
