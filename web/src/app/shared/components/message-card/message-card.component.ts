import { Component, input } from "@angular/core";
import { CommonModule } from "@angular/common";

import { IconName } from "@shared/types/icon-name";

import { IconFrameComponent } from "../icon-frame/icon-frame.component";
import { IconComponent } from "../icon/icon.component";
import { CardComponent } from "../card/card.component";

@Component({
  selector: "app-message-card",
  imports: [CommonModule, IconFrameComponent, IconComponent, CardComponent],
  templateUrl: "./message-card.component.html",
  styleUrl: "./message-card.component.scss",
})
export class MessageCardComponent {
  heading = input<string>();
  subHeading = input<string>();
  icon = input<IconName>();
  variant = input<"success" | "error">("success");

  protected getColorVariant() {
    return this.variant() === "success" ? "text-green-500" : "text-red-500";
  }
}
