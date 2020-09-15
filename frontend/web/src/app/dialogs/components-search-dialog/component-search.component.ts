import {Component, Inject, OnInit, ViewChild} from '@angular/core';
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
  pageSize = CONST_NUMBER.ten;
  dataSource = new MatTableDataSource<IFormulaDetalleReq>();
  displayedColumns: string[] = ['numero', 'descripcion'];
  lengthPaginator = CONST_NUMBER.zero;
  offset = CONST_NUMBER.zero;
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
              private dataService: DataService) {
    this.isFromSearchComponent = this.data.modalType === ComponentSearch.searchComponent;
    this.keywords = this.data.chips && this.data.chips.length > 0 ? this.data.chips : [];
  }

  ngOnInit() {
    this.dataSource.paginator = this.paginator;
    if (this.keywords.length > 0 ) {
      this.getQueryString();
      this.getComponents();
    }
  }

  getComponents() {
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
        }
        , error => {
          this.errorService.httpError(error);
          this.dialogRef.close();
        });
  }

  changeDataEvent(event: PageEvent) {
    this.offset = (event.pageSize * (event.pageIndex));
    this.limit = event.pageSize;
    this.getQueryString();
    this.getComponents();
    return event;
  }

  addChip(event: MatChipInputEvent): void {
    const input = event.input;
    const value = event.value;

    if ((value || '').trim()) {
      this.keywords.push(value.trim());
      this.getQueryString();
      this.getComponents();
    }
    if (input) {
      input.value = '';
    }
  }

  removeChip(word): void {
    const index = this.keywords.indexOf(word);

    if (index >= 0) {
      this.keywords.splice(index, 1);
    }
    this.getQueryString();
    this.getComponents();
  }
  getQueryString() {
    this.queryStringComponents =
        `?offset=${this.offset}&limit=${this.limit}&chips=${this.keywords.toString() !== '' ? this.keywords.toString() : '$$' }`;
  }

  selectComponent(row: any) {
    if (row === this.rowPrevious) {
      row.chips = this.keywords;
      this.dialogRef.close(row);
    } else {
      this.rowPrevious = row;
    }
  }
}
