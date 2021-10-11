import {
  AfterViewInit,
  Component,
  ElementRef,
  Inject,
  OnInit,
  ViewChild
} from '@angular/core';
import {CONST_NUMBER, CONST_STRING} from '../../constants/const';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import {CommentsConfig} from '../../model/device/incidents.model';

@Component({
  selector: 'app-add-comments-dialog',
  templateUrl: './add-comments-dialog.component.html',
  styleUrls: ['./add-comments-dialog.component.scss']
})
export class AddCommentsDialogComponent implements OnInit, AfterViewInit {
  comments = CONST_STRING.empty;
  newComments = CONST_STRING.empty;
  arrayComments: string [] = [];
  isCorrectData = true;
  maxLengthComments = CONST_STRING.empty;
  @ViewChild('finishComments', {static: true}) finishComments: ElementRef;
  constructor( private dialogRef: MatDialogRef<AddCommentsDialogComponent>, @Inject(MAT_DIALOG_DATA) public commentsConfig: any) {
    this.comments = this.commentsConfig.comments || CONST_STRING.empty;
    this.arrayComments = this.comments.trim().split('&').filter( comment => comment !== CONST_STRING.empty);
  }

  ngOnInit() {
    if (this.commentsConfig.isForClose || this.commentsConfig.isForRefuseOrders) {
      this.maxLengthComments = String(CONST_NUMBER.threeHundred - (this.comments.trim().length + CONST_NUMBER.one));
    } else {
      this.maxLengthComments = String(CONST_NUMBER.oneThousand - (this.comments.trim().length + CONST_NUMBER.one));
    }
  }
  saveComments() {
    if (this.commentsConfig.isReadOnly || this.commentsConfig.isForClose || this.commentsConfig.isForRefuseOrders ) {
      this.dialogRef.close({
        isReadOnly: this.commentsConfig.isReadOnly,
        isForClose: this.commentsConfig.isForClose,
        isForRefuseOrders: this.commentsConfig.isForRefuseOrders,
        comments: this.commentsConfig.isForRefuseOrders ? this.newComments.trim() : CONST_STRING.empty});
      return;
    }

    if (this.getIsCorrectData()) {
      this.dialogRef.close({comments: `${this.comments.trim()}${this.newComments.trim()}&`.trim(),
                            isReadOnly: this.commentsConfig.isReadOnly,
                            isForClose: this.commentsConfig.isForClose} as CommentsConfig);
    }
  }
  getIsCorrectData() {
    return (this.comments.trim().length + this.newComments.trim().length) <= CONST_NUMBER.oneThousand;
  }

  scroll() {
    this.finishComments.nativeElement.scrollIntoView();
  }
  ngAfterViewInit(): void {
    this.scroll();
  }

  checkData() {
    this.isCorrectData = this.getIsCorrectData();
  }
}
