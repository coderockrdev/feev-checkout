import { Component, forwardRef, inject, Injector, input, OnInit, signal } from "@angular/core";
import { CommonModule } from "@angular/common";
import { ControlValueAccessor, NG_VALUE_ACCESSOR, NgControl } from "@angular/forms";

import { SelectOption } from "@shared/types/select/select-option";
import { KtSelectDirective } from "@shared/directives/kt-select.directive";

interface SelectEvent<T> extends Event {
  detail: {
    payload: {
      value: T;
      selected: boolean;
      selectedOptions: T[];
    };
  };
}

@Component({
  selector: "app-select",
  imports: [CommonModule, KtSelectDirective],
  templateUrl: "./select.component.html",
  styleUrl: "./select.component.scss",
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => SelectComponent),
      multi: true,
    },
  ],
})
export class SelectComponent<T> implements ControlValueAccessor, OnInit {
  private injector = inject(Injector);

  private ngControl?: Nullable<NgControl>;

  ngOnInit() {
    this.ngControl = this.injector.get(NgControl, null, {
      self: true,
      optional: true,
    });

    if (this.ngControl) {
      this.ngControl.valueAccessor = this;
    }
  }

  protected readonly key = `select-${Math.random()}`;

  readonly label = input.required<string>();
  readonly placeholder = input<string>("");
  readonly description = input<string>("");
  readonly error = input<string>("");
  readonly disabled = input<boolean>(false);
  readonly options = input.required<SelectOption<T>[]>();
  readonly size = input<Nullable<"sm" | "md" | "lg">>("lg");

  protected readonly value = signal<Nullable<T>>(null);

  private onChange = (_: T) => {};

  protected onTouched = () => {};

  writeValue(value: T): void {
    this.value.set(value);
  }

  registerOnChange(fn: typeof this.onChange): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: typeof this.onTouched): void {
    this.onTouched = fn;
  }

  select(event: Event): void {
    const selectEvent = event as SelectEvent<T>;
    const value = selectEvent.detail.payload.value;

    this.value.set(value);
    this.onChange(value);
    this.onTouched();
  }

  get errorMessage(): Nullable<string> {
    const errors = this.ngControl?.control?.errors;
    if (!errors || !this.ngControl?.control?.touched) return null;

    if (errors["required"]) return "Campo obrigatório";
    if (errors["zod"]) return errors["zod"][0];

    return null;
  }

  protected getSizeClass() {
    const size = this.size();
    if (size === "sm") return "kt-select-sm";
    if (size === "lg") return "kt-select-lg";
    return "";
  }
}
