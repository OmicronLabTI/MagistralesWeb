import { TestBed } from '@angular/core/testing';
import { MessageType } from '../constants/const';

import { MessagesService } from './messages.service';

describe('MessagesService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: MessagesService = TestBed.get(MessagesService);
    expect(service).toBeTruthy();
  });
  it('should getMessageTitle', () => {
    const service: MessagesService = TestBed.get(MessagesService);
    expect(service.getMessageTitle(['1234'], MessageType.processOrder)).toEqual(' 1234 \n');
    expect(service.getMessageTitle(['1234'], MessageType.processDetailOrder))
        .toEqual(' 1234 \n');
    expect(service.getMessageTitle(['1234'], MessageType.placeOrder))
        .toEqual('La orden de fabricación  1234 no pudo ser Asignada \n');
    expect(service.getMessageTitle([{reason: 'Hubo un error'}], MessageType.cancelOrder, true))
        .toEqual('Hubo un error \n');
    expect(service.getMessageTitle(['1234'], MessageType.saveBatches))
        .toEqual('Error al asignar lotes a  1234 , por favor verificar \n');
  });
});
