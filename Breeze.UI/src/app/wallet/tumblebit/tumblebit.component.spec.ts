import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { CoinNotationPipe } from '../../shared/pipes/coin-notation.pipe';
import { CoinAbbreviationPipe } from '../../shared/pipes/coin-abbreviation.pipe';

import { TumblebitComponent } from './tumblebit.component';

describe('TumblebitComponent', () => {
  let component: TumblebitComponent;
  let fixture: ComponentFixture<TumblebitComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TumblebitComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TumblebitComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
