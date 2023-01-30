import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkTeamComponent } from './work-team.component';
import { UsersService } from 'src/app/services/users.service';
import { MAT_DIALOG_DATA, MatButtonModule, MatDialog, MatDialogModule } from '@angular/material';
import { of } from 'rxjs';
import { WorkTeamDialogConfig } from 'src/app/model/device/workteam.model';
import { ErrorService } from 'src/app/services/error.service';

describe('WorkTeamComponent', () => {
  let component: WorkTeamComponent;
  let fixture: ComponentFixture<WorkTeamComponent>;
  let userServiceSpy: jasmine.SpyObj<UsersService>;
  let errorServiceSpy: jasmine.SpyObj<ErrorService>;
  beforeEach(async(() => {
    userServiceSpy = jasmine.createSpyObj<UsersService>('UsersService',
      [
        'getWorkTeam',
      ]);
    userServiceSpy.getWorkTeam.and.returnValue(of({
      response: [
        {
          firstName: '',
          id: '',
          lastName: ''
        }
      ]
    }));
    errorServiceSpy = jasmine.createSpyObj<ErrorService>('ErrorService',
      [
        'httpError'
      ]);

    TestBed.configureTestingModule({
      imports: [MatDialogModule, MatButtonModule],
      declarations: [WorkTeamComponent],
      providers: [
        { provide: UsersService, useValue: userServiceSpy },
        { provide: MAT_DIALOG_DATA, useValue: new WorkTeamDialogConfig() },
        { provide: ErrorService, useValue: errorServiceSpy },
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkTeamComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
