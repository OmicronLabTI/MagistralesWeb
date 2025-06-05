import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { IUserReq, SearchUsersData } from '../../model/http/users';
import { MatDialog } from '@angular/material/dialog';
import { AddUserDialogComponent } from '../../dialogs/add-user-dialog/add-user-dialog.component';
import { UsersService } from '../../services/users.service';
import { CONST_NUMBER, CONST_STRING, HttpServiceTOCall } from '../../constants/const';
import { DataService } from '../../services/data.service';
import { ErrorService } from '../../services/error.service';
import { Messages } from '../../constants/messages';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { Title } from '@angular/platform-browser';
import { Subscription } from 'rxjs';
import { SearchUsersDialogComponent } from '../../dialogs/search-users-dialog/search-users-dialog.component';
import { ObservableService } from '../../services/observable.service';
import { MessagesService } from 'src/app/services/messages.service';
import { WorkTeamComponent } from 'src/app/dialogs/work-team/work-team.component';
import { WorkTeamDialogConfig } from 'src/app/model/device/workteam.model';

@Component({
    selector: 'app-user-list',
    templateUrl: './user-list.component.html',
    styleUrls: ['./user-list.component.scss']
})
export class UserListComponent implements OnInit, OnDestroy {
    isAllComplete = false;
    displayedColumns: string[] = ['delete', 'username', 'names', 'lastName', 'role', 'clasfqf', 'pieces', 'asignable', 'status', 'actions'];
    dataSource = new MatTableDataSource<IUserReq>();
    pageSize = CONST_NUMBER.ten;
    pageEvent: PageEvent;
    @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
    lengthPaginator = CONST_NUMBER.zero;
    offset = CONST_NUMBER.zero;
    limit = CONST_NUMBER.ten;
    subscriptionUsers = new Subscription();
    isOnInit = true;
    searchUsersData = new SearchUsersData();
    fullQueryString = CONST_STRING.empty;
    pageIndex = CONST_NUMBER.zero;
    constructor(
        private dialog: MatDialog,
        private usersService: UsersService,
        private dataService: DataService,
        private messagesService: MessagesService,
        private errorService: ErrorService,
        private titleService: Title,
        private observableService: ObservableService) {
        this.observableService.setUrlActive(HttpServiceTOCall.USERS);
    }

    ngOnInit() {
        this.getUsers();
        this.subscriptionUsers = this.observableService.getCallHttpService().subscribe(resultCallHttp => {
            if (resultCallHttp === HttpServiceTOCall.USERS) {
                this.createMessageHttpOk();
                this.getUsers();
            }
        });
        this.titleService.setTitle('OmicronLab - Usuarios');
        this.dataSource.paginator = this.paginator;
    }

    getClassification(type: string): string {
        const dictOptions: { [key: string]: string } = {
            MN: 'Bioelite (MN)',
            BE: 'Bioequal (BE)',
            MG: 'Magistral (MG)',
            DZ: 'Dermazone (DZ)',
        };
        return dictOptions[type];
    }

    getRol(typeRol: number): string {
        const dictOptions: { [key: number]: string } = {
            2: 'QFB',
            1: 'ADMINISTRADOR',
            3: 'LOGÍSTICA',
            4: 'DISEÑO',
            5: 'ALMACÉN',
            6: 'REPARTIDOR',
            7: 'INCIDENCIAS',
            8: 'REPARTIDOR CAC',
            9: 'TÉCNICO',
        };
        return dictOptions[typeRol];
    }

    getUsers() {
        this.usersService.getUsers(`?${this.fullQueryString}&offset=${this.offset}&limit=${this.limit}`).subscribe(userRes => {

            this.lengthPaginator = userRes.comments;
            this.dataSource.data = userRes.response;
            this.dataSource.data.forEach(user => {
                user.isChecked = false;
                user.piezas = this.dataService.getFormattedNumber(user.piezas);
                if (user.classificationDescription) {
                    user.fullClasification = user.classificationDescription;
                }
            });
            this.isAllComplete = false;
            this.isOnInit = false;
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

    getIsSomeChecked(): boolean {
        return this.dataSource.data.some((user) => user.isChecked);
    }
    deleteUsers(idUser: string) {
        if (idUser !== CONST_STRING.empty) {
            this.dataSource.data.filter(user => user.id === idUser).forEach(user => user.isChecked = true);
        }

        this.messagesService.presentToastCustom(Messages.deleteUsers, 'warning', CONST_STRING.empty, true, true)
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

    openWorkFlowDialog(user: IUserReq): void {
        const data = new WorkTeamDialogConfig();
        data.title = user.role === 2 ? 'TÉCNICO' : 'QUÍMICO (S)';
        data.id = user.id;
        this.dialog.open(WorkTeamComponent, {
            panelClass: 'custom-dialog-container',
            data
        });
    }
    changeDataEvent(event: PageEvent) {
        this.pageSize = event.pageSize;
        this.offset = (event.pageSize * (event.pageIndex));
        this.limit = event.pageSize;
        this.pageIndex = event.pageIndex;
        this.getUsers();
        return event;
    }
    createMessageHttpOk() {
        this.observableService.setMessageGeneralCallHttp({ title: Messages.success, icon: 'success', isButtonAccept: false });
    }

    ngOnDestroy() {
        this.subscriptionUsers.unsubscribe();
    }

    openSearchUsers() {
        this.dialog.open(SearchUsersDialogComponent, {
            panelClass: 'custom-dialog-container',
            data: this.searchUsersData
        }).afterClosed().subscribe(searchUsersResult => {
            if (searchUsersResult) {
                this.onSuccessUserResult(searchUsersResult);
            }
        });
    }
    getKeyUserSearchValue(key: string): string {
        const dictOptions: { [key: string]: string } = {
            activoSe: 'status',
            asignableSe: 'assignable',
            firstNameSe: 'fname',
            lastNameSe: 'lname',
            userNameSe: 'user',
            userTypeRSe: 'role',
            classificationQFBSe: 'typeQfb',
            '': ''
        };
        return dictOptions[key];
    }

    validateValue(value: string): boolean {
        return value !== undefined && value !== CONST_STRING.empty;
    }
    onSuccessUserResult(searchUsersResult: any) {
        this.fullQueryString = CONST_STRING.empty;
        this.searchUsersData = searchUsersResult;
        const keyList = Object.keys(searchUsersResult);
        this.fullQueryString = keyList.reduce((query, key) =>
            this.dataService.calculateTernary(this.validateValue(searchUsersResult[key]),
                `${query}${this.getKeyUserSearchValue(key)}=${searchUsersResult[key]}&`,
                query),
            CONST_STRING.empty);
        this.fullQueryString = this.fullQueryString.slice(CONST_NUMBER.zero, CONST_NUMBER.lessOne);
        this.offset = CONST_NUMBER.zero;
        this.limit = CONST_NUMBER.ten;
        this.pageIndex = CONST_NUMBER.zero;
        this.pageSize = CONST_NUMBER.ten;
        this.getUsers();
    }
}
