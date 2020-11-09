import { ChangeDetectorRef, Component, ElementRef, Inject, OnInit, ViewChild} from '@angular/core';
import { MatTableDataSource} from '@angular/material';
import {IFormulaDetalleReq} from '../../model/http/detalleformula';
import {MatPaginator, PageEvent} from '@angular/material/paginator';
import {ComponentSearch, CONST_NUMBER} from '../../constants/const';
import {MatChipInputEvent} from '@angular/material/chips';
import {COMMA, ENTER} from '@angular/cdk/keycodes';
import {PedidosService} from '../../services/pedidos.service';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import {ErrorService} from '../../services/error.service';
import {DataService} from '../../services/data.service';
import { Messages } from 'src/app/constants/messages';

@Component({
  selector: 'app-component-search',
  templateUrl: './component-search.component.html',
  styleUrls: ['./component-search.component.scss']
})
export class ComponentSearchComponent implements OnInit {
  visible = true;
  selectable = true;
  removable = true;
  addOnBlur = true;
  readonly separatorKeysCodes: number[] = [ENTER, COMMA];
  keywords: string[] = [];
  allComplete = false;
  @ViewChild(MatPaginator, {static: true}) paginator: MatPaginator;
  @ViewChild('chipsInput', {static: false}) chipsInput: ElementRef;
  pageSize = CONST_NUMBER.ten;
  dataSource = new MatTableDataSource<IFormulaDetalleReq>();
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
  constructor(private ordersService: PedidosService,
              private dialogRef: MatDialogRef<ComponentSearchComponent>,
              private errorService: ErrorService,
              @Inject(MAT_DIALOG_DATA) public data: any,
              private dataService: DataService,
              private changeDetector: ChangeDetectorRef) {
    this.isFromSearchComponent = this.data.modalType === ComponentSearch.searchComponent
                || this.data.modalType === ComponentSearch.addComponent;
    this.keywords = this.data.chips && this.data.chips.length > 0 ? this.data.chips : [];
  }

  ngOnInit() {
    this.dataSource.paginator = this.paginator;
    if (this.keywords.length > 0 ) {
      this.getQueryString();
      this.getComponentsAction();
    }
    this.changeDetector.detectChanges();
  }

  getComponentsAction() {
    this.isDisableSearch = true;
    this.ordersService.getComponents(this.queryStringComponents, this.isFromSearchComponent).subscribe(resComponents => {
          resComponents.response.forEach( component => {
            if (this.isFromSearchComponent ) {
              component.description = component.description.toUpperCase();
            } else {
              component.productoName = component.productoName.toUpperCase();
            }
          });
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

  changeDataEvent(event: PageEvent) {
    this.offset = (event.pageSize * (event.pageIndex));
    this.limit = event.pageSize;
    this.getQueryString();
    this.getComponentsAction();
    return event;
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
  getQueryString() {
    this.queryStringComponents =
        `?offset=${this.offset}&limit=${this.limit}&chips=${this.keywords.toString() !== '' ? this.keywords.toString() : '$$' }`;
  }

  selectComponent(row: any) {
    this.dataSource.data.forEach(component => component.isItemSelected = false);
    if (this.isFromSearchComponent) {
      this.dataSource.data.filter(component => component.productId === row.productId)[0].isItemSelected = true;
      if (this.data.data.filter(element => element.productId === row.productId).length === 0) {
        this.checkIsPrevious(row);
      } else {
        this.dataService.presentToastCustom(
          `${Messages.repeatedComponent_a }  ${row.productId} ${
               this.data.modalType !== ComponentSearch.addComponent ? Messages.repeatedComponent_b :
                   Messages.repeatedComponent_b_request }`,
          'info',
          '',
          true,
          false
        );
      }
    } else {
      this.dataSource.data.filter(component => component.productoId === row.productoId)[0].isItemSelected = true;
      this.checkIsPrevious(row);
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

  setFocusToChipsInput() {
    setTimeout(() => {
      this.chipsInput.nativeElement.focus();
    }, 100);
  }

  onKeyDown(event: KeyboardEvent): void {
    if (event.key.toLowerCase() == 'backspace' && this.chipsInput.nativeElement.value == '') {
      event.preventDefault();
      let numberOfChilds = this.chipsInput.nativeElement.parentNode.children.length;
      if (numberOfChilds > 1) {
        let node = this.chipsInput.nativeElement.parentNode.children[numberOfChilds - 2];
        if (node.tagName.toLowerCase() == 'mat-chip') {
          node.click();
          node.focus();
        }
      }
    }
  }
}
