import { Component, forwardRef, input, signal } from "@angular/core";
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from "@angular/forms";

@Component({
  selector: "app-radio-group",
  imports: [],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => RadioGroupComponent),
      multi: true,
    },
  ],
  templateUrl: "./radio-group.component.html",
  styleUrl: "./radio-group.component.scss",
  host: {
    class: "",
  },
})
export class RadioGroupComponent<T> implements ControlValueAccessor {
  value = signal<Nullable<T>>(null);

  disabled = input(false);

  protected isDisabled = signal(false);

  private onChange = (_: T) => {};
  private onTouched = () => {};

  writeValue(value: T): void {
    this.value.set(value);
  }

  registerOnChange(fn: typeof this.onChange): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: typeof this.onTouched): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.isDisabled.set(isDisabled);
  }

  select(value: T): void {
    if (this.isDisabled() || this.disabled()) return;

    this.value.set(value);
    this.onChange(value);
    this.onTouched();
  }
}
