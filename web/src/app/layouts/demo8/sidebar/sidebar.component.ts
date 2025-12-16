import { Component, inject } from "@angular/core";
import { SidebarFooterComponent } from "../partials/sidebar-footer/sidebar-footer.component";
import { SidebarMenuComponent } from "../partials/sidebar-menu/sidebar-menu.component";
import { ThemeService } from "../../../core/services/theme.service";

@Component({
  selector: "[app-sidebar]",
  imports: [SidebarFooterComponent, SidebarMenuComponent],
  templateUrl: "./sidebar.component.html",
  styleUrl: "./sidebar.component.scss",
})
export class SidebarComponent {
  theme = inject(ThemeService).theme;
}
