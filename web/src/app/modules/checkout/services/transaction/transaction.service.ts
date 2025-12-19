import { Injectable, resource, Signal } from "@angular/core";
import { environment } from "@environments/environment";

import { TransactionSchema } from "@modules/checkout/schemas/transaction.schema";

const { apiUrl } = environment;

@Injectable({
  providedIn: "root",
})
export class TransactionService {
  getTransaction(id: Signal<Nullable<string>>) {
    return resource({
      params: () => ({ id: id() }),
      loader: ({ params: { id } }) => {
        return fetch(`${apiUrl}/transaction/${id}`)
          .then((response) => response.json())
          .then((response) => TransactionSchema.parse(response));
      },
    });
  }
}
