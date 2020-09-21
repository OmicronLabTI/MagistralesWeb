import { TestBed } from '@angular/core/testing';
import { UsersService } from './users.service';
import {DatePipe} from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import {ConsumeService} from './consume.service';
import {IUserReq} from '../model/http/users';
import {Endpoints} from '../../environments/endpoints';
describe('UsersService', () => {
  let consumeServiceSpy: jasmine.SpyObj<ConsumeService>;
  beforeEach(() => {
    consumeServiceSpy = jasmine.createSpyObj<ConsumeService>('ConsumeService', [
      'httpGet', 'httpPost', 'httpPut', 'httpPatch'
    ]);

    TestBed.configureTestingModule({

      imports: [
        HttpClientTestingModule
      ],
      providers: [DatePipe,
        { provide: ConsumeService, useValue: consumeServiceSpy }]
    });
  });

  it('should be created', () => {
    const service: UsersService = TestBed.get(UsersService);
    expect(service).toBeTruthy();
  });
  it('should createUser', () => {
    const service: UsersService = TestBed.get(UsersService);
    const userReq = new IUserReq();
    userReq.firstName = 'name';
    userReq.lastName = 'lastName';
    userReq.userName = 'luser';
    userReq.activo = 0;
    userReq.password = 'anyPass';
    userReq.role = 1;
    service.createUserService(userReq);
    expect(consumeServiceSpy.httpPost).toHaveBeenCalledWith(Endpoints.users.createUser, userReq);
  });
  it('should getRoles', () => {
    const service: UsersService = TestBed.get(UsersService);
    service.getRoles();
    expect(consumeServiceSpy.httpGet).toHaveBeenCalledWith(Endpoints.users.roles);
  });
  it('should getUsers', () => {
    const service: UsersService = TestBed.get(UsersService);
    service.getUsers(0, 10);
    expect(consumeServiceSpy.httpGet).toHaveBeenCalled();
  });
  it('should deleteUsers', () => {
    const service: UsersService = TestBed.get(UsersService);
    service.deleteUsers(['1234']);
    expect(consumeServiceSpy.httpPatch).toHaveBeenCalledWith(Endpoints.users.delete, ['1234']);
  });
  it('should updateUser', () => {
    const service: UsersService = TestBed.get(UsersService);
    const userReq = new IUserReq();
    userReq.id = '1234';
    userReq.firstName = 'name';
    userReq.lastName = 'lastName';
    userReq.userName = 'luser';
    userReq.activo = 0;
    userReq.password = 'anyPass';
    userReq.role = 1;
    service.updateUser(userReq);
    expect(consumeServiceSpy.httpPut).toHaveBeenCalledWith(Endpoints.users.update, userReq);
  });
});
