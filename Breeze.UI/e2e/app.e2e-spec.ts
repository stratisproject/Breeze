import { BreezeUIPage } from './app.po';

describe('breeze-ui App', function() {
  let page: BreezeUIPage;

  beforeEach(() => {
    page = new BreezeUIPage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
