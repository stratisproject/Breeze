import { browser, element, by } from 'protractor';

export class BreezePage {
  navigateTo() {
    return browser.get('/');
  }

  getParagraphText() {
    return element(by.css('app-root h1')).getText();
}

  getTitle() {
    let title: string;
    return browser.getTitle();
  }
}
