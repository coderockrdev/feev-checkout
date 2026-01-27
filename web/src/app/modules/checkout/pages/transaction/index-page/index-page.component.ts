import { Component, computed, effect, inject, signal } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule } from "@angular/forms";
import { toSignal } from "@angular/core/rxjs-interop";
import { HttpResourceRef } from "@angular/common/http";
import * as z from "zod";

import { IconComponent } from "@shared/components/icon/icon.component";
import { IconFrameComponent } from "@shared/components/icon-frame/icon-frame.component";
import { ThemeService } from "@shared/services/theme/theme.service";
import { PaymentMethod } from "@modules/checkout/enums/payment-method";
import { CardNumberSchema } from "@modules/checkout/schemas/card-number.schema";
import { ValidThruSchema } from "@modules/checkout/schemas/valid-thru.schema";
import { zodForm } from "@shared/utils/zod.utils";
import { PaymentRequestDtoSchema } from "@modules/checkout/schemas/payment-request-dto.schema";
import { PaymentFormComponent } from "@modules/checkout/components/payment-form/payment-form.component";
import { PaymentAttempt } from "@modules/checkout/types/payment-attempt";
import { PaymentFinishComponent } from "@modules/checkout/components/payment-finish/payment-finish.component";
import { TransactionStatus } from "@modules/checkout/enums/transaction-status";
import { PaymentStatus } from "@modules/checkout/enums/payment-status";
import { isFuture } from "@shared/utils/date.utils";
import { TransactionStore } from "@modules/checkout/stores/transaction/transaction.store";
import { TransactionService } from "@modules/checkout/services/transaction/transaction.service";
import { ModalService } from "@app/shared/services/modal/modal.service";
import { CardComponent } from "@app/shared/components/card/card.component";
import { MessageCardComponent } from "@app/shared/components/message-card/message-card.component";

@Component({
  selector: "app-index-page",
  imports: [
    IconComponent,
    IconFrameComponent,
    ReactiveFormsModule,
    PaymentFormComponent,
    PaymentFinishComponent,
    CardComponent,
    MessageCardComponent,
  ],
  templateUrl: "./index-page.component.html",
  styleUrl: "./index-page.component.scss",
  host: {
    class: "h-full",
  },
})
export class IndexPageComponent {
  constructor() {
    effect(() => {
      if (!this.isCreditCard()) {
        this.creditCardForm.reset();
      }
    });

    effect(() => {
      const payment = this.payment();

      if (!payment) return;

      if (payment.error()) {
        this.isSubmitting.set(false);

        this.modal.open({
          title: "Algo saiu errado!",
          message: [
            "Não foi possível concluir a transação com o método de pagamento escolhido.",
            "Por favor, escolha <b>outro método de pagamento</b> ou tente novamente.",
          ],
          type: "error",
          actions: [
            {
              label: "Tentar novamente",
              variant: "secondary",
              onClick: () => {
                this.modal.close();

                // Wait until the modal is fully closed
                setTimeout(() => {
                  this.onSubmit();
                }, 300);
              },
            },
            {
              label: "Alterar forma de pagamento",
              icon: "right",
              onClick: () => this.modal.close(),
            },
          ],
        });
      }

      if (payment.hasValue()) {
        this.isSubmitting.set(false);

        const value = payment.value();

        this.transaction.updatePayment(value);

        if (value.method === PaymentMethod.Boleto) {
          window.open(value.extraData.link, "_blank");
        }
      }
    });
  }

  protected readonly theme = inject(ThemeService).theme;
  private readonly service = inject(TransactionService);
  protected readonly transaction = inject(TransactionStore);
  protected readonly modal = inject(ModalService);

  protected isSubmitting = signal(false);

  protected payment = signal<Nullable<HttpResourceRef<PaymentAttempt | undefined>>>(null);

  protected paymentInfo = computed<Nullable<PaymentAttempt>>(
    () => this.transaction.resource.value()?.successfulPaymentAttempt ?? null,
  );

  protected showPaymentStep = computed(() => {
    const paymentInfo = this.paymentInfo();

    if (paymentInfo?.status === PaymentStatus.Pending || paymentInfo?.status === null) {
      if (paymentInfo.method === PaymentMethod.PIX)
        return isFuture(paymentInfo.extraData.expireAt, 3000);
    }

    return false;
  });

  protected readonly errorMessage = computed(() => {
    if (
      this.transaction.resource.hasValue() &&
      this.transaction.resource.value().status !== TransactionStatus.Available
    ) {
      return this.transaction.resource.value().status === TransactionStatus.Canceled
        ? "A transação solicitada foi cancelada e não está mais disponível"
        : "A transação solicitada expirou e não está mais disponível";
    }

    return "A transação solicitada não pode ser encontrada ou está indisponível.";
  });

  protected readonly hasPaymentMethod = computed(() => this.method() !== null);
  protected readonly isCreditCard = computed(() => this.method() === PaymentMethod.CreditCard);

  protected paymentMethodForm = new FormGroup({
    method: new FormControl<Nullable<PaymentMethod>>(null),
  });

  protected creditCardForm = zodForm({
    cardHolder: {
      defaultValue: "",
      schema: z.string().min(1, { message: "O nome impresso no cartão é obrigatório" }),
    },
    cardNumber: {
      defaultValue: "",
      schema: CardNumberSchema,
    },
    validThru: {
      defaultValue: "",
      schema: ValidThruSchema,
    },
    cvv: {
      defaultValue: "",
      schema: z
        .string()
        .nonempty({ message: "O CVV é obrigatório" })
        .regex(/^\d{3}$/, { message: "CVV inválido" }),
    },
    installments: {
      defaultValue: null,
      schema: z.preprocess(
        (value) => (!value ? undefined : value),
        z.coerce
          .number({ message: "Seleciona a quantidade de parcelas" })
          .int({ message: "Quantidade de parcelas inválidas" })
          .min(1, { message: "Quantidade de parcelas inválidas" }),
      ),
    },
  });

  protected installments = toSignal(this.creditCardForm.controls.installments.valueChanges, {
    initialValue: this.creditCardForm.controls.installments.value,
  });

  protected method = toSignal(this.paymentMethodForm.controls.method.valueChanges, {
    initialValue: this.paymentMethodForm.controls.method.value,
  });

  onSubmit = () => {
    if (this.isSubmitting() || this.transaction.isLoading()) return;

    const isCreditCard = this.isCreditCard();

    if (isCreditCard) {
      this.creditCardForm.markAllAsTouched();
      this.creditCardForm.updateValueAndValidity();

      if (this.creditCardForm.invalid) {
        return;
      }
    }

    const rawValue = isCreditCard ? this.creditCardForm.getRawValue() : {};

    const payload = PaymentRequestDtoSchema.safeParse({
      method: this.method(),
      ...rawValue,
    });

    if (!payload.success) {
      this.modal.open({
        title: "Algo saiu errado!",
        message:
          "Não foi possível concluir a transação, revise os dados inseridos e tente novamente.",
        type: "error",
        actions: [
          {
            label: "Fechar",
            variant: "secondary",
            onClick: () => this.modal.close(),
          },
        ],
      });
      return;
    }

    this.isSubmitting.set(true);

    const payment = this.service.pay(this.transaction.transactionId, payload.data);

    this.payment.set(payment);
  };
}
