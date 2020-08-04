import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TableTestRoutingModule } from './table-test-routing.module';
import {TableTestComponent} from './table-test.component';
import {MatTableModule} from "@angular/material/table";
import {MatPaginatorModule} from "@angular/material/paginator";


@NgModule({
  declarations: [TableTestComponent],
    imports: [
        CommonModule,
        TableTestRoutingModule,
        MatTableModule,
        MatPaginatorModule
    ]
})
export class TableTestModule { }
