import { Component, input } from "@angular/core";

import { IconName } from "@shared/types/icon-name";
import { IconComponent } from "../icon/icon.component";

@Component({
  selector: "app-input",
  imports: [IconComponent],
  templateUrl: "./input.component.html",
  styleUrl: "./input.component.scss",
})
export class InputComponent {
  protected readonly key = `input-${Math.random()}`;

  readonly label = input.required<string>();
  readonly placeholder = input<string>("");
  readonly description = input<string>("");
  readonly icon = input<IconName>();
  readonly error = input<string>("");
  readonly type = input<string>("text");
  readonly disabled = input<boolean>(false);
}
