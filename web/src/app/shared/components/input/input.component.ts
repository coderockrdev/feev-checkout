import {
  AfterViewInit,
  Component,
  ElementRef,
  forwardRef,
  inject,
  Injector,
  input,
  OnInit,
  output,
  signal,
  ViewChild,
} from "@angular/core";
import { MaskitoDirective } from "@maskito/angular";
import { MaskitoOptions } from "@maskito/core";

import { IconName } from "@shared/types/icon-name";
import { IconComponent } from "../icon/icon.component";
import { ControlValueAccessor, NG_VALUE_ACCESSOR, NgControl } from "@angular/forms";
import { CommonModule } from "@angular/common";

@Component({
  selector: "app-input",
  imports: [CommonModule, IconComponent, MaskitoDirective],
  templateUrl: "./input.component.html",
  styleUrl: "./input.component.scss",
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => InputComponent),
      multi: true,
    },
  ],
  host: {
    class: "kt-form-item",
  },
})
export class InputComponent implements ControlValueAccessor, OnInit, AfterViewInit {
  @ViewChild("input", { static: true }) private input!: ElementRef<HTMLInputElement>;

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

  ngAfterViewInit() {
    const value = this.defaultValue();

    if (value) {
      this.value.set(value);
    }
  }

  protected readonly key = `input-${Math.random()}`;

  readonly name = input<string>();
  readonly defaultValue = input<string>();
  readonly label = input<string>();
  readonly placeholder = input<string>("");
  readonly description = input<string>("");
  readonly icon = input<IconName>();
  readonly type = input<string>("text");
  readonly disabled = input<boolean>(false);
  readonly mask = input<Nullable<MaskitoOptions>>(null);
  readonly size = input<Nullable<"sm" | "md" | "lg">>("lg");
  readonly readOnly = input<boolean>(false);
  readonly inputClass = input<string>("");

  protected readonly value = signal("");

  focused = output<FocusEvent>();

  protected handleFocus = (event: FocusEvent) => {
    this.focused.emit(event);
  };

  protected onChange = (_: string) => {};

  protected onTouched = () => {};

  writeValue(value: string): void {
    this.value.set(value);
  }

  registerOnChange(fn: typeof this.onChange): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: typeof this.onTouched): void {
    this.onTouched = fn;
  }

  protected handleChange(event: Event) {
    const value = event.target && "value" in event.target ? (event.target.value as string) : "";

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
    if (size === "sm") return "kt-input-sm";
    if (size === "lg") return "kt-input-lg";
    return "";
  }

  protected getAddonSizeClass() {
    const size = this.size();
    if (size === "sm") return "kt-input-addon-sm";
    if (size === "lg") return "kt-input-addon-lg";
    return "";
  }

  focus() {
    this.input.nativeElement.focus();
  }

  focusAndSelectContent() {
    const input = this.input.nativeElement;
    const length = input.value.length;
    input.setSelectionRange(length, length);
    input.focus();
    input.select();
  }
}
