import { Component, computed, input } from "@angular/core";

import { PixFinishPanelComponent } from "../pix-finish-panel/pix-finish-panel.component";
import { PaymentAttempt } from "../../types/payment-attempt";
import { PaymentMethod } from "../../enums/payment-method";

@Component({
  selector: "app-payment-finish",
  imports: [PixFinishPanelComponent],
  templateUrl: "./payment-finish.component.html",
  styleUrl: "./payment-finish.component.scss",
})
export class PaymentFinishComponent {
  payment = input<Nullable<PaymentAttempt>>(null);

  pixPayment = computed(() => {
    const payment = this.payment();
    return payment?.method === PaymentMethod.PIX ? payment : null;
  });

  cardPayment = computed(() => {
    const payment = this.payment();
    return payment?.method === PaymentMethod.CreditCard ? payment : null;
  });

  boletoPayment = computed(() => {
    const payment = this.payment();
    return payment?.method === PaymentMethod.Boleto ? payment : null;
  });
}
