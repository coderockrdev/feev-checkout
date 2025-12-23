import { AfterViewInit, Directive } from "@angular/core";

declare global {
  interface Window {
    KTSelect: {
      init: () => void;
    };
  }
}

@Directive({
  selector: "[appKtSelect]",
  standalone: true,
})
export class KtSelectDirective implements AfterViewInit {
  constructor() {}

  ngAfterViewInit(): void {
    window.KTSelect?.init();
  }
}
