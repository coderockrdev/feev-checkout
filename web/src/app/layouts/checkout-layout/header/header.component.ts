import { Component } from "@angular/core";
import { LogoComponent } from "../../../partials/logo/logo.component";

@Component({
  selector: "[app-header]",
  imports: [LogoComponent],
  templateUrl: "./header.component.html",
  styleUrl: "./header.component.scss",
})
export class HeaderComponent {}
