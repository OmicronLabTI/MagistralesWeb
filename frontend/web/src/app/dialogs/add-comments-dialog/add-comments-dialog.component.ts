import {Component, Inject, OnInit} from '@angular/core';
import {CONST_NUMBER, CONST_STRING, MODAL_FIND_ORDERS} from '../../constants/const';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';

@Component({
  selector: 'app-add-comments-dialog',
  templateUrl: './add-comments-dialog.component.html',
  styleUrls: ['./add-comments-dialog.component.scss']
})
export class AddCommentsDialogComponent implements OnInit {
  comments = CONST_STRING.empty;
  testComments = 'hola que tal soy german & conque podemos empezar para realizar esto paps&';
  arrayComments: string [] = [];
  constructor( private dialogRef: MatDialogRef<AddCommentsDialogComponent>, @Inject(MAT_DIALOG_DATA) public commentsData: any) {
    // this.comments = this.commentsData || CONST_STRING.empty;
    console.log('comments', this.testComments.trim().split('&'))
    this.arrayComments = this.testComments.trim().split('&');

  }

  ngOnInit() {
  }
  saveComments() {
    if (this.getIsCorrectData()) {
      this.dialogRef.close(this.comments);
    }
  }
  getIsCorrectData() {
    return this.comments.length <= CONST_NUMBER.oneThousand;
  }
}
