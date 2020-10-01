import { ChangeDetectorRef } from '@angular/core';
import { Component, Inject, OnInit, ViewChild  } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { SignaturePad } from 'angular2-signaturepad/signature-pad';

@Component({
  selector: 'app-request-signature-dialog',
  templateUrl: './request-signature-dialog.component.html',
  styleUrls: ['./request-signature-dialog.component.scss']
})
export class RequestSignatureDialogComponent implements OnInit {

  @ViewChild(SignaturePad, {static: false}) signaturePad: SignaturePad;
  public isValidSignature: boolean;
  public signaturePadOptions = {
    minWidth: 2,
    canvasWidth: 500,
    canvasHeight: 300
  };

  constructor(
    @Inject(MAT_DIALOG_DATA) private data: string,
    private dialogRef: MatDialogRef<RequestSignatureDialogComponent>,
    private changeDetector: ChangeDetectorRef
  ) { 
  }

  ngOnInit() {
  }

  ngAfterViewInit() {
    this.reset();
    if (this.data !== null && this.data !== undefined && this.data !== '') {
      this.signaturePad.fromDataURL(`data:image/png;base64,${this.data}`);
      this.validateSignature();
    }
  }

  drawComplete() {
    this.validateSignature();
  }

  reset() {
    this.signaturePad.clear();
    this.validateSignature();
  }

  validateSignature() {
    this.isValidSignature = !this.signaturePad.isEmpty();
    this.changeDetector.detectChanges();
  }

  setSignature() {
    this.dialogRef.close(this.getSignatureValue());
  }

  getSignatureValue() {
    return this.signaturePad.toDataURL().replace('data:image/png;base64,', '');
  }
}
