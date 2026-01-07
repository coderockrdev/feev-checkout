import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PaymentFinishComponent } from './payment-finish.component';

describe('PaymentFinishComponent', () => {
  let component: PaymentFinishComponent;
  let fixture: ComponentFixture<PaymentFinishComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PaymentFinishComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PaymentFinishComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
