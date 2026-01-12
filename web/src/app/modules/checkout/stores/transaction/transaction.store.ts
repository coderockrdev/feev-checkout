import { computed, inject, Injectable } from "@angular/core";
import { Router } from "@angular/router";

import { TransactionService } from "@modules/checkout/services/transaction/transaction.service";
import { TransactionStatus } from "@modules/checkout/enums/transaction-status";
import { PaymentAttempt } from "@modules/checkout/types/payment-attempt";
import { TransactionSchema } from "@modules/checkout/schemas/transaction.schema";
import { PaymentStatus } from "@modules/checkout/enums/payment-status";
import { PaymentMethod } from "../../enums/payment-method";

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

  public readonly isTransactionPaid = computed(() => {
    if (!this.resource.hasValue()) return false;
    const payment = this.resource.value().successfulPaymentAttempt;

    if (!payment || payment.status === PaymentStatus.Failed) return false;

    return (
      payment.status === PaymentStatus.Completed ||
      [PaymentMethod.Boleto, PaymentMethod.CreditCard].includes(payment.method)
    );
  });

  public readonly hasBoletoLink = computed(() => {
    if (!this.resource.hasValue()) return false;
    const payment = this.resource.value().successfulPaymentAttempt;
    return payment?.method === PaymentMethod.Boleto && !!payment.extraData.link;
  });

  public readonly boletoLink = computed(() => {
    if (!this.resource.hasValue()) return null;
    const payment = this.resource.value().successfulPaymentAttempt;
    return payment?.method === PaymentMethod.Boleto ? payment.extraData.link : null;
  });

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
