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
    this.maxLengthComments = this.commentsConfig.isForClose ? CONST_NUMBER.oneHundred.toString() : CONST_NUMBER.oneThousand.toString();
  }
  saveComments() {
    if (this.commentsConfig.isReadOnly || this.commentsConfig.isForClose ) {
      this.dialogRef.close({
        isReadOnly: this.commentsConfig.isReadOnly,
        isForClose: this.commentsConfig.isForClose,
        comments: CONST_STRING.empty});
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
