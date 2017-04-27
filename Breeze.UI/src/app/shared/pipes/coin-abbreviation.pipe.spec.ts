import { CoinAbbreviationPipe } from './coin-abbreviation.pipe';

describe('CoinAbbreviationPipe', () => {
  it('create an instance', () => {
    const pipe = new CoinAbbreviationPipe();
    expect(pipe).toBeTruthy();
  });
});
