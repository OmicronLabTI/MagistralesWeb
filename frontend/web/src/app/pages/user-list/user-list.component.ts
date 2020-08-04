import {Component, OnInit} from '@angular/core';
import {IUserListRes, IUserReq} from '../../model/http/users';
import {MatDialog} from '@angular/material/dialog';
import {AddUserDialogComponent} from '../../dialogs/add-user-dialog/add-user-dialog.component';
import {UsersService} from '../../services/users.service';
import { CONST_STRING} from '../../constants/const';
import {DataService} from '../../services/data.service';
import {ErrorService} from '../../services/error.service';
import {Messages} from '../../constants/messages';

@Component({
    selector: 'app-user-list',
    templateUrl: './user-list.component.html',
    styleUrls: ['./user-list.component.scss']
})
export class UserListComponent implements OnInit {
    isAllComplete = false;
    displayedColumns: string[] = ['delete', 'names', 'lastName', 'role', 'status', 'actions'];
    dataSource: IUserReq[] = [];

    constructor(private dialog: MatDialog, private usersService: UsersService, private dataService: DataService,
                private errorService: ErrorService) {
    }

    ngOnInit() {
        this.getUsers();
    }

    getUsers() {
        this.usersService.getUsers().subscribe((userRes: IUserListRes) => {
                this.dataSource = userRes.response;
                this.dataSource.map(user => {
                    user.isChecked = false;
                });
            },
            error => {
                this.errorService.httpError(error);
            });
    }

    updateAllComplete() {
        this.isAllComplete = this.dataSource.every(user => user.isChecked);
    }

    someComplete(): boolean {
        return this.dataSource.filter(t => t.isChecked).length > 0 && !this.isAllComplete;
    }

    setAll(completed: boolean) {
        this.dataSource.forEach(t => t.isChecked = completed);
    }


    deleteUsers(idUser: string) {
        if (idUser !== CONST_STRING.empty) {
            this.dataSource.filter(user => user.id === idUser).forEach(user => user.isChecked = true);
        }

        this.dataService.presentToastCustom(Messages.deleteUsers, 'warning', CONST_STRING.empty, true, true)
            .then((result: any) => {
                if (result.isConfirmed) {
                    this.usersService.deleteUsers(this.dataSource.filter(user => user.isChecked).map(user => user.id)).subscribe(
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
                userToEditM: userId !== CONST_STRING.empty ? this.dataSource.filter(user => user.id === userId)[0] : {}
            }
        });

        dialogRef.afterClosed().subscribe(() => {
            this.getUsers();
        });

    }
}
