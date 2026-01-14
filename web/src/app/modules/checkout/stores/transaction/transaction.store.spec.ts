import { TestBed } from "@angular/core/testing";

import { TransactionStore } from "./transaction.store";
import { TransactionService } from "../../services/transaction/transaction.service";
import { signal } from "@angular/core";

function createHttpResourceMock<T>(value?: T) {
  const valueSig = signal<T | undefined>(value);
  const statusSig = signal<"idle" | "loading" | "success" | "error">("success");

  return {
    value: valueSig,
    status: statusSig,
    error: signal(null),
    reload: jasmine.createSpy("reload"),
  };
}

describe("TransactionStore", () => {
  let service: TransactionStore;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        TransactionStore,
        {
          provide: TransactionService,
          useFactory: () => ({
            getTransaction: jasmine
              .createSpy("getTransaction")
              .and.returnValue(createHttpResourceMock()),
          }),
        },
      ],
    });

    service = TestBed.inject(TransactionStore);

    TestBed.inject(TransactionService);
  });

  it("should be created", () => {
    expect(service).toBeTruthy();
  });
});
