import { Component, computed, inject, input } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule } from "@angular/forms";

import { ThemeService } from "@shared/services/theme/theme.service";
import { maskDocument } from "@modules/checkout/utils/document.utils";
import { CardComponent } from "@shared/components/card/card.component";
import { TableComponent } from "@shared/components/table/table.component";
import { IconFrameComponent } from "@shared/components/icon-frame/icon-frame.component";
import { IconComponent } from "@shared/components/icon/icon.component";
import { InputComponent } from "@shared/components/input/input.component";
import { SelectComponent } from "@shared/components/select/select.component";
import { ColumnsFor } from "@shared/types/table/column";
import { integerToCurrency } from "@shared/utils/currency.utils";
import { Product } from "@modules/checkout/types/product";
import { FancyRadioButtonComponent } from "@shared/components/fancy-radio-button/fancy-radio-button.component";
import { PAYMENT_METHODS_OPTIONS } from "@modules/checkout/constants/payment-methods-options";
import { PartySummaryComponent } from "@modules/checkout/components/party-summary/party-summary.component";
import { PaymentMethod } from "@modules/checkout/enums/payment-method";
import { SelectOption } from "@shared/types/select/select-option";
import { Mask } from "@modules/checkout/masks/mask";
import { RadioGroupComponent } from "@shared/components/radio-group/radio-group.component";
import { Transaction } from "@modules/checkout/types/transaction";

@Component({
  selector: "app-payment-form",
  imports: [
    PartySummaryComponent,
    CardComponent,
    TableComponent,
    IconFrameComponent,
    IconComponent,
    InputComponent,
    SelectComponent,
    ReactiveFormsModule,
    FancyRadioButtonComponent,
    RadioGroupComponent,
  ],
  templateUrl: "./payment-form.component.html",
  styleUrl: "./payment-form.component.scss",
})
export class PaymentFormComponent {
  protected readonly theme = inject(ThemeService).theme;

  transaction = input<Nullable<Transaction>>(null);
  isLoading = input<boolean>(false);
  isCreditCard = input<boolean>(false);
  hasPaymentMethod = input<boolean>(false);
  paymentMethodForm = input.required<FormGroup<{ method: FormControl<Nullable<PaymentMethod>> }>>();
  creditCardForm = input.required<FormGroup>();
  isSubmitting = input<boolean>(false);
  onSubmit = input.required<() => void>();

  protected readonly cardMask = Mask.card;
  protected readonly validThruMask = Mask.validThru;
  protected readonly cvvMask = Mask.cvv;

  protected readonly customer = computed(() => this.transaction()?.customer);
  protected readonly customerName = computed(() => this.customer()?.name ?? "");
  protected readonly customerDocument = computed(() => this.customer()?.document ?? "");

  protected readonly establishmentDocument = computed(
    () => this.transaction()?.establishment?.cnpj,
  );
  protected readonly establishmentName = computed(
    () => this.transaction()?.establishment?.fullName ?? "",
  );

  protected readonly paymentMethods = computed(() =>
    PAYMENT_METHODS_OPTIONS.filter((method) =>
      this.transaction()?.paymentRules.some((rule) => rule.method === method.value),
    ),
  );

  protected readonly creditCardRules = computed(() =>
    this.transaction()?.paymentRules.find((rule) => rule.method === PaymentMethod.CreditCard),
  );

  protected readonly installmentOptions = computed<SelectOption<number>[]>(
    () =>
      this.creditCardRules()?.installments.map((installment) => ({
        value: installment.number,
        label: `${installment.number}x de ${integerToCurrency(installment.finalAmount)}`,
      })) ?? [],
  );

  protected readonly products = computed(() => this.transaction()?.products ?? []);
  protected readonly total = computed(() =>
    integerToCurrency(this.transaction()?.totalAmount ?? 0),
  );

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

  protected getPartyName(name?: Nullable<string>) {
    return name ?? "";
  }

  protected getPartyDocument(document?: Nullable<string>, obscure = true) {
    return !document ? "" : maskDocument(document, obscure);
  }
}
