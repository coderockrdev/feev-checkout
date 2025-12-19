import { Component, computed, inject } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule } from "@angular/forms";
import { toSignal } from "@angular/core/rxjs-interop";

import { CardComponent } from "@shared/components/card/card.component";
import { ColumnsFor } from "@shared/types/table/column";
import { FancyRadioButtonComponent } from "@shared/components/fancy-radio-button/fancy-radio-button.component";
import { IconComponent } from "@shared/components/icon/icon.component";
import { IconFrameComponent } from "@shared/components/icon-frame/icon-frame.component";
import { PAYMENT_METHODS_OPTIONS } from "@modules/checkout/constants/payment-methods-options";
import { PartySummaryComponent } from "@modules/checkout/components/party-summary/party-summary.component";
import { Product } from "@modules/checkout/types/product";
import { TableComponent } from "@shared/components/table/table.component";
import { ThemeService } from "@shared/services/theme/theme.service";
import { integerToCurrency } from "@shared/utils/currency.utils";
import { RadioGroupComponent } from "@app/shared/components/radio-group/radio-group.component";
import { TransactionService } from "@app/modules/checkout/services/transaction/transaction.service";
import { ActivatedRoute } from "@angular/router";
import { PaymentMethod } from "@app/modules/checkout/enums/payment-method";
import { maskDocument } from "@app/modules/checkout/utils/document.utils";
import { InputComponent } from "@app/shared/components/input/input.component";
import { SelectComponent } from "@app/shared/components/select/select.component";
import { SelectOption } from "@app/shared/types/select/select-option";

@Component({
  selector: "app-index-page",
  imports: [
    CardComponent,
    IconComponent,
    IconFrameComponent,
    PartySummaryComponent,
    TableComponent,
    FancyRadioButtonComponent,
    ReactiveFormsModule,
    RadioGroupComponent,
    InputComponent,
    SelectComponent,
  ],
  templateUrl: "./index-page.component.html",
  styleUrl: "./index-page.component.scss",
})
export class IndexPageComponent {
  private readonly route = inject(ActivatedRoute);
  protected readonly theme = inject(ThemeService).theme;
  protected readonly service = inject(TransactionService);

  private readonly params = toSignal(this.route.paramMap, { initialValue: null });

  protected readonly transactionId = computed(() => this.params()?.get("transaction") ?? null);

  protected readonly transaction = this.service.getTransaction(this.transactionId);
  protected readonly isLoading = computed(() => this.transaction.isLoading());

  protected readonly customer = computed(() => this.transaction.value()?.customer);

  // TODO: Should be replaced with establishment document from the transaction
  protected readonly domainDocument = computed(() => "31743818000128");

  protected readonly paymentMethods = computed(() =>
    PAYMENT_METHODS_OPTIONS.filter((method) =>
      this.transaction.value()?.paymentRules.some((rule) => rule.method === method.value),
    ),
  );

  protected readonly hasPaymentMethod = computed(() => this.method() !== null);
  protected readonly isCreditCard = computed(() => this.method() === PaymentMethod.CreditCard);
  protected readonly total = computed(() =>
    integerToCurrency(this.transaction.value()?.totalAmount ?? 0),
  );
  protected readonly products = computed(() => this.transaction.value()?.products ?? []);

  protected paymentMethodForm = new FormGroup({
    method: new FormControl<Nullable<PaymentMethod>>(null),
  });

  protected method = toSignal(this.paymentMethodForm.controls.method.valueChanges, {
    initialValue: this.paymentMethodForm.controls.method.value,
  });

  protected readonly columns: ColumnsFor<Product> = [
    {
      key: "name",
      label: "Descrição",
      variant: "highlight",
    },
    {
      key: "price",
      label: "Valor Unitário",
      render: integerToCurrency,
    },
  ];

  protected readonly creditCardRules = computed(() =>
    this.transaction.value()?.paymentRules.find((rule) => rule.method === PaymentMethod.CreditCard),
  );

  protected readonly installmentOptions = computed<SelectOption<number>[]>(
    () =>
      this.creditCardRules()?.installments.map((installment) => ({
        value: installment.number,
        label: `${installment.number}x de ${integerToCurrency(installment.finalAmount)}`,
      })) ?? [],
  );

  protected getPartyName(name?: Nullable<string>) {
    return name ?? "";
  }

  protected getPartyDocument(document?: Nullable<string>, obscure = true) {
    return !document ? "" : maskDocument(document, obscure);
  }
}
