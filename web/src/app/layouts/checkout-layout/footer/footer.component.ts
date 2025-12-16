import { Component, computed, inject } from "@angular/core";
import { ThemeService } from "../../../core/services/theme.service";

@Component({
  selector: "[app-footer]",
  imports: [],
  templateUrl: "./footer.component.html",
  styleUrl: "./footer.component.scss",
})
export class FooterComponent {
  theme = inject(ThemeService).theme;
  badges = computed(() => this.theme()?.assets?.badges ?? []);

  readonly currentYear = new Date().getFullYear();
}
