import { ComponentFixture, TestBed } from "@angular/core/testing";
import { FormControl, FormGroup } from "@angular/forms";

import { ThemeService } from "@shared/services/theme/theme.service";

import { PaymentFormComponent } from "./payment-form.component";
import { Transaction } from "../../types/transaction";

const transactionMock = {
  customer: {
    name: "John Doe",
    document: "00000000000",
  },
  establishment: {
    cnpj: "00000000000000",
    fullName: "Test Store",
  },
  paymentRules: [],
  products: [],
  totalAmount: 0,
} as unknown as Transaction;

describe("PaymentFormComponent", () => {
  let component: PaymentFormComponent;
  let fixture: ComponentFixture<PaymentFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PaymentFormComponent],
      providers: [{ provide: ThemeService, useFactory: () => ({ theme: {} }) }],
    }).compileComponents();

    fixture = TestBed.createComponent(PaymentFormComponent);
    component = fixture.componentInstance;

    TestBed.inject(ThemeService);

    fixture.componentRef.setInput("transaction", transactionMock);
    fixture.componentRef.setInput(
      "paymentMethodForm",
      new FormGroup({
        method: new FormControl(null),
      }),
    );
    fixture.componentRef.setInput("creditCardForm", new FormGroup({}));
    fixture.componentRef.setInput("onSubmit", jasmine.createSpy("onSubmit"));

    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });
});
