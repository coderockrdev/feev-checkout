import { ComponentFixture, TestBed } from "@angular/core/testing";

import { ThemeService } from "@shared/services/theme/theme.service";
import { ModalService } from "@shared/services/modal/modal.service";
import { TransactionService } from "@modules/checkout/services/transaction/transaction.service";
import { TransactionStore } from "@modules/checkout/stores/transaction/transaction.store";

import { IndexPageComponent } from "./index-page.component";

describe("IndexPageComponent", () => {
  let component: IndexPageComponent;
  let fixture: ComponentFixture<IndexPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [IndexPageComponent],
      providers: [
        { provide: ThemeService, useFactory: () => ({ theme: {} }) },
        { provide: TransactionService, useFactory: () => ({}) },
        {
          provide: TransactionStore,
          useFactory: () => ({
            resource: { value: jasmine.createSpy("value").and.returnValue(null) },
            isTransactionPaid: jasmine.createSpy("isTransactionPaid"),
            hasBoletoLink: jasmine.createSpy("hasBoletoLink").and.returnValue(false),
            boletoLink: jasmine.createSpy("boletoLink").and.returnValue(null),
            isBoletoPending: jasmine.createSpy("isBoletoPending").and.returnValue(false),
            isLoading: jasmine.createSpy("isLoading").and.returnValue(false),
            error: jasmine.createSpy("error").and.returnValue(false),
          }),
        },
        { provide: ModalService, useFactory: () => ({}) },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(IndexPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });
});
