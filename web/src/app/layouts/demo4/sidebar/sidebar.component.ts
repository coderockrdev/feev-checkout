import { Component, inject } from "@angular/core";
import { TopbarUserDropdownComponent } from "../../../partials/topbar-user-dropdown/topbar-user-dropdown.component";
import { TopbarAppsComponent } from "../../../partials/topbar-apps/topbar-apps.component";
import { TopbarChatComponent } from "../../../partials/topbar-chat/topbar-chat.component";
import { ThemeService } from "../../../core/services/theme.service";

@Component({
  selector: "[app-sidebar]",
  imports: [TopbarUserDropdownComponent, TopbarAppsComponent, TopbarChatComponent],
  templateUrl: "./sidebar.component.html",
  styleUrl: "./sidebar.component.scss",
})
export class SidebarComponent {
  theme = inject(ThemeService).theme;
}
