import { Component, OnInit, Inject } from '@angular/core';

import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';

@Component({
  selector: 'app-mi-lista',
  templateUrl: './mi-lista.component.html',
  styleUrls: ['./mi-lista.component.scss']
})
export class MiListaComponent implements OnInit {

  constructor(
    private dialogRef: MatDialogRef<MiListaComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) { }

  ngOnInit() {
  }

}
