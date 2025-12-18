import { Component, inject } from "@angular/core";
import { ThemeService } from "@shared/services/theme/theme.service";

@Component({
  selector: "app-logo",
  imports: [],
  templateUrl: "./logo.component.html",
  styleUrl: "./logo.component.scss",
})
export class LogoComponent {
  theme = inject(ThemeService).theme;
}
