import { inject, Injectable, Signal } from "@angular/core";
import * as z from "zod";

import { TransactionSchema } from "@modules/checkout/schemas/transaction.schema";
import { PaymentRequestDtoSchema } from "@modules/checkout/schemas/payment-request-dto.schema";
import { ApiService } from "@app/shared/services/api/api.service";
import { PaymentAttemptSchema } from "../../schemas/payment-attempt.schema";

@Injectable({
  providedIn: "root",
})
export class TransactionService {
  protected readonly api = inject(ApiService);

  getTransaction(id: Signal<Nullable<string>>) {
    return this.api.get(
      () => ({
        path: `transaction/${id()}`,
      }),
      { parse: (response) => TransactionSchema.parse(response) },
    );
  }

  pay(transactionId: Signal<Nullable<string>>, body: z.infer<typeof PaymentRequestDtoSchema>) {
    return this.api.post(
      () => ({
        path: `payment/${transactionId()}`,
      }),
      body,
      { parse: (response) => PaymentAttemptSchema.parse(response) },
    );
  }
}
