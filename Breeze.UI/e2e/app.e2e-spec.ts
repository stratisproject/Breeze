import { BreezePage } from './app.po';

describe('breeze App', function() {
  let page: BreezePage;

  beforeEach(() => {
    page = new BreezePage();
  });

  it('title should be Breeze', () => {
    page.navigateTo();
    expect(page.getTitle()).toEqual('Breeze');
  });
});
