import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ShowMnemonicComponent } from './show-mnemonic.component';

describe('ShowMnemonicComponent', () => {
  let component: ShowMnemonicComponent;
  let fixture: ComponentFixture<ShowMnemonicComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ShowMnemonicComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ShowMnemonicComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
