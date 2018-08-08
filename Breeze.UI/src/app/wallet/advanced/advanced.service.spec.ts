import { TestBed, inject } from '@angular/core/testing';

import { AdvancedService } from './advanced.service';

describe('AdvancedService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [AdvancedService]
    });
  });

  it('should be created', inject([AdvancedService], (service: AdvancedService) => {
    expect(service).toBeTruthy();
  }));
});
