import {Component, Inject, OnInit} from '@angular/core';
import {CONST_NUMBER, CONST_STRING} from '../../constants/const';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';

@Component({
  selector: 'app-add-comments-dialog',
  templateUrl: './add-comments-dialog.component.html',
  styleUrls: ['./add-comments-dialog.component.scss']
})
export class AddCommentsDialogComponent implements OnInit {
  comments = CONST_STRING.empty;
  constructor( private dialogRef: MatDialogRef<AddCommentsDialogComponent>, @Inject(MAT_DIALOG_DATA) public commentsData: any) {
    this.comments = this.commentsData || CONST_STRING.empty;
  }

  ngOnInit() {
  }

  saveComments() {
    if (this.comments.length <= CONST_NUMBER.oneThousand) {
      this.dialogRef.close(this.comments);
    }
  }
}
