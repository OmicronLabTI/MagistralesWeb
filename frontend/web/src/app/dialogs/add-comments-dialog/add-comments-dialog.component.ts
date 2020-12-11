import {
  AfterContentInit,
  AfterViewChecked,
  AfterViewInit,
  Component,
  ElementRef,
  Inject,
  OnInit,
  ViewChild
} from '@angular/core';
import {CONST_NUMBER, CONST_STRING} from '../../constants/const';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';

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
  @ViewChild('finishComments', {static: true}) finishComments: ElementRef;
  constructor( private dialogRef: MatDialogRef<AddCommentsDialogComponent>, @Inject(MAT_DIALOG_DATA) public commentsData: any) {
    this.comments = this.commentsData || CONST_STRING.empty;
    this.arrayComments = this.comments.trim().split('&').filter( comment => comment !== CONST_STRING.empty);

  }

  ngOnInit() {}
  saveComments() {
    if (this.getIsCorrectData()) {
      this.dialogRef.close(`${this.comments.trim()}${this.newComments.trim()}&`.trim());
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
