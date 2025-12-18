import { Component, input } from "@angular/core";
import { MatIconModule } from "@angular/material/icon";

import { IconName } from "@shared/types/icon-name";

@Component({
  selector: "app-icon",
  imports: [MatIconModule],
  templateUrl: "./icon.component.html",
  styleUrl: "./icon.component.scss",
  host: {
    class: "",
  },
})
export class IconComponent {
  readonly icon = input.required<IconName>();
  readonly size = input<number>(16);
}
