import { Component, inject } from "@angular/core";
import { ThemeService } from "../../../core/services/theme.service";

@Component({
  selector: "[app-sidebar]",
  templateUrl: "./sidebar.component.html",
})
export class SidebarComponent {
  theme = inject(ThemeService).theme;
}
