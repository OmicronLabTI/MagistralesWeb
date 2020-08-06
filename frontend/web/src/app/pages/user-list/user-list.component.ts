import {Component, OnInit, ViewChild} from '@angular/core';
import {IUserListRes, IUserReq} from '../../model/http/users';
import {MatDialog} from '@angular/material/dialog';
import {AddUserDialogComponent} from '../../dialogs/add-user-dialog/add-user-dialog.component';
import {UsersService} from '../../services/users.service';
import {CONST_NUMBER, CONST_STRING} from '../../constants/const';
import {DataService} from '../../services/data.service';
import {ErrorService} from '../../services/error.service';
import {Messages} from '../../constants/messages';
import {MatPaginator, PageEvent} from '@angular/material/paginator';
import {MatTableDataSource} from '@angular/material/table';

@Component({
    selector: 'app-user-list',
    templateUrl: './user-list.component.html',
    styleUrls: ['./user-list.component.scss']
})
export class UserListComponent implements OnInit {
    isAllComplete = false;
    displayedColumns: string[] = ['delete', 'names', 'lastName', 'role', 'status', 'actions'];
    dataSource = new MatTableDataSource<IUserReq>();
    pageSize = CONST_NUMBER.ten;
    pageEvent: PageEvent;
    @ViewChild(MatPaginator, {static: true}) paginator: MatPaginator;
    lengthPaginator = CONST_NUMBER.zero;
    offset = CONST_NUMBER.zero;
    limit = CONST_NUMBER.ten;
    constructor(private dialog: MatDialog, private usersService: UsersService, private dataService: DataService,
                private errorService: ErrorService) {
    }

    ngOnInit() {
        this.getUsers();
        this.dataSource.paginator = this.paginator;
    }

    getUsers() {

        this.usersService.getUsers(this.offset, this.limit).subscribe((userRes: IUserListRes) => {
                this.lengthPaginator = 21;
                this.dataSource.data = userRes.response;
                console.log('res user: ', userRes);
                this.dataSource.data.map( user => {
                    user.isChecked = false;
                });
            },
            error => {
                this.errorService.httpError(error);
            });
    }

    updateAllComplete() {
        this.isAllComplete = this.dataSource.data.every(user => user.isChecked);
    }

    someComplete(): boolean {
        return this.dataSource.data.filter(t => t.isChecked).length > 0 && !this.isAllComplete;
    }

    setAll(completed: boolean) {
        this.dataSource.data.forEach(t => t.isChecked = completed);
    }


    deleteUsers(idUser: string) {
        if (idUser !== CONST_STRING.empty) {
            this.dataSource.data.filter(user => user.id === idUser).forEach(user => user.isChecked = true);
        }

        this.dataService.presentToastCustom(Messages.deleteUsers, 'warning', CONST_STRING.empty, true, true)
            .then((result: any) => {
                if (result.isConfirmed) {
                    this.usersService.deleteUsers(this.dataSource.data.filter(user => user.isChecked).map(user => user.id)).subscribe(
                        () => {
                            this.getUsers();
                        },
                        error => {
                            this.errorService.httpError(error);
                        }
                    );
                }
            });

    }

    openDialog(modalTypeOpen: string, userId: string) {
        const dialogRef = this.dialog.open(AddUserDialogComponent, {
            panelClass: 'custom-dialog-container',
            data: {
                modalType: modalTypeOpen,
                userToEditM: userId !== CONST_STRING.empty ? this.dataSource.data.filter(user => user.id === userId)[0] : {}
            }
        });

        dialogRef.afterClosed().subscribe(() => {
            this.getUsers();
        });

    }

    changeDataEvent(event: PageEvent) {
        this.offset = (event.pageSize * (event.pageIndex));
        this.limit = (event.pageSize * (event.pageIndex + 1));
        this.getUsers();
        return event;
    }
}
