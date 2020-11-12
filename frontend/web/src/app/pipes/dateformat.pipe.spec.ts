import { DateformatPipe } from './dateformat.pipe';

describe('DateformatPipe', () => {
  it('create an instance', () => {
    const pipe = new DateformatPipe();
    expect(pipe).toBeTruthy();
  });
  it('should get new dataformat', () => {
    const pipe = new DateformatPipe();
    expect(pipe.transform('12/12/2020-12/12/2020')).toEqual('12/12/2020 - 12/12/2020');
  });
});
