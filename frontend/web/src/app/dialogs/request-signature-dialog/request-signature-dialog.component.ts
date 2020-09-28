import { Component, OnInit, ViewChild  } from '@angular/core';
import { MatDialogRef } from '@angular/material';
import { SignaturePad } from 'angular2-signaturepad/signature-pad';

@Component({
  selector: 'app-request-signature-dialog',
  templateUrl: './request-signature-dialog.component.html',
  styleUrls: ['./request-signature-dialog.component.scss']
})
export class RequestSignatureDialogComponent implements OnInit {
  
  @ViewChild(SignaturePad, {static: false}) signaturePad: SignaturePad;
 
  public isValidSignature: Boolean;

  public signaturePadOptions: Object = { 
    'minWidth': 2,
    'canvasWidth': 500,
    'canvasHeight': 300
  };

  constructor(
    private dialogRef: MatDialogRef<RequestSignatureDialogComponent>
  ) { }

  ngOnInit() {
  }

  ngAfterViewInit() {
    this.reset();
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
  }

  setSignature() {
    this.dialogRef.close(this.getSignatureValue());
  }

  getSignatureValue() {
    var result = this.signaturePad.toDataURL();
    return result.replace('data:image/png;base64,', '');
  }
}
