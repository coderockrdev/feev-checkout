import { Component, inject } from "@angular/core";
import { UserDropdownComponent } from "../partials/user-dropdown/user-dropdown.component";
import { NotificationComponent } from "../partials/notification/notification.component";
import { ThemeService } from "../../../core/services/theme.service";

@Component({
  selector: "[app-header]",
  imports: [NotificationComponent, UserDropdownComponent],
  templateUrl: "./header.component.html",
  styleUrl: "./header.component.scss",
})
export class HeaderComponent {
  theme = inject(ThemeService).theme;
}
