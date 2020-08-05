import {Component, OnInit, ViewChild} from '@angular/core';
import {MatTableDataSource} from '@angular/material/table';
import {MatPaginator, PageEvent} from '@angular/material/paginator';
import {UsersService} from "../../services/users.service";
export interface PeriodicElement {
  name: string;
  position: number;
  weight: number;
  symbol: string;
}
@Component({
  selector: 'app-table-test',
  templateUrl: './table-test.component.html',
  styleUrls: ['./table-test.component.scss']
})
export class TableTestComponent implements OnInit {
    pageEvent: PageEvent;

    displayedColumns: string[] = ['position', 'name', 'weight', 'symbol'];
  @ViewChild(MatPaginator, {static: true}) paginator: MatPaginator;
  ELEMENT_DATA: any [] = [
       {position: 1, name: 'Hydrogen', weight: 1.0079, symbol: 'H'},
       {position: 2, name: 'Helium', weight: 4.0026, symbol: 'He'},
       {position: 3, name: 'Lithium', weight: 6.941, symbol: 'Li'},
       {position: 4, name: 'Beryllium', weight: 9.0122, symbol: 'Be'},
       {position: 5, name: 'Boron', weight: 10.811, symbol: 'B'},
       {position: 6, name: 'Carbon', weight: 12.0107, symbol: 'C'},
       {position: 7, name: 'Nitrogen', weight: 14.0067, symbol: 'N'},
       {position: 8, name: 'Oxygen', weight: 15.9994, symbol: 'O'},
       {position: 9, name: 'Fluorine', weight: 18.9984, symbol: 'F'},
       {position: 10, name: 'Neon', weight: 20.1797, symbol: 'Ne'},
       {},
      {},
      {},
      {},
      {},
      {},
      {},
      {},
      {},
      {},
      {},
      {},
      {},
      {},
      {},
      {},
      {},
      {},
      {},
      {}
     ];
  secondLote: any[] = [
      {position: 11, name: 'Sodium', weight: 22.9897, symbol: 'Na'},
      {position: 12, name: 'Magnesium', weight: 24.305, symbol: 'Mg'},
      {position: 13, name: 'Aluminum', weight: 26.9815, symbol: 'Al'},
      {position: 14, name: 'Silicon', weight: 28.0855, symbol: 'Si'},
      {position: 15, name: 'Phosphorus', weight: 30.9738, symbol: 'P'},
      {position: 16, name: 'Sulfur', weight: 32.065, symbol: 'S'},
      {position: 17, name: 'Chlorine', weight: 35.453, symbol: 'Cl'},
      {position: 18, name: 'Argon', weight: 39.948, symbol: 'Ar'},
      {position: 19, name: 'Potassium', weight: 39.0983, symbol: 'K'},
      {position: 20, name: 'Calcium', weight: 40.078, symbol: 'Ca'}
  ];
    thirdLote: any[] = [
        {position: 21, name: 'Sodium', weight: 22.9897, symbol: 'Na'},
        {position: 22, name: 'Magnesium', weight: 24.305, symbol: 'Mg'},
        {position: 23, name: 'Aluminum', weight: 26.9815, symbol: 'Al'},
        {position: 24, name: 'Silicon', weight: 28.0855, symbol: 'Si'},
        {position: 25, name: 'Phosphorus', weight: 30.9738, symbol: 'P'},
        {position: 26, name: 'Sulfur', weight: 32.065, symbol: 'S'},
        {position: 27, name: 'Chlorine', weight: 35.453, symbol: 'Cl'},
        {position: 28, name: 'Argon', weight: 39.948, symbol: 'Ar'},
        {position: 29, name: 'Potassium', weight: 39.0983, symbol: 'K'},
        {position: 30, name: 'Calcium', weight: 40.078, symbol: 'Ca'}
    ];
    pageSize = 10;
  previousPageIndex: number[] = [];
    dataSource = new MatTableDataSource();
    constructor(private usersService: UsersService) {
    }

  ngOnInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.data = this.ELEMENT_DATA;
    console.log('data init: ', this.dataSource.data);
  }

     changeDataEvent(event: PageEvent) {
      console.log('pageSize: ', event.pageSize);
      const oldData: any[] = this.dataSource.data;
      if (event.previousPageIndex < event.pageIndex && !this.previousPageIndex.includes(event.pageIndex)) {
          console.log('exec data')
          if (event.pageIndex === 1) {
             // setTimeout(function(){
                  this.secondLote.forEach((row, index) => {
                      oldData[((event.pageIndex * event.pageSize) + index)] = row;
                      // console.log(' row: ', row, ' index: ', index);
                  });
             // }, 3000);
           /*  await this.usersService.getUsers(0, 10).toPromise().then(
                  user => console.log(' user: ', user)
              );*/
          } else {
              this.thirdLote.forEach((row, index) => {
                  oldData[((event.pageIndex * this.pageSize) + index)] = row;
                  // console.log(' row: ', row, ' index: ', index);
              });
          }
      }
      if (!this.previousPageIndex.includes(event.pageIndex)) {
            this.previousPageIndex.push(event.pageIndex);
      }
      console.log('old data', oldData);
      this.dataSource.data = oldData;
      return event;
    }
}
