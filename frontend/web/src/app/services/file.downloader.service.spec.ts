import { TestBed } from '@angular/core/testing';
import { DatePipe } from '@angular/common';
import { FileDownloaderService } from './file.downloader.service';

describe('File Downloader Service Spec', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [],
    providers: [
      DatePipe,
    ]
  }));

  it('should be created', () => {
    const service: FileDownloaderService = TestBed.get(FileDownloaderService);
    expect(service).toBeTruthy();
  });
});
