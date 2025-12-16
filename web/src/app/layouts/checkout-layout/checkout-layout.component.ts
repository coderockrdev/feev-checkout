import { AfterViewInit, Component, HostBinding, inject } from "@angular/core";
import { RouterOutlet } from "@angular/router";
import { MetronicInitService } from "../../core/services/metronic-init.service";
import { HeaderComponent } from "./header/header.component";
import { FooterComponent } from "./footer/footer.component";

@Component({
  selector: "app-checkout-layout",
  imports: [RouterOutlet, HeaderComponent, FooterComponent],
  templateUrl: "./checkout-layout.component.html",
  styleUrl: "./checkout-layout.component.scss",
})
export class CheckoutLayoutComponent implements AfterViewInit {
  @HostBinding("class") class =
    "flex grow flex-col in-data-kt-[sticky-header=on]:pt-(--header-height-default)";
  private metronicInitService = inject(MetronicInitService);

  ngAfterViewInit(): void {
    this.metronicInitService.init();
  }
}
