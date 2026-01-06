import { Directive, AfterViewInit } from "@angular/core";

@Directive({
  selector: "[appKtModal]",
})
export class KtModalDirective implements AfterViewInit {
  constructor() {}

  ngAfterViewInit(): void {
    window.KTModal?.init();
  }
}
