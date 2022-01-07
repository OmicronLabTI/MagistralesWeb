import { Component, OnInit, Inject } from '@angular/core';

import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import { IMyNewListReq, BaseComponent } from 'src/app/model/http/listacomponentes';
import { DataService } from 'src/app/services/data.service';
import { FormControl, Validators } from '@angular/forms';
import { OrdersService } from 'src/app/services/orders.service';
import { Messages } from 'src/app/constants/messages';
import {MODAL_FIND_ORDERS} from '../../constants/const';
import { LocalStorageService } from 'src/app/services/local-storage.service';
import { MessagesService } from 'src/app/services/messages.service';

@Component({
  selector: 'app-mi-lista',
  templateUrl: './mi-lista.component.html',
  styleUrls: ['./mi-lista.component.scss']
})
export class MiListaComponent implements OnInit {
  name = new FormControl('', [Validators.required]);
  errorService: any;
  constructor(
    private dialogRef: MatDialogRef<MiListaComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dataService: DataService,
    private orderService: OrdersService,
    private localStorageService: LocalStorageService,
    private messagesService: MessagesService
  ) {}

  ngOnInit() {
  }

  saveMyList() {
    this.messagesService.presentToastCustom(Messages.confirmSaveMyList, 'question', '', true, true).then( (res: any) => {
      if (res.isConfirmed) {
        const datos = new IMyNewListReq();
        datos.userId = this.localStorageService.getUserId();
        datos.data = new BaseComponent();
        datos.data.name = this.name.value;
        datos.data.productId = this.data.code;
        datos.data.components = this.data.data;
        const nameFC = this.name.value;
        this.orderService.saveMyListComponent(datos).subscribe( result => {
          if (result.response === 0) {
            this.messagesService.presentToastCustom('Error al guardar la lista', 'error',
            'La lista <b>' + nameFC + '</b> ya existe.', true, false);
          } else {
            this.dialogRef.close(true);
            this.messagesService.presentToastCustom(Messages.successMyList, 'success', '', false, false);
          }
        }, error => this.errorService.httpError(error));
      }
    });
  }
  keyDownFunction(event: KeyboardEvent) {
    if (event.key === MODAL_FIND_ORDERS.keyEnter && this.name.valid) {
      this.saveMyList();
    }
  }


}
