import { AfterViewInit, Directive } from "@angular/core";

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
