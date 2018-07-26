import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AdvancedIcoComponent } from './advanced-ico.component';

describe('AdvancedIcoComponent', () => {
  let component: AdvancedIcoComponent;
  let fixture: ComponentFixture<AdvancedIcoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AdvancedIcoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AdvancedIcoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
