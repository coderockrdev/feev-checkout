import { AfterViewInit, Component, HostBinding } from "@angular/core";
import { RouterOutlet } from "@angular/router";
import { HeaderComponent } from "./header/header.component";
import { FooterComponent } from "./footer/footer.component";
import { ModalComponent } from "@app/shared/components/modal/modal.component";

@Component({
  selector: "app-layout",
  imports: [RouterOutlet, HeaderComponent, FooterComponent, ModalComponent],
  templateUrl: "./app-layout.component.html",
  styleUrl: "./app-layout.component.scss",
})
export class AppLayoutComponent implements AfterViewInit {
  @HostBinding("class") class =
    "flex grow flex-col in-data-kt-[sticky-header=on]:pt-(--header-height-default)";

  ngAfterViewInit(): void {
    // Start KTUI components
    Object.keys(window).forEach((key) => {
      const propKey = key as keyof typeof window;
      if (
        key.startsWith("KT") &&
        typeof window[propKey] === "object" &&
        "init" in window[propKey]
      ) {
        window[propKey].init();
      }
    });
  }
}
