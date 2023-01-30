import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material';
import { WorkTeamDialogConfig } from 'src/app/model/device/workteam.model';
import { ErrorService } from 'src/app/services/error.service';
import { UsersService } from 'src/app/services/users.service';

@Component({
  selector: 'app-work-team',
  templateUrl: './work-team.component.html',
  styleUrls: ['./work-team.component.scss']
})
export class WorkTeamComponent implements OnInit {

  users: string[] = [];
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: WorkTeamDialogConfig,
    private usersService: UsersService,
    private errorService: ErrorService) { }

  ngOnInit() {
    this.callService();
  }

  callService(): void {
    this.usersService.getWorkTeam(this.data.id).subscribe((res) => {
      this.users = res.response.map((item) => `${item.firstName} ${item.lastName}`);
    }, error => {
      this.errorService.httpError(error);
    });
  }

}
