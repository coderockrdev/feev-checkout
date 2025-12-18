import { Component, computed, forwardRef, input, signal } from "@angular/core";
import { CommonModule } from "@angular/common";
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from "@angular/forms";

import { IconName } from "@shared/types/icon-name";
import { IconComponent } from "../icon/icon.component";

@Component({
  selector: "app-fancy-radio-button",
  imports: [CommonModule, IconComponent],
  templateUrl: "./fancy-radio-button.component.html",
  styleUrl: "./fancy-radio-button.component.scss",
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => FancyRadioButtonComponent),
      multi: true,
    },
  ],
  host: {
    class: "w-full",
  },
})
export class FancyRadioButtonComponent<T> implements ControlValueAccessor {
  readonly id = input.required<string>();
  readonly name = input.required<string>();
  readonly label = input.required<string>();
  readonly value = input.required<T>();
  readonly icon = input.required<IconName>();

  protected checked = signal(false);
  protected disabled = false;

  protected stateClass = computed(() =>
    this.checked() ? "inset-ring-[3px] inset-ring-highlight" : "inset-ring inset-ring-gray-200",
  );

  private onChange = (_: T) => {};
  private onTouched = () => {};

  writeValue(value: T): void {
    console.log(value);
    this.checked.set(value === this.value());
  }

  registerOnChange(fn: (val: T) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  protected handleChange(): void {
    this.onChange(this.value());
    this.onTouched();
  }
}
