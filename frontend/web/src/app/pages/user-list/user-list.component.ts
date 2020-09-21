import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {IUserReq} from '../../model/http/users';
import {MatDialog} from '@angular/material/dialog';
import {AddUserDialogComponent} from '../../dialogs/add-user-dialog/add-user-dialog.component';
import {UsersService} from '../../services/users.service';
import {CONST_NUMBER, CONST_STRING, HttpServiceTOCall} from '../../constants/const';
import {DataService} from '../../services/data.service';
import {ErrorService} from '../../services/error.service';
import {Messages} from '../../constants/messages';
import {MatPaginator, PageEvent} from '@angular/material/paginator';
import {MatTableDataSource} from '@angular/material/table';
import { Title } from '@angular/platform-browser';
import {Subscription} from 'rxjs';

@Component({
    selector: 'app-user-list',
    templateUrl: './user-list.component.html',
    styleUrls: ['./user-list.component.scss']
})
export class UserListComponent implements OnInit, OnDestroy {
    isAllComplete = false;
    displayedColumns: string[] = ['delete', 'username', 'names', 'lastName', 'role', 'pieces', 'asignable', 'status', 'actions'];
    dataSource = new MatTableDataSource<IUserReq>();
    pageSize = CONST_NUMBER.ten;
    pageEvent: PageEvent;
    @ViewChild(MatPaginator, {static: true}) paginator: MatPaginator;
    lengthPaginator = CONST_NUMBER.zero;
    offset = CONST_NUMBER.zero;
    limit = CONST_NUMBER.ten;
    subscriptionUsers = new Subscription();
    constructor(private dialog: MatDialog, private usersService: UsersService, private dataService: DataService,
                private errorService: ErrorService,
                private titleService: Title) {
        this.dataService.setUrlActive(HttpServiceTOCall.USERS);
    }

    ngOnInit() {
        this.getUsers();
        this.subscriptionUsers = this.dataService.getCallHttpService().subscribe( resultCallHttp => {
            if (resultCallHttp === HttpServiceTOCall.USERS) {
                this.createMessageHttpOk();
                this.getUsers();
            }
        });
        this.titleService.setTitle('OmicronLab - Usuarios');
        this.dataSource.paginator = this.paginator;
    }

    getUsers() {

        this.usersService.getUsers(this.offset, this.limit).subscribe(userRes => {
                this.lengthPaginator = userRes.comments;
                this.dataSource.data = userRes.response;
                this.dataSource.data.forEach( user => {
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
                            this.createMessageHttpOk();
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
         this.dialog.open(AddUserDialogComponent, {
            panelClass: 'custom-dialog-container',
            data: {
                modalType: modalTypeOpen,
                userToEditM: userId !== CONST_STRING.empty ? this.dataSource.data.filter(user => user.id === userId)[0] : {}
            }
        });

    }

    changeDataEvent(event: PageEvent) {
        this.offset = (event.pageSize * (event.pageIndex));
        this.limit = event.pageSize;
        this.getUsers();
        return event;
    }
    createMessageHttpOk() {
        this.dataService.setMessageGeneralCallHttp({title: Messages.success, icon: 'success', isButtonAccept: false});
    }

    ngOnDestroy() {
        this.subscriptionUsers.unsubscribe();
    }
}
