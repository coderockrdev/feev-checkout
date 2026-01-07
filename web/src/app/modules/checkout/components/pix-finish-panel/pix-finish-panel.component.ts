import {
  Component,
  computed,
  inject,
  input,
  OnDestroy,
  OnInit,
  signal,
  ViewChild,
} from "@angular/core";
import { DecimalPipe } from "@angular/common";
import { QRCodeComponent } from "angularx-qrcode";
import { Clipboard } from "@angular/cdk/clipboard";

import { CardComponent } from "@shared/components/card/card.component";
import { InputComponent } from "@shared/components/input/input.component";
import { IconComponent } from "@shared/components/icon/icon.component";
import { PixPaymentAttempt } from "@modules/checkout/types/pix-payment-attempt";
import { interval, map, startWith, Subscription, timer } from "rxjs";
import { PaymentMethod } from "@modules/checkout/enums/payment-method";
import { PaymentAttempt } from "@modules/checkout/types/payment-attempt";
import { IconName } from "@shared/types/icon-name";

@Component({
  selector: "app-pix-finish-panel",
  imports: [CardComponent, DecimalPipe, QRCodeComponent, InputComponent, IconComponent],
  templateUrl: "./pix-finish-panel.component.html",
  styleUrl: "./pix-finish-panel.component.scss",
})
export class PixFinishPanelComponent implements OnDestroy, OnInit {
  @ViewChild("input", { static: true }) private input!: InputComponent;

  protected clipboard = inject(Clipboard);

  payment = input<Nullable<PixPaymentAttempt>>(null);

  protected readonly code = computed(() => this.payment()?.extraData.code);

  protected copyIcon = computed<IconName>(() => (this.isCopied() ? "copy-success" : "copy"));

  private sub?: Subscription;

  protected minutes = signal(0);
  protected seconds = signal(0);
  protected isExpired = signal(false);
  protected isCopied = signal(false);

  ngOnInit() {
    const payment = this.payment();

    if (!this.isPix(payment)) return;

    const expireAt = new Date(payment.extraData.expireAt).getTime();

    this.sub = interval(1000)
      .pipe(
        startWith(0),
        map(() => Math.max(expireAt - Date.now(), 0)),
      )
      .subscribe((ms) => {
        this.minutes.set(Math.floor(ms / 1000 / 60));
        this.seconds.set(Math.floor((ms / 1000) % 60));

        if (ms === 0) {
          this.isExpired.set(true);
          this.sub?.unsubscribe();
        }
      });
  }

  ngOnDestroy() {
    this.sub?.unsubscribe();
  }

  protected isPix(payment: Nullable<PaymentAttempt>): payment is PixPaymentAttempt {
    return payment?.method === PaymentMethod.PIX;
  }

  protected onCopy() {
    if (this.isCopied()) return;

    this.input.focusAndSelectContent();
    this.clipboard.copy(this.code()!);

    this.isCopied.set(true);

    timer(2000).subscribe(() => this.isCopied.set(false));
  }

  onFocus() {
    this.input.focusAndSelectContent();
  }
}
