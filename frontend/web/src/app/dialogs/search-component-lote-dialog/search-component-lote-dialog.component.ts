import { COMMA, ENTER } from '@angular/cdk/keycodes';
import { ChangeDetectorRef, Component, ElementRef, Inject, OnInit, ViewChild } from '@angular/core';
import { MAT_DIALOG_DATA, MatChipInputEvent, MatDialogRef, MatPaginator, MatTableDataSource, PageEvent } from '@angular/material';
import { CONST_NUMBER } from 'src/app/constants/const';
import { Messages } from 'src/app/constants/messages';
import { IComponentLotes } from 'src/app/model/http/addComponent';
import { DataService } from 'src/app/services/data.service';
import { ErrorService } from 'src/app/services/error.service';
import { MessagesService } from 'src/app/services/messages.service';
import { PedidosService } from 'src/app/services/pedidos.service';

@Component({
  selector: 'app-search-component-lote-dialog',
  templateUrl: './search-component-lote-dialog.component.html',
  styleUrls: ['./search-component-lote-dialog.component.scss']
})
export class SearchComponentLoteDialogComponent implements OnInit {
  selectable = true;
  removable = true;
  addOnBlur = true;
  readonly separatorKeysCodes: number[] = [ENTER, COMMA];
  keywords: string[] = [];
  allComplete = false;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild('chipsInput', { static: false }) chipsInput: ElementRef;
  pageSize = CONST_NUMBER.ten;
  dataSource = new MatTableDataSource<IComponentLotes>();
  displayedColumns: string[] = ['numero', 'descripcion'];
  lengthPaginator = CONST_NUMBER.zero;
  offset = CONST_NUMBER.zero;
  minimumCharacters = CONST_NUMBER.two;
  limit = CONST_NUMBER.ten;
  queryStringComponents = '';
  isDisableSearch = false;
  pageEvent: PageEvent;
  rowPrevious = {};
  count = 0;
  isFromSearchComponent = true;
  catalogGroupName = '';

  constructor(
    private ordersService: PedidosService,
    private dialogRef: MatDialogRef<SearchComponentLoteDialogComponent>,
    private errorService: ErrorService,
    private dataService: DataService,
    private changeDetector: ChangeDetectorRef,
    private messagesService: MessagesService,
    @Inject(MAT_DIALOG_DATA) public data: any,
  ) {
    this.keywords = this.data.chips && this.data.chips.length > 0 ? this.data.chips : [];
  }

  ngOnInit() {
    this.dataSource.paginator = this.paginator;
    if (this.keywords.length > 0) {
      this.getQueryString();
      this.getComponentsAction();
    }
    this.changeDetector.detectChanges();
    this.getCatalogGroupName();
  }

  getQueryString() {
    this.queryStringComponents =
      `?itemcode=${this.keywords.toString() !== '' ? this.keywords.toString() : '$$'}&warehouse=${this.catalogGroupName}`;
  }

  getComponentsAction() {
    this.isDisableSearch = true;
    this.ordersService.getComponentsLotes(this.queryStringComponents).subscribe(resComponents => {
      this.dataSource.data = resComponents.response;
      this.lengthPaginator = resComponents.comments;
      this.isDisableSearch = false;
      this.setFocusToChipsInput();
    }
      , error => {
        this.errorService.httpError(error);
        this.dialogRef.close();
        this.setFocusToChipsInput();
      });
  }

  addChip(event: MatChipInputEvent): void {
    const input = event.input;
    const value = event.value;
    const valueTrim = (value || '').trim();

    if (valueTrim && valueTrim.length >= this.minimumCharacters) {
      this.keywords.push(valueTrim);
      this.getQueryString();
      this.getComponentsAction();
    }
    if (input) {
      input.value = '';
    }
    this.changeDetector.detectChanges();
  }

  removeChip(word): void {
    const index = this.keywords.indexOf(word);
    if (index >= 0) {
      this.keywords.splice(index, 1);
    }
    this.getQueryString();
    this.getComponentsAction();
  }

  onKeyDown(event: KeyboardEvent): void {
    if (event.key.toLowerCase() === 'backspace' && this.chipsInput.nativeElement.value === '') {
      event.preventDefault();
      const numberOfChilds = this.chipsInput.nativeElement.parentNode.children.length;
      if (numberOfChilds > 1) {
        const node = this.chipsInput.nativeElement.parentNode.children[numberOfChilds - 2];
        if (node.tagName.toLowerCase() === 'mat-chip') {
          node.click();
          node.focus();
        }
      }
    }
  }

  selectComponent(row: any) {
    this.dataSource.data.forEach(component => {
      component.isItemSelected = false;
    });
    this.dataSource.data.filter(component => component.codigoProducto === row.codigoProducto)[0].isItemSelected = true;
    if (this.data.data.filter(element => element.codigoProducto === row.codigoProducto).length === 0) {
      this.checkIsPrevious(row);
    } else {
      this.messagesService.presentToastCustom(
        `${Messages.repeatedComponent_a}  ${row.productId} ${Messages.repeatedComponent_b}`,
        'info',
        '',
        true,
        false
      );
    }
  }

  checkIsPrevious(row) {
    if (row === this.rowPrevious) {
      row.chips = this.keywords;
      this.dialogRef.close(row);
    } else {
      this.rowPrevious = row;
    }
  }

  getCatalogGroupName(): void {
    this.catalogGroupName = this.data.catalogGroupName === '' ? 'MN' : this.data.catalogGroupName;
  }

  setFocusToChipsInput() {
    setTimeout(() => {
      this.chipsInput.nativeElement.focus();
    }, 100);
  }

  changeDataEvent(event: PageEvent) {
    this.offset = (event.pageSize * (event.pageIndex));
    this.limit = event.pageSize;
    this.getQueryString();
    this.getComponentsAction();
    return event;
  }

}
