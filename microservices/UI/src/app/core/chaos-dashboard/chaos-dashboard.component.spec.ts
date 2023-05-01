import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChaosDashboardComponent } from './chaos-dashboard.component';

describe('ChaosDashboardComponent', () => {
  let component: ChaosDashboardComponent;
  let fixture: ComponentFixture<ChaosDashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ChaosDashboardComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChaosDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
