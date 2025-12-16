import { Component, inject } from "@angular/core";
import { TopbarUserDropdownComponent } from "../../../partials/topbar-user-dropdown/topbar-user-dropdown.component";
import { TopbarNotificationDropdownComponent } from "../../../partials/topbar-notification-dropdown/topbar-notification-dropdown.component";
import { TopbarAppsComponent } from "../../../partials/topbar-apps/topbar-apps.component";
import { TopbarChatComponent } from "../../../partials/topbar-chat/topbar-chat.component";
import { TopbarSearchModalComponent } from "../../../partials/topbar-search-modal/topbar-search-modal.component";
import { ThemeService } from "../../../core/services/theme.service";

@Component({
  selector: "[app-header]",
  imports: [
    TopbarUserDropdownComponent,
    TopbarNotificationDropdownComponent,
    TopbarAppsComponent,
    TopbarChatComponent,
    TopbarSearchModalComponent,
  ],
  templateUrl: "./header.component.html",
  styleUrl: "./header.component.scss",
})
export class HeaderComponent {
  theme = inject(ThemeService).theme;
}
