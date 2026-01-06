import { Component, computed, input, output } from "@angular/core";
import { CommonModule } from "@angular/common";

import { IconName } from "@shared/types/icon-name";
import { IconComponent } from "../icon/icon.component";

@Component({
  selector: "app-fancy-radio-button",
  imports: [CommonModule, IconComponent],
  templateUrl: "./fancy-radio-button.component.html",
  styleUrl: "./fancy-radio-button.component.scss",
  host: {
    class: "w-full",
  },
})
export class FancyRadioButtonComponent<T> {
  readonly id = input.required<string>();
  readonly name = input.required<string>();
  readonly label = input.required<string>();
  readonly value = input.required<T>();
  readonly icon = input.required<IconName>();
  readonly selected = input.required<boolean>();
  readonly disabled = input(false);

  protected readonly clicked = output<T>();

  protected stateClass = computed(() =>
    this.selected() ? "inset-ring-[3px] inset-ring-primary/20" : "inset-ring inset-ring-gray-200",
  );
}
