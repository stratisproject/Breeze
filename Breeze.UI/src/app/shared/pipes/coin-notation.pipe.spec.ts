import { CoinNotationPipe } from './coin-notation.pipe';

describe('CoinNotationPipe', () => {
  it('create an instance', () => {
    const pipe = new CoinNotationPipe();
    expect(pipe).toBeTruthy();
  });
});
