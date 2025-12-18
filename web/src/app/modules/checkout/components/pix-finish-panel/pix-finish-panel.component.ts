import {
  Component,
  computed,
  effect,
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
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";

import { CardComponent } from "@shared/components/card/card.component";
import { InputComponent } from "@shared/components/input/input.component";
import { IconComponent } from "@shared/components/icon/icon.component";
import { PixPaymentAttempt } from "@modules/checkout/types/pix-payment-attempt";
import { filter, interval, map, startWith, Subscription, timer } from "rxjs";
import { PaymentMethod } from "@modules/checkout/enums/payment-method";
import { PaymentAttempt } from "@modules/checkout/types/payment-attempt";
import { IconName } from "@shared/types/icon-name";
import { TransactionStore } from "@modules/checkout/stores/transaction/transaction.store";
import { PaymentStatus } from "@modules/checkout/enums/payment-status";
import { ToastService } from "@shared/services/toast/toast.service";
import { ModalService } from "@shared/services/modal/modal.service";

@Component({
  selector: "app-pix-finish-panel",
  imports: [CardComponent, DecimalPipe, QRCodeComponent, InputComponent, IconComponent],
  templateUrl: "./pix-finish-panel.component.html",
  styleUrl: "./pix-finish-panel.component.scss",
})
export class PixFinishPanelComponent implements OnDestroy, OnInit {
  constructor() {
    this.startPolling();

    effect(() => {
      if (this.isExpired()) {
        this.stopPolling.set(true);
        this.transaction.resource.reload();
      }
    });
  }

  @ViewChild("input", { static: true }) private input!: InputComponent;

  protected readonly clipboard = inject(Clipboard);
  protected readonly transaction = inject(TransactionStore);
  protected readonly toast = inject(ToastService);
  protected readonly modal = inject(ModalService);

  payment = input<Nullable<PixPaymentAttempt>>(null);

  protected readonly code = computed(() => this.payment()?.extraData.code);

  protected copyIcon = computed<IconName>(() => (this.isCopied() ? "copy-success" : "copy"));

  private sub?: Subscription;

  protected minutes = signal(0);
  protected seconds = signal(0);
  protected isExpired = signal(false);
  protected isCopied = signal(false);

  private readonly stopPolling = signal(false);

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
    const transaction = this.transaction.resource.value();

    if (!this.transaction.resource.hasValue() || !transaction?.successfulPaymentAttempt) return;
    if (!this.isPix(transaction?.successfulPaymentAttempt)) return;

    const expireAt = new Date(transaction.successfulPaymentAttempt.extraData.expireAt);

    if (expireAt.getTime() <= Date.now()) {
      this.dispatchExpiredMessage();
    }
  }

  protected isPix(payment: Nullable<PaymentAttempt>): payment is PixPaymentAttempt {
    return payment?.method === PaymentMethod.PIX;
  }

  protected onCopy() {
    if (this.isCopied()) return;

    this.input.focusAndSelectContent();
    this.clipboard.copy(this.code()!);

    this.isCopied.set(true);

    this.toast.show({
      message: "Código copiado com sucesso!",
      position: "bottom-end",
      progress: true,
    });

    timer(2000).subscribe(() => this.isCopied.set(false));
  }

  onFocus() {
    this.input.focusAndSelectContent();
  }

  private evaluateStopCondition() {
    const transaction = this.transaction.resource.value();

    if (!transaction) return;

    const payment = transaction.successfulPaymentAttempt;

    if (!this.isPix(payment)) return;

    if (payment?.status === PaymentStatus.Completed) {
      this.stopPolling.set(true);
      return;
    }

    if (new Date(payment.extraData.expireAt).getTime() <= Date.now()) {
      this.stopPolling.set(true);
    }
  }

  private startPolling() {
    interval(5000)
      .pipe(
        takeUntilDestroyed(),
        filter(() => !this.stopPolling()),
      )
      .subscribe(() => {
        this.transaction.resource.reload();
        this.evaluateStopCondition();
      });
  }

  private dispatchExpiredMessage() {
    this.toast.show({
      message: "Código expirado.",
      variant: "destructive",
      position: "bottom-end",
    });
  }
}
