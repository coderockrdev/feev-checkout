import { Component, computed, effect, inject, signal, ViewChild } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule } from "@angular/forms";
import { ActivatedRoute } from "@angular/router";
import { toSignal } from "@angular/core/rxjs-interop";
import { HttpResourceRef } from "@angular/common/http";
import * as z from "zod";

import { IconComponent } from "@shared/components/icon/icon.component";
import { IconFrameComponent } from "@shared/components/icon-frame/icon-frame.component";
import { ThemeService } from "@shared/services/theme/theme.service";
import { TransactionService } from "@modules/checkout/services/transaction/transaction.service";
import { PaymentMethod } from "@modules/checkout/enums/payment-method";
import { CardNumberSchema } from "@modules/checkout/schemas/card-number.schema";
import { ValidThruSchema } from "@modules/checkout/schemas/valid-thru.schema";
import { zodForm } from "@shared/utils/zod.utils";
import { PaymentRequestDtoSchema } from "@modules/checkout/schemas/payment-request-dto.schema";
import { ModalComponent } from "@shared/components/modal/modal.component";
import { PaymentFormComponent } from "@modules/checkout/components/payment-form/payment-form.component";
import { PixFinishPanelComponent } from "@app/modules/checkout/components/pix-finish-panel/pix-finish-panel.component";
import { PaymentAttemptSchema } from "@app/modules/checkout/schemas/payment-attempt.schema";

@Component({
  selector: "app-index-page",
  imports: [
    IconComponent,
    IconFrameComponent,
    ReactiveFormsModule,
    ModalComponent,
    PaymentFormComponent,
    PixFinishPanelComponent,
  ],
  templateUrl: "./index-page.component.html",
  styleUrl: "./index-page.component.scss",
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
                this.onSubmit();
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

      if (payment.value()) {
        this.isSubmitting.set(false);
      }
    });
  }

  @ViewChild("modal") private modal!: ModalComponent;

  private readonly route = inject(ActivatedRoute);
  protected readonly theme = inject(ThemeService).theme;
  protected readonly service = inject(TransactionService);

  private readonly params = toSignal(this.route.paramMap, { initialValue: null });

  protected readonly transactionId = computed(() => this.params()?.get("transaction") ?? null);

  protected readonly transaction = this.service.getTransaction(this.transactionId);
  protected readonly isLoading = computed(() => this.transaction.isLoading());
  protected readonly error = computed(() => this.transaction.error());
  protected isSubmitting = signal(false);

  protected payment = signal<Nullable<HttpResourceRef<z.infer<typeof PaymentAttemptSchema>>>>(null);

  protected readonly errorMessage = computed(() => {
    return this.transaction.statusCode() === 404
      ? "Transação não encontrada"
      : "Não foi possível carregar essa transação";
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
    if (this.isSubmitting() || this.isLoading()) return;

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
      throw new Error("Invalid payload");
    }

    this.isSubmitting.set(true);

    // this.payment.set(this.service.pay(this.transactionId, payload.data));
  };
}
