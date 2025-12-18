import { computed, inject, Injectable } from "@angular/core";
import { Router } from "@angular/router";

import { TransactionService } from "@modules/checkout/services/transaction/transaction.service";
import { TransactionStatus } from "@modules/checkout/enums/transaction-status";
import { PaymentAttempt } from "@modules/checkout/types/payment-attempt";
import { TransactionSchema } from "@modules/checkout/schemas/transaction.schema";
import { PaymentStatus } from "@modules/checkout/enums/payment-status";

@Injectable()
export class TransactionStore {
  private readonly router = inject(Router);
  public readonly transactionId = computed(
    () =>
      this.router.routerState.snapshot.root.firstChild!.firstChild!.paramMap?.get("transaction") ??
      null,
  );

  private readonly service = inject(TransactionService);

  public readonly resource = this.service.getTransaction(this.transactionId);

  public readonly isTransactionPaid = computed(
    () =>
      this.resource.hasValue() &&
      this.resource.value().successfulPaymentAttempt?.status === PaymentStatus.Completed,
  );

  public readonly isLoading = computed(
    () => this.resource.isLoading() && !this.resource.hasValue(),
  );

  public readonly error = computed(() => {
    if (this.resource.isLoading()) return false;
    if (this.resource.error()) return true;
    return (
      !this.resource.hasValue() || this.resource.value().status !== TransactionStatus.Available
    );
  });

  public updatePayment(payment: PaymentAttempt) {
    this.resource.update((payload) =>
      TransactionSchema.parse({ ...payload, successfulPaymentAttempt: payment }),
    );
  }
}
