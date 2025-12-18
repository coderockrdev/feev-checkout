import { Component, inject } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule } from "@angular/forms";

import { CardComponent } from "@shared/components/card/card.component";
import { ColumnsFor } from "@shared/types/table/column";
import { FancyRadioButtonComponent } from "@shared/components/fancy-radio-button/fancy-radio-button.component";
import { IconComponent } from "@shared/components/icon/icon.component";
import { IconFrameComponent } from "@shared/components/icon-frame/icon-frame.component";
import { PAYMENT_METHODS_OPTIONS } from "@modules/checkout/constants/payment-methods-options";
import { PartySummaryComponent } from "@modules/checkout/components/party-summary/party-summary.component";
import { PaymentMethod } from "@modules/checkout/types/payment-method";
import { Product } from "@modules/checkout/types/product";
import { TableComponent } from "@shared/components/table/table.component";
import { ThemeService } from "@shared/services/theme/theme.service";
import { integerToCurrency } from "@shared/utils/currency.utils";

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
  ],
  templateUrl: "./index-page.component.html",
  styleUrl: "./index-page.component.scss",
})
export class IndexPageComponent {
  readonly theme = inject(ThemeService).theme;
  readonly paymentMethods = PAYMENT_METHODS_OPTIONS;

  paymentMethodForm = new FormGroup({
    method: new FormControl<Nullable<PaymentMethod>>(null),
  });

  readonly total = integerToCurrency(148189);

  readonly products: Product[] = [
    {
      id: "1",
      name: "Multa por atraso",
      price: 25440,
    },
    {
      id: "2",
      name: "Taxas administrativas",
      price: 850,
    },
    {
      id: "3",
      name: "Valor original do contrato",
      price: 121899,
    },
  ];

  readonly columns: ColumnsFor<Product> = [
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

  onPaymentMethodChange(value: string) {
    console.log(value);
  }
}
