import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfirmMnemonicComponent } from './confirm-mnemonic.component';

describe('ConfirmMnemonicComponent', () => {
  let component: ConfirmMnemonicComponent;
  let fixture: ComponentFixture<ConfirmMnemonicComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ConfirmMnemonicComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConfirmMnemonicComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
