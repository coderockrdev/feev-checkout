import { Component, input } from "@angular/core";

import { SelectOption } from "@shared/types/select/select-option";

@Component({
  selector: "app-select",
  imports: [],
  templateUrl: "./select.component.html",
  styleUrl: "./select.component.scss",
})
export class SelectComponent<T> {
  protected readonly key = `select-${Math.random()}`;

  readonly label = input.required<string>();
  readonly placeholder = input<string>("");
  readonly description = input<string>("");
  readonly error = input<string>("");
  readonly disabled = input<boolean>(false);
  readonly options = input.required<SelectOption<T>[]>();
}
