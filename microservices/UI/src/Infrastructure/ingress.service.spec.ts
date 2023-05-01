import { TestBed } from '@angular/core/testing';

import { IngressService } from './ingress.service';

describe('IngressService', () => {
  let service: IngressService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(IngressService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
