import { Component, HostBinding } from "@angular/core";
import { RouterOutlet } from "@angular/router";
import { HeaderComponent } from "./header/header.component";
import { FooterComponent } from "./footer/footer.component";

@Component({
  selector: "app-layout",
  imports: [RouterOutlet, HeaderComponent, FooterComponent],
  templateUrl: "./app-layout.component.html",
  styleUrl: "./app-layout.component.scss",
})
export class AppLayoutComponent {
  @HostBinding("class") class =
    "flex grow flex-col in-data-kt-[sticky-header=on]:pt-(--header-height-default)";
}
